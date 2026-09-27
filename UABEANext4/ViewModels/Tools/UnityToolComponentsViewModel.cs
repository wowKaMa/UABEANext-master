using CommunityToolkit.Mvvm.ComponentModel;
// 引用 MVVM 组件模型：提供 ObservableProperty 等自动属性生成功能

using CommunityToolkit.Mvvm.Input;
// 引用 MVVM 输入支持：提供 RelayCommand 自动命令生成功能

using CommunityToolkit.Mvvm.Messaging;
// 引用 MVVM 消息总线：用于跨窗口/跨模块接收消息（如资产被选中）

using Dock.Model.Mvvm.Controls;
// 引用 Dock 控件基类：继承 Tool 类，使此 ViewModel 能作为停靠窗口使用

using System;
// 引用系统基础命名空间

using System.Collections.ObjectModel;
// 引用可观察集合：用于 UI 列表绑定的动态集合

using UABEANext4.AssetWorkspace;
// 引用项目资产工作区：包含 Workspace 和 AssetInst 定义

using UABEANext4.Logic;
// 引用项目逻辑层：包含 AssetsSelectedMessage 等消息定义

namespace UABEANext4.ViewModels.Tools;
// 定义命名空间：属于工具视图模型模块

public partial class UnityToolComponentsViewModel : Tool
// 定义类：UnityToolComponentsViewModel，必须继承自 Tool 才能在 Dock 布局中显示
// 必须使用 partial 关键字，以便 Source Generator 生成代码
{
    // 定义常量：工具标题，中文在前，英文原名在括号内
    private const string TOOL_TITLE = "组件汉化 (Component Translator)";

    // 公共属性：工作区。View 需要绑定它来读取 TypeTree 和 Unity 版本信息
    public Workspace Workspace { get; }

    [ObservableProperty]
    // 特性：自动生成 ActiveAssets 属性及其变更通知（OnPropertyChanged）
    private ObservableCollection<AssetInst>? _activeAssets;
    // 私有字段：存储当前选中的资产列表。当它变化时，View 会自动刷新树状图

    // 构造函数：仅供设计器预览使用（标记为 Obsolete）
    [Obsolete("仅供设计器预览 (For designer preview only)")]
    public UnityToolComponentsViewModel()
    {
        Workspace = new Workspace();
        // 创建空工作区防止设计器崩溃

        _activeAssets = new ObservableCollection<AssetInst>();
        // 初始化空集合

        Id = "UnityToolComponents";
        Title = TOOL_TITLE;
    }

    // 构造函数：运行时使用，接收真实的工作区数据
    public UnityToolComponentsViewModel(Workspace workspace)
    {
        Workspace = workspace;
        // 注入当前打开的 Unity 工作区

        _activeAssets = new ObservableCollection<AssetInst>();
        // 初始化集合

        Id = "UnityToolComponents";
        // 设置唯一 ID，用于布局保存和恢复

        Title = TOOL_TITLE;
        // 设置标签页标题

        // 注册消息监听：当用户在资源列表选中新资产时触发
        WeakReferenceMessenger.Default.Register<AssetsSelectedMessage>(this, OnAssetsSelected);

        // 注册消息监听：当工作区（文件）关闭时触发
        WeakReferenceMessenger.Default.Register<WorkspaceClosingMessage>(this, OnWorkspaceClosing);

        // 注册消息监听：当资产数据发生更新时触发（例如被编辑过）
        WeakReferenceMessenger.Default.Register<AssetsUpdatedMessage>(this, OnAssetsUpdated);
    }

    // 消息处理方法：处理资产选中事件
    private void OnAssetsSelected(object recipient, AssetsSelectedMessage message)
    {
        // 确保 ActiveAssets 不为空
        if (ActiveAssets == null) ActiveAssets = new ObservableCollection<AssetInst>();

        // 清空当前列表
        ActiveAssets.Clear();

        // 遍历消息中携带的所有选中资产
        foreach (var asset in message.Value)
        {
            // 将资产加入集合。这会立即触发 View 中的 AssetDataTreeView 重新加载并汉化
            ActiveAssets.Add(asset);
        }
    }

    // 消息处理方法：处理工作区关闭事件
    private void OnWorkspaceClosing(object recipient, WorkspaceClosingMessage message)
    {
        // 清空列表，避免显示残留的旧数据
        ActiveAssets?.Clear();
    }

    // 消息处理方法：处理资产更新事件（例如你在别的窗口修改了值）
    private void OnAssetsUpdated(object recipient, AssetsUpdatedMessage message)
    {
        var asset = message.Value;
        // 获取更新的资产

        if (ActiveAssets != null && ActiveAssets.Contains(asset))
        {
            // 简单粗暴的刷新策略：触发 Refresh 命令
            Refresh();
        }
    }

    [RelayCommand]
    // 特性：自动生成名为 RefreshCommand 的 ICommand 属性
    // View 中绑定 Command="{Binding RefreshCommand}" 即调用此方法
    private void Refresh()
    {
        // 逻辑：强制 UI 重新读取 ActiveAssets
        if (ActiveAssets != null && ActiveAssets.Count > 0)
        {
            // 创建一个临时副本
            var temp = new ObservableCollection<AssetInst>(ActiveAssets);

            // 先清空，通知 UI 列表变空
            ActiveAssets.Clear();

            // 再重新添加，触发 UI 重新渲染和汉化流程
            foreach (var item in temp)
            {
                ActiveAssets.Add(item);
            }
        }
    }
}