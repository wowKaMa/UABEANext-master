using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展命名空间，提供额外的辅助类型与方法（保留英文原名：AssetsTools.NET.Extra）

using AssetsTools.NET.Texture;
// 引用 AssetsTools.NET 的纹理处理命名空间，提供 TextureFile 等纹理读写功能（保留英文原名：AssetsTools.NET.Texture）

using System.Text;
// 引用 System.Text 命名空间，用于 StringBuilder 等文本构建工具（保留英文原名：System.Text）

using TexturePlugin.Helpers;
// 引用本插件的辅助工具命名空间，包含 TextureHelper、TextureOperations 等（保留英文原名：TexturePlugin.Helpers）

using TexturePlugin.ViewModels;
// 引用本插件的视图模型命名空间，包含 EditTextureViewModel 等（保留英文原名：TexturePlugin.ViewModels）

using UABEANext4.AssetWorkspace;
// 引用项目的工作区命名空间，提供 Workspace、AssetInst 等类型（保留英文原名：UABEANext4.AssetWorkspace）

using UABEANext4.Plugins;
// 引用项目的插件接口命名空间，定义 IUavPluginOption、IUavPluginFunctions 等（保留英文原名：UABEANext4.Plugins）

namespace TexturePlugin;
// 定义命名空间 TexturePlugin，用于组织纹理插件相关类型（保留英文原名：TexturePlugin）

public class EditTextureOption : IUavPluginOption
// 定义公共类 EditTextureOption，实现插件选项接口 IUavPluginOption（保留英文原名：EditTextureOption / IUavPluginOption）
{
    // 类体开始（EditTextureOption）

    public string Name => "Edit Texture2D";
    // 只读属性 Name：插件选项显示名称，返回 "Edit Texture2D"（保留英文原名：Name）

    public string Description => "Edits Texture2D settings";
    // 只读属性 Description：插件选项描述，说明功能（保留英文原名：Description）

    public UavPluginMode Options => UavPluginMode.Export;
    // 只读属性 Options：指示此选项属于导出/操作模式（保留英文原名：Options / UavPluginMode.Export）

    public bool SupportsSelection(Workspace workspace, UavPluginMode mode, IList<AssetInst> selection)
    // 公共方法 SupportsSelection：判断当前选择是否支持此选项（保留英文原名：SupportsSelection）
    {
        if (mode != UavPluginMode.Export)
        {
            return false;
        }
        // 如果当前模式不是导出模式则返回 false（保留英文原名：UavPluginMode.Export）

        var texTypeId = (int)AssetClassID.Texture2D;
        // 将 AssetClassID.Texture2D 转为整型以便与 AssetInst.TypeId 比较（保留英文原名：AssetClassID.Texture2D）

        return selection.All(a => a.TypeId == texTypeId);
        // 返回 true 当且仅当 selection 中所有项的 TypeId 都等于 Texture2D 的 typeId（保留英文原名：selection.All / TypeId）
    }

    public async Task<bool> Execute(Workspace workspace, IUavPluginFunctions funcs, UavPluginMode mode, IList<AssetInst> selection)
    // 公共异步方法 Execute：当用户触发此选项时调用，执行编辑流程（保留英文原名：Execute）
    {
        var editTextureVm = new EditTextureViewModel(workspace, selection);
        // 创建 EditTextureViewModel 实例，用于在对话框中显示与编辑纹理设置（保留英文原名：EditTextureViewModel）

        var result = await funcs.ShowDialog(editTextureVm);
        // 使用插件函数显示对话框并等待用户输入结果（保留英文原名：funcs.ShowDialog / result）

        if (!result.HasValue)
        {
            return false;
        }
        // 如果用户取消对话或未确认则返回 false（保留英文原名：result.HasValue）

        var editTexSettings = result.Value;
        // 将用户在对话框中选择的设置保存到 editTexSettings（保留英文原名：editTexSettings）

        // todo: need to support single image import as well
        // 注释：TODO：未来需要支持单张图片导入的场景（保留英文原注释）

        var errorBuilder = new StringBuilder();
        // 创建 StringBuilder 用于收集处理过程中出现的错误信息（保留英文原名：errorBuilder）

        foreach (var asset in selection)
        // 遍历用户选择的每个 AssetInst（保留英文原名：selection / asset）
        {
            var errorAssetName = $"{Path.GetFileName(asset.FileInstance.path)}/{asset.PathId}";
            // 构建用于错误消息的标识字符串，包含文件名与 PathId（保留英文原名：errorAssetName / Path.GetFileName）

            var baseField = TextureHelper.GetByteArrayTexture(workspace, asset);
            // 使用 TextureHelper 获取表示纹理数据的 baseField（通常是一个字节数组字段或 AssetTypeValueField）（保留英文原名：TextureHelper.GetByteArrayTexture）

            if (baseField == null)
            {
                errorBuilder.AppendLine($"[{errorAssetName}]: failed to read");
                continue;
            }
            // 如果无法读取 baseField，则记录错误并跳过当前资产（保留英文原名：baseField）

            var tex = TextureFile.ReadTextureFile(baseField);
            // 使用 TextureFile 解析 baseField，得到可操作的 TextureFile 对象（保留英文原名：TextureFile.ReadTextureFile）

            var needToReencode = false;
            // 标记是否需要重新编码纹理（当更改了纹理格式等需要对像素数据重新编码时为 true）（保留英文原名：needToReencode）

            if (editTexSettings.TextureFormat is not null)
            {
                needToReencode |= tex.m_TextureFormat != (int)editTexSettings.TextureFormat;
            }
            // 如果用户在设置中指定了新的纹理格式，则比较当前格式与目标格式，不同则需要重新编码（保留英文原名：TextureFormat / m_TextureFormat）

            //if (editTexSettings.UsingMips is not null)
            //{
            //    // if we've toggled mips on, only make a change if the current
            //    // mipcount is different from what we would change it to.
            //    var usingMips = editTexSettings.UsingMips.Value;
            //    if (usingMips)
            //        needToReencode |= tex.m_MipCount == TextureHelper.GetMaxMipCount(tex.m_Width, tex.m_Height);
            //    else
            //        needToReencode |= tex.m_MipCount == 1;
            //}
            // 注释掉的代码：关于是否需要基于 mip 设置决定是否重新编码的逻辑（保留英文原注释）

            byte[]? texOrigDecBytes = null;
            // 声明可空字节数组 texOrigDecBytes，用于在需要重新编码时保存解码后的原始像素数据（保留英文原名：texOrigDecBytes）

            if (needToReencode)
            {
                // decode the texture so we can reencode it in the next step
                var texOrigEncBytes = tex.FillPictureData(asset.FileInstance);
                if (texOrigEncBytes is null)
                {
                    errorBuilder.AppendLine($"[{errorAssetName}]: failed to decode for reencoding");
                    continue;
                }
                else
                {
                    texOrigDecBytes = tex.DecodeTextureRaw(texOrigEncBytes, true);
                }
            }
            // 如果需要重新编码，则先从 TextureFile 获取编码数据并解码为原始像素（texOrigDecBytes），若解码失败则记录错误并跳过（保留英文原名：FillPictureData / DecodeTextureRaw）

            if (editTexSettings.Name is not null)
                tex.m_Name = editTexSettings.Name;
            // 如果用户指定了新的名称，则设置 TextureFile 的 m_Name（保留英文原名：m_Name）

            if (editTexSettings.TextureFormat is not null)
                tex.m_TextureFormat = (int)editTexSettings.TextureFormat.Value;
            // 如果用户指定了新的纹理格式，则更新 TextureFile 的 m_TextureFormat（保留英文原名：m_TextureFormat）

            //if (editTexSettings.UsingMips is not null)
            //    tex.m_MipMap = editTexSettings.UsingMips.Value;
            // 注释掉的代码：如果支持 mip 设置则更新 m_MipMap（保留英文原注释）

            if (editTexSettings.IsReadable is not null)
                tex.m_IsReadable = editTexSettings.IsReadable.Value;
            // 如果用户指定了可读性设置，则更新 m_IsReadable（保留英文原名：m_IsReadable）

            if (editTexSettings.FilterMode is not null)
                tex.m_TextureSettings.m_FilterMode = (int)editTexSettings.FilterMode.Value;
            // 如果用户指定了过滤模式，则更新 TextureFile 内部的 TextureSettings.m_FilterMode（保留英文原名：m_TextureSettings.m_FilterMode）

            if (editTexSettings.Filtering is not null)
                tex.m_TextureSettings.m_Aniso = editTexSettings.Filtering.Value;
            // 如果用户指定了各向异性过滤值，则更新 m_Aniso（保留英文原名：m_TextureSettings.m_Aniso）

            if (editTexSettings.MipBias is not null)
                tex.m_TextureSettings.m_MipBias = editTexSettings.MipBias.Value;
            // 如果用户指定了 MipBias，则更新 m_MipBias（保留英文原名：m_TextureSettings.m_MipBias）

            if (editTexSettings.WrapModeU is not null)
                tex.m_TextureSettings.m_WrapU = (int)editTexSettings.WrapModeU.Value;
            // 如果用户指定了 U 方向的 Wrap 模式，则更新 m_WrapU（保留英文原名：m_TextureSettings.m_WrapU）

            if (editTexSettings.WrapModeV is not null)
                tex.m_TextureSettings.m_WrapV = (int)editTexSettings.WrapModeV.Value;
            // 如果用户指定了 V 方向的 Wrap 模式，则更新 m_WrapV（保留英文原名：m_TextureSettings.m_WrapV）

            if (editTexSettings.LightMapFormat is not null)
                tex.m_LightmapFormat = editTexSettings.LightMapFormat.Value;
            // 如果用户指定了光照贴图格式，则更新 m_LightmapFormat（保留英文原名：m_LightmapFormat）

            if (editTexSettings.ColorSpace is not null)
                tex.m_ColorSpace = (int)editTexSettings.ColorSpace.Value;
            // 如果用户指定了颜色空间，则更新 m_ColorSpace（保留英文原名：m_ColorSpace）

            if (needToReencode && texOrigDecBytes is not null)
            {
                try
                {
                    // disable mips until we can support them
                    tex.m_MipCount = 1;
                    tex.m_MipMap = false;

                    tex.EncodeTextureRaw(texOrigDecBytes, tex.m_Width, tex.m_Height, 3, true);
                }
                catch (Exception e)
                {
                    errorBuilder.AppendLine($"[{errorAssetName}]: failed to import: {e}");
                }
            }
            // 如果需要重新编码且已成功解码原始像素，则尝试用 EncodeTextureRaw 重新编码像素到新的格式；在此处临时禁用 mipmaps 并捕获异常记录错误（保留英文原名：EncodeTextureRaw / m_MipCount / m_MipMap）

            tex.WriteTo(baseField);
            // 将修改后的 TextureFile 写回 baseField（将更改应用到 Asset 的字段结构中）（保留英文原名：WriteTo / baseField）

            asset.UpdateAssetDataAndRow(workspace, baseField);
            // 调用 AssetInst 的方法更新内存中资产数据并刷新 UI 行，同时标记工作区为已修改（保留英文原名：UpdateAssetDataAndRow / workspace）
        }

        if (errorBuilder.Length > 0)
        {
            string[] firstLines = errorBuilder.ToString().Split('\n').Take(20).ToArray();
            string firstLinesStr = string.Join('\n', firstLines);
            await funcs.ShowMessageDialog("Error", firstLinesStr);
        }
        // 如果收集到错误信息，则截取前 20 行并通过对话框显示给用户（保留英文原名：errorBuilder / funcs.ShowMessageDialog）

        return true;
        // 方法完成，返回 true 表示操作已执行（具体错误已在对话中提示）（保留英文原名：return true）
    }
}
// 类体结束（EditTextureOption）
