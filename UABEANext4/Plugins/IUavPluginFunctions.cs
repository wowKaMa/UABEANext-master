using Avalonia.Platform.Storage;
// 引用 Avalonia 的存储平台接口命名空间，用于跨平台文件/文件夹对话框（原名：Avalonia.Platform.Storage）

using System.Threading.Tasks;
// 引用异步任务支持命名空间，提供 Task、async/await 等异步功能（原名：System.Threading.Tasks）

using UABEANext4.Interfaces;
// 引用项目内的接口命名空间，用于访问 IDialogAware 等接口定义（原名：UABEANext4.Interfaces）

namespace UABEANext4.Plugins;
// 定义命名空间 UABEANext4.Plugins，用于组织插件相关类型（原名：UABEANext4.Plugins）

public interface IUavPluginFunctions
// 定义公共接口 IUavPluginFunctions（原名：IUavPluginFunctions），声明插件可以调用的宿主功能集合
{
    // 接口体开始

    public Task<string[]> ShowOpenFileDialog(FilePickerOpenOptions options);
    // 异步方法声明 ShowOpenFileDialog（原名：ShowOpenFileDialog）
    // 作用：打开“打开文件”对话框（FilePickerOpenOptions），异步返回所选文件的本地路径数组（Task<string[]>）

    public Task<string?> ShowSaveFileDialog(FilePickerSaveOptions options);
    // 异步方法声明 ShowSaveFileDialog（原名：ShowSaveFileDialog）
    // 作用：打开“保存文件”对话框（FilePickerSaveOptions），异步返回用户选择的保存路径或 null（Task<string?>）

    public Task<string?> ShowOpenFolderDialog(FolderPickerOpenOptions options);
    // 异步方法声明 ShowOpenFolderDialog（原名：ShowOpenFolderDialog）
    // 作用：打开“选择文件夹”对话框（FolderPickerOpenOptions），异步返回所选文件夹路径或 null（Task<string?>）

    public Task<T?> ShowDialog<T>(IDialogAware<T> dialogAware);
    // 泛型异步方法声明 ShowDialog<T>（原名：ShowDialog）
    // 作用：显示一个由插件提供的对话框视图模型（IDialogAware<T>），并异步返回对话框的结果（可能为 null），返回类型为 Task<T?>；T 为结果类型参数

    public Task ShowMessageDialog(string title, string message);
    // 异步方法声明 ShowMessageDialog（原名：ShowMessageDialog）
    // 作用：显示一个简单的消息对话框，接收标题（title）和消息内容（message），返回 Task 表示异步完成
}
// 接口体结束
