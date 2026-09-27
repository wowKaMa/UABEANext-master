using AssetsTools.NET;
using AssetsTools.NET.Extra;

using System.Numerics;
using System.Text.RegularExpressions; // Added for FixAPose update
// using UABEANext4.Logic.Unity; // Removed: Incorrect namespace
using UABEANext4.Logic.Hierarchy;
using UABEANext4.ViewModels.Dialogs;
using UABEANext4.Interfaces;
using UABEANext4.Services;
using CommunityToolkit.Mvvm.DependencyInjection;

using Avalonia.Threading; // 引用 Avalonia 的调度器（Dispatcher）相关类型，用于在 UI 线程上调度操作

using AvaloniaEdit.Utils; // 引用 AvaloniaEdit 的工具类（例如 DispatcherTimer 的辅助类型）

using CommunityToolkit.Mvvm.ComponentModel; // 引用 CommunityToolkit.Mvvm 的组件模型，提供 ObservableProperty 等特性

using CommunityToolkit.Mvvm.Messaging; // 引用 CommunityToolkit.Mvvm 的消息总线（WeakReferenceMessenger）

using Dock.Model.Mvvm.Controls; // 引用 Dock.Model 的 MVVM 控件基类（Tool 等）

using System; // 引用基础系统命名空间，提供基本类型和工具

using System.Collections.Generic; // 引用泛型集合命名空间（List、Dictionary 等）

using System.Collections.ObjectModel; // 引用可观察集合类型（ObservableCollection），用于 UI 绑定

using System.Linq; // 引用 LINQ 扩展方法，用于集合查询与过滤

using System.Threading; // 引用线程与同步原语（CancellationTokenSource、Interlocked 等）

using System.Threading.Tasks; // 引用异步任务支持（Task、async/await）

using UABEANext4.AssetWorkspace; // 引用项目的资产工作区命名空间（Workspace、AssetInst、WorkspaceItem 等）

using UABEANext4.Logic; // 引用项目逻辑层命名空间（WorkspaceItemType 等）


namespace UABEANext4.ViewModels.Tools; // 定义命名空间 UABEANext4.ViewModels.Tools，用于组织工具视图模型类

public partial class HierarchyToolViewModel : Tool // 定义部分类 HierarchyToolViewModel，继承自 Tool（表示“层级”工具的视图模型）
{
    const string TOOL_TITLE = "层级 (Hierarchy)"; // 常量：工具标题，中文显示“层级”，括号中保留英文原名（用于 UI 显示）

    public Workspace Workspace { get; } // 只读属性：保存传入或创建的 Workspace 实例，用于访问已加载的资产和文件

    [ObservableProperty] // 特性：自动生成属性与通知（将生成 ActiveAssets 属性）
    public ObservableCollection<AssetInst> _activeAssets = new(); // 字段：ActiveAssets 的后备字段，保存当前被关注/选中的资产集合

    [ObservableProperty] // 特性：自动生成属性与通知（将生成 RootItems 属性）
    public ObservableCollection<HierarchyItem> _rootItems = new(); // 字段：RootItems 的后备字段，保存树的根节点集合（用于 UI 绑定）

    [ObservableProperty] // 特性：自动生成属性与通知（将生成 SelectedItem 属性）
    public HierarchyItem? _selectedItem = null; // 字段：SelectedItem 的后备字段，表示当前在层级树中选中的项（可为空）

    [ObservableProperty]
    public ObservableCollection<object> _selectedItems = new();

    [ObservableProperty] // 特性：自动生成属性与通知（将生成 SortAlphabetically 属性）
    public bool _sortAlphabetically = false; // 字段：SortAlphabetically 的后备字段，控制是否按字母排序（默认 false）

    [ObservableProperty] // 特性：自动生成属性与通知（将生成 IsLoadingNewItems 属性）
    private bool _isLoadingNewItems = false; // 字段：IsLoadingNewItems 的后备字段，表示是否正在异步加载新项（私有）

    private List<WorkspaceItem>? _itemsToLoad = null; // 私有字段：待加载的 WorkspaceItem 列表（由消息或外部调用设置）

    private readonly List<HierarchyItem> _pendingRootItems = new(); // 私有只读字段：在后台线程生成但尚未添加到 UI 的根层级项缓存

    private DispatcherTimer? _loadingNewItemsTimer = null; // 私有字段：用于定时将 pending 项批量添加到 UI 的计时器（DispatcherTimer）

    private CancellationTokenSource? _loadingNewItemsCts = null; // 私有字段：用于取消正在进行的后台加载任务的 CancellationTokenSource

    [Obsolete("This constructor is for the designer only and should not be used directly.", true)] // 标记：此构造函数仅供设计器使用，不应在运行时直接调用
    public HierarchyToolViewModel() // 无参构造函数（仅供设计器）
    {
        Workspace = new(); // 为设计器创建一个新的 Workspace 实例，便于设计器预览时显示数据

        Id = TOOL_TITLE.Replace(" ", ""); // 设置工具 Id（将 TOOL_TITLE 中的空格移除以生成 Id，用于内部标识）

        Title = TOOL_TITLE; // 设置工具标题（Title），用于 UI 显示（中文 + 英文原名）
    }

    public HierarchyToolViewModel(Workspace workspace) // 运行时构造函数：接收外部注入的 Workspace 实例
    {
        Workspace = workspace; // 将传入的 workspace 赋值给属性 Workspace，供工具使用

        Id = TOOL_TITLE.Replace(" ", ""); // 设置工具 Id（从 TOOL_TITLE 生成）

        Title = TOOL_TITLE; // 设置工具标题（显示中文并保留英文原名）

        WeakReferenceMessenger.Default.Register<AssetsSelectedMessage>(this, OnAssetsSelected); // 注册消息监听：当资产被选中时调用 OnAssetsSelected

        WeakReferenceMessenger.Default.Register<AssetsUpdatedMessage>(this, OnAssetsUpdated); // 注册消息监听：当资产更新时调用 OnAssetsUpdated

        WeakReferenceMessenger.Default.Register<WorkspaceClosingMessage>(this, OnWorkspaceClosing); // 注册消息监听：当工作区关闭时调用 OnWorkspaceClosing

        WeakReferenceMessenger.Default.Register<RequestSceneViewMessage>(this, OnRequestSceneView); // 注册消息监听：当请求场景视图时调用 OnRequestSceneView

        WeakReferenceMessenger.Default.Register<SelectedWorkspaceItemChangedMessage>(this, (r, h) => _ = OnSelectedWorkspaceItemsChanged(r, h)); // 注册消息监听：当选中的工作区项改变时异步调用 OnSelectedWorkspaceItemsChanged
    }

    private void OnAssetsSelected(object recipient, AssetsSelectedMessage message) // 私有方法：处理 AssetsSelectedMessage（当资产被选中）
    {
        ActiveAssets.Clear(); // 清空当前 ActiveAssets 列表，准备填充新的选中资产

        if (message.Value.Count > 0) // 如果消息中包含至少一个资产
        {
            ActiveAssets.Add(message.Value[0]); // 将第一个资产加入 ActiveAssets（仅关注第一个选中项）
        }
    }

    private void OnAssetsUpdated(object recipient, AssetsUpdatedMessage message) // 私有方法：处理 AssetsUpdatedMessage（当某个资产被更新）
    {
        var asset = message.Value; // 从消息中获取被更新的资产实例

        var index = ActiveAssets.IndexOf(asset); // 在 ActiveAssets 中查找该资产的索引

        if (index != -1) // 如果找到了（索引不为 -1）
        {
            ActiveAssets[index] = asset; // 用新的资产实例替换集合中对应位置的项，以触发 UI 刷新
        }
    }

    private void OnWorkspaceClosing(object recipient, WorkspaceClosingMessage message) // 私有方法：处理 WorkspaceClosingMessage（当工作区关闭）
    {
        _loadingNewItemsCts?.Cancel(); // 如果存在取消令牌源，则请求取消后台加载任务

        _itemsToLoad = null; // 清空待加载项引用

        IsLoadingNewItems = false; // 将加载状态标记为 false

        ActiveAssets.Clear(); // 清空当前活动资产集合

        RootItems.Clear(); // 清空根层级项集合（UI 清空树）

        _pendingRootItems.Clear(); // 清空待添加的根项缓存
    }

    private void OnRequestSceneView(object recipient, RequestSceneViewMessage message) // 私有方法：处理请求场景视图消息
    {
        SelectItem(message.Value); // 调用 SelectItem 以在层级树中选择并展开对应资产
    }

    private async Task OnSelectedWorkspaceItemsChanged(object recipient, SelectedWorkspaceItemChangedMessage message) // 私有异步方法：处理选中工作区项改变的消息
    {
        _itemsToLoad = message.Value; // 将消息中的 WorkspaceItem 列表保存到 _itemsToLoad，供后续加载使用

        await LoadRootItems(); // 异步调用 LoadRootItems 加载并构建根层级项
    }

    partial void OnSortAlphabeticallyChanged(bool value) // 部分方法：当 SortAlphabetically 属性改变时由生成的代码调用
    {
        _ = LoadRootItems(); // 触发重新加载根项以应用新的排序设置（不等待结果）
    }

    private async Task LoadRootItems() // 私有异步方法：根据 _itemsToLoad 构建并加载 RootItems（LoadRootItems）
    {
        if (_itemsToLoad == null) // 如果没有待加载项
        {
            return; // 直接返回
        }

        if (IsLoadingNewItems) // 如果当前已经在加载新项
        {
            _loadingNewItemsTimer?.Stop(); // 停止现有的定时器（如果存在）

            if (_loadingNewItemsCts != null) // 如果存在取消令牌源
            {
                await _loadingNewItemsCts.CancelAsync(); // 异步请求取消正在进行的加载任务
            }
        }

        _loadingNewItemsTimer = new( // 创建一个新的 DispatcherTimer，用于定期将后台生成的 pending 项批量添加到 UI
            TimeSpan.FromSeconds(1), // 每隔 1 秒触发一次
            DispatcherPriority.Background, // 使用后台优先级调度
            (s, e) => AddPendingRootItems() // 触发时调用 AddPendingRootItems 将缓存项添加到 RootItems
        );

        List<WorkspaceItem> items = _itemsToLoad; // 将待加载项复制到局部变量 items，避免并发修改问题

        bool sortAlphabetically = SortAlphabetically; // 读取当前的排序设置到局部变量

        IsLoadingNewItems = true; // 标记正在加载新项

        RootItems.Clear(); // 清空当前 RootItems（准备重新填充）

        _loadingNewItemsCts = new CancellationTokenSource(); // 创建新的取消令牌源以便可以取消后台任务

        _pendingRootItems.Clear(); // 清空待添加的根项缓存

        _loadingNewItemsTimer.Start(); // 启动定时器，开始允许批量添加缓存项到 UI

        await Task.Run(() => // 在后台线程池中执行耗时的层级构建工作
        {
            foreach (var file in items) // 遍历每个待处理的 WorkspaceItem（通常是文件或容器）
            {
                _loadingNewItemsCts.Token.ThrowIfCancellationRequested(); // 在循环中检查是否已请求取消，若已取消则抛出异常终止任务

                if (file.ObjectType != WorkspaceItemType.AssetsFile || file.Object is not AssetsFileInstance fileInst) // 如果该项不是 AssetsFile 或无法转换为 AssetsFileInstance
                    continue; // 跳过该项

                var itemObjs = HierarchyItem.CreateRootItems(Workspace, fileInst, sortAlphabetically); // 调用 HierarchyItem.CreateRootItems 在后台构建该文件的子层级项集合

                var item = new HierarchyItem() // 创建一个表示该文件的根层级项（HierarchyItem）
                {
                    Asset = null, // 根项本身不对应具体 AssetInst，设置为 null
                    Name = fileInst.name, // 将根项名称设置为文件名（fileInst.name）
                    FileInstance = fileInst // 设置文件实例，以便拖放识别目标文件
                };

                item.Children.AddRange(itemObjs); // 将创建的子项集合添加到根项的 Children 中

                lock (_pendingRootItems) // 加锁保护对 _pendingRootItems 的并发访问
                {
                    _pendingRootItems.Add(item); // 将构建好的根项加入待添加缓存，稍后由 UI 线程批量添加
                }
            }
        }, _loadingNewItemsCts.Token); // 将取消令牌传入 Task.Run 以支持取消

        // don't try to load the rest of the items if this method
        // is being called again for a different list of items.
        // 注释：如果在加载过程中该方法被再次调用（例如用户改变了选择），则会取消当前任务并不继续添加剩余项
        if (!_loadingNewItemsCts.IsCancellationRequested) // 如果当前加载任务未被取消
        {
            AddPendingRootItems(); // 将剩余的 pending 项一次性添加到 RootItems（确保 UI 最终完整）
            _loadingNewItemsTimer.Stop(); // 停止定时器
            IsLoadingNewItems = false; // 标记加载完成
        }
    }

    private void AddPendingRootItems() // 私有方法：将缓存的 pending 根项批量添加到 RootItems（在 UI 线程调用）
    {
        lock (_pendingRootItems) // 加锁保护对 _pendingRootItems 的并发访问
        {
            RootItems.AddRange(_pendingRootItems); // 将缓存项添加到 RootItems（触发 UI 更新）
            _pendingRootItems.Clear(); // 清空缓存
        }
    }

    private void SelectItem(AssetInst asset) // 私有方法：在 RootItems 中查找并选择包含指定 AssetInst 的层级项
    {
        // slow, but do we really want to make a large lookup
        // every time we load the tree? (depends how much this
        // feature will be used)
        // 注释：作者提示：此查找可能较慢，但避免每次加载树时都做大规模索引

        // 在进行新的代码触发选中前，清空当前的复选集合以确保高亮唯一性 (Clear selection for highlight)
        SelectedItems.Clear();

        var filteredRootItems = RootItems.Where(i => i.Name == asset.FileName); // 过滤出与 asset 所属文件名匹配的根项集合

        foreach (var item in filteredRootItems) // 遍历匹配的根项
        {
            if (SearchForItemAndSelectRecursive(asset, item)) // 递归搜索并尝试选中目标项
            {
                item.Expanded = true; // 如果找到则展开该根项以显示路径
                break; // 找到后退出循环
            }
        }
    }

    private bool SearchForItemAndSelectRecursive(AssetInst asset, HierarchyItem item) // 私有递归方法：在指定层级项及其子项中查找并选中 asset
    {
        if (item.Asset == asset) // 如果当前项直接对应目标 asset
        {
            SelectedItem = item; // 将 SelectedItem 设置为当前项（触发 UI 选中）
            if (!SelectedItems.Contains(item))
            {
                SelectedItems.Add(item); // 同时添加到 SelectedItems 集合以确保 UI 高亮蓝色 (Add to SelectedItems for highlight)
            }
            return true; // 返回 true 表示已找到
        }

        foreach (HierarchyItem child in item.Children) // 遍历当前项的所有子项
        {
            if (SearchForItemAndSelectRecursive(asset, child)) // 递归搜索子项
            {
                item.Expanded = true; // 如果子项中找到目标，则展开当前项以显示路径
                return true; // 返回 true 向上层传播找到结果
            }
        }

        return false; // 未找到则返回 false
    }

    public void SelectedItemsChanged(AssetInst? asset)
    {
        if (asset == null)
            return;

        var gameObjectBf = Workspace.GetBaseField(asset);

        if (gameObjectBf == null)
            return;

        var allAssets = new List<AssetInst>
        {
            asset
        };

        var componentPairs = gameObjectBf["m_Component.Array"];

        foreach (var componentPair in componentPairs)
        {
            var component = Workspace.GetAssetInst(asset.FileInstance, componentPair[componentPair.Children.Count - 1]);
            if (component != null)
            {
                allAssets.Add(component);
            }
        }

        WeakReferenceMessenger.Default.Send(new AssetsSelectedMessage(allAssets));
    }

    public async Task MoveItemsAsync(IList<HierarchyItem> sources, HierarchyItem newParent)
    {
        if (sources == null || sources.Count == 0 || newParent == null) return;

        // Group by source file if they are from multiple files to potentially optimize later.
        // For now, we move them one by one.
        // We use ToList() to avoid issues if move logic updates the source collection.
        foreach (var source in sources.ToList())
        {
            await MoveItemAsync(source, newParent);
        }
    }

    /// <summary>
    /// Move a HierarchyItem to a new parent at the specified index.
    /// Updates the underlying Transform assets (m_Father, m_Children) and persists changes.
    /// </summary>
    public async Task<bool> MoveItemAsync(HierarchyItem source, HierarchyItem newParent, int insertIndex = -1)
    {
        // Validation
        if (source == null || newParent == null) return false;
        if (source == newParent) return false;
        if (source.FileInstance == null || newParent.FileInstance == null) return false;
        if (source.TransformPathId == 0) return false;

        try
        {
            // --- Cross-File Copy Check ---
            if (source.FileInstance != newParent.FileInstance)
            {
                // 1. Run the CrossFileCopier
                var copier = new CrossFileCopier(Workspace, source.FileInstance, newParent.FileInstance, source.TransformPathId);
                long newRootTransformId = copier.CopyHierarchy();

                if (newRootTransformId == 0)
                {
                    await Util.MessageBoxUtil.ShowDialog("写入错误 (Write Error)", "跨文件复制失败。如果您尝试导入巨大的模型，可能由于底层混淆结构过于复杂导致无法生成。请联系开发者并提供模型。\n\nCross-file copy failed due to extreme struct obfuscation or missing dependencies in the source model.");
                    return false;
                }

                // 2. We now have a cloned Transform in the target file loosely floating.
                // We must artificially wrap it in a pseudo-HierarchyItem so we can call ReparentTransformData
                // to attach it to 'newParent'.
                var clonedItem = new HierarchyItem { FileInstance = newParent.FileInstance, TransformPathId = newRootTransformId };

                // 3. Attach standard PPtrs from cloned root to newParent
                // Only if newParent is NOT a file root (PathID != 0)
                if (newParent.TransformPathId != 0)
                {
                    bool attached = ReparentTransformData(newParent.FileInstance, clonedItem, newParent);
                    if (!attached) return false;
                }

                // --- 5. Show Transfer Report ---
                var records = copier.GetTransferRecords();
                if (records.Count > 0)
                {
                    var dialogService = Ioc.Default.GetService<IDialogService>();
                    if (dialogService != null)
                    {
                        var reportVm = new TransferReportViewModel(records);
                        dialogService.Show(reportVm);
                    }
                }

                // 4. Update UI Tree for the target
                var targetFileInst = newParent.FileInstance;

                HierarchyItem BuildTree(long tfmPathId, HierarchyItem parent)
                {
                    var item = new HierarchyItem
                    {
                        TransformPathId = tfmPathId,
                        FileInstance = targetFileInst,
                        Parent = parent
                    };

                    var tfmBf = Workspace.GetBaseField(targetFileInst, tfmPathId);
                    if (tfmBf != null)
                    {
                        long goPathId = tfmBf["m_GameObject"]["m_PathID"].AsLong;
                        var goAsset = Workspace.GetAssetInst(targetFileInst, 0, goPathId);
                        if (goAsset != null)
                        {
                            item.Asset = goAsset;
                            var goBf = Workspace.GetBaseField(goAsset);
                            if (goBf != null) item.Name = goBf["m_Name"].AsString;
                        }

                        var childrenArray = tfmBf["m_Children"]["Array"];
                        if (!childrenArray.IsDummy && childrenArray.Children != null)
                        {
                            foreach (var childPtr in childrenArray.Children)
                            {
                                long childPathId = childPtr["m_PathID"].AsLong;
                                if (childPathId != 0)
                                {
                                    item.Children.Add(BuildTree(childPathId, item));
                                }
                            }
                        }
                    }
                    return item;
                }

                var newUiNode = BuildTree(newRootTransformId, newParent);
                newParent.Children.Add(newUiNode);

                if (SortAlphabetically && newParent.TransformPathId == 0)
                {
                    var sorted = newParent.Children.OrderBy(x => x.Name).ToList();
                    newParent.Children.Clear();
                    foreach (var s in sorted) newParent.Children.Add(s);
                }

                newParent.Expanded = true;
                return true;
            }

            // --- Standard Same-File Move ---
            if (IsDescendantOf(newParent, source)) return false; // Prevent circular reparenting

            var fileInst = source.FileInstance;
            var oldParent = source.Parent;

            // --- 1. Load all three Transform base fields ---
            var sourceTfmBf = Workspace.GetBaseField(fileInst, source.TransformPathId);
            if (sourceTfmBf == null) return false;

            AssetTypeValueField oldParentTfmBf = null!;
            if (oldParent != null && oldParent.TransformPathId != 0)
            {
                var tmp = Workspace.GetBaseField(fileInst, oldParent.TransformPathId);
                if (tmp != null) oldParentTfmBf = tmp;
            }
            bool hasOldParent = oldParent != null && oldParentTfmBf != null;

            var newParentTfmBf = Workspace.GetBaseField(fileInst, newParent.TransformPathId);
            if (newParentTfmBf == null) return false;

            // --- 2. Update source Transform: set m_Father to new parent ---
            sourceTfmBf["m_Father"]["m_FileID"].AsInt = 0;
            sourceTfmBf["m_Father"]["m_PathID"].AsLong = newParent.TransformPathId;

            // --- 3. Remove from old parent's m_Children.Array ---
            if (hasOldParent)
            {
                var oldChildrenArray = oldParentTfmBf["m_Children"]["Array"];
                if (!oldChildrenArray.IsDummy && oldChildrenArray.Children != null)
                {
                    oldChildrenArray.Children.RemoveAll(c => c["m_PathID"].AsLong == source.TransformPathId);
                    var oldSizeField = oldParentTfmBf["m_Children"]["Array"]["size"];
                    if (oldSizeField.IsDummy) oldSizeField = oldParentTfmBf["m_Children"]["size"];
                    if (!oldSizeField.IsDummy) oldSizeField.AsInt = oldChildrenArray.Children.Count;
                }
            }

            // --- 4. Add to new parent's m_Children.Array ---
            var newChildrenArray = newParentTfmBf["m_Children"]["Array"];
            if (!newChildrenArray.IsDummy)
            {
                if (newChildrenArray.Children == null)
                    newChildrenArray.Children = new List<AssetTypeValueField>();

                // Create PPtr child entry from template
                var newChildEntry = ValueBuilder.DefaultValueFieldFromTemplate(
                    newChildrenArray.TemplateField.Children[1]
                );

                newChildEntry["m_FileID"].AsInt = 0;
                newChildEntry["m_PathID"].AsLong = source.TransformPathId;

                if (insertIndex >= 0 && insertIndex < newChildrenArray.Children.Count)
                    newChildrenArray.Children.Insert(insertIndex, newChildEntry);
                else
                    newChildrenArray.Children.Add(newChildEntry);

                // Sync size
                var newSizeField = newParentTfmBf["m_Children"]["Array"]["size"];
                if (newSizeField.IsDummy) newSizeField = newParentTfmBf["m_Children"]["size"];
                if (!newSizeField.IsDummy) newSizeField.AsInt = newChildrenArray.Children.Count;
            }
            else return false;

            // --- 5. Persist changes to all modified assets ---
            var sourceInst = Workspace.GetAssetInst(fileInst, 0, source.TransformPathId);
            if (sourceInst != null) sourceInst.UpdateAssetDataAndRow(Workspace, sourceTfmBf);

            if (hasOldParent && oldParent != null)
            {
                var oldParentInst = Workspace.GetAssetInst(fileInst, 0, oldParent.TransformPathId);
                if (oldParentInst != null) oldParentInst.UpdateAssetDataAndRow(Workspace, oldParentTfmBf);
            }

            var newParentInst = Workspace.GetAssetInst(fileInst, 0, newParent.TransformPathId);
            if (newParentInst != null) newParentInst.UpdateAssetDataAndRow(Workspace, newParentTfmBf);

            // --- 6. Dirty the workspace ---
            var wsItem = Workspace.FindWorkspaceItemByInstance(fileInst);
            if (wsItem != null) Workspace.Dirty(wsItem);

            // --- 7. Update UI tree ---
            if (oldParent != null)
                oldParent.Children.Remove(source);
            else
            {
                // Source was at root level in a file container
                foreach (var root in RootItems)
                {
                    if (root.Children.Contains(source))
                    {
                        root.Children.Remove(source);
                        break;
                    }
                }
            }

            source.Parent = newParent;
            if (insertIndex >= 0 && insertIndex < newParent.Children.Count)
                newParent.Children.Insert(insertIndex, source);
            else
                newParent.Children.Add(source);

            newParent.Expanded = true;

            return true;
        }
        catch (Exception ex)
        {
            await Util.MessageBoxUtil.ShowDialog("序列化异常 (Serialization Crash)", $"数据复制过程中发生崩溃 (Crash during data copy) :\n{ex.Message}\n请检查您的 MonoBehaviour 数据是否损坏 (Please check if MonoBehaviour metadata is corrupted).");
            return false;
        }
    }

    /// <summary>
    /// Check if 'item' is a descendant of 'potentialAncestor' (prevents circular reparenting).
    /// </summary>
    private static bool IsDescendantOf(HierarchyItem item, HierarchyItem potentialAncestor)
    {
        var current = item.Parent;
        while (current != null)
        {
            if (current == potentialAncestor)
                return true;
            current = current.Parent;
        }
        return false;
    }

    /// <summary>
    /// Setup Outfit: match outfit bones to avatar armature bones and reparent them.
    /// Follows Modular Avatar's SetupOutfit algorithm:
    /// Phase 1: Build a complete mapping of matched bones (no modifications)
    /// Phase 2: Reparent all matched bones at the data level
    /// Phase 3: Fully reload the hierarchy tree
    /// Returns a detailed diagnostic report for the report window.
    /// </summary>
    public SetupOutfitReport SetupOutfit(HierarchyItem outfitNode)
    {
        var report = new SetupOutfitReport();
        report.OutfitName = outfitNode?.Name ?? "(null)";

        if (outfitNode == null || outfitNode.Children.Count == 0)
        {
            report.Success = false;
            report.ResultMessage = "未选择服装节点或该节点没有子项 (No outfit node selected or it has no children.)";
            return report;
        }

        // 1. Deep Hips Discovery (MA FindBones Parity)
        if (!FindBones(outfitNode, out var avatarRoot, out var avatarHips, out var outfitHips))
        {
            report.Success = false;
            report.ResultMessage = "无法自动识别头像或服装的 Hips 骨骼。请确保头像具有 Animator 且服装骨架结构标准。 (Cannot identify Hips. Ensure avatar has Animator and outfit structure is standard.)";
            return report;
        }

        // 2. Derive Armature nodes (Parents of Hips)
        var avatarArmature = avatarHips.Parent;
        var outfitArmature = outfitHips.Parent;
        if (avatarArmature == null || outfitArmature == null)
        {
            report.Success = false;
            report.ResultMessage = "骨架寻找失败：Hips 没有父节点。 (Armature find failed: Hips has no parent.)";
            return report;
        }

        // 3. Animator-based Humanoid Map (MA Parity)
        var avatarAnimator = FindComponent(avatarArmature.FileInstance!, avatarRoot.Asset?.PathId ?? 0, AssetClassID.Animator);
        var outfitAnimator = FindComponent(outfitArmature.FileInstance!, outfitNode.Asset?.PathId ?? 0, AssetClassID.Animator);

        // [MA PARITY] Validation: Block if already has Avatar Descriptor or is the avatar itself
        if (avatarRoot == outfitNode)
        {
            report.Success = false;
            report.ResultMessage = "不能在头像根节点上运行设置服装操作。 (Cannot run Setup Outfit on the avatar root itself.)";
            return report;
        }

        var existingDesc = FindComponent(outfitNode.FileInstance!, outfitNode.Asset?.PathId ?? 0, (AssetClassID)114);
        if (existingDesc != null)
        {
            var mb = Workspace.GetBaseField(existingDesc);
            if (mb != null && mb.TemplateField.Name.Contains("VRCAvatarDescriptor"))
            {
                report.Success = false;
                report.ResultMessage = "该物体已包含 Avatar Descriptor。如果您正在制作混合头像，请先将其移除。 (Object already has an Avatar Descriptor. Remove it first if making a hybrid avatar.)";
                return report;
            }
        }

        // Use the same armature guessing as FindBones for consistency
        var guessedAvatarArmature = avatarRoot.Children.FirstOrDefault(c => c.Name.ToLowerInvariant().Contains("armature")) ?? avatarRoot;
        var guessedOutfitArmature = outfitNode.Children.FirstOrDefault(c => c.Name.ToLowerInvariant().Contains("armature")) ?? outfitNode;

        var avatarHumanoidMap = avatarAnimator != null ? GetHumanoidBoneMap(avatarAnimator, guessedAvatarArmature) : new();
        var outfitHumanoidMap = outfitAnimator != null ? GetHumanoidBoneMap(outfitAnimator, guessedOutfitArmature) : new();

        // --- REPORT: FindBones ---
        report.Bones.AvatarRootName = avatarRoot.Name;
        report.Bones.AvatarRootPathId = avatarRoot.TransformPathId;
        report.Bones.AvatarHipsName = avatarHips.Name;
        report.Bones.AvatarHipsPathId = avatarHips.TransformPathId;
        report.Bones.OutfitHipsName = outfitHips.Name;
        report.Bones.OutfitHipsPathId = outfitHips.TransformPathId;
        report.Bones.AvatarArmatureName = avatarArmature.Name;
        report.Bones.AvatarArmaturePathId = avatarArmature.TransformPathId;
        report.Bones.OutfitArmatureName = outfitArmature.Name;
        report.Bones.OutfitArmaturePathId = outfitArmature.TransformPathId;
        report.Bones.HasAvatarAnimator = avatarAnimator != null;
        report.Bones.HasOutfitAnimator = outfitAnimator != null;
        report.Bones.AvatarHumanoidBoneCount = avatarHumanoidMap.Count;
        report.Bones.OutfitHumanoidBoneCount = outfitHumanoidMap.Count;
        report.Bones.AvatarRootItem = avatarRoot;
        report.Bones.AvatarHipsItem = avatarHips;
        report.Bones.OutfitHipsItem = outfitHips;

        // 4. Infer prefix/suffix
        var (prefix, suffix) = BoneNameMatcher.InferPrefixSuffix(avatarHips.Name, outfitArmature);
        report.Prefix = prefix;
        report.Suffix = suffix;

        // 5. PHASE 1: Multi-Pass Bone Mapping (MA AssignBoneMappings Parity)
        var skippedRoots = new List<HierarchyItem>();
        var mappings = AssignBoneMappings(outfitArmature, avatarArmature, prefix, suffix, avatarHumanoidMap, outfitHumanoidMap, skipped: skippedRoots);

        if (mappings.Count == 0)
        {
            report.Success = false;
            report.ResultMessage = $"未找到匹配的骨骼。前缀=\"{prefix}\", 后缀=\"{suffix}\" (No matching bones found. prefix=\"{prefix}\", suffix=\"{suffix}\")";
            return report;
        }

        report.TotalMappings = mappings.Count;

        // --- REPORT: Bone Mappings ---
        foreach (var (outfitBone, avatarBone) in mappings)
        {
            var entry = new BoneMappingEntry
            {
                OutfitBoneName = outfitBone.Name,
                AvatarBoneName = avatarBone.Name,
                OutfitPathId = outfitBone.TransformPathId,
                AvatarPathId = avatarBone.TransformPathId,
                OutfitItem = outfitBone,
                AvatarItem = avatarBone,
                Depth = GetDepthFromArmature(outfitBone, outfitArmature),
                MatchMethod = DetermineMatchMethod(outfitBone, avatarBone, prefix, suffix, avatarHumanoidMap, outfitHumanoidMap)
            };
            report.BoneMappings.Add(entry);
        }

        // --- REPORT: Unmatched bones ---
        var mappedOutfitIds = new HashSet<long>(mappings.Select(m => m.outfit.TransformPathId));
        var mappedAvatarIds = new HashSet<long>(mappings.Select(m => m.avatar.TransformPathId));
        foreach (var bone in outfitArmature.Flatten())
        {
            if (bone == outfitArmature) continue;
            if (!mappedOutfitIds.Contains(bone.TransformPathId))
                report.UnmatchedOutfitBones.Add(new UnmatchedBoneEntry { BoneName = bone.Name, PathId = bone.TransformPathId, Item = bone });
        }
        foreach (var bone in avatarArmature.Flatten())
        {
            if (bone == avatarArmature) continue;
            if (!mappedAvatarIds.Contains(bone.TransformPathId))
                report.UnmatchedAvatarBones.Add(new UnmatchedBoneEntry { BoneName = bone.Name, PathId = bone.TransformPathId, Item = bone });
        }

        // [MA PARITY] Support UpperChest skipping by adding MergeArmature components
        foreach (var skipped in skippedRoots)
        {
            var smr = FindComponent(skipped.FileInstance!, skipped.Asset?.PathId ?? 0, (AssetClassID)114);
            if (smr == null)
            {
                report.Warnings.Add($"跳过了 UpperChest 骨骼 \"{skipped.Name}\"，但未添加 MergeArmature 组件 (Skipped UpperChest bone \"{skipped.Name}\" without adding MergeArmature component)");
            }
        }

        // 6. PHASE 2: Rename bones to match avatar (MA Heuristic Parity)
        // --- REPORT: Bone Renames ---
        foreach (var (outfitBone, avatarBone) in mappings)
        {
            var newName = prefix + avatarBone.Name + suffix;
            report.BoneRenames.Add(new BoneRenameEntry
            {
                OldName = outfitBone.Name,
                NewName = newName,
                PathId = outfitBone.TransformPathId,
                Changed = outfitBone.Name != newName,
                Item = outfitBone
            });
        }
        RenameBones(mappings, prefix, suffix);

        // 7. Check file instance
        var fileInst = avatarArmature.FileInstance;
        if (fileInst == null)
        {
            report.Success = false;
            report.ResultMessage = "无法确定头像骨架的文件实例 (Cannot determine file instance for avatar armature.)";
            return report;
        }

        // 8. PHASE 3: Bone Merge — Redirect all SMR references from outfit transforms to avatar transforms
        // Build redirect map: outfitTransformPathId → avatarTransformPathId
        var redirectMap = new Dictionary<long, long>();
        foreach (var (outfitBone, avatarBone) in mappings)
        {
            if (outfitBone.TransformPathId != 0 && avatarBone.TransformPathId != 0)
            {
                redirectMap[outfitBone.TransformPathId] = avatarBone.TransformPathId;
            }
        }

        // Scan ALL SkinnedMeshRenderers under the outfit node and redirect bone references
        int totalRedirects = 0;
        var allOutfitNodes = outfitNode.Flatten().ToList();
        foreach (var node in allOutfitNodes)
        {
            if (node.FileInstance != fileInst) continue;
            // Find SkinnedMeshRenderer components (TypeId=137)
            var smrAssets = FindAllComponents(fileInst, node.Asset?.PathId ?? 0, AssetClassID.SkinnedMeshRenderer);
            foreach (var smrAsset in smrAssets)
            {
                var smrBf = Workspace.GetBaseField(smrAsset);
                if (smrBf == null) continue;
                bool modified = false;
                int refCount = 0;

                // Track which bone indices were redirected (index → old outfit PathId)
                var redirectedBoneIndices = new Dictionary<int, long>();

                // Redirect m_Bones array
                var bonesArray = smrBf["m_Bones"]["Array"];
                if (!bonesArray.IsDummy && bonesArray.Children != null)
                {
                    for (int boneIdx = 0; boneIdx < bonesArray.Children.Count; boneIdx++)
                    {
                        var bonePtr = bonesArray.Children[boneIdx];
                        long bonePathId = bonePtr["m_PathID"].AsLong;
                        if (bonePathId != 0 && redirectMap.TryGetValue(bonePathId, out long avatarPathId))
                        {
                            redirectedBoneIndices[boneIdx] = bonePathId; // Store old outfit PathId
                            bonePtr["m_PathID"].AsLong = avatarPathId;
                            modified = true;
                            refCount++;
                        }
                    }
                }

                // Redirect m_RootBone
                var rootBone = smrBf["m_RootBone"];
                if (!rootBone.IsDummy)
                {
                    long rbPathId = rootBone["m_PathID"].AsLong;
                    if (rbPathId != 0 && redirectMap.TryGetValue(rbPathId, out long avatarRbPathId))
                    {
                        rootBone["m_PathID"].AsLong = avatarRbPathId;
                        modified = true;
                        refCount++;
                    }
                }

                // Redirect m_ProbeAnchor
                var probeAnchor = smrBf["m_ProbeAnchor"];
                if (!probeAnchor.IsDummy)
                {
                    long paPathId = probeAnchor["m_PathID"].AsLong;
                    if (paPathId != 0 && redirectMap.TryGetValue(paPathId, out long avatarPaPathId))
                    {
                        probeAnchor["m_PathID"].AsLong = avatarPaPathId;
                        modified = true;
                        refCount++;
                    }
                }

                if (modified)
                {
                    smrAsset.UpdateAssetDataAndRow(Workspace, smrBf);
                    totalRedirects += refCount;
                }

                // BIND POSE CORRECTION: Only compensate for m_LocalRotation differences
                if (redirectedBoneIndices.Count > 0)
                {
                    var meshPtr = smrBf["m_Mesh"];
                    if (!meshPtr.IsDummy)
                    {
                        long meshPathId = meshPtr["m_PathID"].AsLong;
                        if (meshPathId != 0)
                        {
                            var meshAsset = Workspace.GetAssetInst(fileInst, meshPtr["m_FileID"].AsInt, meshPathId);
                            if (meshAsset != null)
                            {
                                var meshBf = Workspace.GetBaseField(meshAsset);
                                if (meshBf != null)
                                {
                                    var bindPoseArray = meshBf["m_BindPose"]["Array"];
                                    if (!bindPoseArray.IsDummy && bindPoseArray.Children != null)
                                    {
                                        bool bindPoseModified = false;

                                        foreach (var kvp in redirectedBoneIndices)
                                        {
                                            int boneIdx = kvp.Key;
                                            long outfitPathId = kvp.Value;
                                            long avatarPathId = redirectMap[outfitPathId];

                                            if (boneIdx >= bindPoseArray.Children.Count) continue;

                                            // Read LocalRotation from outfit bone
                                            var outfitTfmBf = Workspace.GetBaseField(fileInst, outfitPathId);
                                            var avatarTfmBf = Workspace.GetBaseField(fileInst, avatarPathId);
                                            if (outfitTfmBf == null || avatarTfmBf == null) continue;

                                            var outfitRot = outfitTfmBf["m_LocalRotation"];
                                            var avatarRot = avatarTfmBf["m_LocalRotation"];

                                            float oqx = outfitRot["x"].AsFloat, oqy = outfitRot["y"].AsFloat;
                                            float oqz = outfitRot["z"].AsFloat, oqw = outfitRot["w"].AsFloat;
                                            float aqx = avatarRot["x"].AsFloat, aqy = avatarRot["y"].AsFloat;
                                            float aqz = avatarRot["z"].AsFloat, aqw = avatarRot["w"].AsFloat;

                                            // Check if rotations are the same (skip if no difference)
                                            const float epsilon = 1e-6f;
                                            if (MathF.Abs(oqx - aqx) < epsilon && MathF.Abs(oqy - aqy) < epsilon &&
                                                MathF.Abs(oqz - aqz) < epsilon && MathF.Abs(oqw - aqw) < epsilon)
                                                continue;

                                            // Correction = outfitLocalRot × inverse(avatarLocalRot)
                                            var outfitQ = new Quaternion(oqx, oqy, oqz, oqw);
                                            var avatarQ = new Quaternion(aqx, aqy, aqz, aqw);
                                            var avatarQInv = Quaternion.Inverse(avatarQ);
                                            var correctionQ = Quaternion.Multiply(outfitQ, avatarQInv);

                                            // Apply rotation-only correction to bind pose
                                            var correctionMatrix = Matrix4x4.CreateFromQuaternion(correctionQ);
                                            var oldBindPose = ReadBindPoseMatrix(bindPoseArray.Children[boneIdx]);
                                            var newBindPose = oldBindPose * correctionMatrix;
                                            WriteBindPoseMatrix(bindPoseArray.Children[boneIdx], newBindPose);
                                            bindPoseModified = true;
                                        }

                                        if (bindPoseModified)
                                        {
                                            meshAsset.UpdateAssetDataAndRow(Workspace, meshBf);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Also check MeshRenderer (TypeId=23) for m_ProbeAnchor / m_StaticBatchRoot
            var mrAssets = FindAllComponents(fileInst, node.Asset?.PathId ?? 0, AssetClassID.MeshRenderer);
            foreach (var mrAsset in mrAssets)
            {
                var mrBf = Workspace.GetBaseField(mrAsset);
                if (mrBf == null) continue;
                bool modified = false;

                var probeAnchor = mrBf["m_ProbeAnchor"];
                if (!probeAnchor.IsDummy)
                {
                    long paPathId = probeAnchor["m_PathID"].AsLong;
                    if (paPathId != 0 && redirectMap.TryGetValue(paPathId, out long avatarPaPathId))
                    {
                        probeAnchor["m_PathID"].AsLong = avatarPaPathId;
                        modified = true;
                        totalRedirects++;
                    }
                }

                if (modified)
                {
                    mrAsset.UpdateAssetDataAndRow(Workspace, mrBf);
                }
            }
        }

        // Report merged bones
        foreach (var (outfitBone, avatarBone) in mappings)
        {
            report.MergedBones.Add(new MergedBoneEntry
            {
                OutfitBoneName = outfitBone.Name,
                OutfitTransformPathId = outfitBone.TransformPathId,
                AvatarBoneName = avatarBone.Name,
                AvatarTransformPathId = avatarBone.TransformPathId,
                OutfitItem = outfitBone,
                AvatarItem = avatarBone
            });
        }
        report.SuccessfulMerges = mappings.Count;

        // 8.5. PHASE 3.5: Add unmatched outfit bones to avatar (MA RecursiveMerge Parity)
        var outfitToAvatarMap = new Dictionary<long, HierarchyItem>();
        foreach (var (outfitBone, avatarBone) in mappings)
        {
            outfitToAvatarMap[outfitBone.TransformPathId] = avatarBone;
        }
        outfitToAvatarMap[outfitArmature.TransformPathId] = avatarArmature;

        AddUnmatchedBonesToAvatar(outfitArmature, outfitToAvatarMap, fileInst, report);

        // 9. PHASE 4: Delete redundant matched outfit bones (Transform + GameObject)
        // After Phase 3.5 moved unmatched children out, matched outfit bones are now leaves or empty.
        // Delete them bottom-up (deepest first) to avoid parent-before-child issues.
        var matchedOutfitBones = mappings.Select(m => m.outfit).ToList();
        matchedOutfitBones.Sort((a, b) => GetDepthFromArmature(b, outfitArmature) - GetDepthFromArmature(a, outfitArmature));

        var allDeletedPathIds = new HashSet<long>();
        foreach (var outfitBone in matchedOutfitBones)
        {
            if (outfitBone.TransformPathId == 0 || outfitBone.FileInstance != fileInst) continue;

            // Gather all assets of this single bone node (Transform + GameObject + non-Transform components on that GO)
            var assetsToDelete = new List<AssetInst>();

            // Transform
            var tfmInst = Workspace.GetAssetInst(fileInst, 0, outfitBone.TransformPathId);
            if (tfmInst != null)
            {
                assetsToDelete.Add(tfmInst);

                // GameObject
                var tfmBf = Workspace.GetBaseField(tfmInst);
                if (tfmBf != null)
                {
                    long goPathId = tfmBf["m_GameObject"]["m_PathID"].AsLong;
                    if (goPathId != 0)
                    {
                        var goInst = Workspace.GetAssetInst(fileInst, 0, goPathId);
                        if (goInst != null)
                        {
                            assetsToDelete.Add(goInst);

                            // All non-Transform components on this GO
                            var goBf = Workspace.GetBaseField(goInst);
                            if (goBf != null)
                            {
                                var comps = goBf["m_Component"]["Array"];
                                if (!comps.IsDummy && comps.Children != null)
                                {
                                    foreach (var comp in comps.Children)
                                    {
                                        var ptr = comp[comp.Children.Count - 1];
                                        long compPathId = ptr["m_PathID"].AsLong;
                                        if (compPathId != 0 && compPathId != outfitBone.TransformPathId)
                                        {
                                            var compInfo = fileInst.file.GetAssetInfo(compPathId);
                                            if (compInfo != null)
                                            {
                                                // Don't delete SMR/MR components - they belong to mesh objects on the outfit
                                                // Only delete components attached to the bone GO itself (like PhysBone, etc.)
                                                var compInst = Workspace.GetAssetInst(fileInst, 0, compPathId);
                                                if (compInst != null)
                                                    assetsToDelete.Add(compInst);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Remove from old parent's m_Children (if not already removed by Phase 3.5)
                    long parentPathId = tfmBf["m_Father"]["m_PathID"].AsLong;
                    if (parentPathId != 0)
                    {
                        var parentTfmBf = Workspace.GetBaseField(fileInst, parentPathId);
                        if (parentTfmBf != null)
                        {
                            var childrenArray = parentTfmBf["m_Children"]["Array"];
                            if (!childrenArray.IsDummy && childrenArray.Children != null)
                            {
                                int before = childrenArray.Children.Count;
                                childrenArray.Children.RemoveAll(c => c["m_PathID"].AsLong == outfitBone.TransformPathId);
                                if (childrenArray.Children.Count != before)
                                {
                                    var sizeField = parentTfmBf["m_Children"]["Array"]["size"];
                                    if (sizeField.IsDummy) sizeField = parentTfmBf["m_Children"]["size"];
                                    if (!sizeField.IsDummy) sizeField.AsInt = childrenArray.Children.Count;

                                    var parentInst = Workspace.GetAssetInst(fileInst, 0, parentPathId);
                                    if (parentInst != null)
                                        parentInst.UpdateAssetDataAndRow(Workspace, parentTfmBf);
                                }
                            }
                        }
                    }
                }
            }

            // Delete from file metadata
            foreach (var asset in assetsToDelete)
            {
                allDeletedPathIds.Add(asset.PathId);
                fileInst.file.Metadata.RemoveAssetInfo(asset);
                if (ActiveAssets.Contains(asset))
                    ActiveAssets.Remove(asset);
            }

            // Update report
            var mergedEntry = report.MergedBones.FirstOrDefault(m => m.OutfitTransformPathId == outfitBone.TransformPathId);
            if (mergedEntry != null)
            {
                mergedEntry.Deleted = true;
            }
        }

        // 9.5. Clean up AssetBundle references for deleted assets
        if (allDeletedPathIds.Count > 0)
        {
            AssetFileInfo? abInfo = fileInst.file.Metadata.AssetInfos.FirstOrDefault(i => i.TypeId == (int)AssetClassID.AssetBundle);
            if (abInfo != null)
            {
                var abAsset = Workspace.GetAssetInst(fileInst, 0, abInfo.PathId);
                if (abAsset != null)
                {
                    var abBf = Workspace.GetBaseField(abAsset);
                    if (abBf != null)
                    {
                        bool abModified = false;

                        var preloadTable = abBf["m_PreloadTable"]["Array"];
                        if (!preloadTable.IsDummy && preloadTable.Children != null)
                        {
                            int beforeCount = preloadTable.Children.Count;
                            preloadTable.Children.RemoveAll(c => c["m_FileID"].AsInt == 0 && allDeletedPathIds.Contains(c["m_PathID"].AsLong));
                            if (preloadTable.Children.Count != beforeCount)
                                abModified = true;
                        }

                        var container = abBf["m_Container"]["Array"];
                        if (!container.IsDummy && container.Children != null)
                        {
                            var toRemove = container.Children.Where(item =>
                            {
                                var assetPtr = item["second"]["asset"];
                                return assetPtr != null && !assetPtr.IsDummy &&
                                       assetPtr["m_FileID"].AsInt == 0 &&
                                       allDeletedPathIds.Contains(assetPtr["m_PathID"].AsLong);
                            }).ToList();

                            foreach (var item in toRemove)
                                container.Children.Remove(item);
                            if (toRemove.Count > 0)
                                abModified = true;

                            // Sync preloadSize
                            if (abModified && preloadTable.Children != null)
                            {
                                foreach (var item in container.Children)
                                {
                                    var second = item["second"];
                                    if (second != null && !second.IsDummy)
                                        second["preloadSize"].AsInt = preloadTable.Children.Count;
                                }
                            }
                        }

                        if (abModified)
                            abAsset.UpdateAssetDataAndRow(Workspace, abBf);
                    }
                }
            }
        }
        report.DeletedAssetsCount = allDeletedPathIds.Count;

        // 10. Dirty workspace
        var wsItem = Workspace.FindWorkspaceItemByInstance(fileInst);
        if (wsItem != null) Workspace.Dirty(wsItem);

        // 11. PHASE 5: Sync Mesh Settings (RootBone / ProbeAnchor / Bounds)
        SyncMeshSettings(outfitNode, avatarRoot, report);

        // 12. PHASE 6: Fix A-Pose (Shoulder/Arm rotation alignment) - MA Strict Parity
        FixAPose(avatarArmature, outfitArmature, prefix, suffix, avatarHumanoidMap, outfitHumanoidMap, report);

        // --- FIX FOR UNITY CACHING (MA PARITY) ---
        var avatarRootChildWithSameName = avatarRoot.Children.FirstOrDefault(c => c.Name == outfitArmature.Name);
        if (string.IsNullOrEmpty(prefix) && string.IsNullOrEmpty(suffix) && avatarRootChildWithSameName != null)
        {
            var oldArmName = outfitArmature.Name;
            outfitArmature.Name += ".1";
            var goBf2 = Workspace.GetBaseField(fileInst, outfitArmature.Asset?.PathId ?? 0);
            if (goBf2 != null)
            {
                goBf2["m_Name"].AsString = outfitArmature.Name;
                var goInst2 = Workspace.GetAssetInst(fileInst, 0, outfitArmature.Asset?.PathId ?? 0);
                if (goInst2 != null) goInst2.UpdateAssetDataAndRow(Workspace, goBf2);
            }
            report.ArmatureRename = new ArmatureRenameInfo
            {
                OldName = oldArmName,
                NewName = outfitArmature.Name,
                PathId = outfitArmature.TransformPathId
            };
        }

        // 13. PHASE 7: Fully reload hierarchy tree
        if (_itemsToLoad != null)
        {
            _ = LoadRootItems();
        }

        report.Success = true;
        report.ResultMessage = $"服装设置完成：{mappings.Count} 个骨骼已合并，{totalRedirects} 个引用已重定向，{allDeletedPathIds.Count} 个资产已删除。前缀=\"{prefix}\", 后缀=\"{suffix}\" (Setup complete: {mappings.Count} bones merged, {totalRedirects} refs redirected, {allDeletedPathIds.Count} assets deleted. prefix=\"{prefix}\", suffix=\"{suffix}\")";
        return report;
    }

    private static int GetDepthFromArmature(HierarchyItem bone, HierarchyItem armature)
    {
        int depth = 0;
        var current = bone;
        while (current != null && current != armature)
        {
            depth++;
            current = current.Parent;
        }
        return depth;
    }

    private string DetermineMatchMethod(HierarchyItem outfitBone, HierarchyItem avatarBone,
        string prefix, string suffix,
        Dictionary<long, int> avatarHumanoidMap, Dictionary<long, int> outfitHumanoidMap)
    {
        var name = outfitBone.Name;
        if (name.StartsWith(prefix) && name.EndsWith(suffix) && name.Length > prefix.Length + suffix.Length)
        {
            var targetName = name.Substring(prefix.Length, name.Length - prefix.Length - suffix.Length);
            if (targetName == avatarBone.Name)
                return "直接名称匹配 (Direct Name)";
        }
        if (outfitBone.Asset != null && outfitHumanoidMap.TryGetValue(outfitBone.Asset.PathId, out int boneIdx))
        {
            if (avatarBone.Asset != null && avatarHumanoidMap.TryGetValue(avatarBone.Asset.PathId, out int avIdx) && avIdx == boneIdx)
                return $"Humanoid 骨骼匹配 (Humanoid: {(HumanBodyBones)boneIdx})";
        }
        return "启发式/模式匹配 (Heuristic/Pattern)";
    }

    /// <summary>
    /// [MA PARITY] Synchronize RootBone and ProbeAnchor for all SkinnedMeshRenderers in the outfit.
    /// This ensures consistent lighting (ProbeAnchor) and culling (RootBone/Bounds) with the avatar.
    /// </summary>
    private void SyncMeshSettings(HierarchyItem outfitNode, HierarchyItem avatarRoot, SetupOutfitReport? report = null)
    {
        var fileInst = outfitNode.FileInstance;
        if (fileInst == null) return;

        long rootBonePathId = 0;
        long probeAnchorPathId = 0;
        Vector3 boundsCenter = Vector3.Zero;
        Vector3 boundsExtent = Vector3.Zero;

        // 1. Find a "main" body mesh to pull default settings from (MA heuristic)
        var bodyMeshGo = FindBodyMesh(avatarRoot);
        if (bodyMeshGo != null)
        {
            var smr = FindComponent(fileInst, bodyMeshGo.Asset?.PathId ?? 0, AssetClassID.SkinnedMeshRenderer);
            if (smr != null)
            {
                var smrBf = Workspace.GetBaseField(smr);
                if (smrBf != null)
                {
                    rootBonePathId = smrBf["m_RootBone"]["m_PathID"].AsLong;
                    probeAnchorPathId = smrBf["m_ProbeAnchor"]["m_PathID"].AsLong;
                    var boundsField = smrBf["m_LocalBounds"];
                    if (boundsField != null && !boundsField.IsDummy)
                    {
                        boundsCenter = new Vector3(boundsField["m_Center"]["x"].AsFloat, boundsField["m_Center"]["y"].AsFloat, boundsField["m_Center"]["z"].AsFloat);
                        boundsExtent = new Vector3(boundsField["m_Extent"]["x"].AsFloat, boundsField["m_Extent"]["y"].AsFloat, boundsField["m_Extent"]["z"].AsFloat);
                    }
                }
            }
        }

        // 2. Multi-mesh Consistency Check (MA PARITY EXTREME)
        // [MA PARITY] Only look at direct children of the avatar root
        bool firstRenderer = true;
        bool firstSkinnedMesh = true;

        foreach (var item in avatarRoot.Children)
        {
            var smr = FindComponent(fileInst, item.Asset?.PathId ?? 0, AssetClassID.SkinnedMeshRenderer);
            var mr = FindComponent(fileInst, item.Asset?.PathId ?? 0, AssetClassID.MeshRenderer);
            var renderer = smr ?? mr;

            if (renderer == null) continue;
            var rb = Workspace.GetBaseField(renderer);
            if (rb == null) continue;

            var probeAnchorField = rb["m_ProbeAnchor"];
            long currentProbeAnchor = (!probeAnchorField.IsDummy && !probeAnchorField["m_PathID"].IsDummy)
                ? probeAnchorField["m_PathID"].AsLong : 0;

            if (firstRenderer)
            {
                probeAnchorPathId = currentProbeAnchor;
                firstRenderer = false;
            }
            else if (probeAnchorPathId != currentProbeAnchor)
            {
                probeAnchorPathId = 0; // Inconsistent
            }

            if (smr != null)
            {
                var rootBoneField = rb["m_RootBone"];
                long currentRootBone = (!rootBoneField.IsDummy && !rootBoneField["m_PathID"].IsDummy)
                    ? rootBoneField["m_PathID"].AsLong : 0;

                var lb = rb["m_LocalBounds"];
                if (lb.IsDummy || lb["m_Center"].IsDummy || lb["m_Extent"].IsDummy)
                {
                    // If bounds are missing, treat as inconsistent to trigger fallback
                    rootBonePathId = 0;
                    continue;
                }

                Vector3 currentCenter = new Vector3(lb["m_Center"]["x"].AsFloat, lb["m_Center"]["y"].AsFloat, lb["m_Center"]["z"].AsFloat);
                Vector3 currentExtent = new Vector3(lb["m_Extent"]["x"].AsFloat, lb["m_Extent"]["y"].AsFloat, lb["m_Extent"]["z"].AsFloat);

                if (firstSkinnedMesh)
                {
                    rootBonePathId = currentRootBone;
                    boundsCenter = currentCenter;
                    boundsExtent = currentExtent;
                    firstSkinnedMesh = false;
                }
                else if (rootBonePathId != currentRootBone ||
                         (currentCenter - boundsCenter).Length() > 0.01f ||
                         (currentExtent - boundsExtent).Length() > 0.01f)
                {
                    rootBonePathId = 0; // Trigger Fallback
                }
            }
        }

        // 3. Apply fallback if inconsistent
        if (rootBonePathId == 0 || probeAnchorPathId == 0)
        {
            HierarchyItem? avatarHips = null;
            if (FindBones(outfitNode, out _, out avatarHips, out _))
            {
                rootBonePathId = avatarHips.TransformPathId;
                probeAnchorPathId = avatarHips.TransformPathId;
            }
            else
            {
                rootBonePathId = avatarRoot.TransformPathId;
                probeAnchorPathId = avatarRoot.TransformPathId;
            }

            // [MA PARITY] TransformBounds logic: bounds are scaled relative to rootBone's lossyScale
            var rootBoneNode = avatarRoot.Flatten().FirstOrDefault(i => i.TransformPathId == rootBonePathId) ?? avatarRoot;
            var lossy = HierarchyMath.GetLossyScale(Workspace, rootBoneNode);
            float avgScale = (lossy.X + lossy.Y + lossy.Z) / 3.0f;
            if (avgScale < 0.0001f) avgScale = 1.0f; // Safety fallback
            boundsCenter = Vector3.Zero;
            boundsExtent = new Vector3(1.0f, 1.0f, 1.0f) / avgScale; // MA PARITY: Default extents are Vector3.one * 2 (total size 2, extent 1)
        }

        // Resolve names for report
        string rootBoneName = "";
        string probeAnchorName = "";
        if (report != null)
        {
            var allNodes = avatarRoot.Flatten().ToList();
            rootBoneName = allNodes.FirstOrDefault(n => n.TransformPathId == rootBonePathId)?.Name ?? $"PathID:{rootBonePathId}";
            probeAnchorName = allNodes.FirstOrDefault(n => n.TransformPathId == probeAnchorPathId)?.Name ?? $"PathID:{probeAnchorPathId}";
        }

        // 4. Apply consistent settings to all SMRs
        foreach (var item in outfitNode.Flatten())
        {
            var smr = FindComponent(fileInst, item.Asset?.PathId ?? 0, AssetClassID.SkinnedMeshRenderer);
            if (smr == null) continue;

            var smrBf = Workspace.GetBaseField(smr);
            if (smrBf == null) continue;

            smrBf["m_RootBone"]["m_FileID"].AsInt = 0;
            smrBf["m_RootBone"]["m_PathID"].AsLong = rootBonePathId;
            smrBf["m_ProbeAnchor"]["m_FileID"].AsInt = 0;
            smrBf["m_ProbeAnchor"]["m_PathID"].AsLong = probeAnchorPathId;

            var targetBounds = smrBf["m_LocalBounds"];
            if (targetBounds != null && !targetBounds.IsDummy)
            {
                targetBounds["m_Center"]["x"].AsFloat = boundsCenter.X;
                targetBounds["m_Center"]["y"].AsFloat = boundsCenter.Y;
                targetBounds["m_Center"]["z"].AsFloat = boundsCenter.Z;
                targetBounds["m_Extent"]["x"].AsFloat = boundsExtent.X;
                targetBounds["m_Extent"]["y"].AsFloat = boundsExtent.Y;
                targetBounds["m_Extent"]["z"].AsFloat = boundsExtent.Z;
            }

            var smrInst = Workspace.GetAssetInst(fileInst, 0, smr.PathId);
            if (smrInst != null) smrInst.UpdateAssetDataAndRow(Workspace, smrBf);

            // --- REPORT: Mesh Sync ---
            report?.MeshSyncs.Add(new MeshSyncEntry
            {
                GameObjectName = item.Name,
                GameObjectPathId = item.Asset?.PathId ?? 0,
                RootBonePathId = rootBonePathId,
                RootBoneName = rootBoneName,
                ProbeAnchorPathId = probeAnchorPathId,
                ProbeAnchorName = probeAnchorName,
                BoundsCenter = boundsCenter,
                BoundsExtent = boundsExtent,
                Item = item
            });
        }
    }

    /// <summary>
    /// Helper to find a specific component on a GameObject.
    /// </summary>
    private AssetInst? FindComponent(AssetsFileInstance fileInst, long goPathId, AssetClassID classId)
    {
        if (goPathId == 0) return null;
        var goBf = Workspace.GetBaseField(fileInst, goPathId);
        if (goBf == null) return null;
        var components = goBf["m_Component"]["Array"];
        if (components.IsDummy || components.Children == null) return null;

        foreach (var comp in components.Children)
        {
            var ptrField = comp[comp.Children.Count - 1]; // Support varying PPtr field names (component vs first/second)
            if (ptrField["m_FileID"].AsInt != 0) continue;
            long compPathId = ptrField["m_PathID"].AsLong;

            var compInfo = fileInst.file.GetAssetInfo(compPathId);
            if (compInfo != null && compInfo.TypeId == (int)classId)
                return Workspace.GetAssetInst(fileInst, 0, compPathId);
        }
        return null;
    }

    /// <summary>
    /// Find ALL components of a given type on a GameObject.
    /// Unlike FindComponent which returns the first match, this returns all matches.
    /// </summary>
    private List<AssetInst> FindAllComponents(AssetsFileInstance fileInst, long goPathId, AssetClassID classId)
    {
        var results = new List<AssetInst>();
        if (goPathId == 0) return results;
        var goBf = Workspace.GetBaseField(fileInst, goPathId);
        if (goBf == null) return results;
        var components = goBf["m_Component"]["Array"];
        if (components.IsDummy || components.Children == null) return results;

        foreach (var comp in components.Children)
        {
            var ptrField = comp[comp.Children.Count - 1];
            if (ptrField["m_FileID"].AsInt != 0) continue;
            long compPathId = ptrField["m_PathID"].AsLong;

            var compInfo = fileInst.file.GetAssetInfo(compPathId);
            if (compInfo != null && compInfo.TypeId == (int)classId)
            {
                var inst = Workspace.GetAssetInst(fileInst, 0, compPathId);
                if (inst != null) results.Add(inst);
            }
        }
        return results;
    }

    /// <summary>
    /// Find an armature-like child within a parent node, excluding 'exclude'.
    /// </summary>
    private HierarchyItem? FindArmature(HierarchyItem parent, HierarchyItem? exclude)
    {
        // MA Logic: Prefer child named "armature" (case-insensitive)
        var armature = parent.Children.FirstOrDefault(c => c != exclude && c.Name.Equals("armature", StringComparison.OrdinalIgnoreCase));
        if (armature != null) return armature;

        // Fallback: contains "armature"
        armature = parent.Children.FirstOrDefault(c => c != exclude && c.Name.ToLowerInvariant().Contains("armature"));
        if (armature != null) return armature;

        // Second pass: look for a child that has bone-like grandchildren (MA heuristic)
        foreach (var child in parent.Children)
        {
            if (child == exclude) continue;
            foreach (var grandchild in child.Children)
            {
                if (BoneNameMatcher.IsBoneName(grandchild.Name))
                    return child;
            }
        }

        return null;
    }

    /// <summary>
    /// [MA PARITY] Retrieve a map of Transform PathId -> HumanBodyBone index from an Animator's Avatar.
    /// Matches bone names from the HumanDescription to the actual hierarchy children.
    /// </summary>
    private Dictionary<long, int> GetHumanoidBoneMap(AssetInst animator, HierarchyItem armature)
    {
        var map = new Dictionary<long, int>();
        var bf = Workspace.GetBaseField(animator);
        if (bf == null) return map;

        var avatarPtr = bf["m_Avatar"];
        if (avatarPtr == null || avatarPtr.IsDummy) return map;

        var avatarInst = Workspace.GetAssetInst(animator.FileInstance, avatarPtr["m_FileID"].AsInt, avatarPtr["m_PathID"].AsLong);
        if (avatarInst == null) return map;

        var avatarBf = Workspace.GetBaseField(avatarInst);
        if (avatarBf == null) return map;

        var humanBones = avatarBf["m_HumanDescription"]["m_Human"];
        if (humanBones == null || humanBones.IsDummy) return map;

        // Build a name lookup for the armature subtree (Handle duplicates gracefully)
        var flatArmature = new Dictionary<string, long>();
        foreach (var item in armature.Flatten())
        {
            if (!flatArmature.ContainsKey(item.Name))
            {
                flatArmature.Add(item.Name, item.TransformPathId);
            }
        }

        for (int i = 0; i < humanBones.Children.Count; i++)
        {
            var bone = humanBones.Children[i];
            var boneNameField = bone["m_BoneName"];
            var humanNameField = bone["m_HumanName"];

            if (boneNameField.IsDummy || humanNameField.IsDummy) continue;

            var boneName = boneNameField.AsString;
            var humanName = humanNameField.AsString;

            if (string.IsNullOrEmpty(boneName) || string.IsNullOrEmpty(humanName)) continue;

            if (flatArmature.TryGetValue(boneName, out long pathId))
            {
                // Find HumanBodyBones enum value for humanName
                if (Enum.TryParse<HumanBodyBones>(humanName.Replace(" ", ""), true, out var boneIdx))
                {
                    map[pathId] = (int)boneIdx;
                }
            }
        }

        return map;
    }

    /// <summary>
    /// [MA PARITY] Deep Hips Discovery. 
    /// Ported from SetupOutfit.FindBones (Modular Avatar).
    /// </summary>
    private bool FindBones(HierarchyItem outfitRoot, out HierarchyItem avatarRoot, out HierarchyItem avatarHips, out HierarchyItem outfitHips)
    {
        avatarRoot = avatarHips = outfitHips = null!;

        // 1. Identification of Avatar Root
        var current = outfitRoot.Parent;
        while (current != null)
        {
            var anim = FindComponent(current.FileInstance!, current.Asset?.PathId ?? 0, AssetClassID.Animator);
            if (anim != null)
            {
                avatarRoot = current;
                break;
            }
            current = current.Parent;
        }
        if (avatarRoot == null) avatarRoot = outfitRoot.Parent!;
        if (avatarRoot == null) return false;

        // 2. Identification of Avatar Hips
        var avatarAnimator = FindComponent(avatarRoot.FileInstance!, avatarRoot.Asset?.PathId ?? 0, AssetClassID.Animator);
        if (avatarAnimator != null)
        {
            var guessedArmature = avatarRoot.Children.FirstOrDefault(c => c.Name.ToLowerInvariant().Contains("armature")) ?? avatarRoot;
            var humanMap = GetHumanoidBoneMap(avatarAnimator, guessedArmature);
            long pathId = humanMap.FirstOrDefault(kvp => kvp.Value == 0).Key; // 0 == Hips
            if (pathId != 0)
            {
                avatarHips = avatarRoot.Flatten().FirstOrDefault(i => i.TransformPathId == pathId)!;
            }
        }
        if (avatarHips == null)
        {
            avatarHips = avatarRoot.Flatten().FirstOrDefault(i =>
                BoneNameMatcher.NormalizeName(i.Name) == "hips" ||
                BoneNameMatcher.NormalizeName(i.Name) == "pelvis" ||
                BoneNameMatcher.BoneNamePatterns[0].Any(p => BoneNameMatcher.NormalizeName(i.Name) == BoneNameMatcher.NormalizeName(p)));
        }
        if (avatarHips == null) return false;

        // 3. Identification of Outfit Hips
        var outfitAnimator = FindComponent(outfitRoot.FileInstance!, outfitRoot.Asset?.PathId ?? 0, AssetClassID.Animator);
        if (outfitAnimator != null)
        {
            var guessedArmature = outfitRoot.Children.FirstOrDefault(c => c.Name.ToLowerInvariant().Contains("armature")) ?? outfitRoot;
            var humanMap = GetHumanoidBoneMap(outfitAnimator, guessedArmature);
            long pathId = humanMap.FirstOrDefault(kvp => kvp.Value == 0).Key;
            if (pathId != 0)
            {
                outfitHips = outfitRoot.Flatten().FirstOrDefault(i => i.TransformPathId == pathId)!;
                if (outfitHips != null && outfitHips.Parent == outfitRoot)
                {
                    outfitHips = null!;
                }
            }
        }

        if (outfitHips == null)
        {
            var hipsCandidates = new List<string> { avatarHips.Name };
            foreach (var hbm in BoneNameMatcher.BoneNamePatterns[0])
            {
                if (hipsCandidates[0] != hbm) hipsCandidates.Add(hbm);
            }

            var extraRoots = new List<HierarchyItem>();
            foreach (var child in outfitRoot.Children)
            {
                foreach (var tempHip in child.Children)
                {
                    if (tempHip.Name.Contains(avatarHips.Name))
                    {
                        outfitHips = tempHip;
                        break;
                    }
                    extraRoots.Add(tempHip);
                }
                if (outfitHips != null) break;
            }

            if (outfitHips == null)
            {
                foreach (var extraRoot in extraRoots)
                {
                    foreach (var tempHip in extraRoot.Children)
                    {
                        if (tempHip.Name.Contains(avatarHips.Name))
                        {
                            outfitHips = tempHip;
                            break;
                        }
                    }
                    if (outfitHips != null) break;
                }
            }

            if (outfitHips == null)
            {
                foreach (var child in outfitRoot.Children)
                {
                    foreach (var tempHip in child.Children)
                    {
                        foreach (var cand in hipsCandidates)
                        {
                            if (BoneNameMatcher.NormalizeName(tempHip.Name).Contains(BoneNameMatcher.NormalizeName(cand)))
                            {
                                outfitHips = tempHip;
                                break;
                            }
                        }
                        if (outfitHips != null) break;
                    }
                    if (outfitHips != null) break;
                }

                if (outfitHips == null)
                {
                    foreach (var extraRoot in extraRoots)
                    {
                        foreach (var tempHip in extraRoot.Children)
                        {
                            foreach (var cand in hipsCandidates)
                            {
                                if (BoneNameMatcher.NormalizeName(tempHip.Name).Contains(BoneNameMatcher.NormalizeName(cand)))
                                {
                                    outfitHips = tempHip;
                                    break;
                                }
                            }
                            if (outfitHips != null) break;
                        }
                        if (outfitHips != null) break;
                    }
                }
            }
        }

        if (outfitHips == null)
        {
            outfitHips = outfitRoot.Flatten().FirstOrDefault(i => i != outfitRoot && (
                BoneNameMatcher.NormalizeName(i.Name) == "hips" ||
                BoneNameMatcher.NormalizeName(i.Name) == "pelvis" ||
                BoneNameMatcher.BoneNamePatterns[0].Any(p => BoneNameMatcher.NormalizeName(i.Name) == BoneNameMatcher.NormalizeName(p))));
        }

        return avatarHips != null && outfitHips != null;
    }

    private List<(HierarchyItem outfit, HierarchyItem avatar)> AssignBoneMappings(
        HierarchyItem src, HierarchyItem target,
        string prefix, string suffix,
        Dictionary<long, int> avatarHumanoidMap,
        Dictionary<long, int> outfitHumanoidMap,
        HashSet<HierarchyItem>? unassigned = null,
        List<HierarchyItem>? skipped = null)
    {
        var mappings = new List<(HierarchyItem outfit, HierarchyItem avatar)>();
        var heuristicAssignmentPass = new List<HierarchyItem>();

        if (unassigned == null)
        {
            unassigned = new HashSet<HierarchyItem>(target.Children);
        }

        // MA Pass 1: Direct name match + Collect for heuristic pass
        foreach (var child in src.Children)
        {
            // [MA PARITY] ScanHierarchy: Skip child MergeArmatures
            var childMerge = FindComponent(child.FileInstance!, child.Asset?.PathId ?? 0, (AssetClassID)114);
            if (childMerge != null)
            {
                var mb = Workspace.GetBaseField(childMerge);
                if (mb != null && mb.TemplateField.Name.Contains("ModularAvatarMergeArmature"))
                    continue;
            }

            var childName = child.Name;
            if (childName.StartsWith(prefix) && childName.EndsWith(suffix) && childName.Length > prefix.Length + suffix.Length)
            {
                var targetObjectName = childName.Substring(prefix.Length, childName.Length - prefix.Length - suffix.Length);
                var targetObject = target.Children.FirstOrDefault(c => c.Name == targetObjectName);

                if (targetObject != null && unassigned.Contains(targetObject))
                {
                    mappings.Add((child, targetObject));
                    unassigned.Remove(targetObject);
                }
                else
                {
                    heuristicAssignmentPass.Add(child);
                }
            }
        }

        // MA Pass 2: Setup lookup dictionary
        var lcNameToXform = new Dictionary<string, HierarchyItem>();
        foreach (var t in unassigned)
        {
            lcNameToXform[BoneNameMatcher.NormalizeName(t.Name)] = t;
        }

        // MA Pass 3: Heuristic Assignment
        foreach (var child in heuristicAssignmentPass)
        {
            var childName = child.Name;
            var targetObjectName = childName.Substring(prefix.Length, childName.Length - prefix.Length - suffix.Length);
            List<int>? bodyBones = null;
            bool isMapped = false;

            // 3.1 Outfit Humanoid check
            if (child.Asset != null && outfitHumanoidMap.TryGetValue(child.Asset.PathId, out var outfitHumanoidBone))
            {
                var avatarBone = target.Flatten().FirstOrDefault(b => b.Asset != null && avatarHumanoidMap.TryGetValue(b.Asset.PathId, out int avIdx) && avIdx == outfitHumanoidBone);

                if (avatarBone != null && unassigned.Contains(avatarBone))
                {
                    mappings.Add((child, avatarBone));
                    unassigned.Remove(avatarBone);
                    lcNameToXform.Remove(BoneNameMatcher.NormalizeName(avatarBone.Name));
                    isMapped = true;
                }
                else
                {
                    bodyBones = new List<int> { outfitHumanoidBone };
                }
            }

            // 3.2 MA specific Name-to-Bone check
            if (!isMapped && bodyBones == null && !BoneNameMatcher.NormalizedNameToGroup.TryGetValue(BoneNameMatcher.NormalizeName(targetObjectName), out bodyBones))
            {
                continue; // [MA STRICT PARITY]: If not mapped, not in outfit humanoid, AND not a known bone name -> SKIP entirely!
            }

            // 3.3 Target Humanoid check
            if (!isMapped)
            {
                foreach (var bodyBone in bodyBones!)
                {
                    var avatarBone = target.Flatten().FirstOrDefault(b => b.Asset != null && avatarHumanoidMap.TryGetValue(b.Asset.PathId, out int avIdx) && avIdx == bodyBone);
                    if (avatarBone != null && unassigned.Contains(avatarBone))
                    {
                        mappings.Add((child, avatarBone));
                        unassigned.Remove(avatarBone);
                        lcNameToXform.Remove(BoneNameMatcher.NormalizeName(avatarBone.Name));
                        isMapped = true;
                        break;
                    }
                }
            }

            // 3.4 Target Heuristic name check
            if (!isMapped)
            {
                foreach (var bodyBone in bodyBones!)
                {
                    if (BoneNameMatcher.GroupToNames.TryGetValue(bodyBone, out var otherNames))
                    {
                        foreach (var otherName in otherNames)
                        {
                            if (lcNameToXform.TryGetValue(otherName, out var targetObject))
                            {
                                mappings.Add((child, targetObject));
                                unassigned.Remove(targetObject);
                                lcNameToXform.Remove(otherName.ToLowerInvariant());
                                isMapped = true;
                                goto mapped_by_name; // Break out of nested loops
                            }
                        }
                    }
                }
            mapped_by_name:;
            }

            // 3.5 UpperChest skip
            int upperChestIdx = BoneNameMatcher.BoneNamePatterns.Length - 1; // 74 is UpperChest in our table
            if (!mappings.Any(m => m.outfit == child) && bodyBones!.Contains(upperChestIdx) && skipped != null)
            {
                skipped.Add(child);

                var subMappings = AssignBoneMappings(child, target, prefix, suffix, avatarHumanoidMap, outfitHumanoidMap, unassigned, skipped);
                mappings.AddRange(subMappings);
            }
        }

        // Pass 4: Recurse for all successfully mapped bones
        var currentMappings = mappings.ToList();
        foreach (var pair in currentMappings)
        {
            var subMappings = AssignBoneMappings(pair.outfit, pair.avatar, prefix, suffix, avatarHumanoidMap, outfitHumanoidMap, unassigned: null, skipped: skipped);
            mappings.AddRange(subMappings);
        }

        return mappings;
    }

    /// <summary>
    /// Compute the world transform matrix for a Transform asset by walking up the parent chain.
    /// Returns Matrix4x4.Identity if transform data cannot be read.
    /// </summary>
    private Matrix4x4 ComputeWorldMatrix(AssetsFileInstance fileInst, long transformPathId)
    {
        var chain = new List<Matrix4x4>();
        long currentPathId = transformPathId;

        while (currentPathId != 0)
        {
            var tfmBf = Workspace.GetBaseField(fileInst, currentPathId);
            if (tfmBf == null) break;

            // Read local transform
            var localPos = tfmBf["m_LocalPosition"];
            var localRot = tfmBf["m_LocalRotation"];
            var localScale = tfmBf["m_LocalScale"];

            float px = localPos["x"].AsFloat;
            float py = localPos["y"].AsFloat;
            float pz = localPos["z"].AsFloat;

            float qx = localRot["x"].AsFloat;
            float qy = localRot["y"].AsFloat;
            float qz = localRot["z"].AsFloat;
            float qw = localRot["w"].AsFloat;

            float sx = localScale["x"].AsFloat;
            float sy = localScale["y"].AsFloat;
            float sz = localScale["z"].AsFloat;

            // Build local TRS matrix: Scale → Rotate → Translate
            var rotation = new Quaternion(qx, qy, qz, qw);
            var rotMatrix = Matrix4x4.CreateFromQuaternion(rotation);
            var scaleMatrix = Matrix4x4.CreateScale(sx, sy, sz);
            var translationMatrix = Matrix4x4.CreateTranslation(px, py, pz);

            // Local = T * R * S (Unity order)
            var localMatrix = scaleMatrix * rotMatrix * translationMatrix;
            chain.Add(localMatrix);

            // Walk up
            currentPathId = tfmBf["m_Father"]["m_PathID"].AsLong;
        }

        // Multiply from root to leaf: world = root * ... * parent * local
        var world = Matrix4x4.Identity;
        for (int i = chain.Count - 1; i >= 0; i--)
        {
            world = chain[i] * world;
        }
        return world;
    }

    /// <summary>
    /// Read a 4x4 matrix from a BindPose array element (16 floats: e00..e33).
    /// Unity stores them as column-major in the asset file.
    /// </summary>
    private static Matrix4x4 ReadBindPoseMatrix(AssetTypeValueField matField)
    {
        return new Matrix4x4(
            matField["e00"].AsFloat, matField["e10"].AsFloat, matField["e20"].AsFloat, matField["e30"].AsFloat,
            matField["e01"].AsFloat, matField["e11"].AsFloat, matField["e21"].AsFloat, matField["e31"].AsFloat,
            matField["e02"].AsFloat, matField["e12"].AsFloat, matField["e22"].AsFloat, matField["e32"].AsFloat,
            matField["e03"].AsFloat, matField["e13"].AsFloat, matField["e23"].AsFloat, matField["e33"].AsFloat
        );
    }

    /// <summary>
    /// Write a 4x4 matrix to a BindPose array element.
    /// </summary>
    private static void WriteBindPoseMatrix(AssetTypeValueField matField, Matrix4x4 m)
    {
        matField["e00"].AsFloat = m.M11; matField["e01"].AsFloat = m.M21; matField["e02"].AsFloat = m.M31; matField["e03"].AsFloat = m.M41;
        matField["e10"].AsFloat = m.M12; matField["e11"].AsFloat = m.M22; matField["e12"].AsFloat = m.M32; matField["e13"].AsFloat = m.M42;
        matField["e20"].AsFloat = m.M13; matField["e21"].AsFloat = m.M23; matField["e22"].AsFloat = m.M33; matField["e23"].AsFloat = m.M43;
        matField["e30"].AsFloat = m.M14; matField["e31"].AsFloat = m.M24; matField["e32"].AsFloat = m.M34; matField["e33"].AsFloat = m.M44;
    }

    /// <summary>
    /// Helper to find a specific bone name anywhere in a subtree.
    /// </summary>
    private bool TryFullRecursiveMatch(HierarchyItem outfitItem, HierarchyItem avatarSubtree, string prefix, string suffix, out HierarchyItem match)
    {
        match = null!;
        var nameMap = new Dictionary<string, string> { { avatarSubtree.Name, avatarSubtree.Name } };
        var matchedName = BoneNameMatcher.FindMatch(outfitItem.Name, prefix, suffix, nameMap);

        if (matchedName != null)
        {
            match = avatarSubtree;
            return true;
        }

        foreach (var child in avatarSubtree.Children)
        {
            if (TryFullRecursiveMatch(outfitItem, child, prefix, suffix, out match))
                return true;
        }

        return false;
    }

    /// <summary>
    /// PHASE 3.5: Recursively walk the outfit armature subtree.
    /// For each child bone:
    ///   - If it's in the outfitToAvatarMap → it was matched → recurse into its children
    ///   - If it's NOT in the map → it's unmatched → reparent the ENTIRE subtree (bone + all descendants + components)
    ///     to the avatar-side parent bone, preserving identical hierarchy structure.
    /// This mirrors MA's RecursiveMerge behavior where unmatched bones are moved into the avatar skeleton.
    /// </summary>
    private void AddUnmatchedBonesToAvatar(
        HierarchyItem outfitParent,
        Dictionary<long, HierarchyItem> outfitToAvatarMap,
        AssetsFileInstance fileInst,
        SetupOutfitReport report)
    {
        // Take a snapshot of children since reparenting may modify the collection
        var children = outfitParent.Children.ToList();

        foreach (var child in children)
        {
            if (outfitToAvatarMap.TryGetValue(child.TransformPathId, out var avatarCounterpart))
            {
                // This bone was matched → it's already reparented under avatarCounterpart.
                // Recurse into its children to find deeper unmatched bones.
                AddUnmatchedBonesToAvatar(child, outfitToAvatarMap, fileInst, report);
            }
            else
            {
                // This bone is UNMATCHED: outfit has it but avatar doesn't.
                // Find the avatar-side parent where this bone should be attached.
                // The outfit parent of this bone is either in the map (matched) or is the armature itself.
                HierarchyItem? avatarTargetParent = null;
                if (outfitToAvatarMap.TryGetValue(outfitParent.TransformPathId, out var avatarParent))
                {
                    avatarTargetParent = avatarParent;
                }

                var addEntry = new AddedBoneEntry
                {
                    BoneName = child.Name,
                    BonePathId = child.TransformPathId,
                    AvatarParentName = avatarTargetParent?.Name ?? "(unknown)",
                    AvatarParentPathId = avatarTargetParent?.TransformPathId ?? 0,
                    ChildCount = CountDescendants(child),
                    BoneItem = child,
                    AvatarParentItem = avatarTargetParent
                };

                if (avatarTargetParent == null || avatarTargetParent.TransformPathId == 0)
                {
                    addEntry.Success = false;
                    addEntry.FailReason = "无法确定素体侧的目标父骨骼 (Cannot determine avatar-side target parent)";
                    report.AddedBones.Add(addEntry);
                    continue;
                }

                if (child.TransformPathId == 0)
                {
                    addEntry.Success = false;
                    addEntry.FailReason = "骨骼 PathID 为 0 (Bone PathID is 0)";
                    report.AddedBones.Add(addEntry);
                    continue;
                }

                if (child.FileInstance != fileInst)
                {
                    addEntry.Success = false;
                    addEntry.FailReason = "跨文件实例 (Cross-file instance)";
                    report.AddedBones.Add(addEntry);
                    continue;
                }

                // Reparent the entire unmatched subtree to the avatar parent
                // NOTE: We do NOT zero out transforms here (unlike matched bones),
                // because these are outfit-specific bones that need to retain their local transform
                // relative to the parent bone for correct positioning.
                bool ok = ReparentTransformDataPreserveTransform(fileInst, child, avatarTargetParent);
                addEntry.Success = ok;
                if (!ok) addEntry.FailReason = "ReparentTransformData 返回 false (returned false)";
                report.AddedBones.Add(addEntry);

                // Do NOT recurse into children of unmatched bones — 
                // the entire subtree was moved as one unit.
            }
        }
    }

    /// <summary>
    /// Count all descendants of a HierarchyItem recursively.
    /// </summary>
    private int CountDescendants(HierarchyItem item)
    {
        int count = 0;
        foreach (var child in item.Children)
        {
            count += 1 + CountDescendants(child);
        }
        return count;
    }

    /// <summary>
    /// Reparent a Transform at the data level, but PRESERVE its local transform
    /// (position/rotation/scale are NOT zeroed out).
    /// Used for unmatched outfit bones that are being added to the avatar skeleton —
    /// they need to keep their original local transforms for correct positioning.
    /// </summary>
    private bool ReparentTransformDataPreserveTransform(AssetsFileInstance fileInst, HierarchyItem source, HierarchyItem newParent)
    {
        var sourceTfmBf = Workspace.GetBaseField(fileInst, source.TransformPathId);
        if (sourceTfmBf == null) return false;

        var newParentTfmBf = Workspace.GetBaseField(fileInst, newParent.TransformPathId);
        if (newParentTfmBf == null) return false;

        long oldParentPathId = sourceTfmBf["m_Father"]["m_PathID"].AsLong;

        // 1. Update source's m_Father → new parent
        sourceTfmBf["m_Father"]["m_FileID"].AsInt = 0;
        sourceTfmBf["m_Father"]["m_PathID"].AsLong = newParent.TransformPathId;

        // NOTE: Local transforms are PRESERVED (not zeroed) for unmatched bones

        // 2. Remove from old parent's m_Children
        if (oldParentPathId != 0)
        {
            var oldParentTfmBf = Workspace.GetBaseField(fileInst, oldParentPathId);
            if (oldParentTfmBf != null)
            {
                var oldChildrenArray = oldParentTfmBf["m_Children"]["Array"];
                if (!oldChildrenArray.IsDummy && oldChildrenArray.Children != null)
                {
                    oldChildrenArray.Children.RemoveAll(c => c["m_PathID"].AsLong == source.TransformPathId);
                    var oldSizeField = oldParentTfmBf["m_Children"]["Array"]["size"];
                    if (oldSizeField.IsDummy) oldSizeField = oldParentTfmBf["m_Children"]["size"];
                    if (!oldSizeField.IsDummy) oldSizeField.AsInt = oldChildrenArray.Children.Count;
                }
                var oldParentInst = Workspace.GetAssetInst(fileInst, 0, oldParentPathId);
                if (oldParentInst != null)
                    oldParentInst.UpdateAssetDataAndRow(Workspace, oldParentTfmBf);
            }
        }

        // 3. Add to new parent's m_Children
        var newChildrenArray = newParentTfmBf["m_Children"]["Array"];
        if (!newChildrenArray.IsDummy)
        {
            if (newChildrenArray.Children == null)
                newChildrenArray.Children = new List<AssetTypeValueField>();

            var newChildEntry = ValueBuilder.DefaultValueFieldFromTemplate(
                newChildrenArray.TemplateField.Children[1]
            );
            newChildEntry["m_FileID"].AsInt = 0;
            newChildEntry["m_PathID"].AsLong = source.TransformPathId;
            newChildrenArray.Children.Add(newChildEntry);

            var newSizeField = newParentTfmBf["m_Children"]["Array"]["size"];
            if (newSizeField.IsDummy) newSizeField = newParentTfmBf["m_Children"]["size"];
            if (!newSizeField.IsDummy) newSizeField.AsInt = newChildrenArray.Children.Count;
        }

        // 4. Persist source and new parent
        var sourceInst = Workspace.GetAssetInst(fileInst, 0, source.TransformPathId);
        if (sourceInst != null) sourceInst.UpdateAssetDataAndRow(Workspace, sourceTfmBf);

        var newParentInst = Workspace.GetAssetInst(fileInst, 0, newParent.TransformPathId);
        if (newParentInst != null) newParentInst.UpdateAssetDataAndRow(Workspace, newParentTfmBf);

        return true;
    }

    /// <summary>
    /// PHASE 2: Reparent a single Transform at the data level only.
    /// Updates m_Father on source, removes from old parent's m_Children, adds to new parent's m_Children.
    /// Does NOT update the UI tree (that's handled by LoadRootItems).
    /// </summary>
    private bool ReparentTransformData(AssetsFileInstance fileInst, HierarchyItem source, HierarchyItem newParent)
    {
        // Load source transform
        var sourceTfmBf = Workspace.GetBaseField(fileInst, source.TransformPathId);
        if (sourceTfmBf == null) return false;

        // Load new parent transform
        var newParentTfmBf = Workspace.GetBaseField(fileInst, newParent.TransformPathId);
        if (newParentTfmBf == null) return false;

        // Get old parent path id from current m_Father
        long oldParentPathId = sourceTfmBf["m_Father"]["m_PathID"].AsLong;

        // 1. Update source's m_Father → new parent
        sourceTfmBf["m_Father"]["m_FileID"].AsInt = 0;
        sourceTfmBf["m_Father"]["m_PathID"].AsLong = newParent.TransformPathId;

        // --- FIX FOR DISTORTION ---
        // Zero out the local transform so the outfit bone perfectly overlaps the avatar bone.
        // This simulates SetParent(..., worldPositionStays: true) when the models share the same rest pose.
        sourceTfmBf["m_LocalPosition"]["x"].AsFloat = 0;
        sourceTfmBf["m_LocalPosition"]["y"].AsFloat = 0;
        sourceTfmBf["m_LocalPosition"]["z"].AsFloat = 0;

        sourceTfmBf["m_LocalRotation"]["x"].AsFloat = 0;
        sourceTfmBf["m_LocalRotation"]["y"].AsFloat = 0;
        sourceTfmBf["m_LocalRotation"]["z"].AsFloat = 0;
        sourceTfmBf["m_LocalRotation"]["w"].AsFloat = 1;

        sourceTfmBf["m_LocalScale"]["x"].AsFloat = 1;
        sourceTfmBf["m_LocalScale"]["y"].AsFloat = 1;
        sourceTfmBf["m_LocalScale"]["z"].AsFloat = 1;

        // 2. Remove from old parent's m_Children
        if (oldParentPathId != 0)
        {
            var oldParentTfmBf = Workspace.GetBaseField(fileInst, oldParentPathId);
            if (oldParentTfmBf != null)
            {
                var oldChildrenArray = oldParentTfmBf["m_Children"]["Array"];
                if (!oldChildrenArray.IsDummy && oldChildrenArray.Children != null)
                {
                    oldChildrenArray.Children.RemoveAll(c => c["m_PathID"].AsLong == source.TransformPathId);
                    // Sync size
                    var oldSizeField = oldParentTfmBf["m_Children"]["Array"]["size"];
                    if (oldSizeField.IsDummy) oldSizeField = oldParentTfmBf["m_Children"]["size"];
                    if (!oldSizeField.IsDummy) oldSizeField.AsInt = oldChildrenArray.Children.Count;
                }

                // Persist old parent
                var oldParentInst = Workspace.GetAssetInst(fileInst, 0, oldParentPathId);
                if (oldParentInst != null)
                    oldParentInst.UpdateAssetDataAndRow(Workspace, oldParentTfmBf);
            }
        }

        // 3. Add to new parent's m_Children
        var newChildrenArray = newParentTfmBf["m_Children"]["Array"];
        if (!newChildrenArray.IsDummy)
        {
            if (newChildrenArray.Children == null)
                newChildrenArray.Children = new List<AssetTypeValueField>();

            var newChildEntry = ValueBuilder.DefaultValueFieldFromTemplate(
                newChildrenArray.TemplateField.Children[1]
            );
            newChildEntry["m_FileID"].AsInt = 0;
            newChildEntry["m_PathID"].AsLong = source.TransformPathId;
            newChildrenArray.Children.Add(newChildEntry);

            // Sync size
            var newSizeField = newParentTfmBf["m_Children"]["Array"]["size"];
            if (newSizeField.IsDummy) newSizeField = newParentTfmBf["m_Children"]["size"];
            if (!newSizeField.IsDummy) newSizeField.AsInt = newChildrenArray.Children.Count;
        }

        // 4. Persist source and new parent
        var sourceInst = Workspace.GetAssetInst(fileInst, 0, source.TransformPathId);
        if (sourceInst != null) sourceInst.UpdateAssetDataAndRow(Workspace, sourceTfmBf);

        var newParentInst = Workspace.GetAssetInst(fileInst, 0, newParent.TransformPathId);
        if (newParentInst != null) newParentInst.UpdateAssetDataAndRow(Workspace, newParentTfmBf);

        return true;
    }

    /// <summary>
    /// Removes a HierarchyItem from the tree, deletes its underlying GameObject, Transform,
    /// all associated Components, and recursively deletes all its children in the same manner.
    /// Also detaches it from its parent's m_Children array and updates the Workspace.
    /// </summary>
    public void RemoveNode(HierarchyItem node)
    {
        if (node == null || node.TransformPathId == 0 || node.FileInstance == null)
            return;

        var fileInst = node.FileInstance;
        var parent = node.Parent;

        // 1. Remove from parent's m_Children array in the asset data
        if (parent != null && parent.TransformPathId != 0)
        {
            var parentTfmBf = Workspace.GetBaseField(fileInst, parent.TransformPathId);
            if (parentTfmBf != null)
            {
                var childrenArray = parentTfmBf["m_Children"]["Array"];
                if (!childrenArray.IsDummy && childrenArray.Children != null)
                {
                    childrenArray.Children.RemoveAll(c => c["m_PathID"].AsLong == node.TransformPathId);

                    var sizeField = parentTfmBf["m_Children"]["Array"]["size"];
                    if (sizeField.IsDummy) sizeField = parentTfmBf["m_Children"]["size"];
                    if (!sizeField.IsDummy) sizeField.AsInt = childrenArray.Children.Count;

                    var parentInst = Workspace.GetAssetInst(fileInst, 0, parent.TransformPathId);
                    if (parentInst != null)
                    {
                        parentInst.UpdateAssetDataAndRow(Workspace, parentTfmBf);
                    }
                }
            }
        }

        // 2. Recursively gather and remove all objects
        var assetsToRemove = new List<AssetInst>();
        RemoveNodeRecursiveWithDependencies(node, assetsToRemove);

        // 3. Delete from File metadata
        var removedPathIds = assetsToRemove.Select(a => a.PathId).ToHashSet();
        foreach (var asset in assetsToRemove)
        {
            fileInst.file.Metadata.RemoveAssetInfo(asset);

            // If it happens to be in ActiveAssets, clear it so UI doesn't crash trying to inspect it
            if (ActiveAssets.Contains(asset))
            {
                ActiveAssets.Remove(asset);
            }
        }

        // 4. Clean up AssetBundle (m_PreloadTable and m_Container)
        AssetFileInfo? abInfo = fileInst.file.Metadata.AssetInfos.FirstOrDefault(i => i.TypeId == (int)AssetClassID.AssetBundle);
        if (abInfo != null)
        {
            var abAsset = Workspace.GetAssetInst(fileInst, 0, abInfo.PathId);
            if (abAsset != null)
            {
                var abBf = Workspace.GetBaseField(abAsset);
                if (abBf != null)
                {
                    bool abModified = false;

                    // Remove from m_PreloadTable
                    var preloadTable = abBf["m_PreloadTable"]["Array"];
                    if (!preloadTable.IsDummy && preloadTable.Children != null)
                    {
                        int beforeCount = preloadTable.Children.Count;
                        preloadTable.Children.RemoveAll(c => c["m_FileID"].AsInt == 0 && removedPathIds.Contains(c["m_PathID"].AsLong));
                        if (preloadTable.Children.Count != beforeCount)
                        {
                            abModified = true;
                        }
                    }

                    // Remove from m_Container
                    var container = abBf["m_Container"]["Array"];
                    if (!container.IsDummy && container.Children != null)
                    {
                        var toRemove = new List<AssetTypeValueField>();
                        foreach (var item in container.Children)
                        {
                            var assetPtr = item["second"]["asset"];
                            if (assetPtr != null && !assetPtr.IsDummy)
                            {
                                if (assetPtr["m_FileID"].AsInt == 0 && removedPathIds.Contains(assetPtr["m_PathID"].AsLong))
                                {
                                    toRemove.Add(item);
                                }
                            }
                        }

                        if (toRemove.Count > 0)
                        {
                            foreach (var item in toRemove)
                            {
                                container.Children.Remove(item);
                            }
                            abModified = true;
                        }

                        // Always sync preloadSize if we changed preloadTable
                        if (abModified)
                        {
                            foreach (var item in container.Children)
                            {
                                var second = item["second"];
                                if (second != null && !second.IsDummy)
                                {
                                    second["preloadSize"].AsInt = preloadTable.Children.Count;
                                }
                            }
                        }
                    }

                    if (abModified)
                    {
                        abAsset.UpdateAssetDataAndRow(Workspace, abBf);
                    }
                }
            }
        }

        // 5. Dirty workspace
        var wsItem = Workspace.FindWorkspaceItemByInstance(fileInst);
        if (wsItem != null)
            Workspace.Dirty(wsItem);

        // 6. Remove from UI
        if (parent != null)
        {
            parent.Children.Remove(node);
        }
        else
        {
            RootItems.Remove(node);
        }

        // Clear selection if we deleted the selected item
        if (SelectedItem == node || assetsToRemove.Any(a => a == SelectedItem?.Asset))
        {
            SelectedItem = null;
        }
    }

    private void RemoveNodeRecursiveWithDependencies(HierarchyItem rootNode, List<AssetInst> assetsToRemove)
    {
        var fileInst = rootNode.FileInstance;
        if (fileInst == null) return;

        var hierarchyQueue = new Queue<long>();
        var componentQueue = new Queue<long>();
        var dataQueue = new Queue<long>();
        var discoveredPathIds = new HashSet<long>();

        if (rootNode.TransformPathId != 0 && fileInst.file.GetAssetInfo(rootNode.TransformPathId) != null)
        {
            var tfmBf = Workspace.GetBaseField(fileInst, rootNode.TransformPathId);
            if (tfmBf != null)
            {
                long startGoPathId = tfmBf["m_GameObject"]["m_PathID"].AsLong;
                if (startGoPathId != 0)
                {
                    hierarchyQueue.Enqueue(startGoPathId);
                }
            }
        }

        // 1. Hierarchy walk
        while (hierarchyQueue.Count > 0)
        {
            long currentPathId = hierarchyQueue.Dequeue();
            if (currentPathId == 0 || !discoveredPathIds.Add(currentPathId)) continue;

            var info = fileInst.file.GetAssetInfo(currentPathId);
            if (info == null) continue;

            var bf = Workspace.GetBaseField(fileInst, info.PathId);
            if (bf == null) continue;

            if (info.TypeId == (int)AssetClassID.GameObject)
            {
                var components = bf["m_Component"]["Array"];
                if (!components.IsDummy && components.Children != null)
                {
                    foreach (var comp in components.Children)
                    {
                        var ptr = comp[comp.Children.Count - 1];
                        if (ptr["m_FileID"].AsInt == 0)
                        {
                            long compPathId = ptr["m_PathID"].AsLong;
                            if (compPathId != 0 && !discoveredPathIds.Contains(compPathId))
                            {
                                var compInfo = fileInst.file.GetAssetInfo(compPathId);
                                if (compInfo != null)
                                {
                                    if (compInfo.TypeId == (int)AssetClassID.Transform ||
                                        compInfo.TypeId == (int)AssetClassID.RectTransform)
                                    {
                                        hierarchyQueue.Enqueue(compPathId);
                                    }
                                    else
                                    {
                                        discoveredPathIds.Add(compPathId);
                                        componentQueue.Enqueue(compPathId);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (info.TypeId == (int)AssetClassID.Transform || info.TypeId == (int)AssetClassID.RectTransform)
            {
                var children = bf["m_Children"]["Array"];
                if (!children.IsDummy && children.Children != null)
                {
                    foreach (var child in children.Children)
                    {
                        if (child["m_FileID"].AsInt == 0)
                        {
                            long childPathId = child["m_PathID"].AsLong;
                            if (childPathId != 0) hierarchyQueue.Enqueue(childPathId);
                        }
                    }
                }

                var go = bf["m_GameObject"];
                if (!go.IsDummy && go["m_FileID"].AsInt == 0)
                {
                    long goPathId = go["m_PathID"].AsLong;
                    if (goPathId != 0) hierarchyQueue.Enqueue(goPathId);
                }
            }
        }

        // 2. Scan components for data dependencies
        while (componentQueue.Count > 0)
        {
            long compPathId = componentQueue.Dequeue();
            var info = fileInst.file.GetAssetInfo(compPathId);
            if (info == null) continue;

            var bf = Workspace.GetBaseField(fileInst, info.PathId);
            if (bf == null) continue;

            ScanFieldForDataDependencies(bf, dataQueue, discoveredPathIds);
        }

        // 3. Scan data assets for further dependencies
        while (dataQueue.Count > 0)
        {
            long dataPathId = dataQueue.Dequeue();
            if (!discoveredPathIds.Add(dataPathId)) continue;

            var info = fileInst.file.GetAssetInfo(dataPathId);
            if (info == null) continue;

            // Do not delete external Transform/GameObject references (like avatar bones)
            if (info.TypeId == (int)AssetClassID.GameObject ||
                info.TypeId == (int)AssetClassID.Transform ||
                info.TypeId == (int)AssetClassID.RectTransform)
                continue;

            var bf = Workspace.GetBaseField(fileInst, info.PathId);
            if (bf == null) continue;

            ScanFieldForDataDependencies(bf, dataQueue, discoveredPathIds);
        }

        // Add all valid discovered pathIds to assetsToRemove
        foreach (var pathId in discoveredPathIds)
        {
            var info = fileInst.file.GetAssetInfo(pathId);
            if (info != null)
            {
                if (info.TypeId == (int)AssetClassID.GameObject ||
                    info.TypeId == (int)AssetClassID.Transform ||
                    info.TypeId == (int)AssetClassID.RectTransform)
                {
                    // Always add our own GameObjects and Transforms
                    var inst = Workspace.GetAssetInst(fileInst, 0, pathId);
                    if (inst != null) assetsToRemove.Add(inst);
                }
                else
                {
                    // It's a data asset or component. Add it.
                    var inst = Workspace.GetAssetInst(fileInst, 0, pathId);
                    if (inst != null) assetsToRemove.Add(inst);
                }
            }
        }
    }

    private void ScanFieldForDataDependencies(AssetTypeValueField field, Queue<long> queue, HashSet<long> discovered)
    {
        if (field.Children == null) return;

        bool isPtr = field.Children.Count >= 2 &&
                     field.Children[0].TemplateField.Name == "m_FileID" &&
                     field.Children[1].TemplateField.Name == "m_PathID";

        if (isPtr)
        {
            int fileId = field.Children[0].AsInt;
            long pathId = field.Children[1].AsLong;

            if (fileId == 0 && pathId != 0 && !discovered.Contains(pathId))
            {
                queue.Enqueue(pathId);
            }
            return;
        }

        foreach (var child in field.Children)
        {
            ScanFieldForDataDependencies(child, queue, discovered);
        }
    }

    /// <summary>
    /// [MA PARITY] Fix A-Pose by aligning outfit shoulder/arm rotations to match the avatar.
    /// This prevents shoulder distortion when model poses differ slightly.
    /// Ported from SetupOutfit.FixAPose (Modular Avatar).
    /// </summary>
    private void FixAPose(HierarchyItem avatarArmature, HierarchyItem outfitArmature, string prefix, string suffix,
                          Dictionary<long, int> avatarHumanoidMap, Dictionary<long, int> outfitHumanoidMap, SetupOutfitReport? report = null)
    {
        var fileInst = avatarArmature.FileInstance;
        if (fileInst == null) return;

        // PERFECT MA PARITY: Match exact HumanBodyBones sequence
        // MA: LeftShoulder, RightShoulder, LeftUpperArm, RightUpperArm
        var armBones = new[] {
            HumanBodyBones.LeftShoulder, HumanBodyBones.RightShoulder,
            HumanBodyBones.LeftUpperArm, HumanBodyBones.RightUpperArm
        };

        var allAvatarBones = avatarArmature.Flatten().ToList();
        var allOutfitBones = outfitArmature.Flatten().ToList();

        foreach (var boneIdx in armBones)
        {
            var avatarBone = allAvatarBones.FirstOrDefault(b =>
                b.Asset != null && avatarHumanoidMap.TryGetValue(b.Asset.PathId, out int idx) && idx == (int)boneIdx);

            if (avatarBone == null)
            {
                report?.APoseFixes.Add(new APoseFixEntry
                {
                    BoneName = boneIdx.ToString(),
                    Applied = false,
                    SkipReason = $"头像中未找到 {boneIdx} 骨骼 (Avatar bone {boneIdx} not found)"
                });
                continue;
            }

            // [MA PARITY] avatarToOutfit equivalent: match outfit bone by path-based lookup with prefix/suffix
            var relPath = GetRelativePath(avatarArmature, avatarBone);
            var transformedPath = string.Join("/", relPath.Split('/').Select(p => prefix + p + suffix));
            var outfitBone = outfitArmature.Flatten().FirstOrDefault(b => GetRelativePath(outfitArmature, b) == transformedPath);

            if (outfitBone == null)
            {
                report?.APoseFixes.Add(new APoseFixEntry
                {
                    BoneName = boneIdx.ToString(),
                    AvatarBoneName = avatarBone.Name,
                    Applied = false,
                    SkipReason = $"服装中未找到对应骨骼 (Outfit bone not found for path: {transformedPath})"
                });
                continue;
            }

            // Find lower arm (elbow) for vector calculation (MA: HumanBodyBones + 2)
            var lowerBoneIdx = (HumanBodyBones)((int)boneIdx + 2);

            var avatarLower = allAvatarBones.FirstOrDefault(b =>
                b.Asset != null && avatarHumanoidMap.TryGetValue(b.Asset.PathId, out int idx) && idx == (int)lowerBoneIdx);

            HierarchyItem? outfitLower = null;
            if (avatarLower != null)
            {
                var lowerRelPath = GetRelativePath(avatarArmature, avatarLower);
                var transformedLowerPath = string.Join("/", lowerRelPath.Split('/').Select(p => prefix + p + suffix));
                outfitLower = outfitArmature.Flatten().FirstOrDefault(b => GetRelativePath(outfitArmature, b) == transformedLowerPath);
            }

            if (avatarLower != null && outfitLower != null)
            {
                FixSingleArm(avatarBone, outfitBone, avatarLower, outfitLower, fileInst, boneIdx, report);
            }
            else
            {
                report?.APoseFixes.Add(new APoseFixEntry
                {
                    BoneName = boneIdx.ToString(),
                    AvatarBoneName = avatarBone.Name,
                    OutfitBoneName = outfitBone.Name,
                    OutfitItem = outfitBone,
                    Applied = false,
                    SkipReason = $"下臂骨骼未找到 (Lower arm bone not found for {lowerBoneIdx})"
                });
            }
        }
    }

    private void FixSingleArm(HierarchyItem avatarArm, HierarchyItem outfitArm, HierarchyItem avatarLower, HierarchyItem outfitLower, AssetsFileInstance fileInst,
                               HumanBodyBones boneIdx = HumanBodyBones.Hips, SetupOutfitReport? report = null)
    {
        var fixEntry = new APoseFixEntry
        {
            BoneName = boneIdx.ToString(),
            AvatarBoneName = avatarArm.Name,
            OutfitBoneName = outfitArm.Name,
            OutfitItem = outfitArm
        };

        Vector3 avatarArmPos = HierarchyMath.GetWorldPosition(Workspace, avatarArm);
        Vector3 outfitArmPos = HierarchyMath.GetWorldPosition(Workspace, outfitArm);

        Vector3 avatarLowerPos = HierarchyMath.GetWorldPosition(Workspace, avatarLower);
        Vector3 outfitLowerPos = HierarchyMath.GetWorldPosition(Workspace, outfitLower);

        // [MA PARITY] strictMode Check
        if ((avatarArmPos - outfitArmPos).Length() > 0.001f)
        {
            fixEntry.Applied = false;
            fixEntry.SkipReason = $"位置差异过大 (Position diff > 0.001: {(avatarArmPos - outfitArmPos).Length():F6})";
            report?.APoseFixes.Add(fixEntry);
            return;
        }

        // check relative distance to lower arm as well
        var avatarArmLength = (avatarLowerPos - avatarArmPos).Length();
        var outfitArmLength = (outfitLowerPos - outfitArmPos).Length();

        if (Math.Abs(avatarArmLength - outfitArmLength) > 0.001f)
        {
            fixEntry.Applied = false;
            fixEntry.SkipReason = $"臂长差异过大 (Arm length diff > 0.001: {Math.Abs(avatarArmLength - outfitArmLength):F6})";
            report?.APoseFixes.Add(fixEntry);
            return;
        }

        // Rotate the outfit arm to ensure these two bone orientations match.
        Vector3 avVec = avatarLowerPos - avatarArmPos;
        Vector3 outVec = outfitLowerPos - outfitArmPos;

        if (avVec.Length() > 0.0001f && outVec.Length() > 0.0001f)
        {
            float dot = Vector3.Dot(Vector3.Normalize(avVec), Vector3.Normalize(outVec));
            if (dot < 0.999f)
            {
                var relRot = HierarchyMath.FromToRotation(outVec, avVec);
                var currentWorldRot = HierarchyMath.GetWorldRotation(Workspace, outfitArm);
                var newWorldRot = relRot * currentWorldRot;
                var newLocalRot = HierarchyMath.WorldToLocalRotation(Workspace, outfitArm, newWorldRot);

                fixEntry.OldRotation = currentWorldRot;
                fixEntry.NewRotation = newLocalRot;

                var outBf = Workspace.GetBaseField(fileInst, outfitArm.TransformPathId);
                if (outBf != null)
                {
                    outBf["m_LocalRotation"]["x"].AsFloat = newLocalRot.X;
                    outBf["m_LocalRotation"]["y"].AsFloat = newLocalRot.Y;
                    outBf["m_LocalRotation"]["z"].AsFloat = newLocalRot.Z;
                    outBf["m_LocalRotation"]["w"].AsFloat = newLocalRot.W;

                    var inst = Workspace.GetAssetInst(fileInst, 0, outfitArm.TransformPathId);
                    if (inst != null) inst.UpdateAssetDataAndRow(Workspace, outBf);
                }

                fixEntry.Applied = true;
                report?.APoseFixes.Add(fixEntry);
            }
            else
            {
                fixEntry.Applied = false;
                fixEntry.SkipReason = $"方向已对齐 (Already aligned, dot={dot:F6})";
                report?.APoseFixes.Add(fixEntry);
            }
        }
        else
        {
            fixEntry.Applied = false;
            fixEntry.SkipReason = "向量长度不足 (Vector too short)";
            report?.APoseFixes.Add(fixEntry);
        }
    }

    /// <summary>
    /// [MA PARITY] Find the "Body" mesh of the avatar using VRCAvatarDescriptor or size heuristic.
    /// Used for referencing RootBone and Bounds.
    /// </summary>
    private HierarchyItem? FindBodyMesh(HierarchyItem avatarRoot)
    {
        var fileInst = avatarRoot.FileInstance;
        if (fileInst == null) return null;

        // 1. Try to find VRCAvatarDescriptor
        HierarchyItem? descriptorNode = avatarRoot.Flatten().FirstOrDefault(item =>
        {
            var mb = FindComponent(fileInst, item.Asset?.PathId ?? 0, (AssetClassID)114);
            if (mb != null)
            {
                var bf = Workspace.GetBaseField(mb);
                return bf != null && bf.TemplateField.Name.Contains("VRCAvatarDescriptor");
            }
            return false;
        });

        if (descriptorNode != null)
        {
            var descriptorMb = FindComponent(fileInst, descriptorNode.Asset?.PathId ?? 0, (AssetClassID)114);
            if (descriptorMb != null)
            {
                var mbBf = Workspace.GetBaseField(descriptorMb);
                if (mbBf != null)
                {
                    var visemeMeshPtr = mbBf["VisemeSkinnedMesh"];
                    if (visemeMeshPtr != null && !visemeMeshPtr.IsDummy)
                    {
                        long meshGoPathId = visemeMeshPtr["m_PathID"].AsLong;
                        if (meshGoPathId != 0)
                        {
                            return avatarRoot.Flatten().FirstOrDefault(i => i.Asset?.PathId == meshGoPathId);
                        }
                    }
                }
            }
        }

        // 2. Fallback: Find largest SMR (Only look at direct children for parity)
        HierarchyItem? largestSmr = null;
        // int maxVertexCount = -1; // Unused

        foreach (var item in avatarRoot.Children)
        {
            var smr = FindComponent(fileInst, item.Asset?.PathId ?? 0, AssetClassID.SkinnedMeshRenderer);
            if (smr != null)
            {
                var smrBf = Workspace.GetBaseField(smr);
                if (smrBf != null)
                {
                    // For simplicity in this tool, we just pick any SMR found first if we can't get vertex count easily
                    // But we try to look for name "Body" or similar
                    if (item.Name.ToLowerInvariant().Contains("body")) return item;
                    largestSmr ??= item;
                }
            }
        }

        return largestSmr;
    }

    /// <summary>
    /// [MA PARITY] Rename outfit bones to match avatar bone names (with prefix/suffix).
    /// This ensures consistency across the merged hierarchy.
    /// </summary>
    private void RenameBones(List<(HierarchyItem outfit, HierarchyItem avatar)> mappings, string prefix, string suffix)
    {
        if (mappings.Count == 0) return;
        var fileInst = mappings[0].outfit.FileInstance;
        if (fileInst == null) return;

        foreach (var (outfitBone, avatarBone) in mappings)
        {
            var newName = prefix + avatarBone.Name + suffix;
            if (outfitBone.Name != newName)
            {
                outfitBone.Name = newName;
                var goBf = Workspace.GetBaseField(fileInst, outfitBone.Asset?.PathId ?? 0);
                if (goBf != null)
                {
                    goBf["m_Name"].AsString = newName;
                    var goInst = Workspace.GetAssetInst(fileInst, 0, outfitBone.Asset?.PathId ?? 0);
                    if (goInst != null) goInst.UpdateAssetDataAndRow(Workspace, goBf);
                }
            }
        }
    }

    private string GetRelativePath(HierarchyItem root, HierarchyItem target)
    {
        var path = new List<string>();
        var current = target;
        while (current != null && current != root)
        {
            path.Insert(0, current.Name);
            current = current.Parent;
        }
        return string.Join("/", path);
    }

    /// <summary>
    /// Simplified HumanBodyBones enum for parity with Unity's Animator indices.
    /// </summary>
    public enum HumanBodyBones
    {
        Hips = 0,
        LeftUpperLeg = 1,
        RightUpperLeg = 2,
        LeftLowerLeg = 3,
        RightLowerLeg = 4,
        LeftFoot = 5,
        RightFoot = 6,
        Spine = 7,
        Chest = 8,
        Neck = 9,
        Head = 10,
        LeftShoulder = 11,
        RightShoulder = 12,
        LeftUpperArm = 13,
        RightUpperArm = 14,
        LeftLowerArm = 15,
        RightLowerArm = 16,
        LeftHand = 17,
        RightHand = 18,
        LeftToes = 19,
        RightToes = 20,
        LeftEye = 21,
        RightEye = 22,
        Jaw = 23,
        LeftThumbProximal = 24,
        LeftThumbIntermediate = 25,
        LeftThumbDistal = 26,
        LeftIndexProximal = 27,
        LeftIndexIntermediate = 28,
        LeftIndexDistal = 29,
        LeftMiddleProximal = 30,
        LeftMiddleIntermediate = 31,
        LeftMiddleDistal = 32,
        LeftRingProximal = 33,
        LeftRingIntermediate = 34,
        LeftRingDistal = 35,
        LeftLittleProximal = 36,
        LeftLittleIntermediate = 37,
        LeftLittleDistal = 38,
        RightThumbProximal = 39,
        RightThumbIntermediate = 40,
        RightThumbDistal = 41,
        RightIndexProximal = 42,
        RightIndexIntermediate = 43,
        RightIndexDistal = 44,
        RightMiddleProximal = 45,
        RightMiddleIntermediate = 46,
        RightMiddleDistal = 47,
        RightRingProximal = 48,
        RightRingIntermediate = 49,
        RightRingDistal = 50,
        RightLittleProximal = 51,
        RightLittleIntermediate = 52,
        RightLittleDistal = 53,
        UpperChest = 54,
        LastBone = 55
    }
}
