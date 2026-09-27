using CommunityToolkit.Mvvm.ComponentModel;
// 引用 CommunityToolkit MVVM 的组件模型命名空间，用于 ObservableObject 与属性生成特性（原名：CommunityToolkit.Mvvm.ComponentModel）

using DynamicData;
// 引用 DynamicData 库（用于响应式集合等），在此文件中保留引用以匹配原代码（原名：DynamicData）

using System;
// 引用基础系统命名空间，提供 Exception、Type 等基础类型（原名：System）

using System.Collections.Generic;
// 引用泛型集合命名空间，提供 List<T>、IReadOnlyList<T> 等集合类型（原名：System.Collections.Generic）

using System.Collections.ObjectModel;
// 引用只读集合相关命名空间（原名：System.Collections.ObjectModel）

using System.Linq;
// 引用 LINQ 扩展方法命名空间，用于集合查询与转换（原名：System.Linq）

using System.Reflection;
// 引用反射命名空间，用于读取 PropertyInfo、Attribute 等（原名：System.Reflection）

namespace UABEANext4.Logic.Configuration;
// 定义命名空间 UABEANext4.Logic.Configuration（原名：UABEANext4.Logic.Configuration），用于组织配置相关类型

public abstract class ConfigurationItemBase : ObservableObject
// 定义抽象基类 ConfigurationItemBase（原名：ConfigurationItemBase），继承自 ObservableObject（原名：ObservableObject），为具体配置项提供公共成员与辅助方法
{
    // 类体开始（ConfigurationItemBase）

    public string Title { get; init; } = string.Empty;
    // 公共只读初始化属性 Title（原名：Title），用于显示配置项标题；使用 init 仅允许在构造或初始化器中赋值，默认空字符串

    public string Description { get; init; } = string.Empty;
    // 公共只读初始化属性 Description（原名：Description），用于显示配置项描述，默认空字符串

    protected static (string, string?) GetBaseAttrs(PropertyInfo property)
    // 受保护静态方法 GetBaseAttrs（原名：GetBaseAttrs），接收一个 PropertyInfo（反射表示的属性），返回元组 (title, description?)，用于从属性特性读取标题与描述
    {
        // 方法体开始（GetBaseAttrs）

        var titleAttr = property.GetCustomAttribute<ConfigTitle>();
        // 使用反射从属性（property）读取自定义特性 ConfigTitle（原名：ConfigTitle），并赋给 titleAttr；如果不存在则为 null

        var descAttr = property.GetCustomAttribute<ConfigDesc>();
        // 使用反射从属性读取自定义特性 ConfigDesc（原名：ConfigDesc），并赋给 descAttr；如果不存在则为 null

        if (titleAttr is null)
        {
            throw new Exception("Missing title for settings property");
        }
        // 如果没有找到 ConfigTitle 特性，则抛出异常（强制每个设置属性必须有标题特性）

        return (
            titleAttr.Title,
            descAttr?.Description
        );
        // 返回一个元组：第一个元素为 titleAttr.Title（特性中定义的标题），第二个元素为 descAttr?.Description（如果 descAttr 为 null 则返回 null）
    }
    // 方法体结束（GetBaseAttrs）
}
// 类体结束（ConfigurationItemBase）

public partial class ConfigurationIntegerItem : ConfigurationItemBase
// 定义部分类 ConfigurationIntegerItem（原名：ConfigurationIntegerItem），继承自 ConfigurationItemBase，用于表示整型配置项并提供范围信息
{
    // 类体开始（ConfigurationIntegerItem）

    private readonly PropertyInfo _property;
    // 私有只读字段 _property（原名：_property），保存对应的反射属性（PropertyInfo），用于读取/写入实际配置对象的值

    [ObservableProperty] private int? _rangeMin;
    // 使用 ObservableProperty 特性生成公开属性 RangeMin（原名：RangeMin），字段为可空 int _rangeMin；用于表示允许的最小值（如果存在）

    [ObservableProperty] private int? _rangeMax;
    // 使用 ObservableProperty 特性生成公开属性 RangeMax（原名：RangeMax），字段为可空 int _rangeMax；用于表示允许的最大值（如果存在）

    public ConfigurationIntegerItem(PropertyInfo property)
    // 构造函数 ConfigurationIntegerItem(PropertyInfo property)：接收要绑定的属性信息（原名：property）
    {
        // 构造体开始

        _property = property;
        // 将传入的 PropertyInfo 保存到字段 _property，后续用于读取/写入配置值

        var (title, desc) = GetBaseAttrs(property);
        // 调用基类的 GetBaseAttrs 从属性特性中读取标题与描述，解构为 title 与 desc

        Title = title;
        // 将读取到的标题赋值给继承自基类的 Title 属性（用于 UI 显示）

        Description = desc ?? "No description.";
        // 将描述赋值给 Description；如果 desc 为 null 则使用默认文本 "No description."

        var range = property.GetCustomAttribute<ConfigRange>();
        // 使用反射读取可选的 ConfigRange 特性（原名：ConfigRange），用于获取最小/最大范围

        RangeMin = range?.Minimum;
        // 如果存在 range 特性，则将其 Minimum 赋给 RangeMin（否则为 null）

        RangeMax = range?.Maximum;
        // 如果存在 range 特性，则将其 Maximum 赋给 RangeMax（否则为 null）

        ConfigurationManager.Settings.PropertyChanged += (s, e) => {
            if (e.PropertyName == _property.Name)
            {
                OnPropertyChanged(nameof(Value));
            }
        };
        // 订阅全局配置对象（ConfigurationManager.Settings）的 PropertyChanged 事件：
        // 当配置对象中对应的属性（名称等于 _property.Name）发生变化时，触发当前对象的属性变更通知 OnPropertyChanged(nameof(Value))，
        // 以便 UI（或绑定）更新显示的 Value 值
    }
    // 构造体结束

    public int Value
    // 公共属性 Value（原名：Value），用于在 UI 与绑定中读写实际的整型配置值
    {
        get => (int)_property.GetValue(ConfigurationManager.Settings)!;
        // getter：通过反射从 ConfigurationManager.Settings（全局配置实例）读取属性值并强制转换为 int 返回
        set => _property.SetValue(ConfigurationManager.Settings, value);
        // setter：通过反射将新值写回到 ConfigurationManager.Settings 的对应属性
    }
}
// 类体结束（ConfigurationIntegerItem）

public class ConfigurationBooleanItem : ConfigurationItemBase
// 定义类 ConfigurationBooleanItem（原名：ConfigurationBooleanItem），继承自 ConfigurationItemBase，用于表示布尔型配置项
{
    // 类体开始（ConfigurationBooleanItem）

    private readonly PropertyInfo _property;
    // 私有只读字段 _property：保存要绑定的 PropertyInfo（用于读写实际配置对象的布尔属性）

    public ConfigurationBooleanItem(PropertyInfo property)
    // 构造函数 ConfigurationBooleanItem(PropertyInfo property)
    {
        // 构造体开始

        _property = property;
        // 保存传入的 PropertyInfo 到字段 _property

        var (title, desc) = GetBaseAttrs(property);
        // 从属性特性读取标题与描述

        Title = title;
        // 将标题赋给基类的 Title 属性

        Description = desc ?? "No description.";
        // 将描述赋给 Description，若为 null 则使用默认文本

        ConfigurationManager.Settings.PropertyChanged += (s, e) => {
            if (e.PropertyName == _property.Name)
            {
                OnPropertyChanged(nameof(Value));
            }
        };
        // 订阅全局配置对象的 PropertyChanged 事件，当对应属性变化时触发当前对象的 Value 属性变更通知
    }
    // 构造体结束

    public bool Value
    // 公共属性 Value（原名：Value），用于读写布尔配置值
    {
        get => (bool)_property.GetValue(ConfigurationManager.Settings)!;
        // getter：通过反射从 ConfigurationManager.Settings 读取属性并转换为 bool 返回
        set => _property.SetValue(ConfigurationManager.Settings, value);
        // setter：通过反射将新布尔值写回到配置对象
    }
}
// 类体结束（ConfigurationBooleanItem）

public partial class ConfigurationEnumItem : ConfigurationItemBase
// 定义部分类 ConfigurationEnumItem（原名：ConfigurationEnumItem），继承自 ConfigurationItemBase，用于表示枚举类型的配置项
{
    // 类体开始（ConfigurationEnumItem）

    private readonly PropertyInfo _property;
    // 私有只读字段 _property：保存要绑定的 PropertyInfo（对应一个枚举类型的属性）

    private readonly Type _enumType;
    // 私有只读字段 _enumType：保存枚举类型（PropertyInfo.PropertyType），用于枚举值的解析与显示

    [ObservableProperty] private IReadOnlyList<string> _enumValues;
    // 使用 ObservableProperty 生成公开只读属性 EnumValues（原名：EnumValues），字段为 IReadOnlyList<string> _enumValues，
    // 用于在 UI 中显示枚举的可选字符串列表（每个元素为枚举成员名）

    public ConfigurationEnumItem(PropertyInfo property)
    // 构造函数 ConfigurationEnumItem(PropertyInfo property)
    {
        // 构造体开始

        _property = property;
        // 保存传入的 PropertyInfo 到字段 _property

        _enumType = property.PropertyType;
        // 将属性的类型保存到 _enumType（应为枚举类型）

        _enumValues = Enum.GetNames(_enumType).ToList().AsReadOnly();
        // 使用 Enum.GetNames 获取枚举成员名数组，转换为只读列表并赋给 _enumValues（用于 UI 下拉等）

        var (title, desc) = GetBaseAttrs(property);
        // 从属性特性读取标题与描述

        Title = title;
        // 将标题赋给基类的 Title 属性

        Description = desc ?? "No description.";
        // 将描述赋给 Description，若为 null 则使用默认文本

        ConfigurationManager.Settings.PropertyChanged += (s, e) => {
            if (e.PropertyName == _property.Name)
            {
                OnPropertyChanged(nameof(Value));
            }
        };
        // 订阅全局配置对象的 PropertyChanged 事件，当对应属性变化时触发当前对象的 Value 属性变更通知
    }
    // 构造体结束

    public string Value
    // 公共属性 Value（原名：Value），以字符串形式表示当前枚举值（用于绑定与显示）
    {
        get => Enum.GetName(_enumType, _property.GetValue(ConfigurationManager.Settings)!)
            ?? "Unknown enum value";
        // getter：通过反射读取配置对象的枚举值，并使用 Enum.GetName 将其转换为成员名字符串；若失败则返回 "Unknown enum value"

        set
        {
            if (Enum.TryParse(_enumType, value, out var enumValue))
            {
                _property.SetValue(ConfigurationManager.Settings, enumValue);
            }
            else
            {
                var zeroValue = Enum.GetValues(_enumType).GetValue(0);
                _property.SetValue(ConfigurationManager.Settings, zeroValue);
            }
        }
        // setter：尝试将传入的字符串解析为枚举值（Enum.TryParse），若解析成功则写回配置对象；
        // 若解析失败，则取枚举的第一个值（索引 0）作为回退并写回配置对象
    }
}
// 类体结束（ConfigurationEnumItem）
