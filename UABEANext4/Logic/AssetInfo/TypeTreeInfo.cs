using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于访问 Unity 资产解析相关类型（原名：AssetsTools.NET）

using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展功能（原名：AssetsTools.NET.Extra）

using Avalonia.Controls.Documents;
// 引用 Avalonia 的文档控件命名空间，提供 InlineCollection、Span、Bold 等富文本元素（原名：Avalonia.Controls.Documents）

using Avalonia.Markup.Xaml.MarkupExtensions;
// 引用 Avalonia 的 XAML 标记扩展，提供 DynamicResourceExtension 等（原名：Avalonia.Markup.Xaml.MarkupExtensions）

using AvaloniaEdit.Utils;
// 引用 AvaloniaEdit 的工具类命名空间（原名：AvaloniaEdit.Utils）

using CommunityToolkit.Mvvm.ComponentModel;
// 引用 CommunityToolkit MVVM 的组件模型命名空间，提供 ObservableObject 与属性生成特性（原名：CommunityToolkit.Mvvm.ComponentModel）

using System.Collections.Generic;
// 引用泛型集合命名空间，提供 List<T>、Dictionary<TKey,TValue> 等集合类型（原名：System.Collections.Generic）

using System.Collections.ObjectModel;
// 引用可观察集合命名空间，提供 ObservableCollection<T>（原名：System.Collections.ObjectModel）

using UABEANext4.AssetWorkspace;
// 引用项目内的资产工作区命名空间，提供 Workspace、AssetInst 等类型（原名：UABEANext4.AssetWorkspace）

namespace UABEANext4.Logic.AssetInfo;
// 定义命名空间 UABEANext4.Logic.AssetInfo，用于组织与资产类型信息显示相关的类型（原名：UABEANext4.Logic.AssetInfo）

public partial class TypeTreeInfo : ObservableObject
// 定义部分类 TypeTreeInfo，继承自 ObservableObject，用于在 UI 中展示 TypeTree（类型树）信息（原名：TypeTreeInfo / ObservableObject）
{
    // 类体开始（TypeTreeInfo）

    public ObservableCollection<TypeTreeUINode> TypeTreeNodes { get; set; } = [];
    // 公共属性 TypeTreeNodes（ObservableCollection<TypeTreeUINode>）：用于绑定到 UI 的树形节点集合，初始化为空（原名：TypeTreeNodes / TypeTreeUINode）

    public List<TypeTreeTypeInfo> TypeTreeTypeInfos { get; set; } = [];
    // 公共属性 TypeTreeTypeInfos（List<TypeTreeTypeInfo>）：保存文件中所有的 TypeTreeTypeInfo 列表，初始化为空（原名：TypeTreeTypeInfos / TypeTreeTypeInfo）

    private TypeTreeTypeInfo? _selectedType;
    // 私有字段 _selectedType（TypeTreeTypeInfo?）：保存当前选中的类型信息（可空）（原名：_selectedType）

    public TypeTreeTypeInfo? SelectedType
    // 公共属性 SelectedType（TypeTreeTypeInfo?）：对外暴露的选中类型属性，设置时触发 SelectedTypeChanged（原名：SelectedType）
    {
        get => _selectedType;
        // getter：返回私有字段 _selectedType（原名：get / _selectedType）

        set
        {
            _selectedType = value;
            // setter：将传入值赋给私有字段 _selectedType（原名：set / _selectedType）

            SelectedTypeChanged(value);
            // setter：调用 SelectedTypeChanged 方法处理选中类型变化（原名：SelectedTypeChanged）
        }
    }

    private TypeTreeUINode? _selectedNode;
    // 私有字段 _selectedNode（TypeTreeUINode?）：保存当前选中的树节点（可空）（原名：_selectedNode）

    public TypeTreeUINode? SelectedNode
    // 公共属性 SelectedNode（TypeTreeUINode?）：对外暴露的选中节点属性，设置时触发 SelectedNodeChanged（原名：SelectedNode）
    {
        get => _selectedNode;
        // getter：返回私有字段 _selectedNode（原名：get / _selectedNode）

        set
        {
            _selectedNode = value;
            // setter：将传入值赋给私有字段 _selectedNode（原名：set / _selectedNode）

            SelectedNodeChanged(value);
            // setter：调用 SelectedNodeChanged 方法处理选中节点变化（原名：SelectedNodeChanged）
        }
    }

    [ObservableProperty] public string _selectedTypeName = "";
    // 使用 ObservableProperty 生成公开属性 SelectedTypeName（由字段 _selectedTypeName 支持），用于显示选中类型的名称，初始为空（原名：SelectedTypeName / _selectedTypeName）

    [ObservableProperty] public string _selectedTypeId = "";
    // 使用 ObservableProperty 生成公开属性 SelectedTypeId（由字段 _selectedTypeId 支持），用于显示选中类型的 ID，初始为空（原名：SelectedTypeId / _selectedTypeId）

    [ObservableProperty] public string _selectedScriptId = "";
    // 使用 ObservableProperty 生成公开属性 SelectedScriptId（由字段 _selectedScriptId 支持），用于显示脚本 ID 或名称，初始为空（原名：SelectedScriptId / _selectedScriptId）

    [ObservableProperty] public string _selectedTypeHash = "";
    // 使用 ObservableProperty 生成公开属性 SelectedTypeHash（由字段 _selectedTypeHash 支持），用于显示类型哈希，初始为空（原名：SelectedTypeHash / _selectedTypeHash）

    [ObservableProperty] public string _selectedMonoHash = "";
    // 使用 ObservableProperty 生成公开属性 SelectedMonoHash（由字段 _selectedMonoHash 支持），用于显示 Mono/脚本哈希，初始为空（原名：SelectedMonoHash / _selectedMonoHash）

    [ObservableProperty] public string _selectedAligned = "";
    // 使用 ObservableProperty 生成公开属性 SelectedAligned（由字段 _selectedAligned 支持），用于显示对齐标志，初始为空（原名：SelectedAligned / _selectedAligned）

    [ObservableProperty] public string _selectedTypeFlags = "";
    // 使用 ObservableProperty 生成公开属性 SelectedTypeFlags（由字段 _selectedTypeFlags 支持），用于显示类型标志，初始为空（原名：SelectedTypeFlags / _selectedTypeFlags）

    [ObservableProperty] public string _selectedMetaFlags = "";
    // 使用 ObservableProperty 生成公开属性 SelectedMetaFlags（由字段 _selectedMetaFlags 支持），用于显示元标志（MetaFlags），初始为空（原名：SelectedMetaFlags / _selectedMetaFlags）

    public TypeTreeInfo(Workspace workspace, AssetsFileInstance fileInst)
    // 构造函数 TypeTreeInfo(Workspace workspace, AssetsFileInstance fileInst)：根据传入的 Workspace 与文件实例构建 TypeTreeTypeInfos 列表（原名：TypeTreeInfo / Workspace / AssetsFileInstance）
    {
        var ttTypes = fileInst.file.Metadata.TypeTreeTypes;
        // 从文件实例（fileInst）中获取 Metadata 下的 TypeTreeTypes 列表（原名：ttTypes / fileInst.file.Metadata.TypeTreeTypes）

        foreach (var type in ttTypes)
        {
            var ttTypeInfo = new TypeTreeTypeInfo(workspace.Manager, fileInst, type);
            // 为每个 TypeTreeType 创建一个 TypeTreeTypeInfo 实例（传入 manager、fileInst 与 type）（原名：TypeTreeTypeInfo）

            TypeTreeTypeInfos.Add(ttTypeInfo);
            // 将创建的 TypeTreeTypeInfo 添加到 TypeTreeTypeInfos 列表中（原名：TypeTreeTypeInfos）
        }
    }

    private TypeTreeInfo()
    // 私有无参构造函数（TypeTreeInfo），用于创建空实例（原名：TypeTreeInfo）
    {
    }

    private void SelectedTypeChanged(TypeTreeTypeInfo? typeInfo)
    // 私有方法 SelectedTypeChanged：当选中类型改变时更新显示字段与节点（原名：SelectedTypeChanged）
    {
        TypeTreeNodes.Clear();
        // 清空当前显示的 TypeTreeNodes（原名：TypeTreeNodes）

        if (typeInfo != null)
        {
            SelectedTypeName = typeInfo.Name;
            // 将 SelectedTypeName 设为选中类型的名称（原名：typeInfo.Name）

            SelectedTypeId = $"{typeInfo.TypeId} (0x{typeInfo.TypeId:x})";
            // 将 SelectedTypeId 设为带十六进制的类型 ID 字符串（原名：TypeId）

            SelectedScriptId = typeInfo.ScriptName != string.Empty
                ? $"{typeInfo.ScriptId} ({typeInfo.ScriptName})"
                : typeInfo.ScriptId.ToString();
            // 如果 ScriptName 非空则显示 "ScriptId (ScriptName)"，否则仅显示 ScriptId（原名：ScriptId / ScriptName）

            SelectedTypeHash = typeInfo.TypeHash.ToString();
            // 将 SelectedTypeHash 设为类型哈希字符串（原名：TypeHash）

            SelectedMonoHash = typeInfo.MonoHash.ToString();
            // 将 SelectedMonoHash 设为 Mono/脚本哈希字符串（原名：MonoHash）

            AddTypeTreeNodes(typeInfo);
            // 调用 AddTypeTreeNodes 将该类型的节点添加到 TypeTreeNodes（原名：AddTypeTreeNodes）
        }
        else
        {
            SelectedTypeName = string.Empty;
            // 如果没有选中类型，则将所有显示字段清空（SelectedTypeName）

            SelectedTypeId = string.Empty;
            // 清空 SelectedTypeId

            SelectedScriptId = string.Empty;
            // 清空 SelectedScriptId

            SelectedTypeHash = string.Empty;
            // 清空 SelectedTypeHash

            SelectedMonoHash = string.Empty;
            // 清空 SelectedMonoHash

            SelectedAligned = string.Empty;
            // 清空 SelectedAligned

            SelectedTypeFlags = string.Empty;
            // 清空 SelectedTypeFlags

            SelectedMetaFlags = string.Empty;
            // 清空 SelectedMetaFlags
        }
    }

    private void SelectedNodeChanged(TypeTreeUINode? uiNode)
    // 私有方法 SelectedNodeChanged：当选中树节点改变时更新相关显示字段（原名：SelectedNodeChanged）
    {
        if (uiNode != null)
        {
            var node = uiNode.Node;
            // 获取底层的 TypeTreeNode（原名：node / uiNode.Node）

            SelectedAligned = (node.MetaFlags & 0x4000) != 0 ? "true" : "false";
            // 根据节点的 MetaFlags 的 0x4000 位判断是否对齐，并将结果以字符串形式赋给 SelectedAligned（原名：MetaFlags / SelectedAligned）

            SelectedTypeFlags = node.TypeFlags.ToString();
            // 将节点的 TypeFlags 转为字符串并赋给 SelectedTypeFlags（原名：TypeFlags）

            SelectedMetaFlags = node.MetaFlags.ToString("X4");
            // 将节点的 MetaFlags 以 4 位十六进制格式转为字符串并赋给 SelectedMetaFlags（原名：MetaFlags）
        }
        else
        {
            SelectedAligned = string.Empty;
            // 如果没有选中节点，则清空 SelectedAligned

            SelectedTypeFlags = string.Empty;
            // 清空 SelectedTypeFlags

            SelectedMetaFlags = string.Empty;
            // 清空 SelectedMetaFlags
        }
    }

    private static InlineCollection GenerateInlines(TypeTreeType type, TypeTreeNode node)
    // 私有静态方法 GenerateInlines：为给定的 TypeTreeType 与 TypeTreeNode 生成用于 UI 显示的 InlineCollection（富文本片段集合）（原名：GenerateInlines）
    {
        var inlines = new InlineCollection();
        // 创建一个新的 InlineCollection（inlines）用于收集要显示的富文本元素（原名：InlineCollection）

        var typeName = node.GetTypeString(type.StringBufferBytes);
        // 从节点读取类型名字符串（使用 type.StringBufferBytes 作为字符串缓冲区）（原名：GetTypeString / StringBufferBytes）

        var fieldName = node.GetNameString(type.StringBufferBytes);
        // 从节点读取字段名字符串（原名：GetNameString）

        var isValueType = AssetTypeValueField.GetValueTypeByTypeName(typeName) != AssetValueType.None;
        // 判断该类型名是否对应一个“值类型”（非复合/引用类型），通过 AssetTypeValueField.GetValueTypeByTypeName 检查（原名：AssetTypeValueField / AssetValueType）

        var span1 = new Span();
        // 创建第一个 Span（span1），用于包含类型名并设置其前景色资源绑定（原名：Span）

        span1.Bind(TextElement.ForegroundProperty,
            isValueType
                ? new DynamicResourceExtension("TypeTextPrimitive")
                : new DynamicResourceExtension("TypeTextType")
        );
        // 将 span1 的 Foreground（前景色）绑定到动态资源：如果是值类型使用 "TypeTextPrimitive"，否则使用 "TypeTextType"（原名：Bind / DynamicResourceExtension）

        {
            var bold = new Bold();
            // 在局部作用域内创建一个 Bold 元素，用于将类型名加粗显示（原名：Bold）

            {
                bold.Inlines.Add(typeName);
                // 将类型名（typeName）作为文本添加到 bold 的 Inlines 集合中
            }
            span1.Inlines.Add(bold);
            // 将 bold（包含类型名）添加到 span1 的 Inlines 中
        }
        span1.Inlines.Add(" ");
        // 在类型名后添加一个空格，分隔类型名与字段名

        inlines.Add(span1);
        // 将构建好的 span1 添加到最终的 inlines 集合中

        var span2 = new Span();
        // 创建第二个 Span（span2），用于包含字段名并加粗显示

        {
            var bold = new Bold();
            // 创建 Bold 元素用于字段名加粗显示

            {
                bold.Inlines.Add(fieldName);
                // 将字段名（fieldName）添加到 bold 的 Inlines 中
            }
            span2.Inlines.Add(bold);
            // 将 bold（包含字段名）添加到 span2 的 Inlines 中
        }
        inlines.Add(span2);
        // 将 span2 添加到最终的 inlines 集合中

        return inlines;
        // 返回构建好的 InlineCollection，供 UI 使用（例如在树节点中显示）
    }

    private void AddTypeTreeNodes(TypeTreeTypeInfo? typeInfo)
    // 私有方法 AddTypeTreeNodes：将指定类型的扁平节点列表转换为树形节点并添加到 TypeTreeNodes（原名：AddTypeTreeNodes）
    {
        if (typeInfo is null)
        {
            return;
        }
        // 如果传入的 typeInfo 为 null，则直接返回（不做任何操作）

        var typeTreeType = typeInfo.TtType;
        // 获取底层的 TypeTreeType（原名：TtType）

        var flatList = typeTreeType.Nodes;
        // 获取该类型的扁平节点列表（flatList）（原名：Nodes）

        if (flatList == null)
        {
            return;
        }
        // 如果 flatList 为 null，则返回（防御性检查）

        var lookup = new Dictionary<int, TypeTreeUINode>();
        // 创建字典 lookup，用于按层级索引（Level）临时保存最近的节点，以便将子节点附加到父节点（原名：lookup）

        var rootNodes = new List<TypeTreeUINode>();
        // 创建临时列表 rootNodes，用于收集所有根节点（原名：rootNodes）

        foreach (var item in flatList)
        {
            var uiNode = new TypeTreeUINode
            {
                Node = item,
                Display = GenerateInlines(typeTreeType, item),
                Children = new List<TypeTreeUINode>()
            };
            // 为当前节点创建一个 TypeTreeUINode 并初始化其 Node、Display 与 Children（原名：TypeTreeUINode / GenerateInlines）

            if (item.Level == 0)
            {
                rootNodes.Add(uiNode);
            }
            else
            {
                if (lookup.TryGetValue(item.Level - 1, out var parentNode))
                {
                    parentNode.Children.Add(uiNode);
                }
                else
                {
                    // hopefully this doesn't happen
                }
            }
            // 如果当前节点的 Level 为 0，则它是根节点，加入 rootNodes；
            // 否则尝试在 lookup 中找到其父节点（Level - 1），并将当前 uiNode 添加为父节点的子节点；
            // 如果找不到父节点则忽略（保留英文原注释）

            lookup[item.Level] = uiNode;
            // 将当前 uiNode 存入 lookup，覆盖同一层级的最近节点，以便后续层级的节点能找到正确的父节点
        }

        TypeTreeNodes.AddRange(rootNodes);
        // 将构建好的根节点列表添加到绑定集合 TypeTreeNodes 中（原名：AddRange / TypeTreeNodes）
    }

    public static TypeTreeInfo Empty { get; } = new()
    // 公共静态只读属性 Empty：提供一个空的 TypeTreeInfo 实例作为默认值（原名：Empty）
    {
        SelectedTypeName = string.Empty,
        // 初始化 SelectedTypeName 为空字符串（原名：SelectedTypeName）

        SelectedTypeId = string.Empty,
        // 初始化 SelectedTypeId 为空字符串（原名：SelectedTypeId）

        SelectedScriptId = string.Empty,
        // 初始化 SelectedScriptId 为空字符串（原名：SelectedScriptId）

        SelectedTypeHash = string.Empty,
        // 初始化 SelectedTypeHash 为空字符串（原名：SelectedTypeHash）

        SelectedMonoHash = string.Empty,
        // 初始化 SelectedMonoHash 为空字符串（原名：SelectedMonoHash）

        SelectedAligned = string.Empty,
        // 初始化 SelectedAligned 为空字符串（原名：SelectedAligned）

        SelectedTypeFlags = string.Empty,
        // 初始化 SelectedTypeFlags 为空字符串（原名：SelectedTypeFlags）

        SelectedMetaFlags = string.Empty,
        // 初始化 SelectedMetaFlags 为空字符串（原名：SelectedMetaFlags）
    };
    // Empty 初始化结束

}
// 类体结束（TypeTreeInfo）
