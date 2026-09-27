using Avalonia.Media.Imaging;
// 引用 Avalonia 的位图处理命名空间，用于 Bitmap 类型（原名：Avalonia.Media.Imaging）

using UABEANext4.AssetWorkspace;
// 引用项目内的资产工作区命名空间，用于 Workspace、AssetInst 等类型（原名：UABEANext4.AssetWorkspace）

using UABEANext4.Logic.Mesh;
// 引用项目内的网格逻辑命名空间，用于 MeshObj 类型（原名：UABEANext4.Logic.Mesh）

namespace UABEANext4.Plugins;
// 定义命名空间 UABEANext4.Plugins，用于组织插件相关类型（原名：UABEANext4.Plugins）

public interface IUavPluginPreviewer
// 定义公共接口 IUavPluginPreviewer（原名：IUavPluginPreviewer），声明插件预览器必须实现的成员
{
    string Name { get; }
    // 只读属性 Name：返回预览器的名称（类型：string；原名：Name）
    // 用途：用于在 UI 或日志中显示该预览器的可读名称

    string Description { get; }
    // 只读属性 Description：返回预览器的描述（类型：string；原名：Description）
    // 用途：提供关于预览器功能或用途的简短说明

    // this only supports one previewer per plugin, but that's probably fine for now
    // 注释：当前设计每个插件只支持一个预览器（保留英文原注释）

    UavPluginPreviewerType SupportsPreview(Workspace workspace, AssetInst selection);
    // 方法 SupportsPreview：判断该预览器是否支持对给定资产的预览并返回支持的预览类型（返回 UavPluginPreviewerType；原名：SupportsPreview）
    // 参数：workspace（Workspace，当前工作区），selection（AssetInst，待预览的资产）
    // 用途：用于在 UI 中决定是否显示该预览器以及选择哪种预览方式（Text/Image/Mesh 等）

    string? ExecuteText(Workspace workspace, IUavPluginFunctions funcs, AssetInst selection, out string? error);
    // 方法 ExecuteText：尝试以文本形式生成预览内容（返回 string?，可能为 null；原名：ExecuteText）
    // 参数：workspace（Workspace），funcs（IUavPluginFunctions，插件可调用的宿主功能），selection（AssetInst），out error（输出错误信息字符串或 null）
    // 用途：当 SupportsPreview 表示支持文本预览时调用，返回要显示的文本或 null，并通过 error 返回错误信息（如果有）

    Bitmap? ExecuteImage(Workspace workspace, IUavPluginFunctions funcs, AssetInst selection, out string? error);
    // 方法 ExecuteImage：尝试以图像（Bitmap）形式生成预览（返回 Bitmap?，可能为 null；原名：ExecuteImage）
    // 参数：workspace（Workspace），funcs（IUavPluginFunctions），selection（AssetInst），out error（输出错误信息或 null）
    // 用途：当 SupportsPreview 表示支持图像预览时调用，返回生成的 Bitmap 或 null，并通过 error 返回错误信息（如果有）

    MeshObj? ExecuteMesh(Workspace workspace, IUavPluginFunctions funcs, AssetInst selection, out string? error);
    // 方法 ExecuteMesh：尝试以 3D 网格（MeshObj）形式生成预览（返回 MeshObj?，可能为 null；原名：ExecuteMesh）
    // 参数：workspace（Workspace），funcs（IUavPluginFunctions），selection（AssetInst），out error（输出错误信息或 null）
    // 用途：当 SupportsPreview 表示支持网格预览时调用，返回生成的 MeshObj 或 null，并通过 error 返回错误信息（如果有）

    // called when the workspace is closing/resetting
    // 注释：当工作区关闭或重置时调用（保留英文原注释）

    void Cleanup();
    // 方法 Cleanup：无返回值（void；原名：Cleanup）
    // 用途：在工作区关闭或插件卸载前执行清理工作（释放资源、取消订阅等）
}
