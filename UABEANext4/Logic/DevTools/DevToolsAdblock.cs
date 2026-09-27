using Avalonia.Controls;
// 引用 Avalonia 的控件命名空间，用于访问 TopLevel、UserControl、ContentControl、Grid 等控件类型（原名：Avalonia.Controls）

using Avalonia.Diagnostics;
// 引用 Avalonia 的调试工具命名空间，用于 DevTools 相关类型（原名：Avalonia.Diagnostics）

using Avalonia.Input;
// 引用 Avalonia 的输入命名空间，用于 KeyGesture、KeyEventArgs 等输入相关类型（原名：Avalonia.Input）

using Avalonia.Interactivity;
// 引用 Avalonia 的交互事件命名空间，用于路由事件等（原名：Avalonia.Interactivity）

using Avalonia.VisualTree;
// 引用 Avalonia 的视觉树扩展命名空间，用于查找子控件、遍历视觉树等（原名：Avalonia.VisualTree）

using System;
// 引用基础系统命名空间，提供 IDisposable、ArgumentNullException 等类型（原名：System）

using System.Collections;
// 引用非泛型集合命名空间，用于 IDictionary 等接口（原名：System.Collections）

using System.Reflection;
// 引用反射命名空间，用于动态获取类型、方法、字段（原名：System.Reflection）

namespace UABEANext4.Logic.DevTools;
// 定义命名空间 UABEANext4.Logic.DevTools，用于组织开发者工具相关逻辑（原名：UABEANext4.Logic.DevTools）

public static class DevToolsAdblock
// 定义公共静态类 DevToolsAdblock（原名：DevToolsAdblock），用于拦截并修改 Avalonia DevTools 的 UI（例如移除超链接按钮）
{
    // 类体开始（DevToolsAdblock）

#if DEBUG
    // 预处理指令：仅在 DEBUG 模式下编译以下代码块（原名：#if DEBUG）

    public static IDisposable Attach(TopLevel root, KeyGesture gesture)
    // 公共静态方法 Attach(TopLevel root, KeyGesture gesture)：便捷重载，使用 KeyGesture 创建 DevToolsOptions 并附加处理器（原名：Attach）
    {
        // 方法体开始

        return Attach(root, new DevToolsOptions()
        {
            Gesture = gesture,
        });
        // 调用另一个重载 Attach(TopLevel, DevToolsOptions)，传入新建的 DevToolsOptions（将 Gesture 设置为传入的 gesture），并返回其返回的 IDisposable（原名：DevToolsOptions / Gesture）
    }
    // 方法体结束

    public static IDisposable Attach(TopLevel root, DevToolsOptions options)
    // 公共静态方法 Attach(TopLevel root, DevToolsOptions options)：主入口，绑定键盘事件以打开 DevTools 并移除不需要的超链接按钮（原名：Attach）
    {
        // 方法体开始

        void PreviewKeyDown(object? sender, KeyEventArgs e)
        // 局部方法 PreviewKeyDown(object? sender, KeyEventArgs e)：作为按键预览事件处理器（原名：PreviewKeyDown）
        {
            // 局部方法体开始

            if (options.Gesture.Matches(e))
            // 如果按下的按键与 options 中配置的手势匹配（原名：options.Gesture / Matches）
            {
                // 条件块开始

                var dtAsm = typeof(DevToolsOptions).Assembly;
                // 通过 DevToolsOptions 类型获取其所在的程序集（Assembly），用于反射查找 DevTools 类型（原名：DevToolsOptions / Assembly）

                var devTools = dtAsm.GetType("Avalonia.Diagnostics.DevTools");
                // 在该程序集内查找名为 "Avalonia.Diagnostics.DevTools" 的类型（原名：GetType / "Avalonia.Diagnostics.DevTools"）

                if (devTools is null)
                    return;
                // 如果未找到该类型则直接返回（不做任何操作）

                var open = devTools.GetMethod(
                    "Open",
                    BindingFlags.Static | BindingFlags.Public,
                    [typeof(TopLevel), typeof(DevToolsOptions)]
                );
                // 通过反射查找名为 "Open" 的静态公共方法，期望参数类型为 (TopLevel, DevToolsOptions)（原名：GetMethod / BindingFlags / typeof）

                if (open is null)
                    return;
                // 如果未找到 Open 方法则返回

                open.Invoke(null, [root, options]);
                // 调用静态方法 Open，传入 root 与 options（第一个参数为 null 表示静态方法），以打开 DevTools（原名：Invoke）

                var s_open = devTools.GetField(
                    "s_open",
                    BindingFlags.Static | BindingFlags.NonPublic
                );
                // 通过反射获取名为 "s_open" 的静态非公共字段（原名：GetField / "s_open" / BindingFlags）

                if (s_open is null)
                    return;
                // 如果未找到该字段则返回

                if (s_open.GetValue(null) is not IDictionary sOpenVal)
                    return;
                // 读取静态字段的值并尝试将其转换为 IDictionary；如果不是则返回（原名：GetValue / IDictionary）

                foreach (var value in sOpenVal.Values)
                // 遍历 s_open 字典的所有值（原名：Values / foreach）
                {
                    // 循环体开始

                    if (value is not ContentControl contentCtrl)
                        continue;
                    // 如果当前值不是 ContentControl，则跳过（原名：ContentControl）

                    if (contentCtrl.Content is not UserControl content)
                        continue;
                    // 如果 ContentControl 的 Content 不是 UserControl，则跳过（原名：UserControl）

                    var rootGrid = content.FindControl<Grid>("rootGrid");
                    // 在 UserControl 中查找名为 "rootGrid" 的 Grid 控件（原名：FindControl / Grid / "rootGrid"）

                    if (rootGrid is null)
                        continue;
                    // 如果未找到 rootGrid 则跳过

                    var hyperlinkButton = rootGrid.FindDescendantOfType<HyperlinkButton>();
                    // 在 rootGrid 的子孙中查找第一个 HyperlinkButton（原名：FindDescendantOfType / HyperlinkButton）

                    if (hyperlinkButton is null)
                        continue;
                    // 如果未找到 HyperlinkButton 则跳过

                    rootGrid.Children.Remove(hyperlinkButton);
                    // 从 rootGrid 的子元素集合中移除找到的 HyperlinkButton，从而“屏蔽”该超链接按钮（原名：Children.Remove）
                }
                // 循环体结束
            }
            // 条件块结束
        }
        // 局部方法体结束

        return (root ?? throw new ArgumentNullException(nameof(root))).AddDisposableHandler(
            InputElement.KeyDownEvent,
            PreviewKeyDown,
            RoutingStrategies.Tunnel);
        // 将 PreviewKeyDown 作为可释放的事件处理器附加到 root（TopLevel）上，监听 KeyDown 事件（隧道路由），并返回一个 IDisposable 以便后续移除处理器
        // 说明：如果 root 为 null 则抛出 ArgumentNullException（原名：ArgumentNullException / AddDisposableHandler / InputElement.KeyDownEvent / RoutingStrategies.Tunnel）
    }
    // 方法体结束

#endif
    // 预处理指令结束（#endif），仅在 DEBUG 模式下包含上面的实现

}
// 类体结束（DevToolsAdblock）
