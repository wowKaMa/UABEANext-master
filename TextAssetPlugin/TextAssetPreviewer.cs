using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展命名空间，用于处理扩展类型与辅助方法（保留英文原名：AssetsTools.NET.Extra）

using Avalonia.Media.Imaging;
// 引用 Avalonia 的位图类型命名空间，提供 Bitmap/WriteableBitmap 等用于图像显示（保留英文原名：Avalonia.Media.Imaging）

using System.Text;
// 引用 System.Text 命名空间，提供 Encoding、StringBuilder 等文本处理工具（保留英文原名：System.Text）

using UABEANext4.AssetWorkspace;
// 引用项目的工作区命名空间，提供 Workspace、AssetInst 等类型与方法（保留英文原名：UABEANext4.AssetWorkspace）

using UABEANext4.Logic.Mesh;
// 引用项目的网格逻辑命名空间，提供 MeshObj 等类型（保留英文原名：UABEANext4.Logic.Mesh）

using UABEANext4.Plugins;
// 引用项目的插件接口命名空间，定义 IUavPluginPreviewer、IUavPluginFunctions 等（保留英文原名：UABEANext4.Plugins）

namespace TextAssetPlugin;
// 定义命名空间 TexturePlugin（此处为 TextAssetPlugin），用于组织与文本资产预览相关的类型（保留英文原名：TextAssetPlugin）

public class TextAssetPreviewer : IUavPluginPreviewer
// 定义公共类 TextAssetPreviewer，实现插件预览器接口 IUavPluginPreviewer（保留英文原名：TextAssetPreviewer / IUavPluginPreviewer）
{
    // 类体开始（TextAssetPreviewer）

    public string Name => "Preview TextAsset";
    // 只读属性 Name：插件名称，返回 "Preview TextAsset"（用于 UI 显示）（保留英文原名：Name）

    public string Description => "Preview TextAssets";
    // 只读属性 Description：插件描述，返回 "Preview TextAssets"（用于 UI 提示）（保留英文原名：Description）

    const int TEXT_ASSET_MAX_LENGTH = 100000;
    // 常量 TEXT_ASSET_MAX_LENGTH：限制预览文本的最大字节长度，超过则截断并提示（保留英文原名：TEXT_ASSET_MAX_LENGTH）

    public UavPluginPreviewerType SupportsPreview(Workspace workspace, AssetInst selection)
    // 公共方法 SupportsPreview：判断给定选择（AssetInst）是否支持此预览器，返回 UavPluginPreviewerType（保留英文原名：SupportsPreview / UavPluginPreviewerType）
    {
        var previewType = selection.Type == AssetClassID.TextAsset
            ? UavPluginPreviewerType.Text
            : UavPluginPreviewerType.None;
        // 根据 selection.Type 是否为 TextAsset（AssetClassID.TextAsset）决定返回 Text（支持文本预览）或 None（不支持）（保留英文原名：selection.Type / AssetClassID.TextAsset / UavPluginPreviewerType）

        return previewType;
        // 返回计算得到的 previewType（保留英文原名：previewType）
    }

    public string? ExecuteText(Workspace workspace, IUavPluginFunctions funcs, AssetInst selection, out string? error)
    // 公共方法 ExecuteText：尝试从选中的 TextAsset 中读取文本并返回，若失败通过 out 参数返回错误信息（保留英文原名：ExecuteText / IUavPluginFunctions）
    {
        try
        {
            var textAssetBf = workspace.GetBaseField(selection);
            // 使用 Workspace.GetBaseField 获取选中资产的 BaseField（反序列化后的字段结构），用于读取内部字段（保留英文原名：GetBaseField / textAssetBf）

            if (textAssetBf == null)
            {
                error = "No preview available.";
                return null;
            }
            // 如果无法读取 BaseField，则设置错误信息并返回 null（保留英文原名：textAssetBf）

            var text = textAssetBf["m_Script"].AsByteArray;
            // 从 BaseField 中读取名为 "m_Script" 的字段并以字节数组形式获取（保留英文原名："m_Script" / AsByteArray）

            string trimmedText;
            // 声明局部变量 trimmedText 用于保存最终返回的字符串（保留英文原名：trimmedText）

            if (text.Length <= TEXT_ASSET_MAX_LENGTH)
            {
                trimmedText = Encoding.UTF8.GetString(text);
            }
            else
            {
                trimmedText = Encoding.UTF8.GetString(text[..TEXT_ASSET_MAX_LENGTH]) + $"... (and {text.Length - TEXT_ASSET_MAX_LENGTH} bytes more)";
            }
            // 如果字节长度在限制内则完整解码为 UTF-8 字符串，否则截取前 TEXT_ASSET_MAX_LENGTH 字节并追加提示（保留英文原名：Encoding.UTF8 / TEXT_ASSET_MAX_LENGTH）

            error = null;
            // 解码成功，清空错误信息（保留英文原名：error）

            return trimmedText;
            // 返回解码或截断后的文本（保留英文原名：trimmedText）
        }
        catch (Exception ex)
        {
            error = $"TextAsset failed to decode due to an error. Exception:\n{ex}";
            return null;
        }
        // 捕获任何异常，将异常信息写入 error 并返回 null（保留英文原名：Exception / ex）
    }

    public Bitmap? ExecuteImage(Workspace workspace, IUavPluginFunctions funcs, AssetInst selection, out string? error)
        => throw new InvalidOperationException();
    // 公共方法 ExecuteImage：接口要求的方法，但 TextAsset 不支持图像预览，直接抛出 InvalidOperationException（保留英文原名：ExecuteImage / InvalidOperationException）

    public MeshObj? ExecuteMesh(Workspace workspace, IUavPluginFunctions funcs, AssetInst selection, out string? error)
        => throw new InvalidOperationException();
    // 公共方法 ExecuteMesh：接口要求的方法，但 TextAsset 不支持网格预览，直接抛出 InvalidOperationException（保留英文原名：ExecuteMesh / InvalidOperationException）

    public void Cleanup() { }
    // 公共方法 Cleanup：清理资源的占位实现（TextAsset 预览器无需特殊清理），方法体为空（保留英文原名：Cleanup）
}
// 类体结束（TextAssetPreviewer）
