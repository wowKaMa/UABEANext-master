using Avalonia; // 引用 Avalonia 框架的核心命名空间，用于访问 Application 等类型

using Avalonia.Controls; // 引用 Avalonia 的控件命名空间，包含 Window、Control 等 UI 元素

using Avalonia.Controls.ApplicationLifetimes; // 引用 Avalonia 的应用程序生命周期接口，包含 IClassicDesktopStyleApplicationLifetime

using System; // 引用基础系统命名空间，提供 Exception 等基础类型

namespace UABEANext4.Util; // 定义命名空间 UABEANext4.Util，用于组织工具类（保留原始英文命名）

public class WindowUtils // 定义公共类 WindowUtils，作为窗口相关工具方法的容器（保留原始英文类名）
{
    public static Window GetMainWindow() // 定义公共静态方法 GetMainWindow，用于获取主窗口实例（保留原始英文方法名）
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window }) // 检查当前 Application 是否存在且其 ApplicationLifetime 为经典桌面风格，并解构出 MainWindow（保留英文类型名）
        {
            return window; // 如果找到了主窗口，则返回该 Window 实例
        }

        throw new Exception("未找到窗口！ (Window not found!)"); // 如果未找到主窗口，抛出异常并给出中文提示，括号内保留英文原文
    }
}
