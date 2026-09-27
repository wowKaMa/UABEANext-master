using Avalonia;
// 引用 Avalonia 框架的根命名空间（原名：Avalonia），用于访问应用级别与全局对象（例如 Application.Current）

using Avalonia.Controls;
// 引用 Avalonia 的控件命名空间（原名：Avalonia.Controls），提供控件相关类型（此处未直接使用具体控件，但常与 UI 绑定一起使用）

using Avalonia.Data;
// 引用 Avalonia 的数据绑定命名空间（原名：Avalonia.Data），提供 BindingOperations 等绑定辅助 API

using Avalonia.Data.Converters;
// 引用 Avalonia 的数据转换器命名空间（原名：Avalonia.Data.Converters），包含 IValueConverter / IMultiValueConverter 接口

using Avalonia.Media;
// 引用 Avalonia 的媒体/画刷命名空间（原名：Avalonia.Media），提供 IBrush、ISolidColorBrush、Colors、SolidColorBrush 等类型

using System;
// 引用基础系统命名空间（原名：System），提供常用类型（例如 Exception）

using System.Collections.Generic;
// 引用泛型集合命名空间（原名：System.Collections.Generic），提供 IList<T>、Dictionary 等集合类型

using System.Globalization;
// 引用文化信息命名空间（原名：System.Globalization），提供 CultureInfo 等文化相关类型

using UABEANext4.AssetWorkspace;
// 引用项目内的资产工作区命名空间（原名：UABEANext4.AssetWorkspace），用于 WorkspaceItemType 枚举等上下文类型

namespace UABEANext4.Converters;
// 定义命名空间 UABEANext4.Converters（原名：UABEANext4.Converters），用于组织转换器类

public class WsItemColorConverter : IMultiValueConverter
// 定义公共类 WsItemColorConverter（原名：WsItemColorConverter），实现 IMultiValueConverter（原名：IMultiValueConverter），用于根据工作区项类型返回对应的画刷（颜色）
{
    // 类体开始（WsItemColorConverter）

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    // 实现 IMultiValueConverter 的 Convert 方法（原名：Convert）
    // 参数说明：values 为绑定传入的多个值列表；targetType 为目标绑定类型；parameter 为可选参数；culture 为文化信息
    {
        // 方法体开始（Convert）

        if (values?.Count != 2 || !targetType.IsAssignableFrom(typeof(IBrush)))
            throw new NotSupportedException();
        // 校验：确保传入的 values 有且仅有 2 个元素（本转换器期望两个绑定值），且目标类型可分配为 IBrush（原名：IBrush）
        // 如果不满足则抛出 NotSupportedException（原名：NotSupportedException）

        if (values[0] is not WorkspaceItemType type)
            return BindingOperations.DoNothing;
        // 将第一个绑定值尝试转换为 WorkspaceItemType（原名：WorkspaceItemType）
        // 如果转换失败则返回 BindingOperations.DoNothing（表示绑定系统不做更改或忽略此次转换）

        return type switch
        {
            WorkspaceItemType.BundleFile => GetBrushFromName("WorkspaceItemBundleBrush"),
            WorkspaceItemType.AssetsFile => GetBrushFromName("WorkspaceItemAssetsBrush"),
            WorkspaceItemType.ResourceFile => GetBrushFromName("WorkspaceItemResourceBrush"),
            WorkspaceItemType.OtherFile => GetBrushFromName("WorkspaceItemOtherBrush"),
            _ => GetBrushFromName("WorkspaceItemEtcBrush"),
        };
        // 使用 C# 的 switch 表达式根据 WorkspaceItemType 的不同枚举值返回不同的画刷（通过资源键名查找）
        // 各分支调用私有静态方法 GetBrushFromName，传入对应资源键名（例如 "WorkspaceItemBundleBrush"）
    }
    // 方法体结束（Convert）

    private static ISolidColorBrush GetBrushFromName(string key)
    // 私有静态方法 GetBrushFromName（原名：GetBrushFromName），根据资源键名查找并返回 ISolidColorBrush（原名：ISolidColorBrush）
    {
        // 方法体开始（GetBrushFromName）

        var currentApp = Application.Current;
        // 获取当前应用实例（Application.Current，原名：Application.Current），用于从应用资源中查找主题相关资源

        if (currentApp is null)
            return new SolidColorBrush(Colors.Black);
        // 如果当前应用实例为 null（例如在非 UI 环境），返回一个默认的黑色画刷（SolidColorBrush(Colors.Black)）

        if (!currentApp.TryFindResource(key, currentApp.ActualThemeVariant, out object? value))
            return new SolidColorBrush(Colors.Black);
        // 使用 Application.TryFindResource 尝试按键（key）和当前主题（ActualThemeVariant）查找资源
        // 如果未找到资源，则返回默认黑色画刷（避免抛出异常或返回 null）

        if (value is not ISolidColorBrush brush)
            return new SolidColorBrush(Colors.Black);
        // 如果找到的资源不是 ISolidColorBrush 类型，则也返回默认黑色画刷（类型检查与回退）

        return brush;
        // 成功找到并转换为 ISolidColorBrush 后返回该画刷（brush）
    }
    // 方法体结束（GetBrushFromName）

}
// 类体结束（WsItemColorConverter）
