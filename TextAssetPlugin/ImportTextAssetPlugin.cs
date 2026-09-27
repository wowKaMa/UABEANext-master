using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展命名空间，用于处理扩展类型与辅助方法（保留英文原名：AssetsTools.NET.Extra）

using Avalonia.Platform.Storage;
// 引用 Avalonia 的存储/文件对话相关命名空间，提供文件/文件夹选择对话接口（保留英文原名：Avalonia.Platform.Storage）

using System.Text;
// 引用 System.Text 命名空间，提供 StringBuilder、Encoding 等文本处理工具（保留英文原名：System.Text）

using UABEANext4.AssetWorkspace;
// 引用项目的工作区命名空间，提供 Workspace、AssetInst 等类型与方法（保留英文原名：UABEANext4.AssetWorkspace）

using UABEANext4.Plugins;
// 引用项目的插件接口命名空间，定义 IUavPluginOption、IUavPluginFunctions 等（保留英文原名：UABEANext4.Plugins）

using UABEANext4.ViewModels.Dialogs;
// 引用项目的对话框视图模型命名空间，提供 BatchImportViewModel、ImportBatchInfo 等（保留英文原名：UABEANext4.ViewModels.Dialogs）

namespace TextAssetPlugin;
// 定义命名空间 TextAssetPlugin，用于组织文本资产导入插件相关类型（保留英文原名：TextAssetPlugin）

public class ImportTextAssetPlugin : IUavPluginOption
// 定义公共类 ImportTextAssetPlugin，实现插件选项接口 IUavPluginOption（保留英文原名：ImportTextAssetPlugin / IUavPluginOption）
{
    // 类体开始（ImportTextAssetPlugin）

    public string Name => "Import TextAsset";
    // 只读属性 Name：插件选项在 UI 中显示的名称，返回 "Import TextAsset"（保留英文原名：Name）

    public string Description => "Imports TextAssets to txt";
    // 只读属性 Description：插件选项的描述，说明功能（保留英文原名：Description）

    public UavPluginMode Options => UavPluginMode.Import;
    // 只读属性 Options：指示此选项属于导入模式（UavPluginMode.Import）（保留英文原名：Options / UavPluginMode.Import）

    public bool SupportsSelection(Workspace workspace, UavPluginMode mode, IList<AssetInst> selection)
    // 公共方法 SupportsSelection：判断当前工作区/模式/选择是否支持此插件选项（保留英文原名：SupportsSelection）
    {
        if (mode != UavPluginMode.Import)
        {
            return false;
        }
        // 如果当前模式不是导入模式则返回 false（保留英文原名：mode / UavPluginMode.Import）

        var typeId = (int)AssetClassID.TextAsset;
        // 将 AssetClassID.TextAsset 转为整型以便比较（保留英文原名：AssetClassID.TextAsset）

        return selection.All(a => a.TypeId == typeId);
        // 返回 true 当且仅当 selection 中所有项的 TypeId 都等于 TextAsset 的 typeId（保留英文原名：selection.All / TypeId）
    }

    public async Task<bool> Execute(Workspace workspace, IUavPluginFunctions funcs, UavPluginMode mode, IList<AssetInst> selection)
    // 公共异步方法 Execute：当用户触发此选项时调用，决定批量导入或单个导入（保留英文原名：Execute）
    {
        if (selection.Count > 1)
        {
            return await BatchImport(workspace, funcs, selection);
        }
        else
        {
            return await SingleImport(workspace, funcs, selection);
        }
        // 如果选择多于 1 个则调用 BatchImport，否则调用 SingleImport（保留英文原名：BatchImport / SingleImport）
    }

    public async Task<bool> BatchImport(Workspace workspace, IUavPluginFunctions funcs, IList<AssetInst> selection)
    // 公共异步方法 BatchImport：批量导入流程（保留英文原名：BatchImport）
    {
        var dir = await funcs.ShowOpenFolderDialog(new FolderPickerOpenOptions()
        {
            Title = "Select import directory"
        });
        // 使用 IUavPluginFunctions 弹出选择文件夹对话（ShowOpenFolderDialog），获取用户选择的导入目录（保留英文原名：funcs.ShowOpenFolderDialog / FolderPickerOpenOptions）

        if (dir == null)
        {
            return false;
        }
        // 如果用户取消选择目录则返回 false（保留英文原名：dir）

        var extensions = new List<string>() { "*" };
        // 定义允许的扩展名列表（此处为 "*" 表示所有类型），用于 BatchImportViewModel 的匹配（保留英文原名：extensions）

        var batchInfosViewModel = new BatchImportViewModel(workspace, selection.ToList(), dir, extensions);
        // 创建 BatchImportViewModel（对话视图模型），传入工作区、选择列表、目录与扩展名（保留英文原名：BatchImportViewModel）

        if (batchInfosViewModel.DataGridItems.Count == 0)
        {
            await funcs.ShowMessageDialog("Error", "No matching files found in the directory. Make sure the file names are in UABEA's format.");
            return false;
        }
        // 如果没有匹配的文件则弹出错误对话并返回 false（保留英文原名：DataGridItems / funcs.ShowMessageDialog）

        var batchInfosResult = await funcs.ShowDialog(batchInfosViewModel);
        // 显示批量导入对话并等待用户确认，获取结果（保留英文原名：funcs.ShowDialog / batchInfosResult）

        if (batchInfosResult == null)
        {
            return false;
        }
        // 如果用户取消对话则返回 false（保留英文原名：batchInfosResult）

        var errorBuilder = new StringBuilder();
        // 使用 StringBuilder 收集导入过程中发生的错误信息（保留英文原名：errorBuilder / StringBuilder）

        foreach (ImportBatchInfo info in batchInfosResult)
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

            var filePath = info.ImportFile;
            if (filePath == null || !File.Exists(filePath))
            {
                errorBuilder.AppendLine($"[{errorAssetName}]: failed to import because {info.ImportFile ?? "[null]"} does not exist.");
                continue;
            }
            // 验证导入文件路径是否存在，若不存在则记录错误并跳过（保留英文原名：info.ImportFile / File.Exists）

            byte[] byteData = File.ReadAllBytes(filePath);
            // 读取导入文件的全部字节（保留英文原名：File.ReadAllBytes / byteData）

            baseField["m_Script"].AsByteArray = byteData;
            // 将读取到的字节数组写入资产字段 m_Script（TextAsset 的文本内容字段）（保留英文原名：baseField["m_Script"].AsByteArray）

            asset.UpdateAssetDataAndRow(workspace, baseField);
            // 更新内存中资产的数据并刷新 UI 行（UpdateAssetDataAndRow），将更改写回工作区（保留英文原名：UpdateAssetDataAndRow）
        }

        if (errorBuilder.Length > 0)
        {
            string[] firstLines = errorBuilder.ToString().Split('\n').Take(20).ToArray();
            string firstLinesStr = string.Join('\n', firstLines);
            await funcs.ShowMessageDialog("Error", firstLinesStr);
        }
        // 如果收集到错误，截取前 20 行并通过对话框显示给用户（保留英文原名：errorBuilder / funcs.ShowMessageDialog）

        return true;
        // 批量导入完成后返回 true（保留英文原名：return true）
    }

    public async Task<bool> SingleImport(Workspace workspace, IUavPluginFunctions funcs, IList<AssetInst> selection)
    // 公共异步方法 SingleImport：单个 TextAsset 导入流程（保留英文原名：SingleImport）
    {
        var filePaths = await funcs.ShowOpenFileDialog(new FilePickerOpenOptions()
        {
            Title = "Load text asset",
            FileTypeFilter = new List<FilePickerFileType>()
            {
                new("TXT file (*.txt)") { Patterns = ["*.txt"] },
                new("BYTES file (*.bytes)") { Patterns = ["*.bytes"] },
                new("All types (*.*)") { Patterns = ["*"] },
            },
            AllowMultiple = false
        });
        // 弹出文件选择对话，限制为单选并提供文件类型过滤（保留英文原名：ShowOpenFileDialog / FilePickerOpenOptions / FilePickerFileType）

        if (filePaths == null || filePaths.Length == 0)
        {
            return false;
        }
        // 如果用户未选择文件则返回 false（保留英文原名：filePaths）

        var filePath = filePaths[0];
        // 取用户选择的第一个文件路径（保留英文原名：filePath）

        if (!File.Exists(filePath))
        {
            await funcs.ShowMessageDialog("Error", $"Failed to import because {filePath ?? "[null]"} does not exist.");
            return false;
        }
        // 如果文件不存在则提示错误并返回 false（保留英文原名：File.Exists / funcs.ShowMessageDialog）

        var asset = selection[0];
        // 取要导入的目标资产（保留英文原名：asset / selection[0]）

        var baseField = workspace.GetBaseField(asset);
        if (baseField == null)
        {
            await funcs.ShowMessageDialog("Error", "Failed to read");
            return false;
        }
        // 获取目标资产的 BaseField，若读取失败则提示错误并返回 false（保留英文原名：GetBaseField / baseField）

        byte[] byteData = File.ReadAllBytes(filePath);
        // 读取选中文件的全部字节（保留英文原名：File.ReadAllBytes / byteData）

        baseField["m_Script"].AsByteArray = byteData;
        // 将字节数据写入资产的 m_Script 字段（保留英文原名：m_Script）

        asset.UpdateAssetDataAndRow(workspace, baseField);
        // 更新资产数据并刷新 UI（保留英文原名：UpdateAssetDataAndRow）

        return true;
        // 单个导入成功返回 true（保留英文原名：return true）
    }
}
// 类体结束（ImportTextAssetPlugin）
