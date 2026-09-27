using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展命名空间，用于处理扩展类型与辅助方法（保留英文原名：AssetsTools.NET.Extra）

using AssetsTools.NET.Texture;
// 引用 AssetsTools.NET 的纹理处理命名空间，提供纹理解码与编码支持（保留英文原名：AssetsTools.NET.Texture）

using Avalonia.Platform.Storage;
// 引用 Avalonia 的存储/文件对话相关命名空间，提供文件夹/文件选择对话接口（保留英文原名：Avalonia.Platform.Storage）

using System.Text;
// 引用 System.Text 命名空间，提供 StringBuilder 等文本处理类型（保留英文原名：System.Text）

using UABEANext4.AssetWorkspace;
// 引用项目的工作区命名空间，提供 Workspace、AssetInst 等类型（保留英文原名：UABEANext4.AssetWorkspace）

using UABEANext4.Plugins;
// 引用项目的插件接口命名空间，定义 IUavPluginOption、IUavPluginFunctions 等（保留英文原名：UABEANext4.Plugins）

using UABEANext4.ViewModels.Dialogs;
// 引用项目的对话框视图模型命名空间，提供 BatchImportViewModel 等（保留英文原名：UABEANext4.ViewModels.Dialogs）

namespace TexturePlugin;
// 定义命名空间 TexturePlugin，用于组织纹理插件相关类型（保留英文原名：TexturePlugin）

public class ImportBatchTextureOption : IUavPluginOption
// 定义公共类 ImportBatchTextureOption，实现插件选项接口 IUavPluginOption（保留英文原名：ImportBatchTextureOption / IUavPluginOption）
{
    // 类体开始（ImportBatchTextureOption）

    public string Name => "Import Texture2D";
    // 只读属性 Name：插件选项的显示名称，返回 "Import Texture2D"（保留英文原名：Name）

    public string Description => "Imports a folder of png/tga/bmp/jpgs into Texture2Ds";
    // 只读属性 Description：插件选项的描述，说明功能（保留英文原名：Description）

    public UavPluginMode Options => UavPluginMode.Import;
    // 只读属性 Options：指示此选项属于导入模式（UavPluginMode.Import）（保留英文原名：Options / UavPluginMode.Import）

    public bool SupportsSelection(Workspace workspace, UavPluginMode mode, IList<AssetInst> selection)
    // 公共方法 SupportsSelection：判断当前选择是否支持此选项（保留英文原名：SupportsSelection）
    {
        if (mode != UavPluginMode.Import)
        {
            return false;
        }
        // 如果当前模式不是导入模式则返回 false（保留英文原名：UavPluginMode.Import）

        var typeId = (int)AssetClassID.Texture2D;
        // 将 AssetClassID.Texture2D 转为整型以便比较（保留英文原名：AssetClassID.Texture2D）

        return selection.All(a => a.TypeId == typeId);
        // 返回 true 当且仅当 selection 中所有项的 TypeId 都等于 Texture2D 的 typeId（保留英文原名：selection.All / TypeId）
    }

    public async Task<bool> Execute(Workspace workspace, IUavPluginFunctions funcs, UavPluginMode mode, IList<AssetInst> selection)
    // 公共异步方法 Execute：插件被触发时调用，委托给 BatchImport 执行实际导入（保留英文原名：Execute / BatchImport）
    {
        return await BatchImport(workspace, funcs, selection);
    }

    public async Task<bool> BatchImport(Workspace workspace, IUavPluginFunctions funcs, IList<AssetInst> selection)
    // 公共异步方法 BatchImport：批量导入流程，弹出文件夹选择、构建批量信息并调用导入（保留英文原名：BatchImport）
    {
        var dir = await funcs.ShowOpenFolderDialog(new FolderPickerOpenOptions()
        {
            Title = "Select import directory"
        });
        // 使用 IUavPluginFunctions 提示用户选择导入目录（ShowOpenFolderDialog），并传入对话框选项（保留英文原名：funcs.ShowOpenFolderDialog / FolderPickerOpenOptions）

        if (dir == null)
        {
            return false;
        }
        // 如果用户取消选择则返回 false（保留英文原名：dir）

        var extensions = new List<string>() { "bmp", "png", "jpg", "jpeg", "tga" };
        // 定义允许的图像扩展名列表，用于匹配目录中的文件（保留英文原名：extensions）

        var batchInfosViewModel = new BatchImportViewModel(workspace, selection.ToList(), dir, extensions);
        // 创建 BatchImportViewModel（对话视图模型），传入工作区、选择列表、目录与扩展名（保留英文原名：BatchImportViewModel）

        if (batchInfosViewModel.DataGridItems.Count == 0)
        {
            await funcs.ShowMessageDialog("Error", "No matching files found in the directory. Make sure the file names are in UABEA's format.");
            return false;
        }
        // 如果没有匹配的文件则弹出错误对话并返回 false（保留英文原名：DataGridItems / funcs.ShowMessageDialog）

        var batchInfosResult = await funcs.ShowDialog(batchInfosViewModel);
        // 显示批量导入对话（batchInfosViewModel），等待用户确认并获取结果（保留英文原名：funcs.ShowDialog）

        if (batchInfosResult == null)
        {
            return false;
        }
        // 如果用户取消对话则返回 false（保留英文原名：batchInfosResult）

        var success = await ImportTextures(workspace, funcs, batchInfosResult);
        // 调用 ImportTextures 执行实际导入操作，并等待结果（保留英文原名：ImportTextures）

        return success;
        // 返回导入是否成功（保留英文原名：success）
    }

    private async Task<bool> ImportTextures(Workspace workspace, IUavPluginFunctions funcs, List<ImportBatchInfo> infos)
    // 私有异步方法 ImportTextures：遍历导入信息列表，将每个文件编码写入对应 Texture2D 并更新资产（保留英文原名：ImportTextures）
    {
        var errorBuilder = new StringBuilder();
        // 使用 StringBuilder 收集导入过程中发生的错误信息（保留英文原名：errorBuilder / StringBuilder）

        foreach (var info in infos)
        {
            var asset = info.Asset;
            var errorAssetName = $"{Path.GetFileName(asset.FileInstance.path)}/{asset.PathId}";
            // 为错误消息构建标识字符串，包含文件名与 PathId（保留英文原名：asset.FileInstance.path / asset.PathId）

            var baseField = workspace.GetBaseField(asset);
            if (baseField == null)
            {
                errorBuilder.AppendLine($"[{errorAssetName}]: failed to read");
                continue;
            }
            // 获取资产的 BaseField（反序列化后的字段结构），若失败则记录错误并跳过（保留英文原名：workspace.GetBaseField / baseField）

            var tex = TextureFile.ReadTextureFile(baseField);
            // 使用 TextureFile.ReadTextureFile 从 baseField 解析出纹理对象（保留英文原名：TextureFile.ReadTextureFile）

            if (info.ImportFile == null || !File.Exists(info.ImportFile))
            {
                errorBuilder.AppendLine($"[{errorAssetName}]: failed to import because {info.ImportFile ?? "[null]"} does not exist.");
                continue;
            }
            // 验证导入文件路径是否存在，若不存在则记录错误并跳过（保留英文原名：info.ImportFile / File.Exists）

            try
            {
                // disable mips until we can support them
                tex.m_MipCount = 1;
                tex.m_MipMap = false;
                // 临时禁用 mipmaps（m_MipCount 与 m_MipMap），直到支持为止（保留英文原名：m_MipCount / m_MipMap）

                tex.EncodeTextureImage(info.ImportFile);
                // 将磁盘上的图像文件编码到 tex 的像素数据中（保留英文原名：EncodeTextureImage / info.ImportFile）

                tex.WriteTo(baseField);
                // 将修改后的纹理数据写回 baseField（保留英文原名：WriteTo / baseField）

                asset.UpdateAssetDataAndRow(workspace, baseField);
                // 更新内存中资产的数据并刷新 UI 行（UpdateAssetDataAndRow），标记为已修改（保留英文原名：UpdateAssetDataAndRow）
            }
            catch (Exception e)
            {
                errorBuilder.AppendLine($"[{errorAssetName}]: failed to import: {e}");
            }
            // 捕获任何异常并将错误信息追加到 errorBuilder（保留英文原名：Exception / e）
        }

        if (errorBuilder.Length > 0)
        {
            string[] firstLines = errorBuilder.ToString().Split('\n').Take(20).ToArray();
            string firstLinesStr = string.Join('\n', firstLines);
            await funcs.ShowMessageDialog("Error", firstLinesStr);
        }
        // 如果收集到错误，截取前 20 行并通过对话框显示给用户（保留英文原名：Take / funcs.ShowMessageDialog）

        return true;
        // 方法完成后返回 true（表示流程已执行完毕；具体错误已通过对话提示）（保留英文原名：return true）
    }
}
// 类体结束（ImportBatchTextureOption）
