using Avalonia.Controls; // 引用 Avalonia 控件库，用于使用 UserControl、Control 等界面控件（Avalonia.Controls）

// 定义命名空间为 UABEANext4.Views.Dialogs，表示此类属于“视图.对话框”模块（namespace UABEANext4.Views.Dialogs）
namespace UABEANext4.Views.Dialogs;

public partial class MessageBoxView : UserControl // 定义部分类 MessageBoxView，继承自 UserControl（MessageBoxView : UserControl），表示一个消息对话框视图
{
    public MessageBoxView() // 构造函数：当 MessageBoxView 实例被创建时调用（构造器 MessageBoxView）
    {
        InitializeComponent(); // 初始化组件：加载并解析与此控件关联的 .axaml（界面布局），将 XAML 中定义的控件实例化并绑定到此类（InitializeComponent）
    } // 构造函数结束

} // 类定义结束
