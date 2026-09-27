using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展命名空间，用于处理扩展类型与辅助方法（保留英文原名：AssetsTools.NET.Extra）

using Avalonia.Platform.Storage;
// 引用 Avalonia 的存储/文件对话相关命名空间，提供文件/文件夹选择对话接口（保留英文原名：Avalonia.Platform.Storage）

using System.Text;
// 引用 System.Text 命名空间，提供 StringBuilder、Encoding 等文本处理工具（保留英文原名：System.Text）

using UABEANext4.AssetWorkspace;
// 引用工作区相关类型（Workspace、AssetInst 等），用于读取/写入资产数据（保留英文原名：UABEANext4.AssetWorkspace）

using UABEANext4.Plugins;
// 引用插件接口（IUavPluginOption、IUavPluginFunctions 等），用于与宿主交互（保留英文原名：UABEANext4.Plugins）

using UABEANext4.Util;
// 引用工具类（PathUtils、AssetNamer 等），用于生成安全文件名等实用功能（保留英文原名：UABEANext4.Util）

namespace TextAssetPlugin;
// 定义命名空间 TextAssetPlugin，用于组织与文本资产导出相关的类型（保留英文原名：TextAssetPlugin）

public class ExportTextAssetPlugin : IUavPluginOption
// 定义公共类 ExportTextAssetPlugin，实现 IUavPluginOption 接口，表示一个导出选项插件（保留英文原名：ExportTextAssetPlugin / IUavPluginOption）
{
    // 类体开始（ExportTextAssetPlugin）

    public string Name => "Export TextAsset";
    // 只读属性 Name：插件在 UI 中显示的名称（"Export TextAsset"）（保留英文原名：Name）

    public string Description => "Exports TextAssets to txt";
    // 只读属性 Description：插件描述，说明功能（保留英文原名：Description）

    public UavPluginMode Options => UavPluginMode.Export;
    // 只读属性 Options：指示此插件属于导出模式（UavPluginMode.Export）（保留英文原名：Options / UavPluginMode.Export）

    public bool SupportsSelection(Workspace workspace, UavPluginMode mode, IList<AssetInst> selection)
    // 方法 SupportsSelection：判断当前选择是否适用于此插件（保留英文原名：SupportsSelection）
    {
        if (mode != UavPluginMode.Export)
        {
            return false;
        }
        // 如果当前模式不是导出模式则返回 false（保留英文原名：mode / UavPluginMode.Export）

        var typeId = (int)AssetClassID.TextAsset;
        // 将 AssetClassID.TextAsset 转为整型以便比较（保留英文原名：AssetClassID.TextAsset）

        return selection.All(a => a.TypeId == typeId);
        // 返回 true 当且仅当 selection 中所有项的 TypeId 都等于 TextAsset 的 typeId（保留英文原名：selection.All / TypeId）
    }

    public async Task<bool> Execute(Workspace workspace, IUavPluginFunctions funcs, UavPluginMode mode, IList<AssetInst> selection)
    // 异步方法 Execute：当用户触发此选项时调用，分发到批量或单个导出（保留英文原名：Execute）
    {
        if (selection.Count > 1)
        {
            return await BatchExport(workspace, funcs, selection);
        }
        else
        {
            return await SingleExport(workspace, funcs, selection);
        }
        // 如果选择多于 1 个则调用 BatchExport，否则调用 SingleExport（保留英文原名：BatchExport / SingleExport）
    }

    public async Task<bool> BatchExport(Workspace workspace, IUavPluginFunctions funcs, IList<AssetInst> selection)
    // 异步方法 BatchExport：批量导出多个 TextAsset 到指定目录（保留英文原名：BatchExport）
    {
        var dir = await funcs.ShowOpenFolderDialog(new FolderPickerOpenOptions()
        {
            Title = "Select export directory"
        });
        // 弹出选择文件夹对话（ShowOpenFolderDialog），获取用户选择的导出目录（保留英文原名：funcs.ShowOpenFolderDialog / FolderPickerOpenOptions）

        if (dir == null)
        {
            return false;
        }
        // 如果用户取消选择目录则返回 false（保留英文原名：dir）

        var errorBuilder = new StringBuilder();
        // 使用 StringBuilder 收集导出过程中发生的错误信息（保留英文原名：errorBuilder / StringBuilder）

        foreach (var asset in selection)
        // 遍历每个要导出的资产（保留英文原名：selection / asset）
        {
            var errorAssetName = $"{Path.GetFileName(asset.FileInstance.path)}/{asset.PathId}";
            // 构建用于错误消息的标识字符串（文件名/PathId）（保留英文原名：errorAssetName / Path.GetFileName / asset.PathId）

            var textBaseField = workspace.GetBaseField(asset);
            // 获取资产的 BaseField（反序列化后的字段结构），用于读取 m_Name 与 m_Script（保留英文原名：GetBaseField / textBaseField）

            if (textBaseField == null)
            {
                errorBuilder.AppendLine($"[{errorAssetName}]: failed to read");
                continue;
            }
            // 如果读取失败则记录错误并跳过该资产（保留英文原名：textBaseField）

            var name = textBaseField["m_Name"].AsString;
            // 从 BaseField 中读取资产名称字段 m_Name（保留英文原名："m_Name" / AsString）

            var byteData = textBaseField["m_Script"].AsByteArray;
            // 从 BaseField 中读取脚本/文本内容字段 m_Script（字节数组）（保留英文原名："m_Script" / AsByteArray）

            var assetName = PathUtils.ReplaceInvalidPathChars(name);
            // 使用 PathUtils 替换文件名中非法字符，生成安全的文件名片段（保留英文原名：PathUtils.ReplaceInvalidPathChars）

            var filePath = Path.Combine(dir, AssetNamer.GetAssetFileName(asset, assetName, ".txt"));
            // 使用 AssetNamer 生成完整文件名并与目录组合成输出路径（保留英文原名：AssetNamer.GetAssetFileName / Path.Combine）

            File.WriteAllBytes(filePath, byteData);
            // 将字节数据写入磁盘（保留英文原名：File.WriteAllBytes）
        }

        if (errorBuilder.Length > 0)
        {
            string[] firstLines = errorBuilder.ToString().Split('\n').Take(20).ToArray();
            string firstLinesStr = string.Join('\n', firstLines);
            await funcs.ShowMessageDialog("Error", firstLinesStr);
        }
        // 如果收集到错误，截取前 20 行并通过对话框显示给用户（保留英文原名：errorBuilder / funcs.ShowMessageDialog）

        return true;
        // 批量导出完成后返回 true（保留英文原名：return true）
    }

    public async Task<bool> SingleExport(Workspace workspace, IUavPluginFunctions funcs, IList<AssetInst> selection)
    // 异步方法 SingleExport：导出单个 TextAsset（保留英文原名：SingleExport）
    {
        var asset = selection[0];
        // 取第一个（也是唯一一个）资产作为导出目标（保留英文原名：asset / selection[0]）

        var textBaseField = workspace.GetBaseField(asset);
        // 获取该资产的 BaseField（保留英文原名：GetBaseField / textBaseField）

        if (textBaseField == null)
        {
            await funcs.ShowMessageDialog("Error", "Failed to read");
            return false;
        }
        // 如果读取失败则提示错误并返回 false（保留英文原名：funcs.ShowMessageDialog）

        var name = textBaseField["m_Name"].AsString;
        // 读取资产名称（m_Name）（保留英文原名："m_Name"）

        var byteData = textBaseField["m_Script"].AsByteArray;
        // 读取文本/脚本字节数据（m_Script）（保留英文原名："m_Script"）

        string assetName = PathUtils.ReplaceInvalidPathChars(name);
        // 生成安全的文件名（替换非法字符）（保留英文原名：PathUtils.ReplaceInvalidPathChars）

        var filePath = await funcs.ShowSaveFileDialog(new FilePickerSaveOptions()
        {
            Title = "Save text asset",
            FileTypeChoices = new List<FilePickerFileType>()
            {
                new("TXT file (*.txt)") { Patterns = ["*.txt"] },
                new("BYTES file (*.bytes)") { Patterns = ["*.bytes"] },
                new("All types (*.*)") { Patterns = ["*"] },
            },
            SuggestedFileName = AssetNamer.GetAssetFileName(asset, assetName, string.Empty),
            DefaultExtension = "txt"
        });
        // 弹出保存文件对话（ShowSaveFileDialog），设置文件类型选项、建议文件名与默认扩展名（保留英文原名：funcs.ShowSaveFileDialog / FilePickerSaveOptions / AssetNamer.GetAssetFileName）

        if (filePath == null)
        {
            return false;
        }
        // 如果用户取消保存则返回 false（保留英文原名：filePath）

        File.WriteAllBytes(filePath, byteData);
        // 将字节数据写入用户选择的文件路径（保留英文原名：File.WriteAllBytes）

        return true;
        // 单个导出成功返回 true（保留英文原名：return true）
    }
}
// 类体结束（ExportTextAssetPlugin）
