using Avalonia.Controls; // 引用 Avalonia 控件库，用于使用 UserControl、Control 等 UI 基类（Avalonia.Controls）

// 定义命名空间为 UABEANext4.Views.Dialogs，用于组织对话框相关的视图类（UABEANext4.Views.Dialogs）
namespace UABEANext4.Views.Dialogs;

public partial class AddExternalView : UserControl // 定义部分类 AddExternalView，继承自 UserControl；表示“添加外部项”视图（AddExternalView : UserControl）
{
    public AddExternalView() // 构造函数：当创建 AddExternalView 实例时调用（构造函数 AddExternalView）
    {
        InitializeComponent(); // 初始化组件：加载并解析与此控件关联的 .axaml（界面布局），将 XAML 中定义的控件实例化并绑定到此类（InitializeComponent）
    } // 构造函数结束

} // 类定义结束：AddExternalView 的类型声明结束
