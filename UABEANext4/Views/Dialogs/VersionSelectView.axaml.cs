using Avalonia.Controls; // 引用 Avalonia 控件库，用于使用 UserControl、Control 等 UI 控件（Avalonia.Controls）

// 定义命名空间为 UABEANext4.Views.Dialogs，组织对话框视图相关类（namespace UABEANext4.Views.Dialogs）
namespace UABEANext4.Views.Dialogs;

public partial class VersionSelectView : UserControl // 定义部分类 VersionSelectView，继承自 UserControl（VersionSelectView : UserControl）
{
    public VersionSelectView() // 构造函数：当 VersionSelectView 实例被创建时调用（构造函数 VersionSelectView）
    {
        InitializeComponent(); // 初始化组件：加载并解析与此控件关联的 .axaml（界面布局）（InitializeComponent）
        Loaded += VersionSelectView_Loaded; // 注册 Loaded 事件处理器：当控件加载完成时调用 VersionSelectView_Loaded（Loaded += VersionSelectView_Loaded）
    }

    private void VersionSelectView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e) // Loaded 事件的处理方法：控件加载完成后执行（VersionSelectView_Loaded）
    {
        defaultBox.Focus(); // 将焦点设置到名为 defaultBox 的控件上，便于用户直接输入或选择（defaultBox.Focus）
        defaultBox.SelectAll(); // 在 defaultBox 中全选其内容，方便用户替换或确认（defaultBox.SelectAll）
    }
}
