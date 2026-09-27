using Avalonia;
// 引用 Avalonia 框架的核心命名空间，用于应用程序启动与配置（保留英文原名：Avalonia）

using Avalonia.Controls.ApplicationLifetimes;
// 引用 Avalonia 的应用程序生命周期接口命名空间，提供 IClassicDesktopStyleApplicationLifetime 等类型（保留英文原名：Avalonia.Controls.ApplicationLifetimes）

using Avalonia.Markup.Xaml;
// 引用 Avalonia 的 XAML 加载器命名空间，提供 AvaloniaXamlLoader（保留英文原名：Avalonia.Markup.Xaml）

namespace PluginPreviewer.Desktop;
// 定义命名空间 PluginPreviewer.Desktop，用于组织桌面应用相关类型（保留英文原名：PluginPreviewer.Desktop）

public partial class App : Application
// 定义部分类 App，继承自 Avalonia 的 Application，表示应用程序的全局入口与配置点（保留英文原名：App / Application）
{
    // 类体开始（App）

    public override void Initialize()
    // 重写 Initialize 方法：在应用启动早期用于加载资源、样式或 XAML（保留英文原名：Initialize）
    {
        // 方法体开始（Initialize）

        AvaloniaXamlLoader.Load(this);
        // 使用 AvaloniaXamlLoader 加载与此类关联的 XAML（通常用于注册样式、资源和控件模板）（保留英文原名：AvaloniaXamlLoader.Load / this）

    }
    // 方法体结束（Initialize）

    public override void OnFrameworkInitializationCompleted()
    // 重写 OnFrameworkInitializationCompleted：当 Avalonia 框架初始化完成后调用，用于创建主窗口并完成启动流程（保留英文原名：OnFrameworkInitializationCompleted）
    {
        // 方法体开始（OnFrameworkInitializationCompleted）

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        // 检查当前应用的生命周期是否为经典桌面样式（IClassicDesktopStyleApplicationLifetime），如果是则可以设置 MainWindow（保留英文原名：ApplicationLifetime / IClassicDesktopStyleApplicationLifetime）
        {
            // if 体开始

            desktop.MainWindow = new Window1();
            // 创建并分配主窗口实例 Window1 给 desktop.MainWindow，使该窗口成为应用的主窗口（保留英文原名：desktop.MainWindow / new Window1()）

        }
        // if 体结束

        base.OnFrameworkInitializationCompleted();
        // 调用基类实现以确保框架的其余初始化逻辑执行完毕（保留英文原名：base.OnFrameworkInitializationCompleted）

    }
    // 方法体结束（OnFrameworkInitializationCompleted）

}
// 类体结束（App）
