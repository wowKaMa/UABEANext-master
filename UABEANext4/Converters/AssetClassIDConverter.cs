using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展命名空间（保留英文原名：AssetsTools.NET.Extra），用于访问扩展类型如 AssetClassID、AssetsFileInstance 等

using Avalonia.Data;
// 引用 Avalonia 的数据绑定命名空间（保留英文原名：Avalonia.Data），提供 BindingNotification、BindingErrorType 等绑定辅助类型

using Avalonia.Data.Converters;
// 引用 Avalonia 的数据转换器命名空间（保留英文原名：Avalonia.Data.Converters），包含 IValueConverter 接口

using System;
// 引用基础系统命名空间（保留英文原名：System），提供 Enum、InvalidCastException 等基础类型

using System.Collections.Generic;
// 引用泛型集合命名空间（保留英文原名：System.Collections.Generic），提供 Dictionary<TKey,TValue> 等集合类型

using System.Globalization;
// 引用区域性/文化信息命名空间（保留英文原名：System.Globalization），提供 CultureInfo 等

using System.Linq;
// 引用 LINQ 扩展方法命名空间（保留英文原名：System.Linq），用于枚举值转换与集合操作

namespace UABEANext4.Converters;
// 定义命名空间 UABEANext4.Converters（保留英文原名：UABEANext4.Converters），用于组织转换器类

public class AssetClassIDConverter : IValueConverter
// 定义公共类 AssetClassIDConverter（保留英文原名：AssetClassIDConverter），实现 IValueConverter（保留英文原名：IValueConverter）
// 作用：将 AssetClassID 枚举值转换为可显示的字符串名称
{
    // 类体开始（AssetClassIDConverter）

    private Dictionary<AssetClassID, string> _nameLookup = Enum
        // 私有字段 _nameLookup（保留英文原名：_nameLookup）：用于缓存 AssetClassID 到字符串名称的映射
        .GetValues(typeof(AssetClassID))
        // 使用 Enum.GetValues 获取 AssetClassID 枚举的所有值（保留英文原名：GetValues / AssetClassID）
        .Cast<AssetClassID>()
        // 将枚举值集合转换为强类型枚举序列（Cast<AssetClassID>）
        .ToDictionary(enm => enm, enm => enm.ToString());
    // 将枚举序列转换为字典，键为枚举值（enm），值为枚举的字符串表示（enm.ToString()）

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    // 实现 IValueConverter.Convert（保留英文原名：Convert）
    // 作用：将绑定源的值（value）转换为目标类型（通常是字符串）以供 UI 显示
    {
        // 方法体开始（Convert）

        if (value is AssetClassID classId)
        // 检查传入的 value 是否为 AssetClassID（保留英文原名：AssetClassID）
        {
            // 条件成立块开始

            if (_nameLookup.TryGetValue(classId, out string? name))
                // 尝试从 _nameLookup 字典中获取对应的名称（TryGetValue）
                return name;
            // 如果找到则直接返回该名称字符串

            if ((int)classId < 0)
                // 如果枚举值的整数表示小于 0（表示自定义或特殊类型，例如 MonoBehaviour 等）
                return _nameLookup[AssetClassID.MonoBehaviour];
            // 则返回字典中 MonoBehaviour 的名称作为回退显示

            return ((int)classId).ToString();
            // 否则将枚举的整数值转换为字符串并返回（当枚举值不在字典中且为非负数时）
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        // 如果传入的 value 不是 AssetClassID，则返回一个 BindingNotification 表示绑定错误（携带 InvalidCastException）
    }
    // 方法体结束（Convert）

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    // 实现 IValueConverter.ConvertBack（保留英文原名：ConvertBack）
    // 说明：此转换器不支持反向转换，因此直接返回绑定错误通知
    {
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        // 返回 BindingNotification 表示不支持 ConvertBack，携带 InvalidCastException 并标记为错误（BindingErrorType.Error）
    }
    // 方法体结束（ConvertBack）

}
// 类体结束（AssetClassIDConverter）
