using Avalonia.Controls; // 引用 Avalonia 控件库（Avalonia.Controls），提供 UserControl、Control 等 UI 基类

namespace UABEANext4.Views.Dialogs; // 定义命名空间为 UABEANext4.Views.Dialogs（视图.对话框），用于组织对话框相关的视图类

public partial class SelectTypeFilterView : UserControl // 定义部分类 SelectTypeFilterView（选择类型过滤视图），继承自 UserControl（用户控件）
{
    public SelectTypeFilterView() // 构造函数：当创建 SelectTypeFilterView 实例时执行，用于初始化该控件的状态
    {
        InitializeComponent(); // 初始化组件（InitializeComponent），加载并解析与此控件关联的 .axaml（界面布局），将 XAML 中定义的控件实例化并绑定到此类
    } // 构造函数结束

} // 类定义结束
