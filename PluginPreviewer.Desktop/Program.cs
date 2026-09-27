using Avalonia;

// 引用 Avalonia 框架的核心命名空间，用于应用程序启动与配置（保留英文原名：Avalonia）

using System;

// 引用 System 命名空间，提供基础类型与特性（例如 STAThread）（保留英文原名：System）

namespace PluginPreviewer.Desktop;

// 定义命名空间 PluginPreviewer.Desktop，用于组织桌面应用相关类型（保留英文原名：PluginPreviewer.Desktop）

class Program

// 定义类 Program，包含应用程序入口点（保留英文原名：Program）
{

    // 类体开始（Program）

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.

    // 注释：初始化相关的说明性注释，提醒开发者在 AppMain 调用前不要使用 Avalonia 或依赖 SynchronizationContext 的代码（保留英文原注释）

    [STAThread]

    // 特性：标记主线程为单线程单元（Single-Threaded Apartment），通常用于桌面应用的主入口（保留英文原名：STAThread）

    public static void Main(string[] args) => BuildAvaloniaApp()

        // 主入口方法 Main：程序启动点，调用 BuildAvaloniaApp 构建应用并启动（保留英文原名：Main / BuildAvaloniaApp / args）

        .StartWithClassicDesktopLifetime(args);

    // 链式调用：使用 Avalonia 的经典桌面生命周期启动应用（StartWithClassicDesktopLifetime），并传入命令行参数（保留英文原名：StartWithClassicDesktopLifetime）

    // Avalonia configuration, don't remove; also used by visual designer.

    // 注释：说明下面的方法用于 Avalonia 的配置，不能删除，视觉设计器也会使用（保留英文原注释）

    public static AppBuilder BuildAvaloniaApp()

        // 静态方法 BuildAvaloniaApp：构建并返回一个配置好的 AppBuilder 实例（保留英文原名：BuildAvaloniaApp / AppBuilder）
        => AppBuilder.Configure<App>()

            // 使用 AppBuilder.Configure 指定应用程序的根类型（这里是 App），开始构建 Avalonia 应用（保留英文原名：AppBuilder.Configure / App）

            .UsePlatformDetect()

            // 调用 UsePlatformDetect 自动检测并使用当前平台的渲染/窗口实现（保留英文原名：UsePlatformDetect）

            .WithInterFont()

            // 调用 WithInterFont 为应用启用 Inter 字体支持（保留英文原名：WithInterFont）

            .LogToTrace();

    // 调用 LogToTrace 将 Avalonia 日志输出到跟踪（Trace），便于调试（保留英文原名：LogToTrace）

}

// 类体结束（Program）
