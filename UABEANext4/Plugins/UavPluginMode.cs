using System;
// 引用基础系统命名空间（System），提供基础类型与特性（例如 Attribute、Enum 等）

namespace UABEANext4.Plugins;
// 定义命名空间 UABEANext4.Plugins（UABEANext4.Plugins），用于组织插件相关类型

[Flags]
// 应用 Flags 特性（Flags），表示该枚举可作为位域组合使用（允许按位组合枚举值）

public enum UavPluginMode
// 定义公共枚举类型 UavPluginMode（UavPluginMode），表示插件可支持的不同模式
{
    Import = 1,
    // 枚举成员 Import（Import），值为 1（0b0001）
    // 含义：表示“导入”模式，通常用于将外部数据导入到程序中

    Export = 2,
    // 枚举成员 Export（Export），值为 2（0b0010）
    // 含义：表示“导出”模式，通常用于将程序数据导出到外部格式

    Console = 4,
    // 枚举成员 Console（Console），值为 4（0b0100）
    // 含义：表示“控制台”或命令行模式，插件可在无 GUI 的环境下运行

    Create = 8,
    // 枚举成员 Create（Create），值为 8（0b1000）
    // 含义：表示“创建”模式，插件用于创建新资源或新项目

    All = 15
    // 枚举成员 All（All），值为 15（0b1111）
    // 含义：表示包含所有模式（Import | Export | Console | Create 的组合），便于一次性表示插件支持全部功能
}
