using Avalonia; // 引用 Avalonia 框架核心，用于访问 Application 等全局对象
using Avalonia.Controls; // 引用 Avalonia 控件命名空间，包含 Window、TopLevel 等 UI 元素
using Avalonia.Controls.ApplicationLifetimes; // 引用应用生命周期接口，判断当前是桌面还是单视图应用
using Avalonia.Platform.Storage; // 引用存储相关接口，包含 IStorageProvider、FilePickerFileType 等
using Avalonia.VisualTree; // 引用可视树扩展，用于从视图获取 TopLevel 等可视根

namespace UABEANext4.Util; // 定义命名空间 UABEANext4.Util，用于放置工具类

internal static class StorageService // 定义内部静态类 StorageService，封装与平台存储相关的帮助方法
{
    public static FilePickerFileType All { get; } = new("全部 (All)") // 定义一个表示“全部文件”类型的 FilePickerFileType，显示为中文并保留英文原名 (All)
    {
        Patterns = new[] { "*.*" }, // 匹配所有文件名模式
        MimeTypes = new[] { "*/*" } // 匹配所有 MIME 类型
    };

    public static FilePickerFileType Json { get; } = new("Json (Json)") // 定义一个表示 JSON 文件类型的 FilePickerFileType，显示为中文保留英文原名 (Json)
    {
        Patterns = new[] { "*.json" }, // 匹配 .json 文件扩展名
        AppleUniformTypeIdentifiers = new[] { "public.json" }, // 在 macOS/iOS 上使用的统一类型标识符
        MimeTypes = new[] { "application/json" } // 对应的 MIME 类型
    };

    public static IStorageProvider? GetStorageProvider() // 公共静态方法：获取当前应用可用的 IStorageProvider，可能返回 null
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window }) // 如果是经典桌面应用并且存在 MainWindow
        {
            return window.StorageProvider; // 返回主窗口的 StorageProvider（桌面平台常用）
        }

        if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime { MainView: { } mainView }) // 如果是单视图应用（移动或嵌入式场景）
        {
            var visualRoot = mainView.GetVisualRoot(); // 获取主视图的可视根
            if (visualRoot is TopLevel topLevel) // 如果可视根是 TopLevel（顶层窗口）
            {
                return topLevel.StorageProvider; // 返回该 TopLevel 的 StorageProvider（适用于单视图平台）
            }
        }

        return null; // 未能找到合适的 StorageProvider 时返回 null
    }
}
