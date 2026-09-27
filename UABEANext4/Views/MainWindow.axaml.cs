using Avalonia.Controls; // 引用 Avalonia 控件库，用于使用 Window、UserControl、TreeView 等 UI 控件（Avalonia.Controls）

using Avalonia.Input; // 引用输入处理库，用于处理拖放、点击、键盘等输入事件（Avalonia.Input）

using Avalonia.Platform.Storage; // 引用平台存储接口，用于访问文件系统相关的抽象（Avalonia.Platform.Storage）

using System.Linq; // 引用 LINQ 扩展方法，用于对集合进行筛选、投影、聚合等操作（System.Linq）

using System.Threading.Tasks; // 引用异步任务类型，用于定义和返回 Task（System.Threading.Tasks）

using UABEANext4.ViewModels; // 引用项目的视图模型命名空间，MainViewModel 等类型位于此处（UABEANext4.ViewModels）

using System.IO; // 引用文件 IO 命名空间，用于检查文件是否存在等文件操作（System.IO）

#if DEBUG // 条件编译：以下代码仅在 DEBUG（调试）模式下包含
using Avalonia.Diagnostics; // 在调试模式下引用 Avalonia 的诊断工具（Avalonia.Diagnostics）
using UABEANext4.Logic.DevTools; // 在调试模式下引用项目自定义的开发者工具逻辑（UABEANext4.Logic.DevTools）
#endif // 结束条件编译

namespace UABEANext4.Views; // 定义此文件所属的命名空间为 UABEANext4.Views（视图层）

public partial class MainWindow : Window // 定义主窗口类 MainWindow，继承自 Avalonia 的 Window（主窗口）
{
    public MainWindow() // 构造函数：当 MainWindow 实例化时执行（窗口初始化入口）
    {
#if DEBUG // 调试模式下的初始化逻辑（仅在 DEBUG 编译时包含）
        InitializeComponent(attachDevTools: false); // 初始化组件并加载 XAML（不附加原生开发工具）
        DevToolsAdblock.Attach(this, new DevToolsOptions()); // 附加自定义的开发者工具（DevToolsAdblock），便于调试
#else // 发布/非调试模式下的初始化逻辑
        InitializeComponent(); // 标准初始化组件：加载并解析与窗口关联的 XAML
#endif // 结束条件编译分支

        AddHandler(DragDrop.DropEvent, Drop); // 注册拖放事件处理器：当发生 Drop 事件时调用 Drop 方法

        Opened += async (s, e) => await HandleCommandLineArgs(); // 窗口打开（Opened）时异步处理命令行参数（调用 HandleCommandLineArgs）
    }

    private async Task HandleCommandLineArgs() // 异步方法：处理程序启动时的命令行参数
    {
        var args = System.Environment.GetCommandLineArgs(); // 获取启动程序时传入的所有命令行参数（第一个通常是可执行文件路径）

        var filePaths = args.Skip(1) // 跳过第一个参数（程序自身路径），从第二个参数开始处理
            .Where(arg => !string.IsNullOrWhiteSpace(arg)) // 过滤掉空或仅包含空白的参数
            .Where(arg => File.Exists(arg)) // 仅保留在磁盘上实际存在的文件路径
            .ToList(); // 将结果转换为 List<string>（filePaths）

        if (filePaths.Any() && DataContext is MainViewModel viewModel) // 如果存在有效文件路径且 DataContext 是 MainViewModel
        {
            await viewModel.OpenFiles(filePaths); // 调用 ViewModel 的 OpenFiles 方法异步打开这些文件
        }
    }

    private async Task Drop(object? sender, DragEventArgs e) // 异步方法：处理拖放（Drop）事件，sender 为事件源，e 为事件参数
    {
        if (e.DataTransfer.TryGetFiles() is { } files && DataContext is MainViewModel viewModel) // 如果拖放数据包含文件且 DataContext 是 MainViewModel
        {
            var fileNames = files.Select(sf => sf.TryGetLocalPath()).Where(p => p != null); // 将每个 IStorageFile 尝试转换为本地路径（可能为 null），并过滤掉 null 值

            if (fileNames is not null) // 如果 fileNames 非空（注意：fileNames 是 IEnumerable<string?> 的筛选结果）
            {
                await viewModel.OpenFiles(fileNames); // 调用 ViewModel 的 OpenFiles 方法异步打开这些本地路径文件
            }
        }
    }
}
