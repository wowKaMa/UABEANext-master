using Dock.Model.Mvvm.Controls; // 引用 Dock.Model 的 MVVM 控件基类，提供 Document、Tool 等停靠窗口相关类型

namespace UABEANext4.ViewModels.Documents; // 定义命名空间 UABEANext4.ViewModels.Documents，用于组织文档相关的视图模型类

public partial class BlankDocumentViewModel : Document // 定义部分类 BlankDocumentViewModel，继承自 Document（表示一个空白文档标签页的视图模型）
{
    const string TOOL_TITLE = "新标签页 (New Tab)"; // 常量：工具/文档标题，中文显示“新标签页”，括号中保留原始英文名称以便 UI 显示或识别

    public BlankDocumentViewModel() // 构造函数：创建 BlankDocumentViewModel 实例时调用，用于初始化文档的标识与标题
    {
        Id = TOOL_TITLE.Replace(" ", ""); // 设置文档的 Id（内部标识），通过移除 TOOL_TITLE 中的空格生成唯一字符串
        Title = TOOL_TITLE; // 设置文档的显示标题（Title），用于 UI 上显示文档标签的文本
    } // 构造函数结束

} // 类定义结束：BlankDocumentViewModel 的类型声明结束
