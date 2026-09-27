using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展命名空间，用于处理扩展类型与辅助方法（保留英文原名：AssetsTools.NET.Extra）

using AssetsTools.NET.Texture;
// 引用 AssetsTools.NET 的纹理处理命名空间，提供纹理解码/格式支持（保留英文原名：AssetsTools.NET.Texture）

using Avalonia.Media.Imaging;
// 引用 Avalonia 的位图类型命名空间，提供 Bitmap 类型用于图像显示（保留英文原名：Avalonia.Media.Imaging）

using TexturePlugin.Helpers;
// 引用本插件的辅助工具命名空间，包含纹理加载/解码的帮助方法（保留英文原名：TexturePlugin.Helpers）

using UABEANext4.AssetWorkspace;
// 引用项目的工作区命名空间，提供 Workspace、AssetInst 等类型（保留英文原名：UABEANext4.AssetWorkspace）

using UABEANext4.Logic.Mesh;
// 引用项目的网格逻辑命名空间，提供 MeshObj 等类型（保留英文原名：UABEANext4.Logic.Mesh）

using UABEANext4.Plugins;
// 引用项目的插件接口命名空间，定义 IUavPluginPreviewer、IUavPluginFunctions 等（保留英文原名：UABEANext4.Plugins）

namespace TexturePlugin;
// 定义命名空间 TexturePlugin，用于组织插件相关类型（保留英文原名：TexturePlugin）

public class TexturePreviewer : IUavPluginPreviewer
// 定义公共类 TexturePreviewer，实现插件预览器接口 IUavPluginPreviewer（保留英文原名：TexturePreviewer / IUavPluginPreviewer）
{
    // 类体开始（TexturePreviewer）

    public string Name => "Preview Texture2D";
    // 只读属性 Name：插件名称，返回 "Preview Texture2D"（保留英文原名：Name）

    public string Description => "Preview Texture2Ds";
    // 只读属性 Description：插件描述，返回 "Preview Texture2Ds"（保留英文原名：Description）

    public UavPluginPreviewerType SupportsPreview(Workspace workspace, AssetInst selection)
    // 公共方法 SupportsPreview：判断给定选择（AssetInst）是否支持预览，返回 UavPluginPreviewerType（保留英文原名：SupportsPreview / UavPluginPreviewerType）
    {
        var previewType = selection.Type == AssetClassID.Texture2D
            ? UavPluginPreviewerType.Image
            : UavPluginPreviewerType.None;
        // 根据 selection.Type 是否为 Texture2D（AssetClassID.Texture2D）决定返回 Image（支持图像）或 None（不支持）（保留英文原名：selection.Type / AssetClassID.Texture2D / UavPluginPreviewerType）

        return previewType;
        // 返回计算得到的 previewType（保留英文原名：previewType）
    }

    public Bitmap? ExecuteImage(Workspace workspace, IUavPluginFunctions funcs, AssetInst selection, out string? error)
    // 公共方法 ExecuteImage：尝试将选中的 AssetInst 解码为 Bitmap 以供显示，若失败通过 out 参数返回错误信息（保留英文原名：ExecuteImage / Bitmap / IUavPluginFunctions）
    {
        try
        {
            var image = TextureLoader.GetTexture2DBitmap(workspace, selection, out TextureFormat format);
            // 调用 TextureLoader.GetTexture2DBitmap（位于 TexturePlugin.Helpers）尝试解码 Texture2D，返回 Bitmap（image）并输出纹理格式（format）（保留英文原名：TextureLoader.GetTexture2DBitmap / TextureFormat）

            if (image != null)
            {
                error = null;
                return image;
            }
            else
            {
                error = $"Texture failed to decode. The image format may not be supported or the texture is not valid. ({format})";
                return null;
            }
            // 如果解码成功返回 Bitmap 并将 error 设为 null；否则构建错误信息包含 format 并返回 null（保留英文原名：image / format）
        }
        catch (Exception ex)
        {
            error = $"Texture failed to decode due to an error. Exception:\n{ex}";
            return null;
        }
        // 捕获任何异常，将异常信息写入 error 并返回 null（保留英文原名：Exception / ex）
    }

    public MeshObj? ExecuteMesh(Workspace workspace, IUavPluginFunctions funcs, AssetInst selection, out string? error)
        // 公共方法 ExecuteMesh：插件接口的一部分，但此插件不支持将纹理作为网格预览，因此抛出异常（保留英文原名：ExecuteMesh / MeshObj）
        => throw new InvalidOperationException();
    // 直接抛出 InvalidOperationException 表示不支持该操作（保留英文原名：InvalidOperationException）

    public string? ExecuteText(Workspace workspace, IUavPluginFunctions funcs, AssetInst selection, out string? error)
        // 公共方法 ExecuteText：插件接口的一部分，但此插件不支持文本预览，因此抛出异常（保留英文原名：ExecuteText）
        => throw new InvalidOperationException();
    // 直接抛出 InvalidOperationException 表示不支持该操作（保留英文原名：InvalidOperationException）

    public void Cleanup() { }
    // 公共方法 Cleanup：清理资源的占位实现（此插件无需清理），方法体为空（保留英文原名：Cleanup）
}
// 类体结束（TexturePreviewer）
