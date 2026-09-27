using Avalonia.Controls; // 引用 Avalonia 控件库，用于使用 Control、UserControl 等 UI 控件类型（Avalonia.Controls）

// 定义命名空间为 UABEANext4.Views.Tools；该命名空间用于组织工具视图相关的类（UABEANext4.Views.Tools）
namespace UABEANext4.Views.Tools;

public partial class InspectorToolView : UserControl // 定义部分类 InspectorToolView，继承自 UserControl（InspectorToolView 类用于显示检查器工具视图）
{
    public InspectorToolView() // 构造函数：当 InspectorToolView 实例被创建时调用（InspectorToolView constructor）
    {
        InitializeComponent(); // 初始化组件：加载并解析与此控件关联的 .axaml（界面布局），将 XAML 中定义的控件实例化并绑定到此类（InitializeComponent）
    } // 构造函数结束

} // 类定义结束
