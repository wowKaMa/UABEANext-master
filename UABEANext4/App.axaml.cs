using System; // 基础命名空间，包含 Exception、IServiceProvider、基本类型等
using System.Collections.Generic; // 包含泛型集合类型，如 Dictionary<TKey,TValue>
using System.Text.RegularExpressions; // 包含正则表达式类型，用于更灵活的匹配菜单文本
using System.Reflection; // 反射，用于尝试设置自定义菜单项的常见属性
using Avalonia; // Avalonia 框架根命名空间，包含应用启动与平台相关的核心类型
using Avalonia.Collections; // Avalonia 的集合类型（如 AvaloniaList），用于操作 Avalonia 特定集合
using Avalonia.Controls; // Avalonia 控件命名空间，包含 Control、Window、TextBox、ContextMenu、MenuItem 等
using Avalonia.Controls.ApplicationLifetimes; // 应用生命周期接口（IClassicDesktopStyleApplicationLifetime 等）
using Avalonia.Interactivity; // 交互事件命名空间，包含 RoutedEvent 与事件参数类型
using Avalonia.Markup.Xaml; // XAML 加载器，用于加载 App.xaml 等资源
using Avalonia.Threading; // 调度器命名空间，用于在 UI 线程上以特定优先级调度操作
using CommunityToolkit.Mvvm.DependencyInjection; // CommunityToolkit MVVM 的依赖注入辅助（Ioc.Default）
using Microsoft.Extensions.DependencyInjection; // Microsoft 的依赖注入容器类型（ServiceCollection）
using UABEANext4.Logic.Configuration; // 项目配置管理命名空间（ConfigurationManager）
using UABEANext4.Services; // 项目服务接口与实现（IDialogService、DialogService、DummyDialogService）
using UABEANext4.ViewModels; // 视图模型命名空间（MainViewModel 等）
using UABEANext4.Views; // 视图命名空间（MainWindow、MainView 等）

namespace UABEANext4; // 定义当前文件所属的命名空间，组织项目代码结构

public partial class App : Application // 应用程序类，继承自 Avalonia.Application；partial 便于分文件维护
{
    public override void Initialize() // 覆写 Initialize：在应用启动时加载 XAML 资源与样式
    {
        AvaloniaXamlLoader.Load(this); // 加载与 App 关联的 XAML（例如 App.xaml 中的资源字典）
    }

    public override void OnFrameworkInitializationCompleted() // 框架初始化完成回调：配置主窗口、依赖注入与全局行为
    {
        // 原注释：避免 Avalonia 与 CommunityToolkit 的重复数据验证（保留）
        //BindingPlugins.DataValidators.RemoveAt(0);
        // 说明：上面被注释的代码用于移除重复的数据验证插件（如果启用会避免双重验证）

        Window? mainWindow = null; // 可空的主窗口引用，桌面场景时会赋值

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) // 经典桌面生命周期（Windows/Linux）
        {
            mainWindow = desktop.MainWindow = new MainWindow // 创建并设置 MainWindow 为桌面主窗口
            {
                DataContext = new MainViewModel() // 为主窗口设置数据上下文（MainViewModel 实例）
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform) // 单视图平台（移动或单视图）
        {
            singleViewPlatform.MainView = new MainView // 设置单视图平台的主视图为 MainView
            {
                DataContext = new MainViewModel() // 为主视图设置数据上下文（MainViewModel 实例）
            };
        }

        var provider = ConfigureServices(mainWindow); // 配置依赖注入并获取 IServiceProvider
        Ioc.Default.ConfigureServices(provider); // 将服务提供器注册到 CommunityToolkit 的 Ioc.Default

        // 触发配置管理器构造与初始化（保留原文注释）
        if (!ConfigurationManager.IsInitialized) // 检查配置管理器是否已初始化
            throw new Exception("期望配置管理器已初始化 (Expected config man to initialize)"); // 若未初始化则抛出异常

        // ===== 为 TextBox 添加默认右键菜单（剪切/复制/粘贴） =====
        // 关键点：使用 Control.LoadedEvent.AddClassHandler<TextBox> 在控件加载时设置菜单，避免 XAML 绑定不存在的命令

        Control.LoadedEvent.AddClassHandler<TextBox>((tb, e) => // 为所有 TextBox 注册 Loaded 事件的类处理器
        {
            try
            {
                if (tb.ContextMenu == null) // 若 TextBox 没有 ContextMenu，则创建默认菜单
                {
                    var menu = new ContextMenu(); // 新建 ContextMenu

                    var cut = new MenuItem { Header = "剪切（Cut Ctrl+X）" }; // 剪切菜单项，中文 + 英文快捷键提示
                    cut.Click += (_, __) => { try { tb.Cut(); } catch { } }; // 点击时调用 TextBox.Cut()

                    var copy = new MenuItem { Header = "复制（Copy Ctrl+C）" }; // 复制菜单项
                    copy.Click += (_, __) => { try { tb.Copy(); } catch { } }; // 点击时调用 TextBox.Copy()

                    var paste = new MenuItem { Header = "粘贴（Paste Ctrl+V）" }; // 粘贴菜单项
                    paste.Click += (_, __) => { try { tb.Paste(); } catch { } }; // 点击时调用 TextBox.Paste()

                    // Items 是只读集合，正确做法是向集合中添加项而不是整体赋值
                    menu.Items.Add(cut); // 添加剪切项
                    menu.Items.Add(copy); // 添加复制项
                    menu.Items.Add(paste); // 添加粘贴项

                    tb.ContextMenu = menu; // 将菜单赋给 TextBox
                }
                else
                {
                    // 如果已有 ContextMenu，则尝试为其中匹配的 MenuItem 绑定 Click 事件（避免覆盖自定义菜单）
                    if (tb.ContextMenu.Items is System.Collections.IEnumerable items) // 获取已有菜单项集合
                    {
                        foreach (var it in items) // 遍历集合
                        {
                            if (it is MenuItem mi) // 仅处理 MenuItem
                            {
                                var header = mi.Header?.ToString() ?? string.Empty; // 读取 Header 文本
                                if (header.Contains("剪切") && !HasClickHandler(mi)) // 若包含“剪切”且未绑定过
                                    mi.Click += (_, __) => { try { tb.Cut(); } catch { } }; // 绑定 Cut
                                else if (header.Contains("复制") && !HasClickHandler(mi)) // 若包含“复制”
                                    mi.Click += (_, __) => { try { tb.Copy(); } catch { } }; // 绑定 Copy
                                else if (header.Contains("粘贴") && !HasClickHandler(mi)) // 若包含“粘贴”
                                    mi.Click += (_, __) => { try { tb.Paste(); } catch { } }; // 绑定 Paste
                            }
                        }
                    }
                }
            }
            catch
            {
                // 忽略异常，保证应用启动稳健
            }
        });

        // ===== 运行时本地化策略（主方案：MenuItem 加载时本地化；补偿：ContextRequested/Open） =====
        // 说明：优先在 MenuItem 加载时处理，确保第一次右键即可看到本地化；同时保留 ContextRequested/Open 的补偿以覆盖极端动态场景。

        // 本地化映射表（可扩展或外置配置）
        var menuTranslations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Float", "浮动" },
            { "Float all", "全部浮动" },
            { "Dock", "停靠" },
            { "Dock as Tabbed Document", "停靠为选项卡文档" },
            { "Auto Hide", "自动隐藏" },
            { "Close", "关闭" },
            { "Close Other Tabs", "关闭其他选项卡" },
            { "Close All Tabs", "关闭所有选项卡" },
            { "Close Tabs to the Left", "关闭左侧选项卡" },
            { "Close Tabs to the Right", "关闭右侧选项卡" },
            { "Edit Asset", "编辑资源" },
            { "Visit Asset", "访问资源" },
            { "Expand Selection", "展开选择" },
            { "Collapse Selection", "折叠选择" },
            { "Tab Layout", "选项卡布局" },
            { "Place Tabs on the Left", "将选项卡放在左侧" },
            { "Place Tabs on the Top", "将选项卡放在顶部" },
            { "Place Tabs on the Right", "将选项卡放在右侧" }
        };

        // 标记键：用于 Resources 标记，避免与 Tag 冲突
        const string MenuItemMarkKey = "LocalizedByApp_v4";
        const string ContextMenuMarkKey = "ContextMenuLocalizedHandler_v4";

        // 规范化 Header 文本：去除换行与多余空白
        static string NormalizeHeader(string? s) => s?.Trim().Replace("\r", "").Replace("\n", " ") ?? string.Empty;

        // 判断是否已本地化（检测中文括号 '（'）
        static bool IsAlreadyLocalized(string s) => !string.IsNullOrEmpty(s) && s.Contains("（");

        // 尝试设置对象的文本属性（支持 MenuItem、或自定义项通过常见属性）
        void TrySetHeader(object item, string newHeader)
        {
            if (item is MenuItem mi) // 标准 MenuItem 直接设置 Header
            {
                mi.Header = newHeader;
                return;
            }

            // 反射尝试常见属性名（Header、Text、Content、Label、Title）
            var t = item.GetType();
            foreach (var propName in new[] { "Header", "Text", "Content", "Label", "Title" })
            {
                var prop = t.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.CanWrite && prop.PropertyType == typeof(string))
                {
                    prop.SetValue(item, newHeader);
                    return;
                }
            }

            // 若无法设置（自定义类型且无可写字符串属性），则跳过；可在此处扩展以支持更多类型
        }

        // 单项本地化逻辑（对单个 MenuItem 或任意对象尝试匹配并替换）
        void LocalizeSingleItem(object item)
        {
            if (item == null) return;

            string headerText = NormalizeHeader(item is MenuItem m ? m.Header?.ToString() : item.ToString());
            if (string.IsNullOrEmpty(headerText) || IsAlreadyLocalized(headerText)) return;

            // 完全或包含匹配
            foreach (var kv in menuTranslations)
            {
                if (string.Equals(headerText, kv.Key, StringComparison.OrdinalIgnoreCase) ||
                    headerText.IndexOf(kv.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    TrySetHeader(item, $"{kv.Value}（{headerText}）");
                    return;
                }
            }

            // 单词边界正则匹配（匹配变体）
            foreach (var kv in menuTranslations)
            {
                var pattern = @"\b" + Regex.Escape(kv.Key) + @"\b";
                if (Regex.IsMatch(headerText, pattern, RegexOptions.IgnoreCase))
                {
                    TrySetHeader(item, $"{kv.Value}（{headerText}）");
                    return;
                }
            }
        }

        // 递归处理集合（用于 ContextMenu.Opened 的补偿）
        void LocalizeItemsRecursive(System.Collections.IEnumerable items)
        {
            if (items == null) return;
            foreach (var it in items)
            {
                LocalizeSingleItem(it); // 先尝试本地化当前项
                if (it is MenuItem mi && mi.Items is System.Collections.IEnumerable sub) // 若有子项则递归
                    LocalizeItemsRecursive(sub);
            }
        }

        // 类处理器：在 MenuItem 加载时立即本地化（优先级高，解决第一次右键未生效问题）
        Control.LoadedEvent.AddClassHandler<MenuItem>((miObj, e) =>
        {
            try
            {
                var mi = miObj as MenuItem;
                if (mi == null) return;

                // 使用 Resources 标记避免重复处理
                if (mi.Resources.ContainsKey(MenuItemMarkKey)) return;
                mi.Resources[MenuItemMarkKey] = true;

                // 立即本地化自身 Header
                LocalizeSingleItem(mi);

                // 若有子项，递归本地化子项（子项可能尚未加载，但多数情况下会被创建）
                if (mi.Items is System.Collections.IEnumerable sub)
                    LocalizeItemsRecursive(sub);
            }
            catch
            {
                // 忽略异常，保证稳健
            }
        });

        // 补偿：在 ContextRequested 时附加 ContextMenu.Opened 的双阶段处理（立即 + 延迟），以覆盖极端情况
        Control.ContextRequestedEvent.AddClassHandler<Control>((ctrl, args) =>
        {
            try
            {
                var cm = ctrl.ContextMenu;
                if (cm != null)
                {
                    // 标记并附加 Opened（避免重复）
                    if (!cm.Resources.ContainsKey(ContextMenuMarkKey))
                    {
                        cm.Resources[ContextMenuMarkKey] = true;
                        cm.Opened += (s, ev) =>
                        {
                            try
                            {
                                LocalizeItemsRecursive(cm.Items); // 立即处理
                                // 延迟再处理一次，捕获在 Opened 内或异步添加的项
                                Dispatcher.UIThread.Post(() => { try { LocalizeItemsRecursive(cm.Items); } catch { } }, DispatcherPriority.Background);
                            }
                            catch { }
                        };
                    }

                    // 立即尝试一次（若项已存在）
                    LocalizeItemsRecursive(cm.Items);
                }
                else
                {
                    // 延迟检查：某些库在 ContextRequested 后才创建 ContextMenu
                    Dispatcher.UIThread.Post(() =>
                    {
                        try
                        {
                            var cm2 = ctrl.ContextMenu;
                            if (cm2 != null)
                            {
                                if (!cm2.Resources.ContainsKey(ContextMenuMarkKey))
                                {
                                    cm2.Resources[ContextMenuMarkKey] = true;
                                    cm2.Opened += (s, ev) =>
                                    {
                                        try
                                        {
                                            LocalizeItemsRecursive(cm2.Items);
                                            Dispatcher.UIThread.Post(() => { try { LocalizeItemsRecursive(cm2.Items); } catch { } }, DispatcherPriority.Background);
                                        }
                                        catch { }
                                    };
                                }

                                LocalizeItemsRecursive(cm2.Items);
                            }
                        }
                        catch { }
                    }, DispatcherPriority.Input);
                }
            }
            catch
            {
                // 忽略任何异常，保证拦截逻辑不会影响菜单显示或应用稳定性
            }
        });

        base.OnFrameworkInitializationCompleted(); // 调用基类实现，完成框架初始化后的默认行为
    }

    private IServiceProvider ConfigureServices(Window? mainWindow) // 配置依赖注入服务的方法，接收可选的主窗口引用
    {
        var services = new ServiceCollection(); // 创建 ServiceCollection 用于注册服务

        var viewLocator = new ViewLocator(); // 创建 ViewLocator（用于对话框服务定位视图）
        if (mainWindow != null) // 若存在主窗口（桌面场景）
            services.AddSingleton<IDialogService>(new DialogService(mainWindow, viewLocator)); // 注册 DialogService 实例
        else
            services.AddSingleton<IDialogService, DummyDialogService>(); // 否则注册占位 DummyDialogService

        return services.BuildServiceProvider(); // 构建并返回 IServiceProvider
    }

    // 辅助方法：检查 MenuItem 是否已被绑定过 Click 事件（使用 Tag 做简单标记以避免重复绑定）
    private static bool HasClickHandler(MenuItem mi)
    {
        if (mi.Tag is string s && s == "BoundByApp") // 若 Tag 已标记为 "BoundByApp"
            return true; // 返回 true 表示已绑定

        mi.Tag = "BoundByApp"; // 否则标记该 MenuItem，表示已绑定
        return false; // 返回 false 表示之前未绑定
    }
}
