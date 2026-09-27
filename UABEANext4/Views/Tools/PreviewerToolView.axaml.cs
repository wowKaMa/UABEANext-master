using Avalonia.Controls; // 引用 Avalonia 控件库，用于使用 Control、UserControl 等 UI 控件类型 (Avalonia.Controls)
using Avalonia.Controls.Templates; // 引用控件模板接口，用于实现 IDataTemplate（模板选择器） (Avalonia.Controls.Templates)
using Avalonia.Metadata; // 引用元数据特性，例如 [Content]，用于标注 XAML 内容属性 (Avalonia.Metadata)
using System; // 引用系统基础命名空间，包含 Exception、ArgumentNullException 等类型 (System)
using System.Collections.Generic; // 引用泛型集合命名空间，用于 Dictionary<TKey,TValue> 等集合 (System.Collections.Generic)
using UABEANext4.ViewModels.Tools; // 引用项目中预览器相关的 ViewModel 命名空间 (UABEANext4.ViewModels.Tools)

namespace UABEANext4.Views.Tools; // 定义此文件所属的命名空间，组织项目结构 (namespace declaration)

public partial class PreviewerToolView : UserControl // 定义部分类 PreviewerToolView，继承自 UserControl（视图控件） (class declaration)
{
    public PreviewerToolView() // 构造函数：当控件被实例化时调用 (constructor)
    {
        InitializeComponent(); // 初始化组件：加载与此控件关联的 XAML（界面布局） (load XAML)
    }
}

public class PreviewerTemplateSelector : IDataTemplate // 定义 PreviewerTemplateSelector 类，实现 IDataTemplate 接口（模板选择器） (class declaration)
// 该类负责根据传入的数据选择并构建合适的预览模板（View）
{
    [Content] // 标记 AvailableTemplates 为 XAML 的内容属性，允许在 XAML 中直接添加子元素到该字典 (XAML content attribute)
    public Dictionary<string, IDataTemplate> AvailableTemplates { get; } = new(); // 可用模板字典：键为字符串标识，值为对应的 IDataTemplate (stores templates)

    public Control Build(object? param) // IDataTemplate.Build 实现：根据参数构建并返回一个 Control（视图） (Build method)
    {
        var key = param?.ToString() ?? throw new ArgumentNullException(nameof(param)); // 将传入参数转换为字符串作为键；若为 null 则抛出异常 (derive key)
        return AvailableTemplates[key].Build(param)!; // 使用字典中对应的模板构建控件并返回（假定存在且 Build 不返回 null） (build and return control)
    }

    public bool Match(object? data) // IDataTemplate.Match 实现：判断给定数据是否由此模板处理 (Match method)
    {
        var key = data?.ToString(); // 将数据转换为字符串键（可能为 null） (derive key string)
        return data is PreviewerToolPreviewType // 首先检查数据类型是否为预期的 PreviewerToolPreviewType (type check)
                && !string.IsNullOrEmpty(key) // 确保键不为空或 null (non-empty key)
                && AvailableTemplates.ContainsKey(key); // 并且可用模板字典中包含该键 (template exists)
    }
}
