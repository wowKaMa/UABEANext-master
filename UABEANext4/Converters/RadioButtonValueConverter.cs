using Avalonia.Data.Converters;
// 引用 Avalonia 的数据转换器命名空间（原名：Avalonia.Data.Converters），用于实现 IValueConverter 接口并参与数据绑定转换

using Avalonia.Markup.Xaml;
// 引用 Avalonia 的 XAML 标记扩展命名空间（原名：Avalonia.Markup.Xaml），用于继承 MarkupExtension 并在 XAML 中提供转换器实例

using System;
// 引用基础系统命名空间（原名：System），提供 IServiceProvider、Type 等基础类型

using System.Globalization;
// 引用文化信息命名空间（原名：System.Globalization），用于处理区域性相关的转换（CultureInfo）

namespace UABEANext4.Converters;
// 定义命名空间 UABEANext4.Converters（原名：UABEANext4.Converters），用于组织转换器类

public class RadioButtonValueConverter : MarkupExtension, IValueConverter
// 定义公共类 RadioButtonValueConverter（原名：RadioButtonValueConverter），继承 MarkupExtension（原名：MarkupExtension）并实现 IValueConverter（原名：IValueConverter）
// 说明：此类用于在单选按钮（RadioButton）绑定中把“选项值”和“是否选中”互相转换
{
    // 类体开始（RadioButtonValueConverter）

    public RadioButtonValueConverter(object optionValue)
        // 构造函数 RadioButtonValueConverter(object optionValue)：接收一个表示该单选项对应值的对象（原名：optionValue）
        => OptionValue = optionValue;
    // 使用表达式体将传入的 optionValue 赋值给只读属性 OptionValue（原名：OptionValue）

    public object OptionValue { get; }
    // 只读属性 OptionValue（原名：OptionValue）：保存该转换器对应的选项值，用于比较与回写

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        // 实现 IValueConverter.Convert（原名：Convert）：将绑定源的值（value）转换为目标（通常是 RadioButton 的 IsChecked）
        => value?.Equals(OptionValue);
    // 表达式体实现：如果 value 与 OptionValue 相等则返回 true，否则返回 false；若 value 为 null 则返回 null（原名：Equals）

    public object? ConvertBack(object? isChecked, Type targetType, object? parameter, CultureInfo culture)
        // 实现 IValueConverter.ConvertBack（原名：ConvertBack）：将目标值（通常是 RadioButton 的 IsChecked）转换回绑定源的值
        => (bool)(isChecked ?? false)
            ? OptionValue
            : null;
    // 表达式体实现：如果 isChecked 为 true（或可空值为 true），则返回 OptionValue；否则返回 null（表示不选中或不改变绑定值）

    public override object ProvideValue(IServiceProvider serviceProvider)
        // 重写 MarkupExtension.ProvideValue（原名：ProvideValue）：当在 XAML 中使用该扩展时返回要注入的实例
        => this;
    // 返回当前转换器实例（this），使其可以直接在 XAML 中作为静态资源或属性值使用
}
// 类体结束（RadioButtonValueConverter）
