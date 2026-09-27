using Avalonia.Media.Imaging;
// 引用 Avalonia 的位图处理命名空间，用于 Bitmap 类型（原名: Avalonia.Media.Imaging）

using AvaloniaEdit.Document;
// 引用 AvaloniaEdit 的文档模型命名空间，用于 TextDocument 类型（原名: AvaloniaEdit.Document）

using System.Threading.Tasks;
// 引用异步任务支持命名空间，提供 Task、async/await（原名: System.Threading.Tasks）

using UABEANext4.Logic.Mesh;
// 引用项目中与网格逻辑相关的命名空间，用于 MeshObj 类型（原名: UABEANext4.Logic.Mesh）

namespace UABEANext4.Plugins;
// 定义命名空间 UABEANext4.Plugins，用于组织插件相关类型（原名: UABEANext4.Plugins）

public interface IUavPluginPreviewerFunctions
// 定义公共接口 IUavPluginPreviewerFunctions（原名: IUavPluginPreviewerFunctions），声明预览器可调用的功能方法
{
    // 接口体开始

    public Task SetPreviewText(TextDocument document);
    // 异步方法声明 SetPreviewText：接收一个 TextDocument（文本文档）并用于设置/显示文本预览（返回 Task 表示异步操作完成）
    // （方法名与类型原名: SetPreviewText, TextDocument）

    public Task SetPreviewImage(Bitmap image);
    // 异步方法声明 SetPreviewImage：接收一个 Bitmap（位图）并用于设置/显示图像预览（返回 Task 表示异步操作完成）
    // （方法名与类型原名: SetPreviewImage, Bitmap）

    public Task SetPreviewMesh(MeshObj mesh);
    // 异步方法声明 SetPreviewMesh：接收一个 MeshObj（网格对象）并用于设置/显示 3D 网格预览（返回 Task 表示异步操作完成）
    // （方法名与类型原名: SetPreviewMesh, MeshObj）

}
// 接口体结束
