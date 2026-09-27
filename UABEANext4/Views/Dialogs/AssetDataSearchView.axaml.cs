using Avalonia.Controls; // 引用 Avalonia 控件库，用于使用 UserControl、Control 等 UI 基类（Avalonia.Controls）

// 定义命名空间为 UABEANext4.Views.Dialogs（组织对话框视图相关的类）
namespace UABEANext4.Views.Dialogs;

public partial class AssetDataSearchView : UserControl // 定义部分类 AssetDataSearchView，继承自 UserControl（AssetDataSearchView : UserControl），表示“资产数据搜索视图”
{
    public AssetDataSearchView() // 构造函数：当创建 AssetDataSearchView 实例时调用（构造器 AssetDataSearchView）
    {
        InitializeComponent(); // 初始化组件：加载并解析与此控件关联的 .axaml（界面布局），将 XAML 中定义的控件实例化并绑定到此类（InitializeComponent）

        Loaded += AssetDataSearchView_Loaded; // 注册 Loaded 事件处理器：控件加载完成后会触发 Loaded，绑定到 AssetDataSearchView_Loaded 方法（Loaded += AssetDataSearchView_Loaded）
    }

    private void AssetDataSearchView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e) // Loaded 事件的处理方法：控件加载完成后执行（AssetDataSearchView_Loaded），参数为事件发送者和路由事件参数（RoutedEventArgs）
    {
        defaultBox.Focus(); // 将焦点设置到名为 defaultBox 的输入控件上（defaultBox.Focus），便于用户直接输入或开始搜索（保持英文控件名 defaultBox）
        defaultBox.SelectAll(); // 在 defaultBox 中全选其内容（defaultBox.SelectAll），方便用户直接覆盖或替换现有文本
    }
}
