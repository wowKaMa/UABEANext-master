using System; // 引用基础系统命名空间，提供 Enum、IEnumerable 等基础类型和功能（保留原名 System）

using System.Collections.Generic; // 引用泛型集合命名空间，提供 IEnumerable<T> 等集合接口（保留原名 System.Collections.Generic）

namespace UABEANext4.Util; // 定义命名空间 UABEANext4.Util，用于组织工具类（保留原名 UABEANext4.Util）

public static class GeneralExtensionUtils // 定义公共静态类 GeneralExtensionUtils，作为扩展方法的容器（保留原名 GeneralExtensionUtils）
{ // 类体开始

    // https://stackoverflow.com/a/50481101
    // 原始实现参考链接（保留英文原文注释），说明此方法的来源

    public static IEnumerable<T> GetUniqueFlags<T>(this T flags) where T : Enum // 定义泛型扩展方法 GetUniqueFlags：对枚举类型（带 Flags 特性的枚举）返回其包含的独立标志；泛型约束要求 T 为 Enum（保留原名 GetUniqueFlags）
    { // 方法体开始

        foreach (Enum value in Enum.GetValues(flags.GetType())) // 遍历枚举类型的所有可能值（使用 Enum.GetValues 获取枚举的每个成员）
            // 注：这里使用 Enum 类型进行遍历以便与 flags.HasFlag 一致比较
            if (flags.HasFlag(value)) // 如果传入的 flags 包含当前枚举值（HasFlag 返回 true 表示该位被设置）
                yield return (T)value; // 使用 yield 返回该枚举值（转换回泛型 T），逐个产生结果而不是一次性返回集合
    } // 方法体结束
} // 类体结束
