using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展命名空间，用于处理扩展类型与辅助方法（保留英文原名：AssetsTools.NET.Extra）

using AssetsTools.NET.Texture;
// 引用 AssetsTools.NET 的纹理处理命名空间，提供纹理解码与格式支持（保留英文原名：AssetsTools.NET.Texture）

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

public class SpritePreviewer : IUavPluginPreviewer
// 定义公共类 SpritePreviewer，实现插件预览器接口 IUavPluginPreviewer（保留英文原名：SpritePreviewer / IUavPluginPreviewer）
{
    // 类体开始（SpritePreviewer）

    public string Name => "Preview Sprite";
    // 只读属性 Name：插件名称，返回 "Preview Sprite"（保留英文原名：Name）

    public string Description => "Preview Sprites";
    // 只读属性 Description：插件描述，返回 "Preview Sprites"（保留英文原名：Description）

    private readonly TextureLoader _textureLoader = new();
    // 私有只读字段 _textureLoader：TextureLoader 的实例，用于加载/解码 Sprite 相关的纹理（保留英文原名：_textureLoader / TextureLoader）

    public UavPluginPreviewerType SupportsPreview(Workspace workspace, AssetInst selection)
    // 公共方法 SupportsPreview：判断给定选择（AssetInst）是否支持预览，返回 UavPluginPreviewerType（保留英文原名：SupportsPreview / UavPluginPreviewerType）
    {
        var previewType = selection.Type == AssetClassID.Sprite
            ? UavPluginPreviewerType.Image
            : UavPluginPreviewerType.None;
        // 根据 selection.Type 是否为 Sprite（AssetClassID.Sprite）决定返回 Image（支持图像）或 None（不支持）（保留英文原名：selection.Type / AssetClassID.Sprite / UavPluginPreviewerType）

        return previewType;
        // 返回计算得到的 previewType（保留英文原名：previewType）
    }

    public Bitmap? ExecuteImage(Workspace workspace, IUavPluginFunctions funcs, AssetInst selection, out string? error)
    // 公共方法 ExecuteImage：尝试将选中的 AssetInst 解码为 Bitmap 以供显示，若失败通过 out 参数返回错误信息（保留英文原名：ExecuteImage / Bitmap / IUavPluginFunctions）
    {
        var image = _textureLoader.GetSpriteAvaloniaBitmap(workspace, selection, true, out TextureFormat format);
        // 使用 _textureLoader 的 GetSpriteAvaloniaBitmap 方法尝试获取 Avalonia 的 Bitmap（image），并输出纹理格式（format）
        // 参数说明：workspace（工作区），selection（选中资产），true（通常表示使用预览或某种选项），out TextureFormat format（输出纹理格式）
        if (image != null)
        {
            error = null;
            return image;
        }
        else
        {
            error = $"Sprite texture failed to decode. The image format may not be supported or the texture is not valid. ({format})";
            return null;
        }
        // 如果解码成功返回 Bitmap 并将 error 设为 null；否则构建包含 format 的错误信息并返回 null（保留英文原名：image / format）
    }

    public MeshObj? ExecuteMesh(Workspace workspace, IUavPluginFunctions funcs, AssetInst selection, out string? error)
        // 公共方法 ExecuteMesh：插件接口的一部分，但此插件不支持将 Sprite 作为网格预览（保留英文原名：ExecuteMesh / MeshObj）
        => throw new InvalidOperationException();
    // 直接抛出 InvalidOperationException 表示不支持该操作（保留英文原名：InvalidOperationException）

    public string? ExecuteText(Workspace workspace, IUavPluginFunctions funcs, AssetInst selection, out string? error)
        // 公共方法 ExecuteText：插件接口的一部分，但此插件不支持文本预览（保留英文原名：ExecuteText）
        => throw new InvalidOperationException();
    // 直接抛出 InvalidOperationException 表示不支持该操作（保留英文原名：InvalidOperationException）

    public void Cleanup() => _textureLoader.Cleanup();
    // 公共方法 Cleanup：在插件卸载或不再使用时调用以清理 TextureLoader 的资源（保留英文原名：Cleanup / _textureLoader.Cleanup）
}
// 类体结束（SpritePreviewer）
