using AssetsTools.NET;
// 引用 AssetsTools.NET 库

using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 扩展库

using Avalonia;
// 引用 Avalonia 框架

using Avalonia.Collections;
// 引用 Avalonia 集合

using Avalonia.Controls;
// 引用 Avalonia 控件

using Avalonia.Controls.Documents;
// 引用 Avalonia 文档控件

using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Threading;

// 引用 XAML 标记扩展

using CommunityToolkit.Mvvm.Messaging;
// 引用消息总线

using System;
// 引用系统基础类型

using System.Collections.Generic;
// 引用泛型集合

using System.Collections.ObjectModel;
// 引用可观察集合

using System.IO;
// 引用 IO 处理

using System.Linq;
// 引用 LINQ

using System.Text;
using System.Threading.Tasks;

// 引用文本处理

using UABEANext4.AssetWorkspace;
// 引用资产工作区

using UABEANext4.Logic;
// 引用逻辑层

using UABEANext4.Util;
// 引用工具类

namespace UABEANext4.Controls;

public class AssetDataTreeView : TreeView
{
    protected override Type StyleKeyOverride => typeof(TreeView);
    // 默认开启翻译
    private bool _isTranslationEnabled = false;
    private AvaloniaList<object> ListItems = new AvaloniaList<object>();
    private MenuItem menuEditAsset;
    private MenuItem menuVisitAsset;
    private MenuItem menuExpandSel;
    private MenuItem menuCollapseSel;

    // [新增] 汉化字典：将 Unity 字段名映射为“中文 (英文)”格式
    // 这里整合了你之前翻译控件中的核心字典
    // [超级字典]：整合了 10 轮深度扫描结果，涵盖 Unity 2022.3 与 VRChat SDK3 
    // [Modified] Values are now pure Chinese translations. Parentheses are added dynamically in GetTranslatedName.
    private static readonly Dictionary<string, string> TranslationMap = new(StringComparer.OrdinalIgnoreCase)
    {
        // === 1. 核心基础与元数据 (Core & Metadata) ===
        {"m_GameObject", "游戏对象"},
        {"m_Enabled", "启用状态"},
        {"m_Name", "名称"},
        {"m_TagString", "标签"},
        {"m_Layer", "图层"},
        {"m_IsActive", "激活状态"},
        {"m_StaticEditorFlags", "静态编辑器标记"},
        {"m_EditorHideFlags", "编辑器隐藏标记"},
        {"m_EditorClassIdentifier", "编辑器类标识"},
        {"m_CorrespondingSourceObject", "源资产对象"},
        {"m_PrefabInstance", "预制体实例"},
        {"m_PrefabAsset", "预制体资产"},
        {"size", "数值大小/容量"},
        {"data", "数据内容"},
        {"Array", "数组结构"},

        // === 2. 变换与层级 (Transform & Hierarchy) ===
        {"m_LocalPosition", "本地位置"},
        {"m_LocalRotation", "本地旋转"},
        {"m_LocalScale", "本地缩放"},
        {"m_Children", "子物体列表"},
        {"m_Father", "父物体"},
        {"m_RootOrder", "根索引"},
        {"m_LocalEulerAnglesHint", "本地欧拉角提示"},

        // === 3. 渲染与光影底层 (Rendering & Graphics) ===
        {"m_Materials", "材质球列表"},
        {"m_CastShadows", "投射阴影"},
        {"m_ReceiveShadows", "接收阴影"},
        {"m_LightProbeUsage", "光照探针使用"},
        {"m_ReflectionProbeUsage", "反射探针使用"},
        {"m_ProbeAnchor", "探针锚点"},
        {"m_LightmapIndex", "光照贴图索引"},
        {"m_RenderingLayerMask", "渲染层遮罩"},
        {"m_RendererPriority", "渲染优先级"},
        {"m_RayTracingMode", "光线追踪模式"},
        {"m_ReceiveGI", "接收全局光照"},
        {"m_SortingLayer", "排序图层"},
        {"m_SortingOrder", "排序索引"},
        {"m_CustomRenderQueue", "自定义渲染队列"},
        {"m_ZWrite", "深度写入"},
        {"m_ZTest", "深度测试"},

        // === 4. 网格与顶点流数据 (Mesh & Vertex Data) ===
        {"m_Mesh", "网格模型"},
        {"m_Bones", "骨骼列表"},
        {"m_RootBone", "根骨骼"},
        {"m_VertexData", "顶点原始数据"},
        {"m_Stream", "顶点流"},
        {"m_Stride", "数据步幅"},
        {"m_Channel", "数据通道"},
        {"m_IndexBuffer", "索引缓冲区"},
        {"m_SubMeshes", "子网格列表"},
        {"m_Topology", "拓扑结构"},
        {"m_IsReadable", "开启 CPU 读写"},
        {"m_BlendShapeWeights", "混合形状权重"},

        // === 5. 物理骨骼与动力学 (PhysBones & Physics) ===
        {"m_Limits", "限制范围"},
        {"m_Stiffness", "硬度/刚性"},
        {"m_Gravity", "重力影响"},
        {"m_Immobile", "固定程度"},
        {"m_Pull", "拉扯感"},
        {"m_Spring", "弹力"},
        {"m_Inertia", "惯性"},
        {"m_Viscosity", "黏性"},
        {"m_AllowGrabbing", "允许抓取"},
        {"m_AllowPosing", "允许固定姿势"},
        {"m_IsTrigger", "是否为触发器"},
        {"m_Convex", "凸包模式"},// === 8. 组件与纹理高级设置 (Components & Texture Advanced) ===
        {"m_Tag", "标签"},
        {"m_Component", "组件列表"},
        {"m_DownscaleFallback", "降级备选方案"},
        {"m_sAlphaChannelOptional", "Alpha 通道可选"},
        {"m_CompleteImageSize", "完整图像大小"}, // 修正了拼写 CompletelmageSize -> CompleteImageSize
        {"m_MipsStripped", "Mipmaps 已剥离"},
        {"m_MipCount", "Mip 层级数量"},
        {"m_IsPreProcessed", "已预处理"},
        {"m_IgnoreMipmapLimit", "忽略 Mipmap 限制"},
        {"m_StreamingMipmapsPriority", "流式 Mipmap 优先级"},
        {"m_TextureDimension", "纹理维度/类型"},
        {"m_TextureSettings", "纹理设置"},
        {"m_PlatformBlob", "平台二进制数据块"},
        {"m_StreamData", "流数据源"},

        // === 6. 动画、化身与 VRChat SDK (Animator & VRC SDK) ===
        {"m_Avatar", "骨骼映射"},
        {"m_Controller", "动画控制器"},
        {"m_UpdateMode", "更新模式"},
        {"m_WriteDefaultValues", "写入默认值"},
        {"m_VRCExpressionsMenu", "VRC 表情菜单"},
        {"m_VRCExpressionParameters", "VRC 表情参数"},
        {"networkSynced", "网络同步"},
        {"isLocalOnly", "仅本地有效"},
        {"saved", "存档保存"},
        {"parameterName", "参数名称"},
        {"defaultValue", "默认值"},
        {"blueprintId", "蓝图 ID"},

        // === 7. 常规坐标轴与颜色 ===
        {"x", "X轴"}, {"y", "Y轴"}, {"z", "Z轴"}, {"w", "W轴"},
        {"r", "红"}, {"g", "绿"}, {"b", "蓝"}, {"a", "透明度"}
    };

    public AssetDataTreeView() : base()
    {
        menuEditAsset = new MenuItem() { Header = "Edit Asset" };
        menuVisitAsset = new MenuItem() { Header = "Visit Asset" };
        menuExpandSel = new MenuItem() { Header = "Expand Selection" };
        menuCollapseSel = new MenuItem() { Header = "Collapse Selection" };

        // 1. 定义翻译开关菜单项
        MenuItem menuToggleTranslation = new MenuItem() { Header = "自动翻译变量名 (Automatic translation of variables)" };

        DoubleTapped += AssetDataTreeView_DoubleTapped;
        menuEditAsset.Click += MenuEditAsset_Click;
        menuVisitAsset.Click += MenuVisitAsset_Click;
        menuExpandSel.Click += MenuExpandSel_Click;
        menuCollapseSel.Click += MenuCollapseSel_Click;

        // 2. 配置 CheckBox 状态显示
        var toggleCheckBox = new CheckBox()
        {
            IsChecked = _isTranslationEnabled,
            IsHitTestVisible = false // 让 CheckBox 不响应点击，由 MenuItem 统一处理
        };
        menuToggleTranslation.Icon = toggleCheckBox;

        // 3. 绑定开关点击事件，触发界面刷新
        menuToggleTranslation.Click += (s, e) => {
            _isTranslationEnabled = !_isTranslationEnabled;
            toggleCheckBox.IsChecked = _isTranslationEnabled;

            // 刷新当前视图：重新加载资产以应用或取消翻译
            if (_activeAssets != null)
            {
                LoadAssets(_activeAssets);
            }
        };

        // 4. 将菜单项整合进 ContextMenu
        ContextMenu = new ContextMenu();
        ContextMenu.ItemsSource = new AvaloniaList<object>() // 注意这里改为 object 可以同时容纳 MenuItem 和 Separator
        {
            menuEditAsset,
            menuVisitAsset,
            menuExpandSel,
            menuCollapseSel,
            new Separator(), // 添加分割线，区分功能区
            menuToggleTranslation
        };

        ActiveAssetsProperty.Changed.Subscribe(e =>
        {
            var value = e.NewValue.Value;
            if (value != null)
            {
                value.CollectionChanged += (s, e) => LoadAssets(value);
                LoadAssets(value);
            }
        });
    }

    // [新增] 辅助方法：尝试获取翻译后的名称
    private string GetTranslatedName(string originalName)
    {
        if (string.IsNullOrEmpty(originalName)) return originalName;
        // 如果字典里有，就返回 "翻译 (Original)" 格式；没有就返回原版
        return TranslationMap.TryGetValue(originalName, out string translated) ? $"{translated} ({originalName})" : originalName;
    }

    private void MenuEditAsset_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (SelectedItem != null && _workspace != null)
        {
            TreeViewItem item = (TreeViewItem)SelectedItem;
            if (item.Tag != null)
            {
                AssetDataTreeViewItem info = (AssetDataTreeViewItem)item.Tag;
                AssetInst? cont = _workspace.GetAssetInst(info.fromFile, 0, info.fromPathId);
                if (cont == null) return;
                RequestEditAsset(cont);
            }
        }
    }

    private void AssetDataTreeView_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (SelectedItem != null)
        {
            TreeViewItem item = (TreeViewItem)SelectedItem;
            item.IsExpanded = !item.IsExpanded;
        }
    }

    private void MenuVisitAsset_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (SelectedItem != null && _workspace != null)
        {
            TreeViewItem item = (TreeViewItem)SelectedItem;
            if (item != null && item.Tag != null)
            {
                AssetDataTreeViewItem info = (AssetDataTreeViewItem)item.Tag;
                AssetInst? cont = _workspace.GetAssetInst(info.fromFile, 0, info.fromPathId);
                if (cont == null) return;
                RequestVisitAsset(cont);
            }
        }
    }

    private void MenuExpandSel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (SelectedItem != null)
        {
            ExpandAllChildren((TreeViewItem)SelectedItem);
        }
    }

    private void MenuCollapseSel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (SelectedItem != null)
        {
            CollapseAllChildren((TreeViewItem)SelectedItem);
        }
    }

    public bool HasInitialized()
    {
        return _workspace != null;
    }

    public void Init(Workspace workspace)
    {
        _workspace = workspace;
        Reset();
    }

    public void Reset()
    {
        ListItems = new AvaloniaList<object>();
        ItemsSource = ListItems;
    }

    public void LoadComponent(AssetInst asset)
    {
        if (_workspace == null)
            return;

        AssetTypeValueField? baseField = _workspace.GetBaseField(asset);

        if (baseField == null)
        {
            // 反序列化失败的错误提示保持不变
            TreeViewItem errorItem0 = CreateTreeItem("Asset failed to deserialize. A few possibilities:");
            TreeViewItem errorItem1 = CreateTreeItem("The game's version is too new for this version of UABEA");
            TreeViewItem errorItem1I = CreateTreeItem("Try updating UABEA to see if it fixes the problem.");
            TreeViewItem errorItem2 = CreateTreeItem("The asset was a MonoBehaviour that didn't read correctly");
            TreeViewItem errorItem2I = CreateTreeItem("Try disabling Cpp2IL and dumping dlls manually into a (new) folder called Managed.");
            TreeViewItem errorItem3 = CreateTreeItem("The game uses a custom engine");
            TreeViewItem errorItem3I = CreateTreeItem("I can't help with custom/encrypted engines. You're on your own.");
            errorItem0.ItemsSource = new List<TreeViewItem>() { errorItem1, errorItem2, errorItem3 };
            errorItem1.ItemsSource = new List<TreeViewItem>() { errorItem1I };
            errorItem2.ItemsSource = new List<TreeViewItem>() { errorItem2I };
            errorItem3.ItemsSource = new List<TreeViewItem>() { errorItem3I };
            ListItems.Add(errorItem0);
            return;
        }

        // [修改] 根节点显示：这里通常是类型名，我们暂不翻译类型名，只保留原有逻辑
        string baseItemString = $"{baseField.TypeName} {baseField.FieldName}";

        if (asset.Type == AssetClassID.MonoBehaviour || asset.TypeId < 0)
        {
            string monoName;
            lock (asset.FileInstance.LockReader)
            {
                monoName = _workspace.Namer.GetMonoBehaviourNameFast(asset);
            }
            if (monoName != null)
            {
                baseItemString += $" ({monoName})";
            }
        }

        TreeViewItem baseItem = CreateTreeItem(baseItemString);
        TreeViewItem arrayIndexTreeItem = CreateTreeItem("Loading..."); // 加载占位符

        baseItem.ItemsSource = new AvaloniaList<TreeViewItem>() { arrayIndexTreeItem };
        ListItems.Add(baseItem);
        baseItem.IsExpanded = true;
        SetTreeItemEvents(baseItem, asset.FileInstance, asset.PathId, baseField);
    }

    public void ExpandAllChildren(TreeViewItem treeItem)
    {
        string? text = null;
        if (treeItem.Header is string header)
            text = header;
        else if (treeItem.Header is TextBlock rtb)
            text = rtb.Text;

        treeItem.IsExpanded = true;

        if (text != "[view asset]")
        {
            var treeItems = treeItem.Items.Cast<TreeViewItem?>();
            foreach (TreeViewItem? treeItemChild in treeItems)
            {
                if (treeItemChild != null)
                    ExpandAllChildren(treeItemChild);
            }
        }
    }

    public void CollapseAllChildren(TreeViewItem treeItem)
    {
        string? text = null;
        if (treeItem.Header is string header)
            text = header;
        else if (treeItem.Header is TextBlock rtb)
            text = rtb.Text;

        if (text != "[view asset]")
        {
            var treeItems = treeItem.Items.Cast<TreeViewItem?>();
            foreach (TreeViewItem? treeItemChild in treeItems)
            {
                if (treeItemChild != null)
                    CollapseAllChildren(treeItemChild);
            }
            treeItem.IsExpanded = false;
        }
    }

    private TreeViewItem CreateTreeItem(string text)
    {
        return new TreeViewItem() { Header = text };
    }

    private TreeViewItem CreateColorTreeItem(string typeName, string fieldName)
    {
        TextBlock tb = new TextBlock();
        Span span1 = new Span()
        {
            [!ForegroundProperty] = new DynamicResourceExtension("TypeTextType")
        };
        Bold bold1 = new Bold();
        bold1.Inlines.Add(typeName);
        span1.Inlines.Add(bold1);
        tb.Inlines!.Add(span1);

        Bold bold2 = new Bold();
        // 这里 fieldName 是已经翻译过的
        bold2.Inlines.Add($" {fieldName}");
        tb.Inlines.Add(bold2);

        return new TreeViewItem()
        {
            Header = tb
        };
    }

    private TreeViewItem CreateColorTreeItem(string typeName, string fieldName, string middle, string value, string comment = "")
    {
        bool isString = value.StartsWith("\"");
        TextBlock tb = new TextBlock();
        bool primitiveType = AssetTypeValueField.GetValueTypeByTypeName(typeName) != AssetValueType.None;

        Span span1 = new Span()
        {
            [!ForegroundProperty] = primitiveType
                ? new DynamicResourceExtension("TypeTextPrimitive")
                : new DynamicResourceExtension("TypeTextType")
        };
        Bold bold1 = new Bold();
        bold1.Inlines.Add(typeName);
        span1.Inlines.Add(bold1);
        tb.Inlines!.Add(span1);

        Bold bold2 = new Bold();
        // 这里 fieldName 是已经翻译过的
        bold2.Inlines.Add($" {fieldName}");
        tb.Inlines.Add(bold2);

        tb.Inlines.Add(middle);

        if (value != string.Empty)
        {
            Span span2 = new Span()
            {
                [!ForegroundProperty] = isString
                    ? new DynamicResourceExtension("TypeTextString")
                    : new DynamicResourceExtension("TypeTextValue")
            };
            Bold bold3 = new Bold();
            bold3.Inlines.Add(value);
            span2.Inlines.Add(bold3);
            tb.Inlines.Add(span2);
        }

        if (comment != string.Empty)
        {
            tb.Inlines.Add(comment);
        }

        return new TreeViewItem() { Header = tb };
    }

    private void SetTreeItemEvents(TreeViewItem item, AssetsFileInstance fromFile, long fromPathId, AssetTypeValueField field)
    {
        item.Tag = new AssetDataTreeViewItem(fromFile, fromPathId);
        var expandObs = item.GetObservable(TreeViewItem.IsExpandedProperty);

        expandObs.Subscribe(new SimpleObserver<bool>(isExpanded =>
        {
            AssetDataTreeViewItem itemInfo = (AssetDataTreeViewItem)item.Tag;
            if (isExpanded && !itemInfo.loaded)
            {
                itemInfo.loaded = true;
                TreeLoad(fromFile, field, fromPathId, item);
            }
        }));
    }

    private void SetPPtrEvents(TreeViewItem item, AssetsFileInstance fromFile, long fromPathId, AssetInst? asset)
    {
        item.Tag = new AssetDataTreeViewItem(fromFile, fromPathId);
        var expandObs = item.GetObservable(TreeViewItem.IsExpandedProperty);

        expandObs.Subscribe(new SimpleObserver<bool>(isExpanded =>
        {
            AssetDataTreeViewItem itemInfo = (AssetDataTreeViewItem)item.Tag;
            if (isExpanded && !itemInfo.loaded)
            {
                itemInfo.loaded = true;

                if (asset == null)
                {
                    item.ItemsSource = new AvaloniaList<TreeViewItem>() { CreateTreeItem("[null asset]") };
                    return;
                }

                AssetTypeValueField? baseField = _workspace!.GetBaseField(asset);
                if (baseField == null)
                {
                    item.ItemsSource = new AvaloniaList<TreeViewItem>() { CreateTreeItem("[failed to load]") };
                    return;
                }

                // 这里也可以考虑翻译，但通常根节点是类型名，暂且不动
                TreeViewItem baseItem = CreateTreeItem($"{baseField.TypeName} {baseField.FieldName}");
                TreeViewItem arrayIndexTreeItem = CreateTreeItem("Loading...");
                baseItem.ItemsSource = new AvaloniaList<TreeViewItem>() { arrayIndexTreeItem };
                item.ItemsSource = new AvaloniaList<TreeViewItem>() { baseItem };
                SetTreeItemEvents(baseItem, asset.FileInstance, fromPathId, baseField);
            }
        }));
    }

    private void TreeLoad(AssetsFileInstance fromFile, AssetTypeValueField assetField, long fromPathId, TreeViewItem treeItem)
    {
        List<AssetTypeValueField> children;

        if (assetField.Value != null && assetField.Value.ValueType == AssetValueType.ManagedReferencesRegistry)
            children = assetField.AsManagedReferencesRegistry.references.Select(r => r.data).ToList();
        else
            children = assetField.Children;

        if (assetField.Children.Count == 0)
            return;

        int arrayIdx = 0;
        AvaloniaList<TreeViewItem> items = new AvaloniaList<TreeViewItem>(assetField.Children.Count + 1);

        AssetTypeTemplateField assetFieldTemplate = assetField.TemplateField;
        bool isArray = assetFieldTemplate.IsArray;

        // --- 定位到大約 510 行 ---
        if (isArray)
        {
            int size = assetField.AsArray.size;
            AssetTypeTemplateField sizeTemplate = assetFieldTemplate.Children[0];

            // [修改]：根據開關決定是否查表，關閉時強制用原始名 (sizeTemplate.Name)
            string sizeName = _isTranslationEnabled ? GetTranslatedName(sizeTemplate.Name) : sizeTemplate.Name;

            TreeViewItem arrayIndexTreeItem = CreateColorTreeItem(sizeTemplate.Type, sizeName, " = ", size.ToString());
            items.Add(arrayIndexTreeItem);
        }

        foreach (AssetTypeValueField childField in assetField)
        {
            if (childField == null) return;
            string middle = "";
            string value = "";
            string comment = "";

            // [修改逻辑] 核心点：根据开关状态决定初始显示的名称
            string displayName;
            if (_isTranslationEnabled)
            {
                // 开启时，尝试查本地字典
                displayName = GetTranslatedName(childField.FieldName);
            }
            else
            {
                // 关闭时，强制使用原始英文名，这样刷新后中文就会消失
                displayName = childField.FieldName;
            }

            if (childField.Value != null)
            {
                AssetValueType valueType = childField.Value.ValueType;
                if (valueType == AssetValueType.String)
                {
                    middle = " = ";
                    value = EscapeAndQuoteString(childField.AsString);
                }
                else if (1 <= (int)valueType && (int)valueType <= 11)
                {
                    middle = " = ";
                    value = childField.AsString;
                }
                if (valueType == AssetValueType.Array)
                {
                    middle = $" (size {childField.Children.Count})";
                }
                else if (valueType == AssetValueType.ByteArray)
                {
                    byte[] bytes = childField.AsByteArray;
                    int byteArraySize = childField.AsByteArray.Length;
                    middle = $" (size {byteArraySize}) = ";

                    const int MAX_PREVIEW_BYTES = 20;
                    int previewSize = Math.Min(byteArraySize, MAX_PREVIEW_BYTES);

                    StringBuilder valueBuilder = new StringBuilder();
                    for (int i = 0; i < previewSize; i++)
                    {
                        if (i == 0)
                        {
                            valueBuilder.Append(bytes[i].ToString("X2"));
                        }
                        else
                        {
                            valueBuilder.Append(" " + bytes[i].ToString("X2"));
                        }
                    }

                    if (byteArraySize > MAX_PREVIEW_BYTES)
                    {
                        valueBuilder.Append(" ...");
                    }

                    value = valueBuilder.ToString();
                }

                if (valueType == AssetValueType.Int32 && childField.TemplateField.Name == "m_FileID")
                {
                    List<AssetsFileExternal> externals = fromFile.file.Metadata.Externals;
                    int fileId = childField.AsInt;
                    if (fileId == 0)
                    {
                        if (fromFile.parentBundle != null)
                            comment = $" ({fromFile.parentBundle.name}/{fromFile.name})";
                        else
                            comment = $" ({fromFile.name})";
                    }
                    else
                    {
                        int externalIdx = fileId - 1;
                        if (0 <= externalIdx && externalIdx < externals.Count)
                        {
                            BundleFileInstance? parentBun = null;
                            AssetsFileExternal external = externals[externalIdx];
                            string pathName = external.PathName;
                            string externalName;
                            if (pathName == string.Empty && external.Type != AssetsFileExternalType.Normal)
                            {
                                externalName = $"guid: {external.Guid}";
                            }
                            else
                            {
                                string externalPathName = externals[externalIdx].PathName;
                                externalName = Path.GetFileName(externalPathName);

                                string externalFileKey = AssetsManager.GetFileLookupKey(externalPathName);
                                if (_workspace!.Manager.FileLookup.TryGetValue(externalFileKey, out AssetsFileInstance? depInst))
                                {
                                    if (depInst.parentBundle != null)
                                    {
                                        parentBun = depInst.parentBundle;
                                    }
                                }
                            }

                            if (parentBun != null)
                                comment = $" ({parentBun.name}/{externalName})";
                            else
                                comment = $" ({externalName})";
                        }
                    }
                }
            }

            bool hasChildren = childField.Children.Count > 0;

            if (isArray)
            {
                TreeViewItem arrayIndexTreeItem = CreateTreeItem($"{arrayIdx}");
                items.Add(arrayIndexTreeItem);

                // [修改] 数组内部节点同样使用受开关控制的 displayName
                TreeViewItem childTreeItem = CreateColorTreeItem(childField.TypeName, displayName, middle, value);
                arrayIndexTreeItem.ItemsSource = new AvaloniaList<TreeViewItem>() { childTreeItem };

                if (hasChildren)
                {
                    TreeViewItem dummyItem = CreateTreeItem("Loading...");
                    childTreeItem.ItemsSource = new AvaloniaList<TreeViewItem>() { dummyItem };
                    SetTreeItemEvents(childTreeItem, fromFile, fromPathId, childField);
                }

                arrayIdx++;
            }
            else
            {
                // 生成初始 UI 节点
                TreeViewItem childTreeItem = CreateColorTreeItem(childField.TypeName, displayName, middle, value, comment);

                // ======================== [异步翻译逻辑植入] ========================
                // 只有在：1.开关开启 2.字典未命中 3.非数组索引 时，才发起异步翻译
                if (_isTranslationEnabled && displayName == childField.FieldName && !isArray)
                {
                    _ = Task.Run(async () =>
                    {
                        string cloudTrans = await TranslationService.TranslateAsync(childField.FieldName);

                        if (cloudTrans != childField.FieldName)
                        {
                            Dispatcher.UIThread.Post(() => {
                                if (childTreeItem.Header is TextBlock tb)
                                {
                                    // 对应 CreateColorTreeItem 结构：Inlines[1] 是变量名 Bold
                                    if (tb.Inlines != null && tb.Inlines.Count >= 2 && tb.Inlines[1] is Bold b)
                                    {
                                        b.Inlines.Clear();
                                        b.Inlines.Add($" {cloudTrans} ({childField.FieldName})");
                                    }
                                }
                            });
                        }
                    });
                }
                // =====================================================================

                items.Add(childTreeItem);

                if (childField.Value != null && childField.Value.ValueType == AssetValueType.ManagedReferencesRegistry)
                {
                    TreeLoadManagedRegistry(childTreeItem, childField, fromFile, fromPathId);
                }

                if (hasChildren)
                {
                    TreeViewItem dummyItem = CreateTreeItem("Loading...");
                    childTreeItem.ItemsSource = new AvaloniaList<TreeViewItem> { dummyItem };
                    SetTreeItemEvents(childTreeItem, fromFile, fromPathId, childField);
                }
            }
        }

        string templateFieldType = assetField.TypeName;

        if (templateFieldType.StartsWith("PPtr<") && templateFieldType.EndsWith(">"))
        {
            var fileIdField = assetField["m_FileID"];
            var pathIdField = assetField["m_PathID"];
            bool pptrValid = !fileIdField.IsDummy && !pathIdField.IsDummy;

            if (pptrValid)
            {
                int fileId = fileIdField.AsInt;
                long pathId = pathIdField.AsLong;

                AssetInst? cont = _workspace!.GetAssetInst(fromFile, fileId, pathId);

                TreeViewItem childTreeItem = CreateTreeItem("[view asset]");
                items.Add(childTreeItem);

                TreeViewItem dummyItem = CreateTreeItem("Loading...");
                childTreeItem.ItemsSource = new AvaloniaList<TreeViewItem>() { dummyItem };
                SetPPtrEvents(childTreeItem, fromFile, pathId, cont);
            }
        }

        treeItem.ItemsSource = items;
    }

    private void TreeLoadManagedRegistry(TreeViewItem childTreeItem, AssetTypeValueField childField, AssetsFileInstance fromFile, long fromPathId)
    {
        ManagedReferencesRegistry registry = childField.AsManagedReferencesRegistry;

        if (registry.version == 1 || registry.version == 2)
        {
            TreeViewItem versionItem = CreateColorTreeItem("int", "version", " = ", registry.version.ToString());
            TreeViewItem refIdsItem = CreateColorTreeItem("vector", "RefIds");
            TreeViewItem refIdsArrayItem = CreateColorTreeItem("Array", "Array", $" (size {registry.references.Count})", "");

            AvaloniaList<TreeViewItem> refObjItems = new AvaloniaList<TreeViewItem>();

            foreach (AssetTypeReferencedObject refObj in registry.references)
            {
                AssetTypeReference typeRef = refObj.type;

                TreeViewItem refObjItem = CreateColorTreeItem("ReferencedObject", "data");

                TreeViewItem managedTypeItem = CreateColorTreeItem("ReferencedManagedType", "type");
                managedTypeItem.ItemsSource = new AvaloniaList<TreeViewItem>
                {
                    CreateColorTreeItem("string", "class", " = ", EscapeAndQuoteString(typeRef.ClassName)),
                    CreateColorTreeItem("string", "ns", " = ", EscapeAndQuoteString(typeRef.Namespace)),
                    CreateColorTreeItem("string", "asm", " = ", EscapeAndQuoteString(typeRef.AsmName))
                };

                TreeViewItem refObjectItem = CreateColorTreeItem("ReferencedObjectData", "data");

                TreeViewItem dummyItem = CreateTreeItem("Loading...");
                refObjectItem.ItemsSource = new AvaloniaList<TreeViewItem> { dummyItem };
                SetTreeItemEvents(refObjectItem, fromFile, fromPathId, refObj.data);

                if (registry.version == 1)
                {
                    refObjItem.ItemsSource = new AvaloniaList<TreeViewItem>
                    {
                        managedTypeItem,
                        refObjectItem
                    };
                }
                else if (registry.version == 2)
                {
                    refObjItem.ItemsSource = new AvaloniaList<TreeViewItem>
                    {
                        CreateColorTreeItem("SInt64", "rid", " = ", refObj.rid.ToString()),
                        managedTypeItem,
                        refObjectItem
                    };
                }

                refObjItems.Add(refObjItem);
            }

            refIdsArrayItem.ItemsSource = refObjItems;

            refIdsItem.ItemsSource = new AvaloniaList<TreeViewItem>
            {
                refIdsArrayItem
            };

            childTreeItem.ItemsSource = new AvaloniaList<TreeViewItem>
            {
                versionItem,
                refIdsItem
            };
        }
        else
        {
            TreeViewItem errorTreeItem = CreateTreeItem($"[unsupported registry version {registry.version}]");
            childTreeItem.ItemsSource = new AvaloniaList<TreeViewItem> { errorTreeItem };
        }
    }

    private string EscapeAndQuoteString(string str)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("\"");
        if (str.Length > 1000)
        {
            sb.Append(str[..1000]);
            sb.Append("...");
        }
        else
        {
            sb.Append(str);
        }
        sb.Append("\"");
        return sb.ToString();
    }

    private static void RequestEditAsset(AssetInst asset)
    {
        if (asset != null)
        {
            WeakReferenceMessenger.Default.Send(new RequestEditAssetMessage(asset));
        }
    }

    private static void RequestVisitAsset(AssetInst asset)
    {
        if (asset != null)
        {
            WeakReferenceMessenger.Default.Send(new RequestVisitAssetMessage(asset));
        }
    }

    private Workspace? _workspace = null;
    private ObservableCollection<AssetInst>? _activeAssets = null;

    public static readonly DirectProperty<AssetDataTreeView, Workspace?> WorkspaceProperty =
        AvaloniaProperty.RegisterDirect<AssetDataTreeView, Workspace?>(nameof(Workspace), o => o.Workspace, (o, v) => o.Workspace = v);

    public static readonly DirectProperty<AssetDataTreeView, ObservableCollection<AssetInst>?> ActiveAssetsProperty =
        AvaloniaProperty.RegisterDirect<AssetDataTreeView, ObservableCollection<AssetInst>?>(nameof(ActiveAssets), o => o.ActiveAssets, (o, v) => o.ActiveAssets = v);

    public Workspace? Workspace
    {
        get => _workspace;
        set => SetAndRaise(WorkspaceProperty, ref _workspace, value);
    }

    public ObservableCollection<AssetInst>? ActiveAssets
    {
        get => _activeAssets;
        set => SetAndRaise(ActiveAssetsProperty, ref _activeAssets, value);
    }

    private void LoadAssets(ObservableCollection<AssetInst> activeAssets)
    {
        Reset();
        foreach (var item in activeAssets)
        {
            LoadComponent(item);
        }
    }
}

public class AssetDataTreeViewItem
{
    public bool loaded;
    public AssetsFileInstance fromFile;
    public long fromPathId;

    public AssetDataTreeViewItem(AssetsFileInstance fromFile, long fromPathId)
    {
        this.loaded = false;
        this.fromFile = fromFile;
        this.fromPathId = fromPathId;
    }
}