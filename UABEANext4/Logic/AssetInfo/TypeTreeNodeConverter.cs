using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于访问 Unity 资产解析相关类型（保留英文原名：AssetsTools.NET）

using Avalonia.Controls.Documents;
// 引用 Avalonia 的文档控件命名空间，提供 InlineCollection、Span、Bold 等富文本元素（保留英文原名：Avalonia.Controls.Documents）

using Avalonia.Data;
// 引用 Avalonia 的数据绑定命名空间，提供 BindingNotification、BindingErrorType 等（保留英文原名：Avalonia.Data）

using Avalonia.Data.Converters;
// 引用 Avalonia 的数据转换器命名空间，包含 IValueConverter 接口（保留英文原名：Avalonia.Data.Converters）

using Avalonia.Markup.Xaml.MarkupExtensions;
// 引用 Avalonia 的 XAML 标记扩展命名空间，提供 DynamicResourceExtension 等（保留英文原名：Avalonia.Markup.Xaml.MarkupExtensions）

using System;
// 引用基础系统命名空间，提供常用类型（如 Exception、Type 等）（保留英文原名：System）

using System.Collections.Generic;
// 引用泛型集合命名空间，提供 List<T>、Dictionary<TKey,TValue> 等（保留英文原名：System.Collections.Generic）

using System.Collections.ObjectModel;
// 引用只读/可观察集合命名空间，提供 ObservableCollection<T>（保留英文原名：System.Collections.ObjectModel）

using System.Globalization;
// 引用文化信息命名空间，提供 CultureInfo 等（保留英文原名：System.Globalization）

namespace UABEANext4.Logic.AssetInfo;
// 定义命名空间 UABEANext4.Logic.AssetInfo，用于组织与资产类型信息显示相关的类型（保留英文原名：UABEANext4.Logic.AssetInfo）

public class FlatListToTreeConverter : IValueConverter
// 定义公共类 FlatListToTreeConverter，实现 IValueConverter（用于在数据绑定时将扁平类型树节点列表转换为树形 UI 数据）
{
    // 类体开始（FlatListToTreeConverter）

    private static InlineCollection GenerateInlines(TypeTreeType type, TypeTreeNode node)
    // 私有静态方法 GenerateInlines：为给定的 TypeTreeType 与 TypeTreeNode 生成用于 UI 显示的 InlineCollection（富文本片段集合）
    {
        // 方法体开始（GenerateInlines）

        var inlines = new InlineCollection();
        // 创建一个新的 InlineCollection（inlines）用于收集要显示的富文本元素

        var typeName = node.GetTypeString(type.StringBufferBytes);
        // 从节点（node）读取类型名字符串，使用 type.StringBufferBytes 作为字符串缓冲区（保留英文原名：GetTypeString / StringBufferBytes）

        var fieldName = node.GetNameString(type.StringBufferBytes);
        // 从节点读取字段名字符串（保留英文原名：GetNameString）

        var isValueType = AssetTypeValueField.GetValueTypeByTypeName(typeName) != AssetValueType.None;
        // 判断该类型名是否对应一个“值类型”（非复合/引用类型），通过 AssetTypeValueField.GetValueTypeByTypeName 检查（保留英文原名：AssetTypeValueField / AssetValueType）

        var span1 = new Span();
        // 创建第一个 Span（span1），用于包含类型名并设置其前景色资源绑定（保留英文原名：Span）

        span1.Bind(TextElement.ForegroundProperty,
            isValueType
                ? new DynamicResourceExtension("TypeTextValue")
                : new DynamicResourceExtension("TypeTextType")
        );
        // 将 span1 的 Foreground（前景色）绑定到动态资源：如果是值类型使用 "TypeTextValue"，否则使用 "TypeTextType"（保留英文原名：Bind / TextElement.ForegroundProperty / DynamicResourceExtension）

        {
            var bold = new Bold();
            // 在局部作用域内创建一个 Bold 元素，用于将类型名加粗显示（保留英文原名：Bold）

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
    // 方法体结束（GenerateInlines）

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    // 实现 IValueConverter.Convert：将绑定源（value）从扁平类型节点列表转换为树形的 ObservableCollection<TypeTreeUINode>
    {
        // 方法体开始（Convert）

        var rootNodes = new ObservableCollection<TypeTreeUINode>();
        // 创建结果集合 rootNodes（ObservableCollection），用于返回给绑定目标（通常是树形控件的 ItemsSource）

        if (value is null)
        {
            // if selected typetree type info is null, return an list
            return rootNodes;
        }
        // 如果传入的 value 为 null（例如未选择任何类型），直接返回空的 rootNodes（注释保留英文原注释）

        if (value is not TypeTreeTypeInfo typeTreeTypeInfo)
        {
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }
        // 如果 value 不是期望的 TypeTreeTypeInfo 类型，则返回一个 BindingNotification 表示绑定错误（保留英文原名：BindingNotification / BindingErrorType）

        var typeTreeType = typeTreeTypeInfo.TtType;
        // 从 TypeTreeTypeInfo 中取出底层的 TypeTreeType（保留英文原名：TtType）

        var flatList = typeTreeType.Nodes;
        // 获取该类型的扁平节点列表（flatList），每个节点包含 Level、名称、类型等信息（保留英文原名：Nodes）

        var lookup = new Dictionary<int, TypeTreeUINode>();
        // 创建一个字典 lookup，用于按层级索引（Level）临时保存最近的节点，以便将子节点附加到父节点

        foreach (var item in flatList)
        {
            // 遍历 flatList 中的每个节点（按顺序，通常是深度优先或预序）

            var uiNode = new TypeTreeUINode
            {
                Node = item,
                Display = GenerateInlines(typeTreeType, item),
                Children = new List<TypeTreeUINode>()
            };
            // 为当前节点创建一个 TypeTreeUINode：
            // - Node 保存原始 TypeTreeNode（item）
            // - Display 使用 GenerateInlines 生成富文本显示内容
            // - Children 初始化为空列表以便后续填充子节点

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
            // 如果当前节点的 Level 为 0，则它是根节点，直接加入 rootNodes；
            // 否则尝试在 lookup 中找到其父节点（Level - 1），并将当前 uiNode 添加为父节点的子节点；
            // 如果找不到父节点（理论上不应发生），则忽略（保留英文原注释）

            lookup[item.Level] = uiNode;
            // 将当前 uiNode 存入 lookup，覆盖同一层级的最近节点，以便后续层级的节点能找到正确的父节点
        }

        return rootNodes;
        // 返回构建好的树形节点集合，供绑定目标（例如 TreeView）使用
    }
    // 方法体结束（Convert）

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    // 实现 IValueConverter.ConvertBack：此转换器不支持反向转换，始终返回绑定错误通知
    {
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }
    // 方法体结束（ConvertBack）

}
// 类体结束（FlatListToTreeConverter）
