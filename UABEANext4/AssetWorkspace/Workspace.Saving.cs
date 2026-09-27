using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于访问 Unity 资产解析相关类型（保留英文原名：AssetsTools.NET）

using AssetsTools.NET.Extra;
using UABEANext4.Logic.UnityFS;
// 引用 AssetsTools.NET 的扩展功能，提供扩展类型与辅助方法（保留英文原名：AssetsTools.NET.Extra）

using Avalonia.Platform.Storage;
// 引用 Avalonia 的存储/文件对话相关命名空间（FilePicker、IStorageProvider 等）（保留英文原名：Avalonia.Platform.Storage）

using System;
// 引用基础系统命名空间，提供 Exception、Func、Task 等基础类型（保留英文原名：System）

using System.Diagnostics.CodeAnalysis;
// 引用诊断/注解命名空间，提供 NotNullWhen 等可空性注解（保留英文原名：System.Diagnostics.CodeAnalysis）

using System.IO;
// 引用 IO 命名空间，用于文件流、路径处理等（保留英文原名：System.IO）

using System.Linq;
// 引用 LINQ 扩展方法命名空间，用于集合查询与转换（保留英文原名：System.Linq）

using System.Threading.Tasks;
// 引用异步任务命名空间，提供 Task/async 支持（保留英文原名：System.Threading.Tasks）

using UABEANext4.Util;
// 引用项目内的工具/实用程序命名空间（例如 MessageBoxUtil、StorageService 等）（保留英文原名：UABEANext4.Util）

namespace UABEANext4.AssetWorkspace;
// 定义命名空间 UABEANext4.AssetWorkspace，用于组织工作区相关类型（保留英文原名：UABEANext4.AssetWorkspace）

// this contains saving logic for workspace
// 注释：说明该文件包含工作区（Workspace）的保存逻辑（保留英文原注释）

// dialogs are allowed in this class (instead
// 注释：说明在此类中允许弹出对话框（保留英文原注释）

// of being handled in the view model they
// 注释：说明这些对话框不是在视图模型中处理（保留英文原注释）

// were called from)
// 注释：注释续行（保留英文原注释）

public partial class Workspace
// 定义部分类 Workspace（保留英文原名：Workspace），此处实现与保存相关的方法
{
    // 类体开始（Workspace）

    private static async Task<IStorageFile?> ShowSaveAsDialog(IStorageProvider storageProvider, string suggestedFileName)
    // 私有静态异步方法 ShowSaveAsDialog：显示“另存为”对话框并返回用户选择的 IStorageFile（保留英文原名：ShowSaveAsDialog / IStorageProvider / IStorageFile）
    {
        return await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save file",
            FileTypeChoices = new FilePickerFileType[]
            {
                new FilePickerFileType("All files (*.*)") { Patterns = new[] { "*" } }
            },
            SuggestedFileName = suggestedFileName
        });
    }
    // 方法结束：调用 storageProvider.SaveFilePickerAsync 弹出保存对话框并返回用户选择（保留英文原名：SaveFilePickerAsync / FilePickerSaveOptions）

    private static bool TryGetFileStream(WorkspaceItem item, [NotNullWhen(true)] out FileStream? fs)
    // 私有静态方法 TryGetFileStream：尝试从 WorkspaceItem 获取底层 FileStream（如果该项直接使用文件流），返回是否成功（保留英文原名：TryGetFileStream / WorkspaceItem / FileStream）
    {
        if (item.Object is AssetsFileInstance fileInst)
        {
            if (fileInst.AssetsStream is FileStream assetsInstFs)
            {
                fs = assetsInstFs;
                return true;
            }
        }
        else if (item.Object is BundleFileInstance bunInst)
        {
            if (bunInst.BundleStream is FileStream bundleInstFs)
            {
                fs = bundleInstFs;
                return true;
            }
        }

        fs = null;
        return false;
    }
    // 方法结束：根据 item.Object 的实际类型（AssetsFileInstance 或 BundleFileInstance）尝试提取 FileStream（保留英文原名：AssetsStream / BundleStream）

    private static bool TryOpenForWriting(string path, [NotNullWhen(true)] out FileStream? writeFs)
    // 私有静态方法 TryOpenForWriting：尝试以可写模式打开指定路径的文件流，返回是否成功（保留英文原名：TryOpenForWriting）
    {
        try
        {
            writeFs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            return true;
        }
        catch
        {
            writeFs = null;
            return false;
        }
    }
    // 方法结束：使用 FileStream 构造函数打开或创建文件，捕获异常并返回失败（保留英文原名：FileMode / FileAccess / FileShare）

    private void WriteAssetsFile(WorkspaceItem item, Stream stream)
    // 私有方法 WriteAssetsFile：将 WorkspaceItem（AssetsFileInstance）写入指定流（保留英文原名：WriteAssetsFile）
    {
        var fileInst = (AssetsFileInstance)item.Object!;
        fileInst.file.Write(new AssetsFileWriter(stream));
    }
    // 方法结束：将 AssetsFileInstance.file 写入传入的 Stream（保留英文原名：AssetsFileWriter）

    // warning! OriginalName needs to be updated if save overwrite is used
    // 注释：警告：如果覆盖保存，需要更新 OriginalName（保留英文原注释）

    private void WriteBundleFile(WorkspaceItem item, Stream stream, BundleSaveMethod? saveMethod = null)

    // 私有方法 WriteBundleFile：将 BundleFileInstance（及其目录信息）写入指定流（保留英文原名：WriteBundleFile）
    {
        var bunInst = (BundleFileInstance)item.Object!;

        // files that are both unsaved and part of this bundle
        var childrenFiles = item.Children
            .Intersect(UnsavedItems)
            .ToDictionary(f => f.OriginalName);
        // 将当前 bundle 项的子项与未保存集合（UnsavedItems）取交集，并按 OriginalName 建字典，便于后续替换（保留英文原名：Intersect / UnsavedItems / ToDictionary）

        // sync up directory infos
        var infos = bunInst.file.BlockAndDirInfo.DirectoryInfos;
        // 获取 bundle 文件的目录信息列表（DirectoryInfos），用于同步名称与替换数据（保留英文原名：BlockAndDirInfo / DirectoryInfos）

        foreach (var info in infos)
        {
            if (childrenFiles.TryGetValue(info.Name, out var unsavedAssetsFile))
            {
                if (unsavedAssetsFile.Name != unsavedAssetsFile.OriginalName)
                {
                    info.Name = unsavedAssetsFile.Name;
                }

                if (unsavedAssetsFile.Object is AssetsFileInstance fileInst)
                {
                    info.SetNewData(fileInst.file);
                }
                else if (unsavedAssetsFile.Object is AssetBundleDirectoryInfo matchingInfo && info == matchingInfo)
                {
                    // do nothing, already handled by replacer
                }
                else
                {
                    // shouldn't happen
                    info.Replacer = null;
                }
            }
            else
            {
                // remove replacer (if there was one ever set)
                info.Replacer = null;
            }
        }
        // 遍历每个目录信息：如果该目录对应的子项在未保存集合中，则：
        // - 如果子项被重命名则更新 info.Name
        // - 如果子项的 Object 是 AssetsFileInstance，则用其 file 设置新的数据（info.SetNewData）
        // - 如果子项的 Object 是 AssetBundleDirectoryInfo 且与 info 相同，则跳过（由 replacer 处理）
        // 否则清除 info.Replacer（保留英文原名：SetNewData / Replacer）

        if (saveMethod == BundleSaveMethod.ChunkedLZMA)
        {
            UnityFSPacker.PackChunkedLzma(bunInst, stream);
            goto cleanup; // 跳过常规 Pack 逻辑
        }

        AssetBundleCompressionType? compressionType = null;
        if (saveMethod != null)
        {
            compressionType = saveMethod switch
            {
                BundleSaveMethod.LZMA => AssetBundleCompressionType.LZMA,
                BundleSaveMethod.LZ4 => AssetBundleCompressionType.LZ4,
                BundleSaveMethod.None => AssetBundleCompressionType.None,
                _ => null
            };
        }

        if (compressionType != null)
        {
            // Pack with compression
            using (MemoryStream ms = new MemoryStream())
            {
                using (AssetsFileWriter writer = new AssetsFileWriter(ms))
                {
                    bunInst.file.Write(writer);
                }
                var bundleData = ms.ToArray();

                using (MemoryStream readMs = new MemoryStream(bundleData))
                {
                    AssetBundleFile ab = new AssetBundleFile();
                    ab.Read(new AssetsFileReader(readMs));
                    ab.Pack(new AssetsFileWriter(stream), compressionType.Value);
                }
            }
        }
        else
        {
            bunInst.file.Write(new AssetsFileWriter(stream));
        }
        cleanup:;
    }
    // 方法结束：将修改后的 bundle 文件写入流（保留英文原名：AssetsFileWriter）

    private void WriteResource(WorkspaceItem item, Stream stream)
    // 私有方法 WriteResource：将资源类型（非 .assets）写入流（保留英文原名：WriteResource）
    {
        // we need a resource type before we can do any saving
        var dirInfo = (AssetBundleDirectoryInfo)item.Object!;
        if (dirInfo.Replacer != null)
        {
            dirInfo.Replacer.Write(new AssetsFileWriter(stream), true);
        }
        else if (item.Parent != null)
        {
            // shouldn't happen
            // var parentBundle = (BundleFileInstance)item.Parent.Object!;
            // var reader = parentBundle.file.DataReader;
            // reader.Position = dirInfo.Offset;
            // reader.BaseStream.CopyToCompat(stream, dirInfo.DecompressedSize);
        }
        else
        {
            // we can't do anything, not enough information
        }
    }
    // 方法结束：如果目录信息有 Replacer 则使用 Replacer 写入，否则在某些情况下尝试从父 bundle 读取（注：代码中注释掉的分支表示未实现或不常见情况）

    public async Task<(bool saved, bool failed)> Save(WorkspaceItem item, BundleSaveMethod? saveMethod = null)

    // 公共异步方法 Save：保存单个 WorkspaceItem，返回 (saved, failed) 二元组表示是否保存与是否失败（保留英文原名：Save）
    {
        if (!UnsavedItems.Contains(item))
        {
            return (false, false);
        }
        // 如果该项不在未保存集合中，则无需保存，返回 (false, false)（保留英文原名：UnsavedItems）

        var storageProvider = StorageService.GetStorageProvider();
        if (storageProvider is null)
        {
            return (false, false);
        }
        // 获取存储提供者（StorageService.GetStorageProvider），如果不可用则返回（保留英文原名：StorageService）

        var type = item.ObjectType;
        if (type == WorkspaceItemType.AssetsFile && item.Parent != null)
        {
            // pls don't do this
            throw new Exception("Tried to save an assets file that was in a bundle directly");
        }
        // 如果尝试直接保存属于 bundle 的 .assets 文件则抛出异常（不允许直接保存 bundle 内的 assets）（保留英文原名：WorkspaceItemType）

        string origBundlePath;
        if (TryGetFileStream(item, out var stream))
        {
            // file is currently using this filestream
            origBundlePath = stream.Name;
        }
        else if (item.Object is BundleFileInstance bunInst)
        {
            // file is probably a bundle that we reloaded into a memory stream
            // todo: we could skip making the temp file, but we don't do that right now
            origBundlePath = bunInst.path;
        }
        else if (item.Object is AssetsFileInstance fileInst)
        {
            origBundlePath = fileInst.path;
        }
        else
        {
            await MessageBoxUtil.ShowDialog("Error saving", "Workspace item isn't using a FileStream");
            return (false, true);
        }
        // 确定原始文件路径 origBundlePath：
        // - 如果项当前使用 FileStream，则取 stream.Name
        // - 否则如果项是 BundleFileInstance，则取 bunInst.path（可能是内存流重载的情况）
        // - 否则弹出错误对话框并返回失败（保留英文原名：MessageBoxUtil）

        // verify we can write to this file
        if (!TryOpenForWriting(origBundlePath, out var writeStream))
        {
            await MessageBoxUtil.ShowDialog("Error saving", "Couldn't open stream for writing");
            return (false, true);
        }
        // 尝试以写模式打开原始路径以验证写权限；若失败则提示并返回（保留英文原名：TryOpenForWriting）

        var newName = "~" + Path.GetFileName(origBundlePath);
        var dir = Path.GetDirectoryName(origBundlePath)!;
        var tempWriteStreamPath = Path.Combine(dir, newName);
        if (!TryOpenForWriting(tempWriteStreamPath, out var tempWriteStream))
        {
            await MessageBoxUtil.ShowDialog("Error saving", "Couldn't open temp file stream for writing");
            return (false, true);
        }
        // 在同一目录下创建临时文件名（以 ~ 前缀），并尝试打开临时写入流以避免直接覆盖原文件（保留英文原名：Path / Path.GetFileName / Path.Combine）

        if (type == WorkspaceItemType.AssetsFile)
        {
            WriteAssetsFile(item, tempWriteStream);
        }
        else if (type == WorkspaceItemType.BundleFile)
        {
            WriteBundleFile(item, tempWriteStream, saveMethod);
        }
        else if (type == WorkspaceItemType.ResourceFile)
        {
            WriteResource(item, tempWriteStream);
        }

        stream?.Close();
        writeStream.Close();
        tempWriteStream.Close();
        // 关闭原始流（如果存在）、写入流与临时写入流，释放文件句柄（保留英文原名：Close）

        // technically there's a window here where the file could be reopened
        // and block write access, but let's just assume that won't happen
        // since that complicates things a bit...
        // 注释：说明存在短暂竞态窗口（其他程序可能打开文件），但此处忽略该复杂性（保留英文原注释）

        try
        {
            File.Move(tempWriteStreamPath, origBundlePath, true);
            var newStream = File.Open(origBundlePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            if (item.Object is AssetsFileInstance fileInst)
            {
                fileInst.file = new AssetsFile();
                fileInst.file.Read(new AssetsFileReader(newStream));
                item.OriginalName = item.Name;
                foreach (var asset in fileInst.file.AssetInfos)
                {
                    asset.Replacer = null;
                }
                UnsavedItems.Remove(item);
                FixupAssetsFile(fileInst);
            }
            else if (item.Object is BundleFileInstance oldBunInst)
            {
                Manager.UnloadBundleFile(oldBunInst);
                BundleFileInstance newBunInst;
                if (newStream is FileStream fs)
                {
                    // AssetsManager internally checks if the path is already loaded. 
                    // Since we just unloaded it, it should load a fresh one from the new stream.
                    newBunInst = Manager.LoadBundleFile(fs);
                }
                else
                {
                    newBunInst = Manager.LoadBundleFile(newStream, oldBunInst.name);
                }

                item.Object = newBunInst;
                item.OriginalName = item.Name;
                for (var i = 0; i < item.Children.Count; i++)
                {
                    var child = item.Children[i];
                    if (child.Object is AssetsFileInstance childInst)
                    {
                        // workaround to "disable" caching while we reload
                        // a second version of the inst and replace the
                        // AssetsFile inside of the first one with the second's
                        Manager.FileLookup.Remove(AssetsManager.GetFileLookupKey(child.OriginalName));
                        
                        // update parent reference
                        childInst.parentBundle = newBunInst;

                        {
                            var afileObj = LoadAssetsFromBundle(newBunInst, i);
                            if (afileObj.Object == null)
                            {
                                await MessageBoxUtil.ShowDialog("Error saving", "Reopened file appears to be corrupt");
                                continue;
                            }

                            var newFileInst = (AssetsFileInstance)afileObj.Object;

                            var newFile = newFileInst.file;
                            childInst.file = newFile;
                            foreach (var asset in newFile.AssetInfos)
                            {
                                asset.Replacer = null;
                            }
                            UnsavedItems.Remove(item);
                            FixupAssetsFile(newFileInst);
                        }
                        Manager.FileLookup[AssetsManager.GetFileLookupKey(child.Name)] = childInst;
                    }
                    else if (child.Object is AssetBundleDirectoryInfo)
                    {
                        child.Object = newBunInst.file.BlockAndDirInfo.DirectoryInfos[i];
                    }
                    child.OriginalName = child.Name;
                }
                UnsavedItems.Remove(item);
            }
            else
            {
                // can't handle resource or any other files yet
                UnsavedItems.Remove(item);
            }
        }
        catch (Exception ex)
        {
            await MessageBoxUtil.ShowDialog("Error saving", "Unknown error:\n" + ex);
            return (false, true);
        }
        return (true, false);
    }
    // Save 方法结束：将临时文件替换原文件后重新打开并更新内存中的对象（AssetsFileInstance 或 BundleFileInstance），清理 UnsavedItems 并处理错误（保留英文原名：File.Move / File.Open / AssetsFileReader / FixupAssetsFile）

    public async Task<bool> SaveAs(WorkspaceItem item, BundleSaveMethod? saveMethod = null)
    // 公共异步方法 SaveAs：弹出“另存为”对话框并将指定项保存到用户选择的位置（保留英文原名：SaveAs）
    {
        if (!UnsavedItems.Contains(item))
        {
            return false;
        }

        var storageProvider = StorageService.GetStorageProvider();
        if (storageProvider is null)
        {
            return false;
        }

        var type = item.ObjectType;
        if (type == WorkspaceItemType.AssetsFile && item.Parent != null)
        {
            // pls don't do this
            throw new Exception("Tried to save an assets file that was in a bundle directly");
        }

        var result = await ShowSaveAsDialog(storageProvider, item.Name);
        if (result == null)
        {
            return false;
        }

        try
        {
            using var stream = await result.OpenWriteAsync();
            if (type == WorkspaceItemType.AssetsFile)
            {
                WriteAssetsFile(item, stream);
                UnsavedItems.Remove(item);
            }
            else if (type == WorkspaceItemType.BundleFile)
            {
                WriteBundleFile(item, stream, saveMethod);
                UnsavedItems.Remove(item);
            }
            else if (type == WorkspaceItemType.ResourceFile)
            {
                WriteResource(item, stream);
                UnsavedItems.Remove(item);
            }
        }
        catch (Exception ex)
        {
            await MessageBoxUtil.ShowDialog("Error saving", "Unknown error:\n" + ex);
            return false;
        }
        return true;
    }
    // SaveAs 方法结束：通过用户选择的位置写入并在成功后从 UnsavedItems 移除（保留英文原名：OpenWriteAsync）

    // todo
    public async Task SaveAllAs()
    // 公共异步方法 SaveAllAs：为所有未保存项逐个弹出“另存为”并保存（保留英文原名：SaveAllAs）
    {
        var storageProvider = StorageService.GetStorageProvider();
        if (storageProvider is null)
        {
            return;
        }

        var unsavedAssetsFiles = UnsavedItems.Where(i => i.ObjectType == WorkspaceItemType.AssetsFile);
        foreach (var unsavedAssetsFile in unsavedAssetsFiles)
        {
            // skip assets files in bundles
            if (unsavedAssetsFile.Parent != null)
                continue;

            var result = await ShowSaveAsDialog(storageProvider, unsavedAssetsFile.Name);
            if (result != null)
            {
                try
                {
                    using var stream = await result.OpenWriteAsync();
                    WriteAssetsFile(unsavedAssetsFile, stream);
                }
                catch
                {
                    // put error here
                    continue;
                }
            }
        }

        var unsavedBundleFiles = UnsavedItems.Where(i => i.ObjectType == WorkspaceItemType.BundleFile);
        foreach (var unsavedBundleFile in unsavedBundleFiles)
        {
            var result = await ShowSaveAsDialog(storageProvider, unsavedBundleFile.Name);
            if (result != null)
            {
                try
                {
                    using var stream = await result.OpenWriteAsync();
                    WriteBundleFile(unsavedBundleFile, stream);
                }
                catch
                {
                    // put error here
                    continue;
                }
            }
        }

        // this is impossible right now since resource files can't normally be opened
        var unsavedResourceFiles = UnsavedItems.Where(i => i.ObjectType == WorkspaceItemType.ResourceFile);
        foreach (var unsavedResourceFile in unsavedResourceFiles)
        {
            // skip resource files in bundles
            if (unsavedResourceFile.Parent != null)
                continue;

            var result = await ShowSaveAsDialog(storageProvider, unsavedResourceFile.Name);
            if (result != null)
            {
                try
                {
                    using var stream = await result!.OpenWriteAsync();
                    WriteResource(unsavedResourceFile, stream);
                }
                catch
                {
                    // put error here
                    continue;
                }
            }
        }
    }
    // SaveAllAs 方法结束：分别处理未保存的 assets、bundle、resource 三类项，逐个弹出保存对话框并写入（保留英文原名：Where / OpenWriteAsync）

}
// 类体结束（Workspace）
