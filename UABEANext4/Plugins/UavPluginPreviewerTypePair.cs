namespace UABEANext4.Plugins;
// 定义命名空间 UABEANext4.Plugins（UABEANext4.Plugins），用于组织插件相关类型

public class PluginPreviewerTypePair(IUavPluginPreviewer previewer, UavPluginPreviewerType previewType)
// 定义公共类 PluginPreviewerTypePair（PluginPreviewerTypePair），使用主构造参数 previewer（IUavPluginPreviewer）和 previewType（UavPluginPreviewerType）
// 说明：此处采用 C# 的主构造（primary constructor）语法，构造参数在类声明处定义

{
    // 类体开始

    public IUavPluginPreviewer Previewer { get; } = previewer;
    // 定义只读公共属性 Previewer（IUavPluginPreviewer），并用构造参数 previewer 初始化
    // 作用：外部通过 PluginPreviewerTypePair.Previewer 访问该预览器实例（保留英文名 Previewer 与 IUavPluginPreviewer）

    public UavPluginPreviewerType PreviewType { get; } = previewType;
    // 定义只读公共属性 PreviewType（UavPluginPreviewerType），并用构造参数 previewType 初始化
    // 作用：外部通过 PluginPreviewerTypePair.PreviewType 获取该预览器对应的类型（保留英文名 PreviewType 与 UavPluginPreviewerType）

    public override string ToString()
    // 重写 ToString() 方法（ToString），用于返回对象的字符串表示
    {
        // 方法体开始

        return Previewer.Name;
        // 返回 Previewer 的 Name 属性作为该对象的字符串表示（保留英文名 Previewer.Name）
        // 作用：当将 PluginPreviewerTypePair 转为字符串时，通常显示预览器的名称
    }

}
// 类体结束
