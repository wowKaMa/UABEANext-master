using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于访问 Unity 资产解析相关类型（原名：AssetsTools.NET）

using Avalonia.Controls.Documents;
// 引用 Avalonia 的文档控件命名空间，用于 InlineCollection 等富文本显示类型（原名：Avalonia.Controls.Documents）

using System.Collections.Generic;
// 引用泛型集合命名空间，提供 List<T> 等集合类型（原名：System.Collections.Generic）

namespace UABEANext4.Logic.AssetInfo;
// 定义命名空间 UABEANext4.Logic.AssetInfo，用于组织与资产信息显示相关的类型（原名：UABEANext4.Logic.AssetInfo）

public class TypeTreeUINode
// 定义公共类 TypeTreeUINode，表示用于 UI 展示的类型树节点包装（原名：TypeTreeUINode）
{
    // 类体开始

    public required TypeTreeNode Node { get; init; }
    // 必需属性 Node（TypeTreeNode）：保存底层的类型树节点数据，用于访问类型信息（原名：Node；类型原名：TypeTreeNode）
    // 说明：使用 C# 11 的 required 修饰符表示在初始化对象时必须提供该属性；init 表示只能在初始化时赋值

    public required InlineCollection Display { get; init; }
    // 必需属性 Display（InlineCollection）：保存用于在 UI 中显示该节点的富文本片段集合（原名：Display；类型原名：InlineCollection）
    // 说明：InlineCollection 通常用于构建带样式的文本显示（例如不同颜色或字体的字段名与类型）

    public required List<TypeTreeUINode> Children { get; init; }
    // 必需属性 Children（List<TypeTreeUINode>）：保存该节点的子节点列表，用于构建树形 UI（原名：Children；元素类型原名：TypeTreeUINode）
    // 说明：使用 List 来维护可枚举的子节点集合，init 表示只能在初始化时赋值
}
// 类体结束
