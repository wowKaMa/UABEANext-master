using Avalonia.Controls; // 引用 Avalonia 控件库（Avalonia.Controls），提供 UserControl、Control 等 UI 基类

// 定义命名空间为 UABEANext4.Views.Dialogs（namespace UABEANext4.Views.Dialogs），用于组织对话框相关的视图类
namespace UABEANext4.Views.Dialogs;

public partial class BatchImportView : UserControl // 定义部分类 BatchImportView（BatchImportView），继承自 UserControl（UserControl），表示一个可复用的界面控件（批量导入视图）
{
    public BatchImportView() // 构造函数：当创建 BatchImportView 实例时执行（构造器 BatchImportView）
    {
        InitializeComponent(); // 初始化组件（InitializeComponent）：加载并解析与此控件关联的 .axaml（界面布局），将 XAML 中定义的控件实例化并绑定到此类
    } // 构造函数结束：此处没有额外逻辑，仅完成 XAML 加载

} // 类定义结束：BatchImportView 的类型声明结束
