using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace VRChatAvatarTools.Editor
{
    public class BlendShapeAppendTool_Fast : EditorWindow
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

        [MenuItem("Tools/VRChat/形态键融合仪 (原模型)")]
        public static void ShowWindow()
        {
            var window = GetWindow<BlendShapeAppendTool_Fast>("和美极速融合仪");
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
            int autoFixedAAOCount = 0; // 记录成功修复了多少个 AAO 绑定

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
                                string sVal = sourceProp.stringValue; // 源
                                string tVal = targetProp.stringValue; // 目标

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
                        // 兼容其他可能没有 Bindings 的零散组件
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

                    // 保存篡改的数据
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
                // 如果 MA 需要它，且它没有被 AAO 优化掉，就打上勾
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
                reportMsg += $"\n\n🛠️ 【AAO自适应成功】：自动修正了 {autoFixedAAOCount} 处 MA 绑定！\n并且已成功锁定目标 Blendshape，防止了连带修改的错误。";
            }

            EditorUtility.DisplayDialog("扫描与修正报告", reportMsg, "太牛逼了");
        }

        // -----------------------------------------------------------------
        // 极速静默生成逻辑 (保持不变)
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

            Mesh newMesh = Instantiate(targetMesh);
            newMesh.name = targetMesh.name;

            int vertexCount = sourceMesh.vertexCount;
            int renameCount = 0;
            int currentProcessed = 0;

            try
            {
                for (int i = 0; i < sourceMesh.blendShapeCount; i++)
                {
                    if (!shapeSelections[i]) continue;

                    string shapeName = sourceShapeNames[i];
                    if (newMesh.GetBlendShapeIndex(shapeName) != -1) { shapeName += "_Merged"; renameCount++; }

                    int frameCount = sourceMesh.GetBlendShapeFrameCount(i);
                    currentProcessed++;
                    EditorUtility.DisplayProgressBar("极速生成中", $"正在编译物理数据: {shapeName}", (float)currentProcessed / transferCount);

                    for (int frame = 0; frame < frameCount; frame++)
                    {
                        float frameWeight = sourceMesh.GetBlendShapeFrameWeight(i, frame);
                        Vector3[] deltaVerts = new Vector3[vertexCount], deltaNorms = new Vector3[vertexCount], deltaTans = new Vector3[vertexCount];
                        sourceMesh.GetBlendShapeFrameVertices(i, frame, deltaVerts, deltaNorms, deltaTans);
                        newMesh.AddBlendShapeFrame(shapeName, frameWeight, deltaVerts, deltaNorms, deltaTans);
                    }
                }

                EditorUtility.ClearProgressBar();

                string safeFileName = string.Join("_", targetMesh.name.Split(Path.GetInvalidFileNameChars()));
                string targetPath = $"Assets/{safeFileName}.asset";
                string uniqueSavePath = AssetDatabase.GenerateUniqueAssetPath(targetPath);

                AssetDatabase.CreateAsset(newMesh, uniqueSavePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (targetObj != null)
                {
                    SkinnedMeshRenderer smr = targetObj.GetComponent<SkinnedMeshRenderer>();
                    Undo.RecordObject(smr, "Auto Replace Mesh & Zero Transferred Weights");
                    
                    // 记录 B 原有形态键的权重
                    int oldBCount = targetMesh.blendShapeCount;
                    float[] oldBWeights = new float[oldBCount];
                    for (int w = 0; w < oldBCount; w++) oldBWeights[w] = smr.GetBlendShapeWeight(w);

                    smr.sharedMesh = newMesh;

                    // 还原 B 的原有权重，并将所有从 A 转移过来的新形态键设为 0
                    for (int w = 0; w < newMesh.blendShapeCount; w++)
                    {
                        if (w < oldBCount)
                            smr.SetBlendShapeWeight(w, oldBWeights[w]);
                        else
                            smr.SetBlendShapeWeight(w, 0); // 将 A 转移来的新项强制设为 0
                    }

                    EditorUtility.SetDirty(smr);
                }

                EditorGUIUtility.PingObject(newMesh);
                Debug.Log($"[极速融合仪] 转移完成！文件已静默生成于: {uniqueSavePath}");
            }
            catch (System.Exception ex) { Debug.LogError($"[极速融合仪 报错]: {ex.Message}"); EditorUtility.ClearProgressBar(); }
            finally { EditorUtility.ClearProgressBar(); }
        }
    }
}