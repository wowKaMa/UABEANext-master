using CommunityToolkit.Mvvm.ComponentModel; // 引用 CommunityToolkit.Mvvm 的组件模型（提供 ObservableProperty、ObservableObject 等 MVVM 基类），保留原名 CommunityToolkit.Mvvm.ComponentModel

using CommunityToolkit.Mvvm.Messaging; // 引用 CommunityToolkit.Mvvm 的消息总线（WeakReferenceMessenger），保留原名 CommunityToolkit.Mvvm.Messaging

using Dock.Model.Mvvm.Controls; // 引用 Dock.Model 的 MVVM 控件基类（Tool 等），保留原名 Dock.Model.Mvvm.Controls

using System; // 引用基础系统命名空间，提供基本类型与工具，保留原名 System

using System.Collections.ObjectModel; // 引用可观察集合类型（ObservableCollection），用于 UI 绑定，保留原名 System.Collections.ObjectModel

using UABEANext4.AssetWorkspace; // 引用项目的资产工作区命名空间（Workspace、AssetInst 等），保留原名 UABEANext4.AssetWorkspace

using UABEANext4.Logic; // 引用项目逻辑层命名空间（包含 WorkspaceClosingMessage、AssetsSelectedMessage 等），保留原名 UABEANext4.Logic

namespace UABEANext4.ViewModels.Tools; // 定义命名空间 UABEANext4.ViewModels.Tools，用于组织工具视图模型类（保留原名）

public partial class InspectorToolViewModel : Tool // 定义部分类 InspectorToolViewModel，继承自 Tool（Tool 保留原名），表示“检查器”工具的视图模型
{
    const string TOOL_TITLE = "检查器 (Inspector)"; // 常量：工具标题，中文显示“检查器”，括号内保留英文原名 (Inspector)

    public Workspace Workspace { get; } // 只读属性：保存传入或创建的 Workspace 实例，用于访问已加载的资产（Workspace 保留原名）

    [ObservableProperty] // 特性：由 CommunityToolkit 自动生成属性与属性变更通知（将生成 ActiveAssets 属性的封装）
    public ObservableCollection<AssetInst> _activeAssets; // 字段：ActiveAssets 的后备字段，类型为 ObservableCollection<AssetInst>，用于绑定当前被检查的资产列表

    [Obsolete("This constructor is for the designer only and should not be used directly.", true)] // 标记：此构造函数仅供设计器使用，不应直接调用（保留原始英文说明）
    public InspectorToolViewModel() // 无参构造函数（仅供设计器使用）
    {
        Workspace = new(); // 为设计器创建一个新的 Workspace 实例（Workspace = new()），以便设计器预览时有数据源

        ActiveAssets = new(); // 初始化生成的 ActiveAssets 属性为一个空的 ObservableCollection（便于绑定）

        Id = TOOL_TITLE.Replace(" ", ""); // 设置工具 Id（将 TOOL_TITLE 中的空格移除以生成 Id），Id 保留英文/中文混合来源
        Title = TOOL_TITLE; // 设置工具标题（Title），显示为中文并保留英文原名
    }

    public InspectorToolViewModel(Workspace workspace) // 运行时构造函数：接收外部注入的 Workspace 实例
    {
        Workspace = workspace; // 将传入的 workspace 赋值给属性 Workspace，供工具使用

        ActiveAssets = new(); // 初始化 ActiveAssets 为一个空的 ObservableCollection（用于 UI 绑定）

        Id = TOOL_TITLE.Replace(" ", ""); // 设置工具 Id（从 TOOL_TITLE 生成）
        Title = TOOL_TITLE; // 设置工具标题（显示中文并保留英文原名）

        WeakReferenceMessenger.Default.Register<AssetsSelectedMessage>(this, OnAssetsSelected); // 注册消息监听：当资产被选中（AssetsSelectedMessage）时调用 OnAssetsSelected
        WeakReferenceMessenger.Default.Register<AssetsUpdatedMessage>(this, OnAssetsUpdated); // 注册消息监听：当资产更新（AssetsUpdatedMessage）时调用 OnAssetsUpdated
        WeakReferenceMessenger.Default.Register<WorkspaceClosingMessage>(this, OnWorkspaceClosing); // 注册消息监听：当工作区关闭（WorkspaceClosingMessage）时调用 OnWorkspaceClosing
    }

    private void OnAssetsSelected(object recipient, AssetsSelectedMessage message) // 私有方法：处理 AssetsSelectedMessage（当资产被选中时触发）
    {
        ActiveAssets.Clear(); // 清空当前 ActiveAssets 列表，准备填充新的选中资产

        foreach (var asset in message.Value) // 遍历消息中携带的资产集合（message.Value）
        {
            ActiveAssets.Add(asset); // 将每个资产添加到 ActiveAssets（触发 UI 更新）
        }
    }

    private void OnAssetsUpdated(object recipient, AssetsUpdatedMessage message) // 私有方法：处理 AssetsUpdatedMessage（当某个资产被更新时触发）
    {
        var asset = message.Value; // 从消息中获取被更新的资产实例（asset）

        var index = ActiveAssets.IndexOf(asset); // 在 ActiveAssets 中查找该资产的索引（IndexOf）
        if (index != -1) // 如果找到了（索引不为 -1）
        {
            ActiveAssets[index] = asset; // 用新的资产实例替换集合中对应位置的项，以触发集合变更和 UI 刷新
        }
    }

    private void OnWorkspaceClosing(object recipient, WorkspaceClosingMessage message) // 私有方法：处理 WorkspaceClosingMessage（当工作区关闭时触发）
    {
        ActiveAssets.Clear(); // 清空 ActiveAssets，释放对资产的引用并更新 UI
    }
}
