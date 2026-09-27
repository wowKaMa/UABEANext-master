using Avalonia; // 引用 Avalonia 框架的根命名空间，用于构建跨平台 UI 应用
using System; // 引用基础系统命名空间，包含 Exception、AppDomain 等类型
using System.Diagnostics; // 引用诊断命名空间，用于启动外部进程（例如显示系统级弹窗）
using System.IO; // 引用 IO 命名空间，用于读写崩溃日志文件

namespace UABEANext4.Desktop; // 定义此代码所属的命名空间，和项目结构保持一致

class Program // 程序入口类
{
    // 注意：在调用 AppMain 之前不要使用 Avalonia 或依赖 SynchronizationContext 的 API

    [STAThread] // 指定主线程为单线程单元（Windows UI 线程要求）
    public static void Main(string[] args) // 程序入口方法，接收命令行参数
    {
#if !DEBUG
        // 仅在发布（Release）模式下注册全局未处理异常处理器
        var currentDomain = AppDomain.CurrentDomain; // 获取当前应用域
        currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UABEAExceptionHandler);
        // 将 UABEAExceptionHandler 注册为未处理异常回调，确保崩溃时能记录日志并提示用户
#endif

        // 启动 Avalonia 应用并进入经典桌面生命周期（会创建主窗口并处理消息循环）
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // 构建并配置 Avalonia 应用的 AppBuilder（供 Main 调用）
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>() // 指定应用的 App 类型（App 类应在项目中定义）
            .UsePlatformDetect() // 自动检测并使用当前平台的渲染/窗口实现（Windows/Linux/macOS）
            .WithInterFont() // 使用 Inter 字体（项目中可能通过扩展方法引入）
            .LogToTrace(); // 将 Avalonia 日志输出到调试跟踪（便于开发时查看）

    // 全局未处理异常处理器：在发布版捕获未处理异常并记录/提示
    public static void UABEAExceptionHandler(object sender, UnhandledExceptionEventArgs args)
    {
        // 尝试将异常对象转换为 Exception 类型
        if (args.ExceptionObject is Exception ex)
        {
            // 将异常详细信息写入到当前工作目录下的 uabeacrash.log 文件
            // 便于用户上报或开发者排查（注意：写文件可能在极端崩溃场景失败）
            File.WriteAllText("uabeacrash.log", ex.ToString());

            // 如果运行在 Windows 平台，尝试使用系统级弹窗提示用户（避免依赖 Avalonia UI）
            if (OperatingSystem.IsWindows())
            {
                // 构造通过 mshta 执行的 VBScript 参数，脚本会读取日志并弹出系统对话框
                // 这里的消息包含中文提示并保留英文原文，便于不同语言环境的用户理解
                var mshtaArgs = "vbscript:Execute(\"CreateObject(\"\"WScript.Shell\"\").Popup CreateObject(\"\"Scripting.FileSystemObject\"\").OpenTextFile(\"\"uabeacrash.log\"\", 1).ReadAll,,\"\"uabea crash exception (please report this crash with uabeacrash.log)\"\" :close\")";

                // 启动 mshta 进程执行上面的 VBScript，从而显示系统级弹窗
                // 使用 ProcessStartInfo 可以进一步设置 UseShellExecute、Verb 等（此处保持最小化）
                Process.Start(new ProcessStartInfo("mshta", mshtaArgs));
            }
            else
            {
                // 非 Windows 平台（如 Linux/macOS）：在控制台输出崩溃提示与异常详情
                // 这在无 GUI 或 GUI 不可靠时仍能让开发者或用户看到错误信息
                Console.WriteLine("uabea crash exception (please report this crash with uabeacrash.log)");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
