using Avalonia.Controls;
// 引用 Avalonia 的控件命名空间，用于访问窗口与控件类型（保留英文原名：Avalonia.Controls）

namespace PluginPreviewer.Desktop;
// 定义命名空间 PluginPreviewer.Desktop，用于组织该窗口类（保留英文原名：PluginPreviewer.Desktop）

public partial class Window1 : Window
// 定义一个公共的部分类 Window1，继承自 Avalonia 的 Window（表示一个窗口）（保留英文原名：Window1 / Window）
{
    // 类体开始：Window1 的实现块（保留英文原名：Window1）

    public Window1()
    // 构造函数 Window1：当创建 Window1 实例时调用，用于初始化窗口（保留英文原名：Window1）
    {
        // 构造函数体开始（保留英文原名：Window1）

        InitializeComponent();
        // 调用 InitializeComponent 方法以加载由 XAML（或相应 UI 定义）生成的子控件与资源，完成界面初始化（保留英文原名：InitializeComponent）
    }
    // 构造函数体结束（保留英文原名：Window1）

}
// 类体结束（Window1）
