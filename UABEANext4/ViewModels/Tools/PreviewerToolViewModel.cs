using Avalonia.Media.Imaging; // 引用 Avalonia 的位图类型，用于显示和处理图像（保持原名 Avalonia.Media.Imaging）

using AvaloniaEdit.Document; // 引用 AvaloniaEdit 的文档类型，用于文本预览（保持原名 AvaloniaEdit.Document）

using CommunityToolkit.Mvvm.ComponentModel; // 引用 MVVM 工具包的组件模型，提供 ObservableProperty 等特性（保持原名 CommunityToolkit.Mvvm.ComponentModel）

using CommunityToolkit.Mvvm.Messaging; // 引用 MVVM 工具包的消息总线，用于组件间通信（保持原名 CommunityToolkit.Mvvm.Messaging）

using Dock.Model.Mvvm.Controls; // 引用 Dock.Model 的 MVVM 控件基类（Tool 等），用于停靠工具（保持原名 Dock.Model.Mvvm.Controls）

using System; // 引用基础系统命名空间，提供基本类型和工具（保持原名 System）

using UABEANext4.AssetWorkspace; // 引用项目的资产工作区命名空间，包含 Workspace、AssetInst 等（保持原名 UABEANext4.AssetWorkspace）

using UABEANext4.Logic; // 引用项目逻辑层命名空间（保持原名 UABEANext4.Logic）

using UABEANext4.Logic.Mesh; // 引用网格相关逻辑命名空间，提供 MeshObj 等类型（保持原名 UABEANext4.Logic.Mesh）

using UABEANext4.Plugins; // 引用插件相关命名空间，提供预览器插件接口（保持原名 UABEANext4.Plugins）

namespace UABEANext4.ViewModels.Tools; // 定义命名空间 UABEANext4.ViewModels.Tools（保持原名），组织工具视图模型类

public partial class PreviewerToolViewModel : Tool // 定义部分类 PreviewerToolViewModel，继承自 Tool（保持类名 PreviewerToolViewModel）
{
    const string TOOL_TITLE = "预览器 (Previewer)"; // 常量：工具标题，显示为中文并在括号中保留原始英文（TOOL_TITLE）

    public Workspace Workspace { get; } // 只读属性：保存传入的 Workspace 实例，用于访问已加载的资产（Workspace）

    [ObservableProperty] // 特性：自动生成属性与通知（将生成 ActiveImage 属性等）
    public Bitmap? _activeImage; // 字段：当前显示的图像（ActiveImage 的后备字段，类型 Bitmap，可为空）

    [ObservableProperty] // 特性：自动生成属性与通知
    public TextDocument? _activeDocument; // 字段：当前显示的文本文档（ActiveDocument 的后备字段，类型 TextDocument，可为空）

    [ObservableProperty] // 特性：自动生成属性与通知
    public MeshObj? _activeMesh; // 字段：当前显示的网格对象（ActiveMesh 的后备字段，类型 MeshObj，可为空）

    [ObservableProperty] // 特性：自动生成属性与通知
    public PreviewerToolPreviewType _activePreviewType = PreviewerToolPreviewType.Text; // 字段：当前预览类型（Image/Text/Mesh），默认 Text（ActivePreviewType）

    // defer this to first preview since dialogs won't exist until after initial load
    // 注释：延迟创建插件辅助函数，直到第一次预览时再实例化，因为对话框在初始加载前可能不存在
    private readonly Lazy<UavPluginFunctions> _uavPluginFuncs = new(() => new UavPluginFunctions()); // 延迟初始化的插件函数包装器（_uavPluginFuncs）

    [Obsolete("This constructor is for the designer only and should not be used directly.", true)] // 标记：此构造函数仅供设计器使用，不应直接调用（保留原始英文说明）
    public PreviewerToolViewModel() // 无参构造函数（仅供设计器）
    {
        Workspace = new(); // 为设计器创建一个新的 Workspace 实例（Workspace = new()）

        Id = TOOL_TITLE.Replace(" ", ""); // 设置工具 Id（将 TOOL_TITLE 中的空格移除以生成 Id）
        Title = TOOL_TITLE; // 设置工具标题（Title），显示为中文并保留英文原名

        _activeImage = null; // 初始化活动图像为 null（无图像）
        _activeDocument = new TextDocument(); // 初始化活动文档为空的 TextDocument（用于设计器预览）
        _activeMesh = new MeshObj(); // 初始化活动网格为一个空的 MeshObj（用于设计器预览）
    }

    public PreviewerToolViewModel(Workspace workspace) // 运行时构造函数：接收 Workspace 实例
    {
        Workspace = workspace; // 将传入的 workspace 赋值给属性 Workspace

        Id = TOOL_TITLE.Replace(" ", ""); // 设置工具 Id（从 TOOL_TITLE 生成）
        Title = TOOL_TITLE; // 设置工具标题（显示中文并保留英文原名）

        _activeImage = null; // 初始无图像
        _activeDocument = new TextDocument("无预览可用 (No preview available)."); // 初始文档显示中文提示并保留英文原文

        WeakReferenceMessenger.Default.Register<AssetsSelectedMessage>(this, OnAssetsSelected); // 注册消息监听：当资产被选中时调用 OnAssetsSelected
        WeakReferenceMessenger.Default.Register<WorkspaceClosingMessage>(this, OnWorkspaceClosing); // 注册消息监听：当工作区关闭时调用 OnWorkspaceClosing
    }

    private void OnAssetsSelected(object recipient, AssetsSelectedMessage message) // 私有方法：处理 AssetsSelectedMessage（当资产被选中）
    {
        var assets = message.Value; // 从消息中获取资产列表（assets）
        if (assets.Count == 0) // 如果没有选中任何资产
        {
            return; // 直接返回，不做处理
        }

        var asset = assets[0]; // 取第一个资产作为预览目标（asset）
        HandleAssetPreview(asset); // 调用 HandleAssetPreview 处理该资产的预览
    }

    private void OnWorkspaceClosing(object recipient, WorkspaceClosingMessage message) // 私有方法：处理工作区关闭消息
    {
        HandleAssetPreview(null); // 在工作区关闭时清空预览（传入 null）
    }

    private void HandleAssetPreview(AssetInst? asset) // 私有方法：根据传入的 AssetInst 选择并显示合适的预览（HandleAssetPreview）
    {
        if (asset is null) // 如果 asset 为 null（表示清空或无选中）
        {
            SetDisplayText(string.Empty); // 将显示设置为空文本（清空预览文本）
            return; // 返回
        }

        var pluginsList = Workspace.Plugins.GetPreviewersThatSupport(Workspace, asset); // 从 Workspace 的插件管理器获取支持该资产的预览器列表（pluginsList）
        if (pluginsList == null || pluginsList.Count == 0) // 如果没有可用的预览器
        {
            SetDisplayText("无预览可用 (No preview available)."); // 显示中文提示并保留英文原文
            return; // 返回
        }

        var firstPrevPair = pluginsList[0]; // 取第一个可用的预览器对（通常为最优先的）
        var prevType = firstPrevPair.PreviewType; // 获取该预览器的类型（Image/Text/Mesh）
        var prev = firstPrevPair.Previewer; // 获取实际的预览器实例（prev）

        switch (prevType) // 根据预览器类型分支处理
        {
            case UavPluginPreviewerType.Image: // 如果预览类型为图像
            {
                ActivePreviewType = PreviewerToolPreviewType.Image; // 将 ActivePreviewType 设置为 Image
                DisposeCurrentImage(); // 释放当前图像资源（如果有）

                var image = prev.ExecuteImage(Workspace, _uavPluginFuncs.Value, asset, out string? error); // 调用插件的 ExecuteImage 获取图像，返回可能的错误信息
                if (image != null) // 如果插件返回了图像
                {
                    ActiveImage = image; // 将 ActiveImage 设置为返回的图像
                }
                else // 如果插件未返回图像
                {
                    SetDisplayText(error ?? "空错误 ([null error])"); // 显示错误信息，优先显示插件返回的 error，否则显示占位中文并保留英文原文
                }
                break; // 结束该分支
            }
            case UavPluginPreviewerType.Text: // 如果预览类型为文本
            {
                ActivePreviewType = PreviewerToolPreviewType.Text; // 将 ActivePreviewType 设置为 Text

                var textString = prev.ExecuteText(Workspace, _uavPluginFuncs.Value, asset, out string? error); // 调用插件的 ExecuteText 获取文本预览
                if (textString != null) // 如果插件返回了文本
                {
                    ActiveDocument = new TextDocument(textString); // 将 ActiveDocument 设置为包含返回文本的新 TextDocument
                }
                else // 如果插件未返回文本
                {
                    SetDisplayText(error ?? "空错误 ([null error])"); // 显示错误信息或占位文本
                }
                break; // 结束该分支
            }
            case UavPluginPreviewerType.Mesh: // 如果预览类型为网格
            {
                ActivePreviewType = PreviewerToolPreviewType.Mesh; // 将 ActivePreviewType 设置为 Mesh

                var meshObj = prev.ExecuteMesh(Workspace, _uavPluginFuncs.Value, asset, out string? error); // 调用插件的 ExecuteMesh 获取网格对象
                if (meshObj != null) // 如果插件返回了网格对象
                {
                    ActiveMesh = meshObj; // 将 ActiveMesh 设置为返回的网格对象
                }
                else // 如果插件未返回网格
                {
                    SetDisplayText(error ?? "空错误 ([null error])"); // 显示错误信息或占位文本
                }
                break; // 结束该分支
            }
            default: // 其他未知类型
            {
                SetDisplayText($"不支持的预览类型 {prevType} (Preview type {prevType} not supported)."); // 显示不支持的类型，中文提示并保留英文原文
                break; // 结束默认分支
            }
        }
    }

    private void SetDisplayText(string text) // 私有方法：将预览切换到文本模式并显示指定文本（SetDisplayText）
    {
        ActivePreviewType = PreviewerToolPreviewType.Text; // 将预览类型设置为文本
        ActiveDocument = new TextDocument(text); // 创建并设置 ActiveDocument 为包含传入文本的 TextDocument
    }

    private void DisposeCurrentImage() // 私有方法：释放当前活动图像的资源（DisposeCurrentImage）
    {
        if (ActiveImage != null) // 如果当前有活动图像
        {
            ActiveImage.Dispose(); // 释放图像占用的非托管资源
            ActiveImage = null; // 将 ActiveImage 设为 null，表示无图像
        }
    }
} // 类 PreviewerToolViewModel 结束

public enum PreviewerToolPreviewType // 定义枚举 PreviewerToolPreviewType，表示预览器支持的三种预览类型
{
    Image, // 图像预览（Image）
    Text, // 文本预览（Text）
    Mesh, // 网格预览（Mesh）
}
