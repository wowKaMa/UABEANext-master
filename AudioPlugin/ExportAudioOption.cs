using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于处理 Unity 资产文件（保留英文原名：AssetsTools.NET）

using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展命名空间，提供额外辅助类型与方法（保留英文原名：AssetsTools.NET.Extra）

using Avalonia.Platform.Storage;
// 引用 Avalonia 的存储/文件对话相关命名空间，提供文件/文件夹选择对话接口（保留英文原名：Avalonia.Platform.Storage）

using Fmod5Sharp;
// 引用 Fmod5Sharp 库，用于解析 FMOD 声音银行（保留英文原名：Fmod5Sharp）

using Fmod5Sharp.FmodTypes;
// 引用 Fmod5Sharp 的类型定义命名空间，包含 FmodSoundBank、FmodSample 等（保留英文原名：Fmod5Sharp.FmodTypes）

using System.Text;
// 引用 System.Text 命名空间，提供 StringBuilder、Encoding 等文本处理工具（保留英文原名：System.Text）

using UABEANext4.AssetWorkspace;
// 引用项目的工作区命名空间，提供 Workspace、AssetInst 等类型与方法（保留英文原名：UABEANext4.AssetWorkspace）

using UABEANext4.Plugins;
// 引用项目的插件接口命名空间，定义 IUavPluginOption、IUavPluginFunctions 等（保留英文原名：UABEANext4.Plugins）

using UABEANext4.Util;
// 引用项目的工具/实用程序命名空间（例如 PathUtils、AssetNamer）（保留英文原名：UABEANext4.Util）

namespace AudioPlugin;
// 定义命名空间 AudioPlugin，用于组织音频导出插件相关类型（保留英文原名：AudioPlugin）

public class ExportAudioOption : IUavPluginOption
// 定义公共类 ExportAudioOption，实现 IUavPluginOption 接口，表示一个导出音频的插件选项（保留英文原名：ExportAudioOption / IUavPluginOption）
{
    // 类体开始（ExportAudioOption）

    public string Name => "Export AudioClip";
    // 只读属性 Name：插件在 UI 中显示的名称，返回 "Export AudioClip"（保留英文原名：Name）

    public string Description => "Exports AudioClips to their respective format";
    // 只读属性 Description：插件描述，说明功能（保留英文原名：Description）

    public UavPluginMode Options => UavPluginMode.Export;
    // 只读属性 Options：指示此选项属于导出模式（UavPluginMode.Export）（保留英文原名：Options）

    public bool SupportsSelection(Workspace workspace, UavPluginMode mode, IList<AssetInst> selection)
    // 方法 SupportsSelection：判断当前选择是否适用于此插件（保留英文原名：SupportsSelection）
    {
        if (mode != UavPluginMode.Export)
        {
            return false;
        }
        // 如果当前模式不是导出模式则返回 false（保留英文原名：mode / UavPluginMode.Export）

        var typeId = (int)AssetClassID.AudioClip;
        // 将 AssetClassID.AudioClip 转为整型以便比较（保留英文原名：AssetClassID.AudioClip）

        return selection.All(a => a.TypeId == typeId);
        // 返回 true 当且仅当 selection 中所有项的 TypeId 都等于 AudioClip 的 typeId（保留英文原名：selection.All / TypeId）
    }

    public async Task<bool> Execute(Workspace workspace, IUavPluginFunctions funcs, UavPluginMode mode, IList<AssetInst> selection)
    // 异步方法 Execute：当用户触发此选项时调用，决定批量导出或单个导出（保留英文原名：Execute）
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
    // 异步方法 BatchExport：批量导出多个 AudioClip 到指定目录（保留英文原名：BatchExport）
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

        StringBuilder errorBuilder = new StringBuilder();
        // 使用 StringBuilder 收集导出过程中发生的错误信息（保留英文原名：errorBuilder / StringBuilder）

        foreach (AssetInst asset in selection)
        // 遍历每个要导出的资产（保留英文原名：selection / asset）
        {
            string errorAssetName = $"{Path.GetFileName(asset.FileInstance.path)}/{asset.PathId}";
            // 构建用于错误消息的标识字符串（文件名/PathId）（保留英文原名：errorAssetName / Path.GetFileName / asset.PathId）

            AssetTypeValueField? baseField = workspace.GetBaseField(asset);
            // 获取资产的 BaseField（反序列化后的字段结构），用于读取音频元数据与资源指针（保留英文原名：GetBaseField / baseField）

            if (baseField == null)
            {
                errorBuilder.AppendLine($"[{errorAssetName}]: failed to read");
                continue;
            }
            // 如果读取失败则记录错误并跳过该资产（保留英文原名：baseField）

            string name = baseField["m_Name"].AsString;
            // 从 BaseField 中读取资产名称字段 m_Name（保留英文原名："m_Name" / AsString）

            name = PathUtils.ReplaceInvalidPathChars(name);
            // 使用 PathUtils 替换文件名中非法字符，生成安全的文件名片段（保留英文原名：PathUtils.ReplaceInvalidPathChars）

            CompressionFormat compressionFormat = (CompressionFormat)baseField["m_CompressionFormat"].AsInt;
            // 读取音频的压缩格式字段 m_CompressionFormat 并转换为枚举 CompressionFormat（保留英文原名：m_CompressionFormat / CompressionFormat）

            string extension = GetExtension(compressionFormat);
            // 根据压缩格式调用 GetExtension 获取对应的文件扩展名（保留英文原名：GetExtension / extension）

            string file = Path.Combine(dir, $"{name}-{Path.GetFileName(asset.FileInstance.path)}-{asset.PathId}.{extension}");
            // 生成输出文件路径，包含资产名、源文件名与 PathId，确保唯一性（保留英文原名：Path.Combine / file）

            string ResourceSource = baseField["m_Resource.m_Source"].AsString;
            // 读取资源来源路径字段 m_Resource.m_Source（保留英文原名：ResourceSource / "m_Resource.m_Source"）

            ulong ResourceOffset = baseField["m_Resource.m_Offset"].AsULong;
            // 读取资源偏移字段 m_Resource.m_Offset（保留英文原名：ResourceOffset / AsULong）

            ulong ResourceSize = baseField["m_Resource.m_Size"].AsULong;
            // 读取资源大小字段 m_Resource.m_Size（保留英文原名：ResourceSize / AsULong）

            if (!GetAudioBytes(asset, ResourceSource, ResourceOffset, ResourceSize, out byte[] resourceData))
            {
                continue;
            }
            // 调用 GetAudioBytes 尝试从资源源读取原始字节数据，失败则跳过该资产（保留英文原名：GetAudioBytes / resourceData）

            if (!FsbLoader.TryLoadFsbFromByteArray(resourceData, out FmodSoundBank? bank) || bank == null)
            {
                continue;
            }
            // 使用 Fmod5Sharp 的 FsbLoader 尝试解析字节为 FmodSoundBank，解析失败则跳过（保留英文原名：FsbLoader / FmodSoundBank）

            List<FmodSample> samples = bank.Samples;
            // 从解析得到的声音银行中获取样本列表（保留英文原名：bank.Samples / FmodSample）

            samples[0].RebuildAsStandardFileFormat(out byte[]? sampleData, out string? sampleExtension);
            // 将第一个样本重建为标准文件格式（例如 wav/ogg/mp3），输出字节数组与扩展名（保留英文原名：RebuildAsStandardFileFormat / sampleData / sampleExtension）

            if (sampleData == null)
            {
                continue;
            }
            // 如果重建失败（sampleData 为 null）则跳过（保留英文原名：sampleData）

            if (sampleExtension?.ToLowerInvariant() == "wav")
            {
                // since fmod5sharp gives us malformed wav data, we have to correct it
                FixWAV(ref sampleData);
            }
            // 如果样本扩展名为 wav，则调用 FixWAV 修正 FMOD 解析后可能存在的损坏 WAV 头（保留英文原名：FixWAV）

            File.WriteAllBytes(file, sampleData);
            // 将重建后的音频字节写入磁盘（保留英文原名：File.WriteAllBytes）
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
    // 异步方法 SingleExport：导出单个 AudioClip（保留英文原名：SingleExport）
    {
        AssetInst asset = selection[0];
        // 取第一个（也是唯一一个）资产作为导出目标（保留英文原名：asset / selection[0]）

        AssetTypeValueField? baseField = workspace.GetBaseField(asset);
        // 获取该资产的 BaseField（保留英文原名：GetBaseField / baseField）

        if (baseField == null)
        {
            await funcs.ShowMessageDialog("Error", "Failed to read AudioClip");
            return false;
        }
        // 如果读取失败则提示错误并返回 false（保留英文原名：funcs.ShowMessageDialog）

        CompressionFormat compressionFormat = (CompressionFormat)baseField["m_CompressionFormat"].AsInt;
        // 读取压缩格式（m_CompressionFormat）并转换为 CompressionFormat（保留英文原名：compressionFormat）

        string assetName = PathUtils.ReplaceInvalidPathChars(baseField["m_Name"].AsString);
        // 读取资产名称并生成安全文件名（替换非法字符）（保留英文原名：PathUtils.ReplaceInvalidPathChars）

        string extension = GetExtension(compressionFormat);
        // 根据压缩格式获取文件扩展名（保留英文原名：GetExtension）

        var filePath = await funcs.ShowSaveFileDialog(new FilePickerSaveOptions()
        {
            Title = "Save audioclip",
            FileTypeChoices = new List<FilePickerFileType>()
            {
                new FilePickerFileType($"{extension.ToUpper()} file (*.{extension})") { Patterns = new List<string>() { "*." + extension } }
            },
            SuggestedFileName = AssetNamer.GetAssetFileName(asset, assetName, string.Empty),
            DefaultExtension = extension
        });
        // 弹出保存文件对话（ShowSaveFileDialog），设置文件类型选项、建议文件名与默认扩展名（保留英文原名：ShowSaveFileDialog / FilePickerSaveOptions / AssetNamer.GetAssetFileName）

        if (filePath == null)
        {
            return false;
        }
        // 如果用户取消保存则返回 false（保留英文原名：filePath）

        string resourceSource = baseField["m_Resource.m_Source"].AsString;
        // 读取资源来源路径（m_Resource.m_Source）（保留英文原名：resourceSource）

        ulong resourceOffset = baseField["m_Resource.m_Offset"].AsULong;
        // 读取资源偏移（m_Resource.m_Offset）（保留英文原名：resourceOffset）

        ulong resourceSize = baseField["m_Resource.m_Size"].AsULong;
        // 读取资源大小（m_Resource.m_Size）（保留英文原名：resourceSize）

        if (!GetAudioBytes(asset, resourceSource, resourceOffset, resourceSize, out byte[] resourceData))
        {
            return false;
        }
        // 从资源源读取原始字节数据，失败则返回 false（保留英文原名：GetAudioBytes）

        if (!FsbLoader.TryLoadFsbFromByteArray(resourceData, out FmodSoundBank? bank) || bank == null)
        {
            return false;
        }
        // 解析为 FmodSoundBank，失败则返回 false（保留英文原名：FsbLoader / FmodSoundBank）

        List<FmodSample> samples = bank.Samples;
        // 获取样本列表（保留英文原名：bank.Samples）

        samples[0].RebuildAsStandardFileFormat(out byte[]? sampleData, out string? sampleExtension);
        // 将第一个样本重建为标准文件格式，输出字节数组与扩展名（保留英文原名：RebuildAsStandardFileFormat）

        if (sampleData == null)
        {
            return false;
        }
        // 如果重建失败则返回 false（保留英文原名：sampleData）

        if (sampleExtension?.ToLowerInvariant() == "wav")
        {
            // since fmod5sharp gives us malformed wav data, we have to correct it
            FixWAV(ref sampleData);
        }
        // 若为 wav，则修正 WAV 头（保留英文原名：FixWAV）

        File.WriteAllBytes(filePath, sampleData);
        // 将最终音频字节写入用户选择的文件路径（保留英文原名：File.WriteAllBytes）

        return true;
        // 单个导出成功返回 true（保留英文原名：return true）
    }

    private static void FixWAV(ref byte[] wavData)
    // 私有静态方法 FixWAV：修正由 fmod5sharp 生成但格式不正确的 WAV 数据头（保留英文原名：FixWAV）
    {
        int origLength = wavData.Length;
        // 保存原始字节数组长度（保留英文原名：origLength）

        // remove ExtraParamSize field from fmt subchunk
        // 注释：说明接下来要从 fmt 子块中移除 ExtraParamSize 字段（保留英文原注释）

        for (int i = 36; i < origLength - 2; i++)
        {
            wavData[i] = wavData[i + 2];
        }
        // 将数据向前移动 2 字节以删除 ExtraParamSize（保留英文原名：for 循环）

        Array.Resize(ref wavData, origLength - 2);
        // 缩短数组长度以反映删除的 2 字节（保留英文原名：Array.Resize）

        // write ChunkSize to RIFF chunk
        // 注释：更新 RIFF 头的 ChunkSize 字段（保留英文原注释）

        byte[] riffHeaderChunkSize = BitConverter.GetBytes(wavData.Length - 8);
        // 计算并获取 RIFF ChunkSize（文件长度 - 8）的小端字节表示（保留英文原名：BitConverter.GetBytes）

        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(riffHeaderChunkSize);
        }
        // 如果系统不是小端序则反转字节顺序以确保写入小端格式（保留英文原名：BitConverter.IsLittleEndian）

        riffHeaderChunkSize.CopyTo(wavData, 4);
        // 将计算好的 RIFF ChunkSize 写入 wavData 的偏移 4（保留英文原名：CopyTo）

        // write ChunkSize to fmt chunk
        // 注释：更新 fmt 子块的 ChunkSize（保留英文原注释）

        byte[] fmtHeaderChunkSize = BitConverter.GetBytes(16); // it is always 16 for pcm data, which this always
                                                               // fmt 子块的大小对 PCM 数据总是 16，获取其小端字节表示（保留英文原名：fmtHeaderChunkSize / BitConverter.GetBytes）

        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(fmtHeaderChunkSize);
        }
        // 同样处理字节序（保留英文原名：BitConverter.IsLittleEndian）

        fmtHeaderChunkSize.CopyTo(wavData, 16);
        // 将 fmt 子块大小写入 wavData 的偏移 16（保留英文原名：CopyTo）

        // write ChunkSize to data chunk
        // 注释：更新 data 子块的 ChunkSize（保留英文原注释）

        byte[] dataHeaderChunkSize = BitConverter.GetBytes(wavData.Length - 44);
        // 计算 data 子块大小（总长度 - 44）并获取小端字节表示（保留英文原名：dataHeaderChunkSize）

        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(dataHeaderChunkSize);
        }
        // 处理字节序（保留英文原名：BitConverter.IsLittleEndian）

        dataHeaderChunkSize.CopyTo(wavData, 40);
        // 将 data 子块大小写入 wavData 的偏移 40（保留英文原名：CopyTo）
    }

    private static string GetExtension(CompressionFormat format)
    // 私有静态方法 GetExtension：根据 CompressionFormat 返回对应的文件扩展名（保留英文原名：GetExtension / CompressionFormat）
    {
        return format switch
        {
            CompressionFormat.PCM => "wav",
            CompressionFormat.Vorbis => "ogg",
            CompressionFormat.ADPCM => "wav",
            CompressionFormat.MP3 => "mp3",
            CompressionFormat.VAG => "dat", // proprietary
            CompressionFormat.HEVAG => "dat", // proprietary
            CompressionFormat.XMA => "dat", // proprietary
            CompressionFormat.AAC => "aac",
            CompressionFormat.GCADPCM => "wav", // nintendo adpcm
            CompressionFormat.ATRAC9 => "dat", // proprietary
            _ => ""
        };
    }
    // 使用 switch 表达式将不同的压缩格式映射到常见的文件扩展名（保留英文原名：CompressionFormat）

    private bool GetAudioBytes(AssetInst asset, string filepath, ulong offset, ulong size, out byte[] audioData)
    // 私有方法 GetAudioBytes：根据资源路径/偏移/大小从多种可能位置读取音频字节（文件系统、bundle 等），返回是否成功并输出字节数组（保留英文原名：GetAudioBytes）
    {
        if (string.IsNullOrEmpty(filepath))
        {
            audioData = Array.Empty<byte>();
            return false;
        }
        // 如果 filepath 为空或 null，则返回 false（保留英文原名：string.IsNullOrEmpty）

        if (asset.FileInstance.parentBundle != null)
        {
            // read from parent bundle archive
            // some versions apparently don't use archive:/
            // 注释：如果资产位于 bundle 内，则尝试从父 bundle 的目录中读取资源（保留英文原注释）

            string searchPath = filepath;
            // 将 filepath 赋给 searchPath 以便后续处理（保留英文原名：searchPath）

            if (searchPath.StartsWith("archive:/"))
                searchPath = searchPath.Substring(9);
            // 如果路径以 "archive:/" 开头则去掉该前缀（保留英文原名：StartsWith / Substring）

            searchPath = Path.GetFileName(searchPath);
            // 取出文件名部分用于在 bundle 目录中匹配（保留英文原名：Path.GetFileName）

            AssetBundleFile bundle = asset.FileInstance.parentBundle.file;
            // 获取父 bundle 的 AssetBundleFile 对象（保留英文原名：parentBundle / AssetBundleFile）

            AssetsFileReader reader = bundle.DataReader;
            // 获取 bundle 的数据读取器（保留英文原名：DataReader / AssetsFileReader）

            List<AssetBundleDirectoryInfo> dirInf = bundle.BlockAndDirInfo.DirectoryInfos;
            // 获取 bundle 中的目录信息列表（保留英文原名：DirectoryInfos）

            for (int i = 0; i < dirInf.Count; i++)
            {
                AssetBundleDirectoryInfo info = dirInf[i];
                if (info.Name == searchPath)
                {
                    lock (bundle.DataReader)
                    {
                        reader.Position = info.Offset + (long)offset;
                        audioData = reader.ReadBytes((int)size);
                    }
                    return true;
                }
            }
            // 遍历 bundle 的目录信息，找到匹配的文件名后在对应偏移处读取指定大小的字节并返回 true（保留英文原名：lock / ReadBytes）
        }

        string assetsFileDirectory = Path.GetDirectoryName(asset.FileInstance.path)!;
        // 获取 assets 文件所在目录（保留英文原名：Path.GetDirectoryName）

        if (asset.FileInstance.parentBundle != null)
        {
            // inside of bundles, the directory contains the bundle path. let's get rid of that.
            assetsFileDirectory = Path.GetDirectoryName(assetsFileDirectory)!;
        }
        // 如果资产在 bundle 内，目录可能包含 bundle 路径，向上再取一级以得到实际资源目录（保留英文原名：parentBundle）

        string resourceFilePath = Path.Combine(assetsFileDirectory, filepath);
        // 构建资源的候选路径（保留英文原名：Path.Combine / resourceFilePath）

        if (File.Exists(resourceFilePath))
        {
            // read from file
            AssetsFileReader reader = new AssetsFileReader(resourceFilePath);
            reader.Position = (long)offset;
            audioData = reader.ReadBytes((int)size);
            return true;
        }
        // 如果候选路径存在则直接从文件读取指定偏移和大小的字节并返回 true（保留英文原名：File.Exists / AssetsFileReader）

        // if that fails, check current directory
        // 注释：如果上一步失败，则尝试在 assets 文件目录下查找文件名（保留英文原注释）

        string resourceFileName = Path.Combine(assetsFileDirectory, Path.GetFileName(filepath));
        // 构建另一个候选路径，仅使用 filepath 的文件名部分（保留英文原名：Path.GetFileName）

        if (File.Exists(resourceFileName))
        {
            // read from file
            AssetsFileReader reader = new AssetsFileReader(resourceFileName);
            reader.Position = (long)offset;
            audioData = reader.ReadBytes((int)size);
            return true;
        }
        // 如果该候选路径存在则读取并返回 true（保留英文原名：File.Exists）

        audioData = Array.Empty<byte>();
        // 若所有尝试均失败，则返回空字节数组（保留英文原名：Array.Empty）

        return false;
        // 返回 false 表示未能读取到音频数据（保留英文原名：return false）
    }
}
// 类体结束（ExportAudioOption）
