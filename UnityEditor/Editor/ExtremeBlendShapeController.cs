using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ExtremeBlendShapeController : EditorWindow
{
    private SkinnedMeshRenderer targetSMR;
    private Mesh sharedMesh;
    private Vector3[] baseVertices;
    private Dictionary<int, Vector3[]> cachedDeltas = new Dictionary<int, Vector3[]>();
    private float[] customWeights;
    private HashSet<string> aaoNames = new HashSet<string>();
    
    private Vector2 scrollPos;
    private float sliderMin = -200f;
    private float sliderMax = 500f;
    private string filterString = "";
    private int activeDragIndex = -1;

    [MenuItem("Tools/Extreme BlendShape Controller (Hard Breaking)")]
    public static void ShowWindow()
    {
        GetWindow<ExtremeBlendShapeController>("极限顶点注入器");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("💥 极限顶点注入器 (Hard Breaking Mode)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("该工具通过‘直接注入物理位移’来绕过 Unity 内部限制。数值越大，变形越夸张，无视任何上限。", MessageType.Warning);
        
        EditorGUI.BeginChangeCheck();
        targetSMR = (SkinnedMeshRenderer)EditorGUILayout.ObjectField("目标网格 (Hierarchy)", targetSMR, typeof(SkinnedMeshRenderer), true);
        if (EditorGUI.EndChangeCheck())
        {
            InitializeMeshData();
        }
        EditorGUILayout.EndVertical();

        if (targetSMR == null || targetSMR.sharedMesh == null)
        {
            EditorGUILayout.HelpBox("请先拖入模型，点击下方初始化数据。", MessageType.Info);
            if (GUILayout.Button("手动初始化/刷新数据")) InitializeMeshData();
            return;
        }

        if (baseVertices == null)
        {
            if (GUILayout.Button("🚀 核心物理数据注入初始化")) InitializeMeshData();
            return;
        }

        // --- 全局拖拽拦截 (核心逻辑 1/2) ---
        HandleGlobalDragging();

        // 工具栏
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("范围:", GUILayout.Width(40));
        sliderMin = EditorGUILayout.FloatField(sliderMin, GUILayout.Width(50));
        GUILayout.Label("~", GUILayout.Width(10));
        sliderMax = EditorGUILayout.FloatField(sliderMax, GUILayout.Width(50));
        
        GUILayout.Space(20);
        filterString = EditorGUILayout.TextField(filterString, EditorStyles.toolbarSearchField);
        
        if (GUILayout.Button("复原原始网格", EditorStyles.toolbarButton))
        {
            RevertMesh();
        }
        EditorGUILayout.EndHorizontal();

        // 列表区域
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < sharedMesh.blendShapeCount; i++)
        {
            string name = sharedMesh.GetBlendShapeName(i);
            if (!string.IsNullOrEmpty(filterString) && !name.ToLower().Contains(filterString.ToLower()))
                continue;

            EditorGUILayout.BeginHorizontal();
            
            // --- 拖放探测区 (核心逻辑 2/2) ---
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = aaoNames.Contains(name) ? Color.green : Color.white;
            if (activeDragIndex == i) labelStyle.normal.textColor = Color.yellow; 

            // 占位并渲染标签，ExpandWidth 确保名称不被切断
            Rect labelRect = GUILayoutUtility.GetRect(new GUIContent(name), labelStyle, GUILayout.ExpandWidth(true), GUILayout.MinWidth(180));
            EditorGUIUtility.AddCursorRect(labelRect, MouseCursor.SlideArrow);
            
            if (Event.current.type == EventType.Repaint)
                labelStyle.Draw(labelRect, $"[{i}] {name}", false, false, false, false);

            // 开始拖拽检测
            if (Event.current.type == EventType.MouseDown && labelRect.Contains(Event.current.mousePosition))
            {
                activeDragIndex = i;
                EditorGUIUtility.SetWantsMouseJumping(1); 
                Event.current.Use();
            }

            // --- 控件区 ---
            EditorGUI.BeginChangeCheck();
            float newWeightSlider = GUILayout.HorizontalSlider(customWeights[i], sliderMin, sliderMax, GUILayout.Width(80));
            if (EditorGUI.EndChangeCheck())
            {
                ApplyWeight(i, newWeightSlider);
            }

            EditorGUI.BeginChangeCheck();
            float newWeightField = EditorGUILayout.FloatField(customWeights[i], GUILayout.Width(60));
            if (EditorGUI.EndChangeCheck())
            {
                ApplyWeight(i, newWeightField);
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("💾 固化当前形态为新网格 (Bake)"))
        {
            BakeCurrentToNewAsset();
        }
    }

    private void HandleGlobalDragging()
    {
        if (activeDragIndex == -1) return;

        if (Event.current.type == EventType.MouseDrag)
        {
            float multiplier = Event.current.shift ? 10f : (Event.current.control ? 0.1f : 1f);
            float delta = Event.current.delta.x * multiplier * 0.5f;
            ApplyWeight(activeDragIndex, customWeights[activeDragIndex] + delta);
            Event.current.Use();
        }
        else if (Event.current.type == EventType.MouseUp || Event.current.rawType == EventType.MouseUp)
        {
            activeDragIndex = -1;
            EditorGUIUtility.SetWantsMouseJumping(0);
            Event.current.Use();
        }
    }

    private void InitializeMeshData()
    {
        if (targetSMR == null || targetSMR.sharedMesh == null) return;

        sharedMesh = targetSMR.sharedMesh;
        baseVertices = (Vector3[])sharedMesh.vertices.Clone();
        customWeights = new float[sharedMesh.blendShapeCount];
        cachedDeltas.Clear();
        aaoNames.Clear();

        for (int i = 0; i < sharedMesh.blendShapeCount; i++)
        {
            string name = sharedMesh.GetBlendShapeName(i);
            if (name.StartsWith("AAO_Merged_")) aaoNames.Add(name);

            Vector3[] dv = new Vector3[sharedMesh.vertexCount];
            sharedMesh.GetBlendShapeFrameVertices(i, 0, dv, null, null);
            cachedDeltas[i] = dv;
        }
        Debug.Log($"[极限注入] 数据初始化成功！已提取 {sharedMesh.blendShapeCount} 个位移流。");
    }

    private void ApplyWeight(int index, float weight)
    {
        if (index < 0 || index >= customWeights.Length) return;
        customWeights[index] = weight;
        RefreshMeshDeformation();
        Repaint(); 
    }

    private void RefreshMeshDeformation()
    {
        if (baseVertices == null || targetSMR == null) return;

        if (targetSMR.sharedMesh == sharedMesh)
        {
            Mesh instanceMesh = Instantiate(sharedMesh);
            instanceMesh.name = sharedMesh.name + " (Extreme Instance)";
            targetSMR.sharedMesh = instanceMesh;
        }

        Vector3[] morphedVertices = (Vector3[])baseVertices.Clone();
        for (int i = 0; i < customWeights.Length; i++)
        {
            float w = customWeights[i] / 100f;
            if (Mathf.Abs(w) < 0.0001f) continue;
            Vector3[] deltas = cachedDeltas[i];
            for (int v = 0; v < morphedVertices.Length; v++)
                morphedVertices[v] += deltas[v] * w;
        }

        targetSMR.sharedMesh.vertices = morphedVertices;
        targetSMR.sharedMesh.RecalculateBounds();
        targetSMR.sharedMesh.UploadMeshData(false);
    }

    private void RevertMesh()
    {
        if (targetSMR != null && sharedMesh != null)
        {
            targetSMR.sharedMesh = sharedMesh;
            for (int i = 0; i < customWeights.Length; i++) customWeights[i] = 0;
            Repaint();
        }
    }

    private void BakeCurrentToNewAsset()
    {
        if (targetSMR == null || targetSMR.sharedMesh == null) return;
        Mesh bakedMesh = Instantiate(targetSMR.sharedMesh);
        bakedMesh.name = sharedMesh.name + "_ExtremeBake";
        string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/{bakedMesh.name}.asset");
        AssetDatabase.CreateAsset(bakedMesh, path);
        AssetDatabase.SaveAssets();
        targetSMR.sharedMesh = bakedMesh;
        EditorGUIUtility.PingObject(bakedMesh);
    }
}
