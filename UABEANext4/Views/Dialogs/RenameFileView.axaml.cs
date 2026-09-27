using Avalonia.Controls; // 引用 Avalonia 控件库（Avalonia.Controls），提供 UserControl、Control 等 UI 基类

// 定义命名空间为 UABEANext4.Views.Dialogs（组织对话框相关的视图类）
namespace UABEANext4.Views.Dialogs;

public partial class RenameFileView : UserControl // 定义部分类 RenameFileView，继承自 UserControl（RenameFileView : UserControl），表示重命名文件的对话框视图
{
    public RenameFileView() // 构造函数：当 RenameFileView 实例被创建时调用（构造器 RenameFileView）
    {
        InitializeComponent(); // 初始化组件（InitializeComponent），加载并解析与此控件关联的 .axaml（界面布局）
        Loaded += RenameFileView_Loaded; // 注册 Loaded 事件处理器：控件加载完成后调用 RenameFileView_Loaded（Loaded += RenameFileView_Loaded）
    } // 构造函数结束

    private void RenameFileView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e) // Loaded 事件的处理方法：控件加载完成后执行（RenameFileView_Loaded）
    {
        defaultBox.Focus(); // 将焦点设置到名为 defaultBox 的输入控件上（defaultBox.Focus），便于用户直接输入或编辑文件名
        defaultBox.SelectAll(); // 在 defaultBox 中全选其内容（defaultBox.SelectAll），方便用户直接覆盖或替换现有文本
    } // Loaded 事件处理方法结束

} // 类定义结束
