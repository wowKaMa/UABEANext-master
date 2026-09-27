using Avalonia.Controls; // 引用 Avalonia 控件库（Avalonia.Controls），提供 UserControl、Control 等 UI 基类

// 定义命名空间为 UABEANext4.Views.Dialogs（组织对话框相关的视图类）
namespace UABEANext4.Views.Dialogs;

public partial class SelectDumpView : UserControl // 定义部分类 SelectDumpView，继承自 UserControl（SelectDumpView : UserControl）
{
    public SelectDumpView() // 构造函数：当 SelectDumpView 实例被创建时调用（构造器 SelectDumpView）
    {
        InitializeComponent(); // 初始化组件：加载并解析与此控件关联的 .axaml（界面布局），将 XAML 中定义的控件实例化并绑定到此类（InitializeComponent）
    } // 构造函数结束

} // 类定义结束
