using Avalonia.Platform.Storage;
// 引用 Avalonia 的存储平台接口命名空间，用于跨平台文件/文件夹对话框（原名：Avalonia.Platform.Storage）

using CommunityToolkit.Mvvm.DependencyInjection;
// 引用 CommunityToolkit 的依赖注入工具，用于通过 Ioc 获取服务（原名：CommunityToolkit.Mvvm.DependencyInjection）

using System;
// 引用基础系统命名空间，提供基础类型与异常等（原名：System）

using System.Threading.Tasks;
// 引用异步任务支持命名空间，提供 Task、async/await（原名：System.Threading.Tasks）

using UABEANext4.Interfaces;
// 引用项目内接口命名空间，包含 IUavPluginFunctions、IDialogAware 等接口（原名：UABEANext4.Interfaces）

using UABEANext4.Services;
// 引用项目内服务命名空间，包含 IDialogService、StorageService 等（原名：UABEANext4.Services）

using UABEANext4.Util;
// 引用项目内工具命名空间，包含 FileDialogUtils 等实用方法（原名：UABEANext4.Util）

using UABEANext4.ViewModels.Dialogs;
// 引用对话框视图模型命名空间，包含 MessageBoxViewModel、MessageBoxType（原名：UABEANext4.ViewModels.Dialogs）

namespace UABEANext4.Plugins;
// 定义命名空间 UABEANext4.Plugins，用于组织插件相关类型（原名：UABEANext4.Plugins）

public class UavPluginFunctions : IUavPluginFunctions
// 定义公共类 UavPluginFunctions（原名：UavPluginFunctions），实现接口 IUavPluginFunctions（原名：IUavPluginFunctions）
{
    // 类体开始

    private readonly IDialogService _dialogService;
    // 私有只读字段 _dialogService：保存注入的对话框服务实例（类型：IDialogService，原名：IDialogService）

    private readonly IStorageProvider _storageProvider;
    // 私有只读字段 _storageProvider：保存平台存储提供者实例（类型：IStorageProvider，原名：IStorageProvider）

    public UavPluginFunctions()
    // 公共构造函数 UavPluginFunctions()：用于初始化服务依赖（原名：UavPluginFunctions）
    {
        // 构造函数体开始

        _dialogService = Ioc.Default.GetRequiredService<IDialogService>();
        // 通过依赖注入容器 Ioc 获取必需的 IDialogService 实例并赋值给 _dialogService（原名：Ioc.Default.GetRequiredService<IDialogService>）

        var storageProvider = StorageService.GetStorageProvider() ??
            throw new InvalidOperationException("The requested service type was not registered.");
        // 尝试通过 StorageService 获取 IStorageProvider（原名：StorageService.GetStorageProvider），
        // 如果返回 null 则抛出 InvalidOperationException（原名：InvalidOperationException）提示服务未注册

        _storageProvider = storageProvider;
        // 将获取到的 storageProvider 赋值给私有字段 _storageProvider
    }
    // 构造函数体结束

    public async Task<string[]> ShowOpenFileDialog(FilePickerOpenOptions options)
    // 公共异步方法 ShowOpenFileDialog：接收 FilePickerOpenOptions（原名：FilePickerOpenOptions），返回所选文件的本地路径数组（原名：ShowOpenFileDialog）
    {
        // 方法体开始

        var result = await _storageProvider.OpenFilePickerAsync(options);
        // 调用 IStorageProvider 的 OpenFilePickerAsync 异步方法打开文件选择对话框并等待结果（原名：OpenFilePickerAsync）

        return FileDialogUtils.GetOpenFileDialogFiles(result);
        // 使用 FileDialogUtils.GetOpenFileDialogFiles 将 IStorageFile 列表转换为本地路径字符串数组并返回（原名：FileDialogUtils.GetOpenFileDialogFiles）
    }
    // 方法体结束

    public async Task<string?> ShowSaveFileDialog(FilePickerSaveOptions options)
    // 公共异步方法 ShowSaveFileDialog：接收 FilePickerSaveOptions（原名：FilePickerSaveOptions），返回保存文件的本地路径或 null（原名：ShowSaveFileDialog）
    {
        // 方法体开始

        var result = await _storageProvider.SaveFilePickerAsync(options);
        // 调用 IStorageProvider 的 SaveFilePickerAsync 异步方法打开保存对话框并等待结果（原名：SaveFilePickerAsync）

        return FileDialogUtils.GetSaveFileDialogFile(result);
        // 使用 FileDialogUtils.GetSaveFileDialogFile 将 IStorageFile 转换为本地路径字符串并返回（原名：FileDialogUtils.GetSaveFileDialogFile）
    }
    // 方法体结束

    public async Task<string?> ShowOpenFolderDialog(FolderPickerOpenOptions options)
    // 公共异步方法 ShowOpenFolderDialog：接收 FolderPickerOpenOptions（原名：FolderPickerOpenOptions），返回所选文件夹路径或 null（原名：ShowOpenFolderDialog）
    {
        // 方法体开始

        var result = await _storageProvider.OpenFolderPickerAsync(options);
        // 调用 IStorageProvider 的 OpenFolderPickerAsync 异步方法打开选择文件夹对话框并等待结果（原名：OpenFolderPickerAsync）

        var folders = FileDialogUtils.GetOpenFolderDialogFolders(result);
        // 使用 FileDialogUtils.GetOpenFolderDialogFolders 将 IStorageFolder 列表转换为本地路径字符串数组（原名：FileDialogUtils.GetOpenFolderDialogFolders）

        if (folders.Length != 1)
            return null;
        // 如果返回的文件夹数量不是 1，则返回 null（表示未选择单一文件夹或多选不被接受）

        return folders[0];
        // 否则返回第一个（也是唯一一个）文件夹路径
    }
    // 方法体结束

    public async Task<T?> ShowDialog<T>(IDialogAware<T> dialogAware)
    // 公共泛型异步方法 ShowDialog：接收实现 IDialogAware<T> 的对话框视图模型并返回可能的 TResult（原名：ShowDialog）
    {
        // 方法体开始

        return await _dialogService.ShowDialog(dialogAware);
        // 通过注入的 _dialogService 调用 ShowDialog 并返回其结果（原名：_dialogService.ShowDialog）
    }
    // 方法体结束

    public async Task ShowMessageDialog(string title, string message)
    // 公共异步方法 ShowMessageDialog：显示一个简单的消息对话框，接收标题 title 与消息 message（原名：ShowMessageDialog）
    {
        // 方法体开始

        var messageBoxVm = new MessageBoxViewModel(title, message, MessageBoxType.OK);
        // 创建 MessageBoxViewModel 实例，传入标题、消息与按钮类型 MessageBoxType.OK（原名：MessageBoxViewModel、MessageBoxType.OK）

        await _dialogService.ShowDialog(messageBoxVm);
        // 使用 _dialogService 异步显示该消息框视图模型并等待其关闭（原名：_dialogService.ShowDialog）
    }
    // 方法体结束

}
// 类体结束
