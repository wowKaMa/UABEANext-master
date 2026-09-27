using Avalonia.Controls; // 引用 Avalonia 控件库（Avalonia.Controls），提供 UserControl、Control 等 UI 基类

// 定义命名空间为 UABEANext4.Views.Documents（视图.文档）
namespace UABEANext4.Views.Documents;

// 定义部分类 BlankDocumentView，继承自 UserControl（空白文档视图 BlankDocumentView）
public partial class BlankDocumentView : UserControl
{
    // 构造函数：当 BlankDocumentView 实例被创建时调用（BlankDocumentView constructor）
    public BlankDocumentView()
    {
        // 初始化组件：加载并解析与此控件关联的 .axaml（界面布局），将 XAML 中定义的控件实例化并绑定到此类（InitializeComponent）
        InitializeComponent();
    } // 构造函数结束

} // 类定义结束
