using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展命名空间，用于访问扩展类型（保留英文原名：AssetsTools.NET.Extra）

using Avalonia.Data;
// 引用 Avalonia 的数据绑定命名空间，提供 BindingNotification、BindingErrorType 等（保留英文原名：Avalonia.Data）

using Avalonia.Data.Converters;
// 引用 Avalonia 的数据转换器命名空间，包含 IValueConverter 接口（保留英文原名：Avalonia.Data.Converters）

using System;
// 引用基础系统命名空间，提供 Exception、Type 等基础类型（保留英文原名：System）

using System.Globalization;
// 引用文化信息命名空间，提供 CultureInfo（保留英文原名：System.Globalization）

namespace UABEANext4.Converters;
// 定义命名空间 UABEANext4.Converters，用于组织转换器类（保留英文原名：UABEANext4.Converters）

public class AssetsFileInstanceNameConverter : IValueConverter
// 定义公共类 AssetsFileInstanceNameConverter（保留英文原名：AssetsFileInstanceNameConverter），实现 IValueConverter（保留英文原名：IValueConverter）
// 作用：在数据绑定时将 AssetsFileInstance 转换为其显示名称（fileInst.name）
{
    // 类体开始（AssetsFileInstanceNameConverter）

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    // 实现 IValueConverter.Convert（保留英文原名：Convert）
    // 参数说明：value 为绑定源的值；targetType 为目标类型；parameter 为可选参数；culture 为区域信息
    {
        // 方法体开始（Convert）

        if (value is AssetsFileInstance fileInst)
        // 检查传入的 value 是否为 AssetsFileInstance 类型（保留英文原名：AssetsFileInstance）
        {
            return fileInst.name;
            // 如果是 AssetsFileInstance，则返回其 name 属性（保留英文原名：name），用于在 UI 中显示文件名
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        // 如果 value 不是期望类型，则返回一个 BindingNotification（保留英文原名：BindingNotification）
        // 该通知携带 InvalidCastException（保留英文原名：InvalidCastException）并标记为绑定错误（BindingErrorType.Error）
    }
    // 方法体结束（Convert）

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    // 实现 IValueConverter.ConvertBack（保留英文原名：ConvertBack）
    // 说明：此转换器不支持反向转换，因此直接返回绑定错误通知
    {
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        // 返回 BindingNotification 表示不支持 ConvertBack，携带 InvalidCastException 并标记为错误（保留英文原名：BindingNotification / InvalidCastException / BindingErrorType）
    }
    // 方法体结束（ConvertBack）

}
// 类体结束（AssetsFileInstanceNameConverter）
