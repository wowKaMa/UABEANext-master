using Avalonia.Platform.Storage;
// 引用 Avalonia 的存储平台接口命名空间，包含 IStorageFile、IStorageFolder、IStorageProvider 等类型，用于跨平台文件/文件夹对话框交互

using System.Collections.Generic;
// 引用泛型集合命名空间，提供 IReadOnlyList<T>、List<T> 等集合接口和类型

using System.Linq;
// 引用 LINQ 扩展方法命名空间，用于集合的 Select、Where、ToArray 等操作

namespace UABEANext4.Util;
// 定义命名空间 UABEANext4.Util，用于组织工具类（保留英文原名）

public static class FileDialogUtils
// 定义公共静态类 FileDialogUtils，封装与文件/文件夹对话框结果处理的辅助方法（保留英文原名）
{
    public static string[] GetOpenFileDialogFiles(IReadOnlyList<IStorageFile> files)
    // 公共静态方法 GetOpenFileDialogFiles：接收一个只读的 IStorageFile 列表（来自文件打开对话框），返回本地路径字符串数组
    {
        return files.Select(sf => sf.TryGetLocalPath()).Where(p => p != null).ToArray()!;
        // 将每个 IStorageFile 调用 TryGetLocalPath()（尝试获取本地文件路径），
        // 过滤掉返回 null 的项（表示无法获取本地路径或非本地文件），
        // 最后将结果转换为字符串数组并返回；末尾的 ! 表示断言非 null（在此上下文中保证安全）
    }

    public static string[] GetOpenFolderDialogFolders(IReadOnlyList<IStorageFolder> folders)
    // 公共静态方法 GetOpenFolderDialogFolders：接收一个只读的 IStorageFolder 列表（来自选择文件夹对话框），返回本地路径字符串数组
    {
        return folders.Select(sf => sf.TryGetLocalPath()).Where(p => p != null).ToArray()!;
        // 将每个 IStorageFolder 调用 TryGetLocalPath()（尝试获取本地文件夹路径），
        // 过滤掉 null，转换为字符串数组并返回；与上面方法逻辑一致但针对文件夹
    }

    public static string? GetSaveFileDialogFile(IStorageFile? file)
    // 公共静态方法 GetSaveFileDialogFile：接收一个可空的 IStorageFile（来自保存对话框），返回本地路径或 null
    {
        return file?.TryGetLocalPath();
        // 如果 file 为 null 则返回 null；否则调用 TryGetLocalPath() 返回本地路径字符串（可能为 null，取决于平台与实现）
    }
}
