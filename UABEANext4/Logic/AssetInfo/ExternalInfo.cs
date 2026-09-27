using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于访问 Unity 资产解析相关类型（原名：AssetsTools.NET）

using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展功能（原名：AssetsTools.NET.Extra）

using CommunityToolkit.Mvvm.ComponentModel;
// 引用 CommunityToolkit MVVM 的组件模型命名空间，提供 ObservableObject 与属性生成特性（原名：CommunityToolkit.Mvvm.ComponentModel）

using CommunityToolkit.Mvvm.DependencyInjection;
// 引用 CommunityToolkit 的依赖注入支持，用于获取服务（原名：CommunityToolkit.Mvvm.DependencyInjection）

using DynamicData;
// 引用 DynamicData 库，用于响应式集合与变更集（原名：DynamicData）

using DynamicData.Binding;
// 引用 DynamicData 的绑定扩展，用于将变更集绑定到 ObservableCollection（原名：DynamicData.Binding）

using System;
// 引用基础系统命名空间，提供常用类型（原名：System）

using System.Collections.ObjectModel;
// 引用可观察集合命名空间，提供 ObservableCollection 与 ReadOnlyObservableCollection（原名：System.Collections.ObjectModel）

using UABEANext4.AssetWorkspace;
// 引用项目内的资产工作区命名空间，提供 Workspace、AssetInst、AssetPPtr 等类型（原名：UABEANext4.AssetWorkspace）

using UABEANext4.Services;
// 引用项目内的服务接口命名空间（原名：UABEANext4.Services），例如 IDialogService

using UABEANext4.ViewModels;
// 引用项目内的视图模型基类命名空间（原名：UABEANext4.ViewModels）

using UABEANext4.ViewModels.Dialogs;
// 引用项目内对话框相关的视图模型命名空间（原名：UABEANext4.ViewModels.Dialogs）

namespace UABEANext4.Logic.AssetInfo;
// 定义命名空间 UABEANext4.Logic.AssetInfo，用于组织与资产信息显示相关的类型（原名：UABEANext4.Logic.AssetInfo）

public partial class ExternalInfo : ViewModelBase
// 定义部分类 ExternalInfo，继承自 ViewModelBase（用于 MVVM），用于管理并展示文件外部依赖信息（原名：ExternalInfo / ViewModelBase）
{
    // 类体开始（ExternalInfo）

    public ObservableCollection<AssetsFileExternal> Externals { get; set; } = [];
    // 公共属性 Externals（ObservableCollection<AssetsFileExternal>）：保存文件的外部依赖列表并初始化为空集合（原名：Externals / AssetsFileExternal）

    public ReadOnlyObservableCollection<string> ExternalsDisplay { get; init; }
    // 公共只读初始化属性 ExternalsDisplay（ReadOnlyObservableCollection<string>）：用于绑定到 UI 的外部依赖显示字符串集合（原名：ExternalsDisplay）

    private Workspace _workspace;
    // 私有字段 _workspace（Workspace）：保存传入的工作区引用，用于后续查找依赖或读取 BaseField（原名：_workspace / Workspace）

    private AssetsFileInstance _fileInst;
    // 私有字段 _fileInst（AssetsFileInstance）：保存当前处理的文件实例引用（原名：_fileInst / AssetsFileInstance）

    [ObservableProperty]
    // 特性：由 CommunityToolkit 自动生成对应的公开属性与通知代码（原名：ObservableProperty）

    [NotifyPropertyChangedFor(nameof(IsAssetSelected))]
    // 特性：当生成的属性改变时，通知 IsAssetSelected 属性也应触发变更通知（原名：NotifyPropertyChangedFor）

    [NotifyPropertyChangedFor(nameof(CanSelectedAssetMoveUp))]
    // 特性：当生成的属性改变时，通知 CanSelectedAssetMoveUp 属性也应触发变更通知（原名：NotifyPropertyChangedFor）

    [NotifyPropertyChangedFor(nameof(CanSelectedAssetMoveDown))]
    // 特性：当生成的属性改变时，通知 CanSelectedAssetMoveDown 属性也应触发变更通知（原名：NotifyPropertyChangedFor）

    public int _selectedExternIndex = -1;
    // 由 ObservableProperty 标注的字段 _selectedExternIndex：表示当前选中的外部依赖索引，初始为 -1（表示未选中）（原名：_selectedExternIndex）

    public bool IsAssetSelected => SelectedExtern != null;
    // 只读计算属性 IsAssetSelected：当 SelectedExtern 非空时返回 true，表示有外部依赖被选中（原名：IsAssetSelected / SelectedExtern）

    public bool CanSelectedAssetMoveUp => SelectedExternIndex != 0;
    // 只读计算属性 CanSelectedAssetMoveUp：当选中索引不是第一个（0）时返回 true，表示可以上移（原名：CanSelectedAssetMoveUp）

    public bool CanSelectedAssetMoveDown => SelectedExternIndex != Externals.Count - 1;
    // 只读计算属性 CanSelectedAssetMoveDown：当选中索引不是最后一个时返回 true，表示可以下移（原名：CanSelectedAssetMoveDown）

    public AssetsFileExternal? SelectedExtern => SelectedExternIndex != -1 ? Externals[SelectedExternIndex] : null;
    // 只读计算属性 SelectedExtern：根据 SelectedExternIndex 返回对应的 AssetsFileExternal 或 null（原名：SelectedExtern）

    public ExternalInfo(Workspace workspace, AssetsFileInstance fileInst)
    // 构造函数 ExternalInfo(Workspace workspace, AssetsFileInstance fileInst)：从文件实例初始化外部依赖列表并设置工作区引用（原名：ExternalInfo）
    {
        // 构造体开始

        var externals = fileInst.file.Metadata.Externals;
        // 从传入的 fileInst（AssetsFileInstance）中读取 Metadata.Externals（外部依赖数组），赋给局部变量 externals（原名：externals）

        foreach (var external in externals)
        {
            Externals.Add(external);
        }
        // 将文件元数据中的每个外部依赖添加到本实例的 Externals 集合中（原名：Externals）

        _workspace = workspace;
        // 将传入的 workspace 保存到私有字段 _workspace（原名：_workspace）

        _fileInst = fileInst;
        // 将传入的 fileInst 保存到私有字段 _fileInst（原名：_fileInst）

        Externals
            .ToObservableChangeSet()
            .Transform(ExternalsNameTransFac)
            .Bind(out var externalsItems)
            .DisposeMany()
            .Subscribe();
        // 使用 DynamicData 将 Externals 转为变更集：
        // - ToObservableChangeSet() 创建可观察的变更集
        // - Transform(ExternalsNameTransFac) 将每个 AssetsFileExternal 转换为显示字符串（使用 ExternalsNameTransFac）
        // - Bind(out var externalsItems) 将转换结果绑定到本地变量 externalsItems（将成为 ObservableCollection<string>）
        // - DisposeMany() 在元素移除时处理资源释放（惯用）
        // - Subscribe() 订阅变更集以使绑定生效

        ExternalsDisplay = externalsItems!;
        // 将绑定得到的 externalsItems 赋值给只读属性 ExternalsDisplay（使用 null-forgiving 操作符 ! 假定非空）
    }
    // 构造体结束

    private string ExternalsNameTransFac(AssetsFileExternal dep, int idx)
    // 私有方法 ExternalsNameTransFac：将单个 AssetsFileExternal（dep）与其索引（idx）转换为用于显示的字符串
    {
        // 方法体开始

        var origNameStr = "";
        // 初始化 origNameStr 为空字符串，用于在存在父 bundle 时追加原始包名信息（原名：origNameStr）

        if (_fileInst.parentBundle is not null)
        {
            var possibleDep = _fileInst.GetDependency(_workspace.Manager, idx);
            if (possibleDep is { parentBundle: not null })
            {
                origNameStr = $" - {possibleDep.parentBundle.name}";
            }
        }
        // 如果当前文件（_fileInst）属于一个 bundle（parentBundle 非空），尝试通过 GetDependency 获取索引对应的依赖信息：
        // 如果该依赖解析出 parentBundle，则将其名称追加到 origNameStr（格式 " - 包名"），以便在显示中提示来源包

        if (dep.PathName != string.Empty)
            return $"{idx} - {dep.PathName}{origNameStr}";
        else
            return $"{idx} - {dep.Guid}{origNameStr}";
    }
    // 方法体结束（ExternalsNameTransFac）
    // 说明：如果外部依赖有 PathName 则显示 "索引 - PathName"，否则显示 "索引 - Guid"，并在后面追加 origNameStr（如果存在）

    public async void Add_Click()
    // 公共异步方法 Add_Click：响应“添加外部依赖”操作，弹出对话框获取新外部依赖并添加到集合
    {
        // 方法体开始

        var dialogService = Ioc.Default.GetRequiredService<IDialogService>();
        // 通过依赖注入容器（Ioc.Default）获取 IDialogService 服务实例，用于显示对话框（原名：IDialogService）

        var newExtern = await dialogService.ShowDialog(new AddExternalViewModel(null));
        // 弹出 AddExternalViewModel 对话框（传入 null 表示新建），等待用户输入并返回新外部依赖（newExtern）

        if (newExtern == null)
        {
            return;
        }
        // 如果用户取消或未返回有效外部依赖，则直接返回不做任何修改

        Externals.Add(newExtern);
        // 将新创建的外部依赖添加到 Externals 集合中（集合变更会通过 DynamicData 更新 ExternalsDisplay）
    }
    // 方法体结束（Add_Click）

    public async void Edit_Click()
    // 公共异步方法 Edit_Click：响应“编辑外部依赖”操作，弹出对话框编辑当前选中项并替换集合中的项
    {
        // 方法体开始

        if (SelectedExtern == null)
        {
            return;
        }
        // 如果当前没有选中外部依赖则直接返回

        var dialogService = Ioc.Default.GetRequiredService<IDialogService>();
        // 通过依赖注入获取对话框服务（IDialogService）

        var newExtern = await dialogService.ShowDialog(new AddExternalViewModel(SelectedExtern));
        // 弹出 AddExternalViewModel 对话框并传入当前选中项以便编辑，等待返回编辑后的外部依赖

        if (newExtern == null)
        {
            return;
        }
        // 如果用户取消或未返回有效结果则返回

        int oldSelectedIndex = SelectedExternIndex;
        // 保存当前选中索引以便在替换后恢复选中状态

        Externals[SelectedExternIndex] = newExtern;
        // 将集合中对应索引的项替换为编辑后的 newExtern（这会触发集合变更）

        SelectedExternIndex = oldSelectedIndex;
        // 恢复选中索引，确保 UI 仍然选中刚才编辑的位置
    }
    // 方法体结束（Edit_Click）

    public void Remove_Click()
    // 公共方法 Remove_Click：响应“移除外部依赖”操作，从集合中移除当前选中项
    {
        Externals.RemoveAt(SelectedExternIndex);
        // 从 Externals 集合中按索引移除元素（注意：调用前应确保 SelectedExternIndex 有效）
    }
    // 方法体结束（Remove_Click）

    // we can't use .Move() because it doesn't update indices.
    // swapping only two items is fine because moving only
    // once up or down means no other indices update.
    // 注释：说明为什么不使用集合的 Move 方法，而是通过交换两个元素来实现上移/下移（保留英文原注释）

    public void MoveUp_Click()
    // 公共方法 MoveUp_Click：将当前选中项上移一位（通过交换实现）
    {
        // 方法体开始

        if (SelectedExternIndex != -1 && SelectedExternIndex > 0)
        {
            int index = SelectedExternIndex;
            AssetsFileExternal newExternal = Externals[index - 1];
            AssetsFileExternal oldExternal = Externals[index];
            Externals[index - 1] = oldExternal;
            Externals[index] = newExternal;
            SelectedExternIndex = index - 1;
        }
    }
    // 方法体结束（MoveUp_Click）
    // 说明：如果选中项存在且不是第一个，则与上一个元素交换位置，并更新 SelectedExternIndex 指向新的位置

    public void MoveDown_Click()
    // 公共方法 MoveDown_Click：将当前选中项下移一位（通过交换实现）
    {
        // 方法体开始

        if (SelectedExternIndex != -1 && SelectedExternIndex < Externals.Count - 1)
        {
            int index = SelectedExternIndex;
            AssetsFileExternal newExternal = Externals[index + 1];
            AssetsFileExternal oldExternal = Externals[index];
            Externals[index + 1] = oldExternal;
            Externals[index] = newExternal;
            SelectedExternIndex = index + 1;
        }
    }
    // 方法体结束（MoveDown_Click）
    // 说明：如果选中项存在且不是最后一个，则与下一个元素交换位置，并更新 SelectedExternIndex 指向新的位置

    private ExternalInfo()
    // 私有无参构造函数：用于创建空实例（例如作为 Empty 单例），并初始化 ExternalsDisplay 为空集合
    {
        ExternalsDisplay = new ReadOnlyObservableCollection<string>(new ObservableCollection<string>());
    }
    // 私有构造体结束

    public static ExternalInfo Empty { get; } = new()
    {
        Externals = [],
        ExternalsDisplay = new ReadOnlyObservableCollection<string>(new ObservableCollection<string>())
    };
    // 公共静态只读属性 Empty：提供一个空的 ExternalInfo 实例作为默认值（Externals 与 ExternalsDisplay 均为空）
}
// 类体结束（ExternalInfo）
