using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于处理 Unity 资产文件（保留英文原名：AssetsTools.NET）

using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展功能，提供额外类型与辅助方法（保留英文原名：AssetsTools.NET.Extra）

using AssetsTools.NET.Texture;
// 引用 AssetsTools.NET 的纹理处理命名空间，提供纹理解码/编码支持（保留英文原名：AssetsTools.NET.Texture）

using Avalonia.Platform.Storage;
// 引用 Avalonia 的存储/文件对话相关命名空间（FilePicker、FolderPicker 等）（保留英文原名：Avalonia.Platform.Storage）

using System.Text;
// 引用 System.Text 命名空间，提供 StringBuilder 等文本处理类型（保留英文原名：System.Text）

using TexturePlugin.Helpers;
// 引用本插件的辅助工具命名空间，包含纹理加载/处理的帮助方法（保留英文原名：TexturePlugin.Helpers）

using TexturePlugin.ViewModels;
// 引用本插件的视图模型命名空间，包含导出选项对话的 ViewModel（保留英文原名：TexturePlugin.ViewModels）

using UABEANext4.AssetWorkspace;
// 引用项目的工作区命名空间，提供 Workspace、AssetInst 等类型（保留英文原名：UABEANext4.AssetWorkspace）

using UABEANext4.Plugins;
// 引用项目的插件接口命名空间，定义 IUavPluginOption、IUavPluginFunctions 等（保留英文原名：UABEANext4.Plugins）

using UABEANext4.Util;
// 引用项目的工具/实用程序命名空间（PathUtils、AssetNamer 等）（保留英文原名：UABEANext4.Util）

namespace TexturePlugin;
// 定义命名空间 TexturePlugin，用于组织纹理插件相关类型（保留英文原名：TexturePlugin）

public class ExportTextureOption : IUavPluginOption
// 定义公共类 ExportTextureOption，实现插件选项接口 IUavPluginOption（保留英文原名：ExportTextureOption / IUavPluginOption）
{
    // 类体开始（ExportTextureOption）

    public string Name => "Export Texture2D/Sprite";
    // 只读属性 Name：插件选项的显示名称（保留英文原名：Name）

    public string Description => "Exports Texture2D/Sprites to png/tga/bmp/jpg";
    // 只读属性 Description：插件选项的描述（保留英文原名：Description）

    public UavPluginMode Options => UavPluginMode.Export;
    // 只读属性 Options：指示此选项属于导出模式（UavPluginMode.Export）（保留英文原名：Options / UavPluginMode.Export）

    public bool SupportsSelection(Workspace workspace, UavPluginMode mode, IList<AssetInst> selection)
    // 公共方法 SupportsSelection：判断当前选择是否支持此导出选项（保留英文原名：SupportsSelection）
    {
        if (mode != UavPluginMode.Export)
        {
            return false;
        }
        // 如果当前模式不是导出模式则返回 false（保留英文原名：UavPluginMode.Export）

        var texTypeId = (int)AssetClassID.Texture2D;
        // 将 AssetClassID.Texture2D 转为整型以便比较（保留英文原名：AssetClassID.Texture2D）

        var sprTypeId = (int)AssetClassID.Sprite;
        // 将 AssetClassID.Sprite 转为整型以便比较（保留英文原名：AssetClassID.Sprite）

        return selection.All(a => a.TypeId == texTypeId || a.TypeId == sprTypeId);
        // 返回 true 当且仅当 selection 中所有项的 TypeId 都是 Texture2D 或 Sprite（保留英文原名：selection.All / TypeId）
    }

    public async Task<bool> Execute(Workspace workspace, IUavPluginFunctions funcs, UavPluginMode mode, IList<AssetInst> selection)
    // 公共异步方法 Execute：插件被触发时调用，决定是批量导出还是单个导出（保留英文原名：Execute）
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
    // 公共异步方法 BatchExport：批量导出流程（保留英文原名：BatchExport）
    {
        ExportBatchOptionsViewModel dialog = new ExportBatchOptionsViewModel();
        // 创建导出选项对话的 ViewModel（保留英文原名：ExportBatchOptionsViewModel）

        ExportBatchOptionsResult? optionsRes = await funcs.ShowDialog(dialog);
        // 使用 IUavPluginFunctions 显示对话并等待用户选择（保留英文原名：funcs.ShowDialog / ExportBatchOptionsResult）

        // bug fix for double dialog box freezing in windows
        // 注释：修复 Windows 上双对话框导致冻结的问题（保留英文原注释）

        await Task.Yield();
        // 异步让出一次调度，避免 UI 阻塞或对话框问题（保留英文原名：Task.Yield）

        if (optionsRes == null)
        {
            return false;
        }
        // 如果用户取消对话则返回 false（保留英文原名：optionsRes）

        string fileExtension = optionsRes.Value.Extension;
        // 从对话结果读取用户选择的文件扩展名（保留英文原名：fileExtension / optionsRes.Value.Extension）

        ImageExportType exportType = optionsRes.Value.ImageType;
        // 从对话结果读取导出图像类型（保留英文原名：exportType / optionsRes.Value.ImageType）

        var dir = await funcs.ShowOpenFolderDialog(new FolderPickerOpenOptions()
        {
            Title = "Select export directory"
        });
        // 弹出文件夹选择对话，获取导出目录（保留英文原名：funcs.ShowOpenFolderDialog / FolderPickerOpenOptions）

        if (dir == null)
        {
            return false;
        }
        // 如果用户取消选择目录则返回 false（保留英文原名：dir）

        TextureLoader texLoader = new TextureLoader();
        // 创建 TextureLoader 实例用于处理 Sprite 解码等（保留英文原名：TextureLoader）

        StringBuilder errorBuilder = new StringBuilder();
        // 使用 StringBuilder 收集错误信息（保留英文原名：errorBuilder / StringBuilder）

        int emptyTextureCount = 0;
        // 计数器：记录空纹理（0x0）数量（保留英文原名：emptyTextureCount）

        foreach (AssetInst asset in selection)
        // 遍历每个要导出的资产（保留英文原名：selection / asset）
        {
            var errorAssetName = $"{Path.GetFileName(asset.FileInstance.path)}/{asset.PathId}";
            // 构建用于错误消息的标识字符串（文件名/PathId）（保留英文原名：errorAssetName / Path.GetFileName / asset.PathId）

            if (asset.Type == AssetClassID.Texture2D)
            {
                // we don't need any processing, use assetstools.net.texture to export
                // 注释：对于 Texture2D，直接使用 assetstools.net.texture 导出，无需额外裁剪（保留英文原注释）

                var texBaseField = TextureHelper.GetByteArrayTexture(workspace, asset);
                // 获取表示纹理数据的 baseField（字节数组字段），通过 TextureHelper（保留英文原名：TextureHelper.GetByteArrayTexture）

                if (texBaseField == null)
                {
                    errorBuilder.AppendLine($"[{errorAssetName}]: failed to read");
                    continue;
                }
                // 如果读取失败则记录错误并跳过（保留英文原名：texBaseField）

                var texFile = TextureFile.ReadTextureFile(texBaseField);
                // 使用 TextureFile 解析 baseField 为 TextureFile（保留英文原名：TextureFile.ReadTextureFile）

                TextureHelper.SwizzleOptIn(texFile, asset.FileInstance.file);
                // 根据文件或配置对 texFile 应用 swizzle（颜色通道重排）选项（保留英文原名：SwizzleOptIn）

                // 0x0 texture, usually called like Font Texture or something
                // 注释：处理 0x0 大小的纹理（通常是字体纹理等占位）（保留英文原注释）

                if (texFile.m_Width == 0 && texFile.m_Height == 0)
                {
                    emptyTextureCount++;
                    continue;
                }
                // 如果纹理尺寸为 0x0，则计数并跳过导出（保留英文原名：m_Width / m_Height）

                string assetName = PathUtils.ReplaceInvalidPathChars(asset.AssetName ?? "Texture2D");
                // 生成安全的文件名（替换非法路径字符），若 AssetName 为空则使用 "Texture2D"（保留英文原名：PathUtils.ReplaceInvalidPathChars / asset.AssetName）

                string filePath = AssetNamer.GetAssetFileName(asset, assetName, fileExtension);
                // 使用 AssetNamer 生成导出文件名（保留英文原名：AssetNamer.GetAssetFileName）

                using FileStream outputStream = File.OpenWrite(Path.Combine(dir, filePath));
                // 打开输出文件流准备写入（保留英文原名：File.OpenWrite / Path.Combine）

                byte[] encTextureData = texFile.FillPictureData(asset.FileInstance);
                // 从 texFile 填充/获取编码后的纹理数据（保留英文原名：FillPictureData）

                bool success = texFile.DecodeTextureImage(encTextureData, outputStream, exportType);
                // 将编码数据解码并写入输出流，返回是否成功（保留英文原名：DecodeTextureImage / exportType）

                if (!success)
                {
                    errorBuilder.AppendLine($"[{errorAssetName}]: failed to decode or write image to disk (missing resS, invalid texture format, etc.)");
                }
                // 若解码或写入失败则记录错误（保留英文原名：errorBuilder）
            }
            else if (asset.Type == AssetClassID.Sprite)
            {
                // need to do crop processing, use TextureLoader
                // 注释：Sprite 需要裁剪/合成处理，使用 TextureLoader（保留英文原注释）

                byte[]? decTextureData = texLoader.GetSpriteRawBytes(workspace, asset, true, out var _, out var width, out var height);
                // 使用 TextureLoader 获取解码后的原始像素字节（decTextureData），并输出宽高（width/height）（保留英文原名：GetSpriteRawBytes）

                if (decTextureData == null)
                {
                    errorBuilder.AppendLine($"[{errorAssetName}]: failed to decode (missing resS, invalid texture format, invalid sprite, etc.)");
                    continue;
                }
                // 如果解码失败则记录错误并跳过（保留英文原名：decTextureData）

                string assetName = PathUtils.ReplaceInvalidPathChars(asset.AssetName ?? "Sprite");
                // 生成安全的文件名（保留英文原名：asset.AssetName / PathUtils.ReplaceInvalidPathChars）

                string filePath = AssetNamer.GetAssetFileName(asset, assetName, fileExtension);
                // 生成导出文件名（保留英文原名：AssetNamer.GetAssetFileName）

                // SKBitmap is RGBA32 but StbIws expects BGRA32. swap R and B.
                // 注释：SKBitmap 使用 RGBA32，但写出库期望 BGRA32，因此需要交换 R 与 B 通道（保留英文原注释）

                TextureOperations.SwapRBComponents(decTextureData);
                // 调用工具函数交换像素数据中的 R 与 B 分量（保留英文原名：TextureOperations.SwapRBComponents）

                // image is also upside down. flip it (normally assetstools.net.texture handles this)
                // 注释：图像通常是上下颠倒的，需要垂直翻转（保留英文原注释）

                TextureOperations.FlipBGRA32Vertically(decTextureData, width, height);
                // 垂直翻转 BGRA32 像素数据（保留英文原名：TextureOperations.FlipBGRA32Vertically）

                using FileStream outputStream = File.OpenWrite(Path.Combine(dir, filePath));
                // 打开输出文件流准备写入（保留英文原名：File.OpenWrite / Path.Combine）

                if (!TextureOperations.WriteRawImage(decTextureData, width, height, outputStream, exportType))
                {
                    errorBuilder.AppendLine($"[{errorAssetName}]: failed to write image to disk");
                }
                // 将原始像素写入磁盘，若失败则记录错误（保留英文原名：TextureOperations.WriteRawImage）
            }
        }

        if (emptyTextureCount == selection.Count)
        {
            await funcs.ShowMessageDialog("Error", "All textures are empty. No textures were exported.");
            return false;
        }
        // 如果所有纹理都是空纹理，则提示用户并返回失败（保留英文原名：emptyTextureCount / funcs.ShowMessageDialog）

        if (errorBuilder.Length > 0)
        {
            string[] firstLines = errorBuilder.ToString().Split('\n').Take(20).ToArray();
            string firstLinesStr = string.Join('\n', firstLines);
            await funcs.ShowMessageDialog("Error", firstLinesStr);
        }
        // 如果有错误信息，截取前 20 行并通过对话框显示给用户（保留英文原名：errorBuilder / funcs.ShowMessageDialog）

        return true;
        // 批量导出完成，返回 true（保留英文原名：return true）
    }

    public Task<bool> SingleExport(Workspace workspace, IUavPluginFunctions funcs, IList<AssetInst> selection)
    // 公共方法 SingleExport：处理单个资产导出，分发到具体的 Texture2D 或 Sprite 导出方法（保留英文原名：SingleExport）
    {
        AssetInst asset = selection[0];
        // 取第一个（也是唯一一个）资产（保留英文原名：asset / selection[0]）

        if (asset.Type == AssetClassID.Texture2D)
            return SingleExportTexture2D(workspace, funcs, asset);
        // 如果是 Texture2D 则调用 SingleExportTexture2D（保留英文原名：SingleExportTexture2D）

        else if (asset.Type == AssetClassID.Sprite)
            return SingleExportTextureSprite(workspace, funcs, asset);
        // 如果是 Sprite 则调用 SingleExportTextureSprite（保留英文原名：SingleExportTextureSprite）

        else
            return Task.FromResult(false);
        // 其他类型不支持，返回已完成的 false 任务（保留英文原名：Task.FromResult）
    }

    private async Task<bool> SingleExportTexture2D(Workspace workspace, IUavPluginFunctions funcs, AssetInst asset)
    // 私有异步方法 SingleExportTexture2D：导出单个 Texture2D（保留英文原名：SingleExportTexture2D）
    {
        AssetTypeValueField? texBaseField = TextureHelper.GetByteArrayTexture(workspace, asset);
        // 获取纹理的 baseField（字节数组字段）（保留英文原名：TextureHelper.GetByteArrayTexture）

        TextureFile texFile = TextureFile.ReadTextureFile(texBaseField);
        // 解析 baseField 为 TextureFile（保留英文原名：TextureFile.ReadTextureFile）

        TextureHelper.SwizzleOptIn(texFile, asset.FileInstance.file);
        // 应用 swizzle 选项（保留英文原名：SwizzleOptIn）

        // 0x0 texture, usually called like Font Texture or something
        // 注释：处理 0x0 大小的纹理（保留英文原注释）

        if (texFile.m_Width == 0 && texFile.m_Height == 0)
        {
            await funcs.ShowMessageDialog("Error", "Texture size is 0x0 which is not exportable.");
            return false;
        }
        // 如果纹理尺寸为 0x0，则提示用户并返回失败（保留英文原名：texFile.m_Width / funcs.ShowMessageDialog）

        string assetName = PathUtils.ReplaceInvalidPathChars(asset.AssetName ?? "Texture2D");
        // 生成安全的文件名（保留英文原名：PathUtils.ReplaceInvalidPathChars）

        var filePath = await ShowImageSaveFileDialog(funcs, asset, assetName);
        // 弹出保存文件对话，获取用户选择的保存路径（保留英文原名：ShowImageSaveFileDialog）

        if (filePath == null)
        {
            return false;
        }
        // 如果用户取消保存则返回 false（保留英文原名：filePath）

        ImageExportType exportType = ExportTypeFromFileName(filePath);
        // 根据文件名后缀确定导出类型（保留英文原名：ExportTypeFromFileName）

        using FileStream outputStream = File.OpenWrite(filePath);
        // 打开输出文件流（保留英文原名：File.OpenWrite）

        byte[] encTextureData = texFile.FillPictureData(asset.FileInstance);
        // 获取编码后的纹理数据（保留英文原名：FillPictureData）

        if (!texFile.DecodeTextureImage(encTextureData, outputStream, exportType))
        {
            string errorAssetName = $"{Path.GetFileName(asset.FileInstance.path)}/{asset.PathId}";
            await funcs.ShowMessageDialog("Error", $"[{errorAssetName}]: failed to decode (missing resS, invalid texture format, etc.)");
            return false;
        }
        // 解码并写入文件，若失败则提示用户并返回 false（保留英文原名：DecodeTextureImage / funcs.ShowMessageDialog）

        return true;
        // 成功导出返回 true（保留英文原名：return true）
    }

    private async Task<bool> SingleExportTextureSprite(Workspace workspace, IUavPluginFunctions funcs, AssetInst asset)
    // 私有异步方法 SingleExportTextureSprite：导出单个 Sprite（保留英文原名：SingleExportTextureSprite）
    {
        string assetName = PathUtils.ReplaceInvalidPathChars(asset.AssetName ?? "Sprite");
        // 生成安全的文件名（保留英文原名：PathUtils.ReplaceInvalidPathChars）

        var filePath = await ShowImageSaveFileDialog(funcs, asset, assetName);
        // 弹出保存文件对话，获取用户选择的保存路径（保留英文原名：ShowImageSaveFileDialog）

        if (filePath == null)
        {
            return false;
        }
        // 如果用户取消保存则返回 false（保留英文原名：filePath）

        ImageExportType exportType = ExportTypeFromFileName(filePath);
        // 根据文件名后缀确定导出类型（保留英文原名：ExportTypeFromFileName）

        string errorAssetName = $"{Path.GetFileName(asset.FileInstance.path)}/{asset.PathId}";
        // 构建用于错误消息的标识字符串（保留英文原名：errorAssetName）

        TextureLoader texLoader = new TextureLoader();
        // 创建 TextureLoader 实例用于解码 Sprite（保留英文原名：TextureLoader）

        byte[]? decTextureData = texLoader.GetSpriteRawBytes(workspace, asset, true, out var _, out var width, out var height);
        // 获取解码后的原始像素字节并输出宽高（保留英文原名：GetSpriteRawBytes）

        if (decTextureData == null)
        {
            await funcs.ShowMessageDialog("Error", $"[{errorAssetName}]: failed to decode (missing resS, invalid texture format, invalid sprite, etc.)");
            return false;
        }
        // 如果解码失败则提示用户并返回 false（保留英文原名：decTextureData / funcs.ShowMessageDialog）

        // SKBitmap is RGBA32 but StbIws expects BGRA32. swap R and B.
        // 注释：交换 R 与 B 通道以匹配写出库的期望格式（保留英文原注释）

        TextureOperations.SwapRBComponents(decTextureData);
        // 交换像素数据中的 R 与 B 分量（保留英文原名：TextureOperations.SwapRBComponents）

        // image is also upside down. flip it (normally assetstools.net.texture handles this)
        // 注释：垂直翻转图像（保留英文原注释）

        TextureOperations.FlipBGRA32Vertically(decTextureData, width, height);
        // 垂直翻转 BGRA32 像素数据（保留英文原名：TextureOperations.FlipBGRA32Vertically）

        using FileStream outputStream = File.OpenWrite(filePath);
        // 打开输出文件流（保留英文原名：File.OpenWrite）

        if (!TextureOperations.WriteRawImage(decTextureData, width, height, outputStream, exportType))
        {
            await funcs.ShowMessageDialog("Error", $"[{errorAssetName}]: failed to write image to disk");
            return false;
        }
        // 将原始像素写入磁盘，若失败则提示用户并返回 false（保留英文原名：TextureOperations.WriteRawImage）

        return true;
        // 成功导出返回 true（保留英文原名：return true）
    }

    private static Task<string?> ShowImageSaveFileDialog(IUavPluginFunctions funcs, AssetInst asset, string assetName)
    // 私有静态方法 ShowImageSaveFileDialog：弹出保存文件对话并返回用户选择的路径（保留英文原名：ShowImageSaveFileDialog）
    {
        return funcs.ShowSaveFileDialog(new FilePickerSaveOptions()
        {
            Title = "Save texture",
            FileTypeChoices =
            [
                new("PNG file") { Patterns = ["*.png"] },
                new("BMP file") { Patterns = ["*.bmp"] },
                new("JPG file") { Patterns = ["*.jpg", "*.jpeg"] },
                new("TGA file") { Patterns = ["*.tga"] },
            ],
            SuggestedFileName = AssetNamer.GetAssetFileName(asset, assetName, string.Empty),
            DefaultExtension = "png"
        });
        // 调用 IUavPluginFunctions.ShowSaveFileDialog 并传入保存选项（文件类型、建议文件名、默认扩展名等），返回用户选择的路径（保留英文原名：funcs.ShowSaveFileDialog / FilePickerSaveOptions / AssetNamer.GetAssetFileName）
    }

    private static ImageExportType ExportTypeFromFileName(string fileName)
    // 私有静态方法 ExportTypeFromFileName：根据文件名后缀返回 ImageExportType（保留英文原名：ExportTypeFromFileName）
    {
        return Path.GetExtension(fileName) switch
        {
            ".bmp" => ImageExportType.Bmp,
            ".png" => ImageExportType.Png,
            ".jpg" or ".jpeg" => ImageExportType.Jpg,
            ".tga" => ImageExportType.Tga,
            _ => ImageExportType.Png
        };
        // 使用 switch 表达式根据扩展名映射到对应的 ImageExportType，默认返回 PNG（保留英文原名：Path.GetExtension / ImageExportType）
    }
}
// 类体结束（ExportTextureOption）
