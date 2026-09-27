using Avalonia.Controls; // 引用 Avalonia 控件库，用于使用 UserControl、Control 等 UI 基类（Avalonia.Controls）

// 定义命名空间为 UABEANext4.Views.Dialogs，用于组织对话框相关的视图类（命名空间：UABEANext4.Views.Dialogs）
namespace UABEANext4.Views.Dialogs;

public partial class AddAssetView : UserControl // 定义部分类 AddAssetView（AddAssetView），继承自 UserControl（UserControl），表示“添加资源”对话框视图
{
    public AddAssetView() // 构造函数：当创建 AddAssetView（AddAssetView）实例时调用，用于初始化该控件的实例
    {
        InitializeComponent(); // 初始化组件（InitializeComponent）：加载并解析与此控件关联的 XAML/AXAML（界面布局），并将 XAML 中声明的子控件实例化并绑定到此类
    } // 构造函数结束

} // 类定义结束：AddAssetView 类型声明结束
