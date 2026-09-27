namespace UABEANext4.Plugins;
// 定义命名空间 UABEANext4.Plugins（保留英文原名），用于组织插件相关类型

public class PluginOptionModePair(IUavPluginOption option, UavPluginMode mode)
// 定义公共类 PluginOptionModePair（保留英文原名），使用主构造参数 option（类型 IUavPluginOption）和 mode（类型 UavPluginMode）
// 说明：此处使用 C# 的主构造（primary constructor）语法，构造参数在类声明处定义

{
    // 类体开始

    public IUavPluginOption Option { get; } = option;
    // 定义只读公共属性 Option（类型 IUavPluginOption，保留英文原名）
    // 用构造参数 option 初始化该属性
    // 作用：外部通过 PluginOptionModePair.Option 访问该插件选项实例

    public UavPluginMode Mode { get; } = mode;
    // 定义只读公共属性 Mode（类型 UavPluginMode，保留英文原名）
    // 用构造参数 mode 初始化该属性
    // 作用：外部通过 PluginOptionModePair.Mode 获取该选项对应的模式（例如 Import/Export 等）

    public override string ToString()
    // 重写 ToString() 方法（保留英文原名 ToString），用于返回对象的字符串表示
    {
        // 方法体开始

        return Option.Name;
        // 返回 Option 的 Name 属性作为该对象的字符串表示（保留英文原名 Option.Name）
        // 作用：当将 PluginOptionModePair 转为字符串时，通常显示选项的名称
    }

}
// 类体结束
