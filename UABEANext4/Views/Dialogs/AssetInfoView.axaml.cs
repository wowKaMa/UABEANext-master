using Avalonia.Controls; // 引用 Avalonia 控件库（Avalonia.Controls），提供 UserControl、Control 等 UI 基类

using System.Linq; // 引用 LINQ 扩展方法（System.Linq），用于集合操作（如 FirstOrDefault、Where、Select 等）

using UABEANext4.ViewModels.Dialogs; // 引用项目中对话框相关的视图模型命名空间（UABEANext4.ViewModels.Dialogs），以访问 AssetInfoViewModel

// 定义命名空间为 UABEANext4.Views.Dialogs（组织对话框视图相关的类）
namespace UABEANext4.Views.Dialogs;

public partial class AssetInfoView : UserControl // 定义部分类 AssetInfoView，继承自 UserControl（表示一个可复用的界面控件）
{
    public AssetInfoView() // 构造函数：当创建 AssetInfoView 实例时执行（用于初始化控件）
    {
        InitializeComponent(); // 初始化组件：加载并解析与此控件关联的 .axaml（界面布局），将 XAML 中定义的控件实例化并绑定到此类

        Loaded += AssetInfoView_Loaded; // 注册 Loaded 事件处理器：控件加载完成后会触发 Loaded，绑定到 AssetInfoView_Loaded 方法
    }

    private void AssetInfoView_Loaded( // Loaded 事件的处理方法签名开始：当控件加载完成时被调用
        object? sender, // 事件发送者（通常是当前控件实例），类型为 object，可为 null
        Avalonia.Interactivity.RoutedEventArgs e) // 路由事件参数（包含事件相关信息），类型为 RoutedEventArgs
    {
        if (DataContext is AssetInfoViewModel aivm) // 检查 DataContext 是否为期望的 AssetInfoViewModel；若是则将其赋值给局部变量 aivm
        {
            SelectedAssetComboBox.ItemsSource = aivm.Items; // 将名为 SelectedAssetComboBox 的下拉框的数据源设置为 ViewModel 的 Items（绑定项集合）
            SelectedAssetComboBox.SelectedIndex = 0; // 将下拉框的选中索引设置为 0（默认选中第一个项）
            aivm.SelectedItem = aivm.Items.FirstOrDefault(); // 将 ViewModel 的 SelectedItem 设置为 Items 的第一个元素（若无则为 null）



        } // if 语句结束：DataContext 不是 AssetInfoViewModel 时不会执行上面的初始化逻辑
    } // AssetInfoView_Loaded 方法结束
} // 类定义结束：AssetInfoView 的类型声明结束
