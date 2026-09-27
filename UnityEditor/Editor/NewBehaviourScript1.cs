using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace VRChatAvatarTools.Editor
{
    public class BlendShapeAppendTool_Physical : EditorWindow
    {
        private GameObject sourceObj;
        private GameObject targetObj;
        private GameObject maSyncObj;

        private Mesh sourceMesh;
        private Mesh targetMesh;

        private List<string> sourceShapeNames = new List<string>();
        private List<bool> shapeSelections = new List<bool>();

        // 【新增】：AAO 映射字典 (Key: A的原名, Value: B的AAO新名)
        private Dictionary<string, string> aaoMapping = new Dictionary<string, string>();

        private Vector2 scrollPosition;

        [MenuItem("Tools/VRChat/形态键融合仪 (克隆模型)")]
        public static void ShowWindow()
        {
            var window = GetWindow<BlendShapeAppendTool_Physical>("和美物理替换仪");
            window.minSize = new Vector2(480, 750);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Space(15);
            EditorGUILayout.LabelField("🧬 拓扑级形态键融合引擎 (AAO桥接自动化版)", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("极速模式：支持 AAO 自动查重规避，扫描时会自动修复 MA Sync 的旧绑定！直接在 Assets 生成网格并替换。", MessageType.Info);
            GUILayout.Space(10);

            // === 1. 源模型输入区 ===
            EditorGUI.BeginChangeCheck();
            sourceObj = (GameObject)EditorGUILayout.ObjectField("源模型 A (提供形态键):", sourceObj, typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck()) UpdateSourceMesh();

            if (sourceMesh != null)
                EditorGUILayout.LabelField($"  └ 网格: {sourceMesh.name} ({sourceMesh.vertexCount} 顶点)", EditorStyles.miniLabel);

            GUILayout.Space(5);

            // === 2. 目标模型输入区 ===
            EditorGUI.BeginChangeCheck();
            targetObj = (GameObject)EditorGUILayout.ObjectField("目标模型 B (接收形态键):", targetObj, typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck()) UpdateTargetMesh();

            if (targetMesh != null)
                EditorGUILayout.LabelField($"  └ 网格: {targetMesh.name} ({targetMesh.vertexCount} 顶点)", EditorStyles.miniLabel);

            GUILayout.Space(15);

            // === 3. MA 终极暴力扫描与 AAO 修正区 ===
            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUILayout.LabelField("🤖 MA 深层扫描 & AAO 绑定修正", EditorStyles.boldLabel);
            maSyncObj = (GameObject)EditorGUILayout.ObjectField("加载衣服预制件/节点:", maSyncObj, typeof(GameObject), true);

            GUI.backgroundColor = new Color(1.0f, 0.5f, 0.0f); // 橙色警示按钮
            if (GUILayout.Button("🔍 X光扫描匹配项 + 自动篡改修复 AAO 绑定", GUILayout.Height(35)))
            {
                ScanMABlendshapesForcefully();
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndVertical();

            GUILayout.Space(15);

            // === 4. 形态键勾选列表区 ===
            if (sourceMesh != null && sourceShapeNames.Count > 0)
            {
                EditorGUILayout.LabelField($"📦 A 模型形态键列表 (共 {sourceShapeNames.Count} 个):", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("☑ 全选", GUILayout.Width(80)))
                {
                    // 全选时，如果有 AAO 映射的，依然保持不勾选！
                    for (int i = 0; i < shapeSelections.Count; i++)
                        shapeSelections[i] = !aaoMapping.ContainsKey(sourceShapeNames[i]);
                }

                if (GUILayout.Button("☐ 全不选", GUILayout.Width(80)))
                    for (int i = 0; i < shapeSelections.Count; i++) shapeSelections[i] = false;
                GUILayout.EndHorizontal();

                GUIStyle listStyle = new GUIStyle("HelpBox");
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, listStyle, GUILayout.Height(250));

                for (int i = 0; i < sourceShapeNames.Count; i++)
                {
                    string sName = sourceShapeNames[i];

                    // 【新增】：UI 提示 AAO 已经被优化的形态键
                    if (aaoMapping.ContainsKey(sName))
                    {
                        GUILayout.BeginHorizontal();
                        shapeSelections[i] = EditorGUILayout.ToggleLeft($"[{i}] {sName}", shapeSelections[i], GUILayout.Width(220));
                        GUI.contentColor = new Color(0.3f, 0.8f, 0.3f); // 绿色提示文字
                        EditorGUILayout.LabelField($"→ 已被AAO保留: {aaoMapping[sName]}", EditorStyles.miniLabel);
                        GUI.contentColor = Color.white;
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        shapeSelections[i] = EditorGUILayout.ToggleLeft($"[{i}] {sName}", shapeSelections[i]);
                    }
                }

                EditorGUILayout.EndScrollView();
            }
            else if (sourceMesh != null && sourceShapeNames.Count == 0)
            {
                EditorGUILayout.HelpBox("该源网格不包含任何形态键。", MessageType.Warning);
            }

            GUILayout.Space(20);

            // === 5. 一键执行按钮 ===
            GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f); // 绿色极速通行按钮
            if (GUILayout.Button("⚡ 一键静默生成并替换网格", GUILayout.Height(45)))
            {
                ExecuteAppendSilent();
            }
            GUI.backgroundColor = Color.white;
        }

        private void UpdateSourceMesh()
        {
            sourceMesh = null; sourceShapeNames.Clear(); shapeSelections.Clear();

            if (sourceObj != null)
            {
                SkinnedMeshRenderer smr = sourceObj.GetComponent<SkinnedMeshRenderer>();
                if (smr != null && smr.sharedMesh != null)
                {
                    sourceMesh = smr.sharedMesh;
                    for (int i = 0; i < sourceMesh.blendShapeCount; i++)
                    {
                        sourceShapeNames.Add(sourceMesh.GetBlendShapeName(i));
                        shapeSelections.Add(true);
                    }
                    RefreshAAOMapping(); // 刷新 AAO 字典
                }
                else { EditorUtility.DisplayDialog("提示", "源对象没有 SkinnedMeshRenderer！", "确定"); sourceObj = null; }
            }
        }

        private void UpdateTargetMesh()
        {
            targetMesh = null;
            if (targetObj != null)
            {
                SkinnedMeshRenderer smr = targetObj.GetComponent<SkinnedMeshRenderer>();
                if (smr != null && smr.sharedMesh != null)
                {
                    targetMesh = smr.sharedMesh;
                    RefreshAAOMapping(); // 刷新 AAO 字典
                }
                else { EditorUtility.DisplayDialog("提示", "目标对象没有 SkinnedMeshRenderer！", "确定"); targetObj = null; }
            }
        }

        private void RefreshAAOMapping()
        {
            aaoMapping.Clear();
            if (sourceMesh == null || targetMesh == null || sourceShapeNames.Count == 0) return;

            for (int i = 0; i < targetMesh.blendShapeCount; i++)
            {
                string bName = targetMesh.GetBlendShapeName(i);

                // 侦测 AAO 打包的形态键
                if (bName.StartsWith("AAO_Merged_"))
                {
                    string bestMatchAName = null;

                    // 寻找 A 模型中最长匹配的原名
                    foreach (string aName in sourceShapeNames)
                    {
                        if (bName == "AAO_Merged_" + aName || bName.StartsWith("AAO_Merged_" + aName + "_"))
                        {
                            if (bestMatchAName == null || aName.Length > bestMatchAName.Length)
                            {
                                bestMatchAName = aName;
                            }
                        }
                    }

                    if (bestMatchAName != null && !aaoMapping.ContainsKey(bestMatchAName))
                    {
                        aaoMapping.Add(bestMatchAName, bName);

                        // 既然 B 模型里已经有了，就在 UI 列表中自动取消勾选，避免重复生成！
                        int index = sourceShapeNames.IndexOf(bestMatchAName);
                        if (index != -1) shapeSelections[index] = false;
                    }
                }
            }
        }

        // -----------------------------------------------------------------
        // 🔥 【核心修复】：防止目标 Blendshape 连带改变的智能扫描器
        // -----------------------------------------------------------------
        private void ScanMABlendshapesForcefully()
        {
            if (maSyncObj == null || sourceShapeNames.Count == 0) return;

            Component[] allComponents = maSyncObj.GetComponentsInChildren<Component>(true);
            HashSet<string> allHiddenStrings = new HashSet<string>();
            int scannedMACount = 0;
            int autoFixedAAOCount = 0;

            foreach (var comp in allComponents)
            {
                if (comp == null) continue;

                string compName = comp.GetType().Name;
                if (compName.Contains("ModularAvatar") || compName.Contains("Blendshape"))
                {
                    scannedMACount++;
                    SerializedObject so = new SerializedObject(comp);
                    bool compModified = false;

                    // 精准定位 MA 的 Bindings 数组
                    SerializedProperty bindingsProp = so.FindProperty("Bindings");

                    if (bindingsProp != null && bindingsProp.isArray)
                    {
                        for (int i = 0; i < bindingsProp.arraySize; i++)
                        {
                            SerializedProperty element = bindingsProp.GetArrayElementAtIndex(i);
                            SerializedProperty sourceProp = element.FindPropertyRelative("Blendshape");
                            SerializedProperty targetProp = element.FindPropertyRelative("LocalBlendshape");

                            if (sourceProp != null && targetProp != null)
                            {
                                string sVal = sourceProp.stringValue;
                                string tVal = targetProp.stringValue;

                                if (!string.IsNullOrEmpty(sVal))
                                {
                                    allHiddenStrings.Add(sVal);

                                    if (aaoMapping.ContainsKey(sVal))
                                    {
                                        // 1. 优先记录目标 blendshape 的参数（如果为空，默认其与原源名称相同）
                                        string originalTarget = string.IsNullOrEmpty(tVal) ? sVal : tVal;

                                        // 2. 将源 blendshape 改变为 AAO 优化名
                                        sourceProp.stringValue = aaoMapping[sVal];

                                        // 3. 强行将记录着的目标 blendshape 改回去，切断连带效应
                                        targetProp.stringValue = originalTarget;

                                        compModified = true;
                                        autoFixedAAOCount++;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        SerializedProperty sp = so.GetIterator();
                        while (sp.Next(true))
                        {
                            if (sp.propertyType == SerializedPropertyType.String)
                            {
                                string val = sp.stringValue;
                                if (!string.IsNullOrEmpty(val))
                                {
                                    allHiddenStrings.Add(val);
                                    if (aaoMapping.ContainsKey(val))
                                    {
                                        sp.stringValue = aaoMapping[val];
                                        compModified = true;
                                        autoFixedAAOCount++;
                                    }
                                }
                            }
                        }
                    }

                    if (compModified)
                    {
                        so.ApplyModifiedProperties();
                        EditorUtility.SetDirty(comp);
                    }
                }
            }

            if (scannedMACount == 0) return;

            int matchCount = 0;
            for (int i = 0; i < sourceShapeNames.Count; i++)
            {
                if (allHiddenStrings.Contains(sourceShapeNames[i]) && !aaoMapping.ContainsKey(sourceShapeNames[i]))
                {
                    shapeSelections[i] = true;
                    matchCount++;
                }
                else
                {
                    shapeSelections[i] = false;
                }
            }

            string reportMsg = $"扫描完毕！\n✅ 成功勾选了 {matchCount} 个缺失的形态键。";
            if (autoFixedAAOCount > 0)
            {
                reportMsg += $"\n\n🛠️ 【AAO自适应成功】：自动修正了 {autoFixedAAOCount} 处 MA 绑定！\n并且已成功锁定目标 Blendshape，防止了连带修改错误。";
            }

            EditorUtility.DisplayDialog("扫描与修正报告", reportMsg, "太牛逼了");
        }

        // -----------------------------------------------------------------
        // 极速静默生成逻辑
        // -----------------------------------------------------------------
        private void ExecuteAppendSilent()
        {
            if (sourceMesh == null || targetMesh == null)
            {
                EditorUtility.DisplayDialog("拦截", "A 和 B 槽位不能为空！", "确定"); return;
            }
            if (sourceMesh.vertexCount != targetMesh.vertexCount)
            {
                EditorUtility.DisplayDialog("灾难错误", "拓扑不匹配！必须完全相等！", "确定"); return;
            }

            int transferCount = 0;
            for (int i = 0; i < shapeSelections.Count; i++) if (shapeSelections[i]) transferCount++;

            if (transferCount == 0)
            {
                EditorUtility.DisplayDialog("拦截", "没有勾选任何需要转移的项！", "确定"); return;
            }

            SkinnedMeshRenderer targetSMR = targetObj.GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer sourceSMR = sourceObj.GetComponent<SkinnedMeshRenderer>();

            float scaleFactor = CalculateScaleCompensation(sourceMesh, targetMesh);
            
            Mesh newMesh = new Mesh();
            newMesh.name = targetMesh.name + "_PhysicalFixed";
            
            Vector3[] resetVerts = new Vector3[sourceMesh.vertexCount];
            for (int i = 0; i < sourceMesh.vertexCount; i++) resetVerts[i] = sourceMesh.vertices[i] * scaleFactor;
            
            newMesh.vertices = resetVerts;
            newMesh.normals = sourceMesh.normals;
            newMesh.tangents = sourceMesh.tangents;
            newMesh.uv = targetMesh.uv;
            newMesh.uv2 = targetMesh.uv2;
            newMesh.uv3 = targetMesh.uv3;
            newMesh.uv4 = targetMesh.uv4;
            newMesh.colors = targetMesh.colors;
            newMesh.colors32 = targetMesh.colors32;
            newMesh.bindposes = targetMesh.bindposes;
            newMesh.boneWeights = targetMesh.boneWeights;

            newMesh.subMeshCount = targetMesh.subMeshCount;
            for (int i = 0; i < targetMesh.subMeshCount; i++)
                newMesh.SetIndices(targetMesh.GetIndices(i), targetMesh.GetTopology(i), i);

            // 1. 搬运 B 现存的形态键
            int oldBCount = targetMesh.blendShapeCount;
            for (int i = 0; i < oldBCount; i++)
            {
                TransferBlendShapeFrame(targetMesh, newMesh, i, targetMesh.GetBlendShapeName(i), 1f);
            }

            // 2. 注入 A 的新形态键
            int renameCount = 0;
            for (int i = 0; i < sourceMesh.blendShapeCount; i++)
            {
                if (!shapeSelections[i]) continue;
                string sName = sourceShapeNames[i];
                if (newMesh.GetBlendShapeIndex(sName) != -1) { sName += "_Merged"; renameCount++; }
                TransferBlendShapeFrame(sourceMesh, newMesh, i, sName, scaleFactor);
            }

            newMesh.RecalculateBounds();

            string safeFileName = string.Join("_", targetMesh.name.Split(Path.GetInvalidFileNameChars()));
            string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/{safeFileName}.asset");
            AssetDatabase.CreateAsset(newMesh, path);
            AssetDatabase.SaveAssets();

            Undo.RecordObject(targetSMR, "Physical Replace");

            Transform[] targetBones = targetSMR.bones;
            Transform[] sourceBones = sourceSMR.bones;
            Dictionary<string, Transform> targetBoneDict = new Dictionary<string, Transform>();
            foreach (Transform t in targetBones) if (t != null && !targetBoneDict.ContainsKey(t.name)) targetBoneDict.Add(t.name, t);

            Transform[] newBones = new Transform[sourceBones.Length];
            for (int i = 0; i < sourceBones.Length; i++)
            {
                if (sourceBones[i] != null && targetBoneDict.ContainsKey(sourceBones[i].name))
                    newBones[i] = targetBoneDict[sourceBones[i].name];
            }
            targetSMR.bones = newBones;

            if (sourceSMR.rootBone != null && targetBoneDict.ContainsKey(sourceSMR.rootBone.name))
                targetSMR.rootBone = targetBoneDict[sourceSMR.rootBone.name];

            // 2. 状态重置与网格替换
            for (int i = 0; i < targetSMR.sharedMesh.blendShapeCount; i++)
                targetSMR.SetBlendShapeWeight(i, 0);

            targetSMR.sharedMesh = newMesh;
            targetSMR.localBounds = newMesh.bounds;

            // 3. 将新滑块全部归零，呈现 A 原始美态
            for (int i = 0; i < targetSMR.sharedMesh.blendShapeCount; i++)
                targetSMR.SetBlendShapeWeight(i, 0);

            EditorUtility.SetDirty(targetSMR);
            EditorGUIUtility.PingObject(newMesh);
            Debug.Log($"[完美修复] 权重、对齐、材质已全部重构，网格已启用渲染。");
        }

        private void TransferBlendShapeFrame(Mesh source, Mesh dest, int sourceIdx, string destName, float scaleFactor)
        {
            int frameCount = source.GetBlendShapeFrameCount(sourceIdx);
            for (int frame = 0; frame < frameCount; frame++)
            {
                float weight = source.GetBlendShapeFrameWeight(sourceIdx, frame);
                Vector3[] dv = new Vector3[source.vertexCount], dn = new Vector3[source.vertexCount], dt = new Vector3[source.vertexCount];
                source.GetBlendShapeFrameVertices(sourceIdx, frame, dv, dn, dt);

                for (int v = 0; v < dv.Length; v++) dv[v] *= scaleFactor;
                dest.AddBlendShapeFrame(destName, weight, dv, dn, dt);
            }
        }

        private float CalculateScaleCompensation(Mesh a, Mesh b)
        {
            for (int i = 0; i < a.vertexCount; i++) {
                if (a.vertices[i].sqrMagnitude > 0.1f && b.vertices[i].sqrMagnitude > 0.1f)
                    return b.vertices[i].magnitude / a.vertices[i].magnitude;
            }
            return 1.0f;
        }
    }
}