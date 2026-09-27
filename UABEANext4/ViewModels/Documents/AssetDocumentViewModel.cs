using AssetsTools.NET; // 引用 AssetsTools.NET 库，用于处理 Unity 资产文件的低层 API（保持原名 AssetsTools.NET）

using AssetsTools.NET.Extra; // 引用 AssetsTools.NET 的扩展功能（额外工具），用于更高层的资产操作（保持原名 AssetsTools.NET.Extra）

using Avalonia.Collections; // 引用 Avalonia 的集合类型（AvaloniaList 等），用于 UI 绑定（保持原名 Avalonia.Collections）

using Avalonia.Platform.Storage; // 引用 Avalonia 的存储接口，用于文件/文件夹选择对话框（保持原名 Avalonia.Platform.Storage）

using CommunityToolkit.Mvvm.ComponentModel; // 引用 CommunityToolkit.Mvvm 的组件模型，提供 ObservableProperty 等 MVVM 特性（保持原名 CommunityToolkit.Mvvm.ComponentModel）

using CommunityToolkit.Mvvm.Input; // 引用 RelayCommand 特性（CommunityToolkit.Mvvm.Input）

using CommunityToolkit.Mvvm.DependencyInjection; // 引用 CommunityToolkit 的依赖注入支持（Ioc.Default），用于获取服务（保持原名 CommunityToolkit.Mvvm.DependencyInjection）

using CommunityToolkit.Mvvm.Messaging; // 引用 CommunityToolkit 的消息总线（WeakReferenceMessenger），用于组件间通信（保持原名 CommunityToolkit.Mvvm.Messaging）

using Dock.Model.Mvvm.Controls; // 引用 Dock.Model 的 MVVM 控件基类（Document、Tool 等），用于停靠式 UI（保持原名 Dock.Model.Mvvm.Controls）

using DynamicData; // 引用 DynamicData 库，用于响应式集合与数据流（保持原名 DynamicData）

using DynamicData.Binding; // 引用 DynamicData 的绑定扩展（RangeObservableCollection 等），用于集合绑定（保持原名 DynamicData.Binding）

using System; // 引用基础系统命名空间，提供基本类型与工具（保持原名 System）

using System.Collections.Generic; // 引用泛型集合命名空间（List、Dictionary 等）（保持原名 System.Collections.Generic）

using System.Collections.ObjectModel; // 引用可观察集合类型（ObservableCollection），用于 UI 绑定（保持原名 System.Collections.ObjectModel）

using System.IO; // 引用 IO 操作命名空间，用于文件读写（保持原名 System.IO）

using System.Linq; // 引用 LINQ 扩展方法，用于集合查询与转换（保持原名 System.Linq）

using System.Reactive.Linq; // 引用 Reactive 扩展，用于可观察序列操作（保持原名 System.Reactive.Linq）

using System.Text; // 引用文本编码与处理命名空间（Encoding 等）（保持原名 System.Text）

using System.Text.RegularExpressions; // 引用正则表达式支持（Regex 等）（保持原名 System.Text.RegularExpressions）

using System.Threading; // 引用线程与同步原语（CancellationTokenSource 等）（保持原名 System.Threading）

using System.Threading.Tasks; // 引用异步任务支持（Task、async/await）（保持原名 System.Threading.Tasks）

using UABEANext4.AssetWorkspace; // 引用项目的资产工作区命名空间（Workspace、AssetInst 等）（保持原名 UABEANext4.AssetWorkspace）

using UABEANext4.Logic; // 引用项目逻辑层命名空间（工具与帮助类）（保持原名 UABEANext4.Logic）

using UABEANext4.Logic.Configuration; // 引用配置相关逻辑（ConfigurationManager 等）（保持原名 UABEANext4.Logic.Configuration）

using UABEANext4.Logic.ImportExport; // 引用导入导出逻辑（AssetImport、AssetExport 等）（保持原名 UABEANext4.Logic.ImportExport）

using UABEANext4.Plugins; // 引用插件系统相关命名空间（UAV 插件接口等）（保持原名 UABEANext4.Plugins）

using UABEANext4.Services; // 引用服务层（StorageService、IDialogService 等）（保持原名 UABEANext4.Services）

using UABEANext4.Util; // 引用工具类（FileDialogUtils、MessageBoxUtil、DebounceUtils 等）（保持原名 UABEANext4.Util）

using UABEANext4.ViewModels.Dialogs; // 引用对话框视图模型命名空间（AddAssetViewModel、BatchImportViewModel 等）（保持原名 UABEANext4.ViewModels.Dialogs）

namespace UABEANext4.ViewModels.Documents; // 定义命名空间 UABEANext4.ViewModels.Documents（组织文档相关的视图模型类）

public partial class AssetDocumentViewModel : Document // 定义部分类 AssetDocumentViewModel，继承自 Document（表示资产文档视图模型）
{
    const string TOOL_TITLE = "资产文档 (Asset Document)"; // 常量：文档标题，中文显示并在括号保留英文原名（TOOL_TITLE）

    public Workspace Workspace { get; } // 只读属性：引用传入的 Workspace 实例，管理已加载的文件与资产（Workspace）

    public bool LoadContainers { get; } // 只读属性：指示是否加载容器（Bundle）信息（LoadContainers）

    public List<AssetInst> SelectedItems { get; set; } = []; // 属性：当前选中的资产列表（SelectedItems），初始化为空列表（保留英文名）

    public List<AssetsFileInstance> FileInsts { get; set; } = []; // 属性：当前文档关联的文件实例列表（FileInsts），初始化为空列表

    public ReadOnlyObservableCollection<AssetInst> Items { get; set; } = new([]); // 属性：只读可观察集合，表示当前显示在表格中的资产项（Items），初始化为空集合

    public Dictionary<AssetClassID, string> ClassIdToString { get; } // 属性：将 AssetClassID 枚举映射为字符串的字典（ClassIdToString）

    [ObservableProperty] // 特性：自动生成属性与通知（将生成 CollectionView 属性）
    public DataGridCollectionView _collectionView = new(new List<object>()); // 字段：DataGridCollectionView 的后备字段，用于表格视图的集合视图（CollectionView）

    [ObservableProperty] // 特性：自动生成属性与通知（将生成 SearchText 属性）
    public string _searchText = ""; // 字段：搜索文本的后备字段（SearchText），默认空字符串

    [ObservableProperty] // 特性：自动生成属性与通知（将生成 PluginsItems 属性）
    public ObservableCollection<PluginItemInfo> _pluginsItems = []; // 字段：插件菜单项集合的后备字段（PluginsItems），初始化为空集合

    [ObservableProperty] // 特性：自动生成属性与通知（将生成 IsSearchCaseSensitive 属性）
    public bool _isSearchCaseSensitive = false; // 字段：搜索是否区分大小写的后备字段（IsSearchCaseSensitive），默认 false

    [ObservableProperty] // 特性：自动生成属性与通知（将生成 SearchKind 属性）
    public AssetTextSearchKind _searchKind = 0; // 字段：搜索类型（PlainSearch 或 RegexSearch）的后备字段（SearchKind），默认 PlainSearch（枚举值 0）

    [ObservableProperty]
    private bool _isAnimationClipSelected = false; // 是否选中了 AnimationClip

    [ObservableProperty]
    private bool _isSMRSelected = false; // 是否选中了 SkinnedMeshRenderer

    public event Action? ShowPluginsContextMenuAction; // 事件：请求显示插件上下文菜单时触发（ShowPluginsContextMenuAction）

    public event Action<List<AssetInst>>? SetSelectedItemsAction; // 事件：请求设置选中项时触发（SetSelectedItemsAction），传递选中资产列表

    private List<TypeFilterTypeEntry>? _filterTypes = null; // 私有字段：可供选择的类型过滤项列表（_filterTypes），懒加载

    private HashSet<TypeFilterTypeEntry> _filterTypesFiltered = []; // 私有字段：当前被选中的类型过滤集合（_filterTypesFiltered），初始化为空集合

    private Dictionary<AssetsFileInstance, AssetTypeReference?[]> _typeRefLookup = []; // 私有字段：文件实例到脚本引用数组的查找表（_typeRefLookup），初始化为空字典

    private readonly Action<string> _setDataGridFilterDb; // 私有只读字段：防抖包装后的设置过滤器方法（_setDataGridFilterDb）

    private IDisposable? _disposableLastList; // 私有字段：保存上一次绑定的可观察订阅以便释放（_disposableLastList）

    private CancellationTokenSource? _loadCtSrc; // 私有字段：用于取消加载任务的 CancellationTokenSource（_loadCtSrc）

    [Obsolete("This constructor is for the designer only and should not be used directly.", true)] // 标记：此构造函数仅供设计器使用，不应直接调用（保留英文说明）
    public AssetDocumentViewModel() // 无参构造函数（仅供设计器）
    {
        Workspace = new(); // 为设计器创建一个新的 Workspace 实例（Workspace = new()）

        LoadContainers = false; // 设计器默认不加载容器（LoadContainers = false）

        ClassIdToString = Enum // 初始化 ClassIdToString 字典：将 AssetClassID 枚举值映射为其字符串表示
            .GetValues(typeof(AssetClassID)) // 获取 AssetClassID 枚举的所有值
            .Cast<AssetClassID>() // 将枚举值转换为 AssetClassID 类型序列
            .ToDictionary(enm => enm, enm => enm.ToString()); // 将每个枚举值映射为其 ToString() 字符串

        Id = TOOL_TITLE.Replace(" ", ""); // 设置文档 Id（移除 TOOL_TITLE 中的空格以生成内部标识）

        Title = TOOL_TITLE; // 设置文档标题（Title），用于 UI 显示（中文 + 英文原名）

        _setDataGridFilterDb = DebounceUtils.Debounce<string>((searchText) => // 使用防抖工具创建一个延迟执行的过滤器设置方法，避免频繁更新 UI
        {
            CollectionView.Filter = SetDataGridFilter(searchText); // 在防抖触发时设置 CollectionView 的过滤器
        }, 300); // 防抖延迟 300 毫秒
    }

    public AssetDocumentViewModel(Workspace workspace, bool loadContainers) // 运行时构造函数：接收 Workspace 与 LoadContainers 标志
    {
        Workspace = workspace; // 将传入的 workspace 赋值给属性 Workspace

        LoadContainers = loadContainers; // 将传入的 loadContainers 标志赋值给属性 LoadContainers

        ClassIdToString = Enum // 初始化 ClassIdToString 字典（同上）
            .GetValues(typeof(AssetClassID))
            .Cast<AssetClassID>()
            .ToDictionary(enm => enm, enm => enm.ToString());

        Id = TOOL_TITLE.Replace(" ", ""); // 设置文档 Id（移除空格）

        Title = TOOL_TITLE; // 设置文档标题

        _setDataGridFilterDb = DebounceUtils.Debounce<string>((searchText) => // 初始化防抖过滤器设置方法（同上）
        {
            CollectionView.Filter = SetDataGridFilter(searchText); // 设置 CollectionView 过滤器
        }, 300); // 300 毫秒防抖

        WeakReferenceMessenger.Default.Register<WorkspaceClosingMessage>(this, (r, h) => _ = OnWorkspaceClosing(r, h)); // 注册消息监听：当工作区关闭时调用 OnWorkspaceClosing（异步）
        WeakReferenceMessenger.Default.Register<AssetFileModifiedMessage>(this, (r, h) => OnAssetFileModified(h));
    }

    private void OnAssetFileModified(AssetFileModifiedMessage message)
    {
        if (FileInsts.Contains(message.Value))
        {
            _filterTypes = null; // Reset filter types to include any new types added by patching
        }
    }

    partial void OnSearchTextChanged(string value) => _setDataGridFilterDb(value); // 部分方法：当 SearchText 改变时触发防抖过滤器设置（调用 _setDataGridFilterDb）

    private Func<object, bool> SetDataGridFilter(string searchText) // 私有方法：根据 searchText 与当前类型过滤设置返回一个用于 CollectionView 的过滤函数
    {
        var strCmp = IsSearchCaseSensitive // 根据是否区分大小写选择比较方式（strCmp）
            ? StringComparison.Ordinal // 区分大小写时使用 Ordinal
            : StringComparison.OrdinalIgnoreCase; // 不区分大小写时使用 OrdinalIgnoreCase

        Regex? regex; // 局部变量：可能的正则表达式（如果使用正则搜索）
        try
        {
            regex = SearchKind == AssetTextSearchKind.RegexSearch // 如果搜索类型为正则搜索则尝试构造 Regex
                ? new Regex(searchText, IsSearchCaseSensitive
                    ? RegexOptions.None
                    : RegexOptions.IgnoreCase)
                : null; // 否则 regex 为 null
        }
        catch
        {
            // skip invalid regex
            regex = null; // 如果正则构造失败（无效正则），忽略并将 regex 设为 null
        }

        if (_filterTypesFiltered is null || _filterTypesFiltered.Count == 0) // 如果没有按类型过滤的条件
        {
            // don't need to filter on types, use simpler branch
            // 不需要按类型过滤，使用更简单的分支逻辑

            if (string.IsNullOrEmpty(searchText)) // 如果搜索文本为空
                return a => true; // 返回一个始终为真的过滤器（不过滤任何项）

            if (SearchKind == AssetTextSearchKind.PathId)
            {
                if (long.TryParse(searchText, out long pathId))
                    return o => o is AssetInst a && a.PathId == pathId;
                return o => false;
            }

            if (regex is not null) // 如果存在有效的正则表达式
            {
                // simple + regex
                // 简单文本 + 正则匹配的过滤器
                return o =>
                {
                    if (o is not AssetInst a) // 如果对象不是 AssetInst
                        return false; // 过滤掉

                    if (regex.IsMatch(a.DisplayName)) // 如果正则匹配 DisplayName
                        return true; // 通过过滤

                    if (ClassIdToString.TryGetValue(a.Type, out string? classIdName) && regex.IsMatch(classIdName)) // 或者正则匹配类型名
                        return true; // 通过过滤

                    return false; // 否则不通过
                };
            }
            else
            {
                // simple + no regex
                // 简单文本匹配且不使用正则的过滤器
                return o =>
                {
                    if (o is not AssetInst a) // 如果对象不是 AssetInst
                        return false; // 过滤掉

                    if (a.DisplayName.Contains(searchText, strCmp)) // 如果 DisplayName 包含搜索文本（按 strCmp 比较）
                        return true; // 通过过滤

                    if (ClassIdToString.TryGetValue(a.Type, out string? classIdName) && classIdName == searchText) // 或者类型名完全等于搜索文本
                        return true; // 通过过滤

                    return false; // 否则不通过
                };
            }
        }
        else
        {
            // need to filter on types
            // 需要按类型过滤的分支

            // allocate one object and overwrite its fields
            // 分配一个临时 TypeFilterTypeEntry 对象以复用，减少分配
            var baseTypeEntry = new TypeFilterTypeEntry
            {
                DisplayText = string.Empty, // 初始化 DisplayText 为空
                TypeId = 0, // 初始化 TypeId 为 0
                ScriptRef = null // 初始化 ScriptRef 为 null
            };

            if (SearchKind == AssetTextSearchKind.PathId)
            {
                if (long.TryParse(searchText, out long pathId))
                {
                    return o =>
                    {
                        if (o is not AssetInst a) return false;
                        if (a.PathId != pathId) return false;
                        return DoesTypeFilterPass(a, baseTypeEntry);
                    };
                }
                return o => false;
            }

            if (SearchKind == AssetTextSearchKind.Date)
            {
                return o =>
                {
                    if (o is not AssetInst a) return false;
                    // First check type pass to avoid expensive dump if possible (though order matters for performance)
                    if (!DoesTypeFilterPass(a, baseTypeEntry)) return false;
                    
                    return DumpAndMatches(a, searchText, strCmp);
                };
            }

            if (regex is not null) // 如果使用正则
            {
                // type + regex
                // 类型过滤 + 正则匹配
                return o =>
                {
                    if (o is not AssetInst a) // 如果对象不是 AssetInst
                        return false; // 过滤掉

                    if (!regex.IsMatch(a.DisplayName)) // 如果 DisplayName 不匹配正则
                        return false; // 不通过

                    return DoesTypeFilterPass(a, baseTypeEntry); // 否则再检查类型过滤是否通过
                };
            }
            else
            {
                // type + no regex
                // 类型过滤 + 非正则文本匹配
                return o =>
                {
                    if (o is not AssetInst a) // 如果对象不是 AssetInst
                        return false; // 过滤掉

                    if (!a.DisplayName.Contains(searchText, strCmp)) // 如果 DisplayName 不包含搜索文本
                        return false; // 不通过

                    return DoesTypeFilterPass(a, baseTypeEntry); // 再检查类型过滤是否通过
                };
            }
        }
    }

    private bool DumpAndMatches(AssetInst asset, string searchText, StringComparison comparison)
    {
        try
        {
            var baseField = Workspace.GetBaseField(asset);
            if (baseField == null) return false;

            using (var ms = new MemoryStream())
            {
                var exporter = new AssetExport(ms);
                exporter.DumpJsonAsset(baseField);
                
                string json = Encoding.UTF8.GetString(ms.ToArray());
                return json.Contains(searchText, comparison);
            }
        }
        catch
        {
            return false;
        }
    }

    private bool DoesTypeFilterPass(AssetInst assetInst, TypeFilterTypeEntry baseTypeEntry) // 私有方法：判断给定资产是否通过当前类型过滤集合
    {
        var scriptIndex = assetInst.GetScriptIndex(assetInst.FileInstance.file); // 获取该资产在文件中的脚本索引（如果是 MonoBehaviour）

        baseTypeEntry.TypeId = assetInst.TypeId; // 将临时对象的 TypeId 设置为资产的 TypeId
        if (baseTypeEntry.TypeId < 0) // 如果 TypeId 小于 0（未设置）
        {
            baseTypeEntry.TypeId = (int)AssetClassID.MonoBehaviour; // 将其视为 MonoBehaviour（兼容处理）
        }

        if (scriptIndex != ushort.MaxValue) // 如果脚本索引有效（不是最大值占位）
        {
            var typeList = _typeRefLookup[assetInst.FileInstance]; // 从查找表获取该文件对应的脚本引用数组

            // just in case, let's check the bounds here
            // 为安全起见检查索引边界
            if (scriptIndex < typeList.Length)
                baseTypeEntry.ScriptRef = typeList[scriptIndex]; // 在范围内则设置 ScriptRef
            else
                baseTypeEntry.ScriptRef = null; // 否则设为 null
        }
        else
        {
            baseTypeEntry.ScriptRef = null; // 如果没有脚本索引则设为 null
        }

        return _filterTypesFiltered.Contains(baseTypeEntry); // 判断临时对象是否在被过滤的类型集合中（Contains 使用 TypeFilterTypeEntry 的相等性）
    }

    public async Task Load(List<AssetsFileInstance> fileInsts) // 公共异步方法：加载给定的文件实例并构建 Items（Load）
    {
        if (Workspace == null) // 如果 Workspace 为 null（防御性检查）
            return; // 返回

        _disposableLastList?.Dispose(); // 释放上一次绑定的订阅（如果存在），避免内存泄漏

        var sourceList = new SourceList<RangeObservableCollection<AssetFileInfo>>(); // 创建 DynamicData 的 SourceList，用于合并多个文件的 AssetFileInfo 集合

        _loadCtSrc?.Cancel(); // 如果已有加载任务，先请求取消
        _loadCtSrc = new CancellationTokenSource(); // 创建新的取消令牌源
        var loadCt = _loadCtSrc.Token; // 获取取消令牌
        try
        {
            await Task.Run(() => // 在后台线程中遍历文件并将其 AssetInfos 添加到 sourceList
            {
                foreach (var fileInst in fileInsts) // 遍历传入的文件实例
                {
                    if (loadCt.IsCancellationRequested) // 检查是否请求取消
                        loadCt.ThrowIfCancellationRequested(); // 抛出以终止任务

                    var infosObsCol = (RangeObservableCollection<AssetFileInfo>)fileInst.file.Metadata.AssetInfos; // 获取文件的 AssetInfos（RangeObservableCollection）
                    sourceList.Add(infosObsCol); // 将该集合添加到 sourceList

                    if (LoadContainers) // 如果需要加载容器信息
                        LoadContainersIntoInfos(fileInst, infosObsCol); // 将容器路径信息填充到对应的 AssetInst 中
                }
            }, loadCt);
        }
        catch (OperationCanceledException) // 如果任务被取消
        {
            sourceList.Clear(); // 清空 sourceList
        }

        var observableList = sourceList // 使用 DynamicData 将多个 RangeObservableCollection 合并并转换为 AssetInst 序列
            .Connect() // 连接 sourceList
            .MergeMany(e => e.ToObservableChangeSet()) // 合并内部集合的变更
            .Transform(a => (AssetInst)a); // 将 AssetFileInfo 转换为 AssetInst（类型转换）

        _disposableLastList = observableList.Bind(out var items).Subscribe(); // 绑定到本地变量 items 并订阅，保存订阅以便后续释放
        Items = items; // 将 Items 属性设置为绑定得到的只读可观察集合
        FileInsts = fileInsts; // 保存当前文件实例列表到 FileInsts

        _filterTypes = null; // 重置类型过滤候选
        _filterTypesFiltered = []; // 清空已选过滤集合
        _typeRefLookup = []; // 清空类型引用查找表

        CollectionView = new DataGridCollectionView(Items); // 创建 DataGridCollectionView 并绑定 Items（用于表格显示与排序/过滤）
        CollectionView.Filter = SetDataGridFilter(SearchText); // 设置初始过滤器（根据当前 SearchText）
    }

    private void LoadContainersIntoInfos(AssetsFileInstance fileInst, IList<AssetFileInfo> fileInfos) // 私有方法：将容器路径信息填充到 fileInfos 中的 AssetInst（LoadContainersIntoInfos）
    {
        ContainerTool? contToolRes = null; // 局部变量：容器工具结果（可能为 null）
        AssetsFileInstance? contFile; // 局部变量：存储容器映射的文件实例（contFile）
        AssetTypeValueField? contBf; // 局部变量：存储容器映射的基础字段（contBf）
        if (ContainerTool.TryGetBundleContainerBaseField(Workspace, fileInst, out contFile, out contBf)) // 尝试从 bundle 中获取容器映射
        {
            contToolRes = ContainerTool.FromAssetBundle(Workspace.Manager, contFile, contBf); // 如果成功则创建 ContainerTool
        }
        else if (ContainerTool.TryGetRsrcManContainerBaseField(Workspace, fileInst, out contFile, out contBf)) // 否则尝试从资源管理器中获取容器映射
        {
            contToolRes = ContainerTool.FromResourceManager(Workspace.Manager, contFile, contBf); // 创建 ContainerTool（资源管理器方式）
        }

        if (contToolRes is null) // 如果没有找到容器映射工具
            return; // 返回，不做任何填充

        foreach (var assetInf in fileInfos) // 遍历文件中的每个 AssetFileInfo
        {
            var assetPtr = new AssetPPtr(fileInst.path, assetInf.PathId); // 构造 AssetPPtr（文件路径 + PathId）用于查找容器路径
            var path = contToolRes.GetContainerPath(assetPtr); // 使用容器工具获取该资产的容器路径
            if (path is not null && assetInf is AssetInst asset) // 如果找到了路径且 assetInf 可转换为 AssetInst
            {
                asset.DisplayContainer = path; // 将容器路径写入 AssetInst 的 DisplayContainer 字段
            }
        }
    }

    public async void ViewScene() // 公共异步方法：在场景视图中查看选中项（ViewScene）
    {
        if (SelectedItems.Count >= 1) // 如果至少有一个选中项
        {
            var asset = SelectedItems.First(); // 取第一个选中资产作为目标

            // select gameobject if this is a component
            // 如果选中的是组件而非 GameObject，则尝试定位其所属的 GameObject
            if (asset.Type != AssetClassID.GameObject) // 如果类型不是 GameObject
            {
                var assetBf = Workspace.GetBaseField(asset); // 获取该资产的 BaseField（反序列化后的字段树）
                if (assetBf is null) // 如果无法反序列化
                {
                    await MessageBoxUtil.ShowDialog("读取错误(Read error)", "尝试检查Component字段，但无法反序列化资源。(Tried to check for component fields but couldn't deserialize the asset.)"); // 弹出错误对话框（英文原文）
                    return; // 返回
                }

                var assetBfGoPtr = assetBf["m_GameObject"]; // 读取 m_GameObject 引用字段
                if (assetBfGoPtr.IsDummy) // 如果引用是占位（无效）
                {
                    await MessageBoxUtil.ShowDialog("不是GameObject或Component(Not a GameObject or Component)", "所选资源必须是 GameObject 或 GameObject 组件。(The selected asset must be a GameObject or GameObject component.)"); // 弹出提示（英文原文）
                    return; // 返回
                }

                asset = Workspace.GetAssetInst(asset.FileInstance, assetBfGoPtr); // 根据引用解析出对应的 GameObject AssetInst
                if (asset is null) // 如果解析失败
                {
                    await MessageBoxUtil.ShowDialog("无效的GameObject引用(Invalid GameObject reference)", "找不到组件 GameObject。是否需要加载依赖项？(Can't find component's GameObject. Do you need to load a dependency?)"); // 弹出错误提示（英文原文）
                    return; // 返回
                }
            }

            WeakReferenceMessenger.Default.Send(new RequestSceneViewMessage(asset)); // 发送 RequestSceneViewMessage，请求在场景视图中显示该 GameObject（消息总线）
        }
    }

    public void Import() // 公共方法：根据选中项决定批量导入或单个导入（Import）
    {
        if (SelectedItems.Count > 1) // 如果选中多于一个
        {
            ImportBatch(SelectedItems.ToList()); // 调用批量导入（ImportBatch）
        }
        else if (SelectedItems.Count == 1) // 如果仅选中一个
        {
            ImportSingle(SelectedItems.First()); // 调用单个导入（ImportSingle）
        }
    }

    public async void ImportBatch(List<AssetInst> assets) // 公共异步方法：批量导入（ImportBatch）
    {
        var storageProvider = StorageService.GetStorageProvider(); // 获取平台存储提供者（StorageService）
        if (storageProvider is null) // 如果没有可用的存储提供者
        {
            return; // 返回
        }

        var result = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions // 弹出文件夹选择对话框
        {
            Title = "Choose folder to import", // 对话框标题（英文原文）
            AllowMultiple = false // 不允许多选
        });

        var folders = FileDialogUtils.GetOpenFolderDialogFolders(result); // 解析选择结果为文件夹路径数组
        if (folders == null || folders.Length != 1) // 如果未选择或选择数量不为 1
            return; // 返回

        List<string> exts = new List<string>() // 定义允许的扩展名列表（用于导入）
        {
            "json", "txt", "dat"
        };

        var dialogService = Ioc.Default.GetRequiredService<IDialogService>(); // 通过依赖注入获取对话框服务

        var fileNamesToDirty = new HashSet<string>(); // 用于记录需要标记为已修改的文件名集合
        var batchInfos = await dialogService.ShowDialog(new BatchImportViewModel(Workspace, assets, folders[0], exts)); // 弹出批量导入对话框并等待结果
        if (batchInfos == null) // 如果用户取消或没有返回信息
        {
            return; // 返回
        }

        foreach (ImportBatchInfo batchInfo in batchInfos) // 遍历每个批量导入项
        {
            var selectedFilePath = batchInfo.ImportFile; // 获取要导入的文件路径
            if (selectedFilePath == null) // 如果路径为空
                continue; // 跳过

            var selectedAsset = batchInfo.Asset; // 获取目标资产
            var selectedInst = selectedAsset.FileInstance; // 获取资产所属的文件实例

            using FileStream fs = File.OpenRead(selectedFilePath); // 打开导入文件的只读流

            Workspace.CheckAndSetMonoTempGenerators(selectedInst, selectedAsset); // 检查并设置 Mono 临时生成器（与 MonoBehaviour 相关）
            var importer = new AssetImport(fs, Workspace.Manager.GetRefTypeManager(selectedInst)); // 创建 AssetImport 实例用于解析导入数据

            byte[]? data; // 局部变量：导入得到的字节数据
            string? exceptionMessage; // 局部变量：可能的异常信息

            if (selectedFilePath.EndsWith(".json")) // 如果是 .json 文件
            {
                var tempField = Workspace.GetTemplateField(selectedAsset); // 获取模板字段（用于 JSON 导入）
                data = importer.ImportJsonAsset(tempField, out exceptionMessage); // 使用 ImportJsonAsset 导入并返回字节数据或异常信息
            }
            else if (selectedFilePath.EndsWith(".txt")) // 如果是 .txt 文件
            {
                data = importer.ImportTextAsset(out exceptionMessage); // 使用 ImportTextAsset 导入
            }
            else
            {
                exceptionMessage = string.Empty; // 其他情况先清空异常信息
                data = importer.ImportRawAsset(); // 直接导入原始二进制数据
            }

            if (data == null) // 如果导入失败（data 为 null）
            {
                await MessageBoxUtil.ShowDialog("解析错误(Parse error)", "读取转储文件时出错(Something went wrong when reading the dump file):\n" + exceptionMessage); // 弹出解析错误对话框（英文原文）
                goto dirtyFiles; // 跳转到 dirtyFiles 标签以处理已修改文件标记
            }

            selectedAsset.UpdateAssetDataAndRow(Workspace, data); // 使用导入的数据更新资产并刷新表格行
            fileNamesToDirty.Add(selectedAsset.FileInstance.name); // 将所属文件名加入需要标记为已修改的集合
        }

    dirtyFiles: // 标签：处理需要标记为已修改的文件
        foreach (var fileName in fileNamesToDirty) // 遍历需要标记的文件名
        {
            var fileToDirty = Workspace.ItemLookup[fileName]; // 通过文件名查找对应的 WorkspaceItem
            Workspace.Dirty(fileToDirty); // 标记该 WorkspaceItem 为已修改（触发保存提示等）
        }
    }

    public async void ImportSingle(AssetInst asset) // 公共异步方法：单个资产导入（ImportSingle）
    {
        var storageProvider = StorageService.GetStorageProvider(); // 获取存储提供者
        if (storageProvider is null) // 如果不可用
        {
            return; // 返回
        }

        var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions // 弹出文件选择对话框
        {
            Title = "选择要导入的文件(Choose file to import)", // 对话框标题（英文原文）
            AllowMultiple = true, // 允许多选（但后续只取第一个）
            FileTypeFilter = new FilePickerFileType[] // 文件类型过滤器
            {
                new FilePickerFileType("UABEA json dump (*.json)") { Patterns = new[] { "*.json" } },
                new FilePickerFileType("UABE txt dump (*.txt)") { Patterns = new[] { "*.txt" } },
                new FilePickerFileType("Raw dump (*.dat)") { Patterns = new[] { "*.dat" } },
                new FilePickerFileType("Raw dump (*.*)") { Patterns = new[] { "*" } },
            },
        });

        var files = FileDialogUtils.GetOpenFileDialogFiles(result); // 解析选择结果为文件路径数组
        if (files == null || files.Length == 0) // 如果未选择文件
            return; // 返回

        var file = files[0]; // 取第一个文件路径
        using var fs = File.OpenRead(file); // 打开文件流

        Workspace.CheckAndSetMonoTempGenerators(asset.FileInstance, asset); // 检查并设置 Mono 临时生成器
        var importer = new AssetImport(fs, Workspace.Manager.GetRefTypeManager(asset.FileInstance)); // 创建 AssetImport 实例

        byte[]? data = null; // 局部变量：导入数据
        string? exception; // 局部变量：异常信息

        if (file.EndsWith(".json") || file.EndsWith(".txt")) // 如果是文本或 JSON 文件
        {
            if (file.EndsWith(".json")) // JSON 情况
            {
                var baseField = Workspace.GetTemplateField(asset); // 获取模板字段
                if (baseField != null) // 如果模板字段存在
                {
                    data = importer.ImportJsonAsset(baseField, out exception); // 导入 JSON 并返回字节数据
                }
                else
                {
                    // handle template read error
                    // 注释：处理模板读取错误（原代码留空）
                }
            }
            else if (file.EndsWith(".txt")) // TXT 情况
            {
                data = importer.ImportTextAsset(out exception); // 导入文本并返回字节数据
            }
        }
        else //if (file.EndsWith(".dat"))
        {
            using var stream = File.OpenRead(file); // 打开原始二进制文件流
            data = importer.ImportRawAsset(); // 导入原始二进制数据
        }

        if (data != null) // 如果导入成功
        {
            asset.UpdateAssetDataAndRow(Workspace, data); // 更新资产数据并刷新表格行
        }

        var fileToDirty = Workspace.ItemLookup[asset.FileInstance.name]; // 查找所属 WorkspaceItem
        Workspace.Dirty(fileToDirty); // 标记为已修改
    }

    public async void Export() // 公共异步方法：导出选中资产（Export）
    {
        var storageProvider = StorageService.GetStorageProvider(); // 获取存储提供者
        if (storageProvider is null) // 如果不可用
        {
            return; // 返回
        }

        var maxNameLen = ConfigurationManager.Settings.ExportNameLength; // 从配置读取导出文件名最大长度
        var filesToWrite = new List<(AssetInst, string)>(); // 列表：要写入的 (asset, path) 对

        if (SelectedItems.Count > 1) // 如果选中多个资产（批量导出）
        {
            var dialogService = Ioc.Default.GetRequiredService<IDialogService>(); // 获取对话框服务

            var exportType = await dialogService.ShowDialog(new SelectDumpViewModel(true)); // 弹出导出类型选择对话框（批量模式）
            if (exportType == null) // 如果用户取消
            {
                return; // 返回
            }

            // bug fix for double dialog box freezing in windows
            // 注释：在 Windows 上避免双对话框导致冻结，使用 Task.Yield 让出线程
            await Task.Yield();

            var exportExt = exportType switch // 根据选择的导出类型决定扩展名
            {
                SelectedDumpType.JsonDump => ".json",
                SelectedDumpType.TxtDump => ".txt",
                SelectedDumpType.RawDump => ".dat",
                _ => ".dat"
            };

            var result = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions // 弹出选择导出目标文件夹对话框
            {
                Title = "Choose file to export to", // 对话框标题（英文原文）
                AllowMultiple = false // 不允许多选
            });

            var folders = FileDialogUtils.GetOpenFolderDialogFolders(result); // 解析选择结果为文件夹路径数组
            if (folders.Length == 0) // 如果未选择
            {
                return; // 返回
            }

            var folder = folders[0]; // 取第一个文件夹路径
            foreach (var asset in SelectedItems) // 遍历每个选中资产
            {
                var exportFileName = AssetNamer.GetAssetFileName(Workspace, asset, exportExt, maxNameLen); // 生成导出文件名（遵循最大长度）
                var exportFilePath = Path.Combine(folder, exportFileName); // 组合成完整路径
                filesToWrite.Add((asset, exportFilePath)); // 将 (asset, path) 加入待写列表
            }
        }
        else if (SelectedItems.Count == 1) // 如果仅选中一个资产
        {
            var asset = SelectedItems.First(); // 取第一个资产
            var exportFileName = AssetNamer.GetAssetFileName(Workspace, asset, string.Empty, maxNameLen); // 生成建议文件名（不带扩展）

            var result = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions // 弹出保存文件对话框
            {
                Title = "Choose file to export", // 对话框标题（英文原文）
                FileTypeChoices = new FilePickerFileType[] // 文件类型选项
                {
                    new FilePickerFileType("UABEA json dump (*.json)") { Patterns = new[] { "*.json" } },
                    new FilePickerFileType("UABE txt dump (*.txt)") { Patterns = new[] { "*.txt" } },
                    new FilePickerFileType("Raw dump (*.dat)") { Patterns = new[] { "*.dat" } }
                },
                DefaultExtension = "json", // 默认扩展名
                SuggestedFileName = exportFileName // 建议的文件名
            });

            var file = FileDialogUtils.GetSaveFileDialogFile(result); // 解析保存对话框结果为文件路径
            if (file == null) // 如果用户取消
            {
                return; // 返回
            }

            filesToWrite.Add((asset, file)); // 将单个 (asset, path) 加入待写列表
        }

        foreach (var (asset, file) in filesToWrite) // 遍历所有要写入的文件对
        {
            using var fs = File.OpenWrite(file); // 打开写入流（覆盖写入）
            var exporter = new AssetExport(fs); // 创建 AssetExport 用于导出数据

            if (file.EndsWith(".json") || file.EndsWith(".txt")) // 如果目标为文本或 JSON
            {
                var baseField = Workspace.GetBaseField(asset); // 获取资产的 BaseField（反序列化）
                if (baseField == null) // 如果无法反序列化
                {
                    fs.Write(Encoding.UTF8.GetBytes("Asset failed to deserialize.")); // 写入错误信息到文件
                }
                else
                {
                    if (file.EndsWith(".json")) // JSON 导出
                    {
                        exporter.DumpJsonAsset(baseField); // 导出为 JSON
                    }
                    else if (file.EndsWith(".txt")) // TXT 导出
                    {
                        exporter.DumpTextAsset(baseField); // 导出为文本
                    }
                }
            }
            else if (file.EndsWith(".dat")) // 如果目标为原始二进制 (.dat)
            {
                if (asset.IsReplacerPreviewable) // 如果资产使用 Replacer 并且可预览
                {
                    var previewStream = asset.Replacer.GetPreviewStream(); // 获取替换器的预览流
                    var previewReader = new AssetsFileReader(previewStream); // 用 AssetsFileReader 包装流
                    lock (previewStream) // 锁定流以保证线程安全读取
                    {
                        exporter.DumpRawAsset(previewReader, 0, (uint)previewStream.Length); // 导出预览流的全部内容
                    }
                }
                else
                {
                    lock (asset.FileInstance.LockReader) // 锁定文件实例的读取器以保证线程安全
                    {
                        exporter.DumpRawAsset(asset.FileReader, asset.AbsoluteByteStart, asset.ByteSize); // 导出资产在文件中的原始字节区间
                    }
                }
            }
        }
    }

    public void ShowPlugins() // 公共方法：显示针对选中资产可用的插件选项（ShowPlugins）
    {
        if (SelectedItems.Count == 0) // 如果没有选中任何资产
        {
            PluginsItems.Clear(); // 清空插件项集合
            PluginsItems.Add(new PluginItemInfo("No assets selected", null, this)); // 添加一项提示（英文原文）
            return; // 返回
        }

        var pluginTypes = UavPluginMode.Export | UavPluginMode.Import; // 需要的插件模式：导出或导入（组合标志）
        var pluginsList = Workspace.Plugins.GetOptionsThatSupport(Workspace, SelectedItems, pluginTypes); // 获取支持当前选中资产的插件选项列表
        if (pluginsList == null) // 如果没有返回列表
        {
            return; // 返回
        }

        if (pluginsList.Count == 0) // 如果列表为空（没有可用插件）
        {
            PluginsItems.Clear(); // 清空集合
            PluginsItems.Add(new PluginItemInfo("No plugins available", null, this)); // 添加提示项（英文原文）
        }
        else
        {
            PluginsItems.Clear(); // 清空集合
            foreach (var plugin in pluginsList) // 遍历每个插件选项
            {
                PluginsItems.Add(new PluginItemInfo(plugin.Option.Name, plugin.Option, this)); // 将插件信息包装为 PluginItemInfo 并加入集合
            }
        }

        ShowPluginsContextMenuAction?.Invoke(); // 触发显示插件上下文菜单的回调（如果已订阅）
    }

    public void EditDump() // 公共方法：请求编辑选中资产的转储（EditDump）
    {
        if (SelectedItems.Count > 0) // 如果至少有一个选中项
        {
            WeakReferenceMessenger.Default.Send(new RequestEditAssetMessage(SelectedItems[^1])); // 发送 RequestEditAssetMessage，请求编辑最后一个选中资产（消息总线）
        }
    }

    public async void AddAsset() // 公共异步方法：添加新资产（AddAsset）
    {
        var dialogService = Ioc.Default.GetRequiredService<IDialogService>(); // 获取对话框服务
        var result = await dialogService.ShowDialog(new AddAssetViewModel(Workspace, FileInsts)); // 弹出添加资产对话框并等待结果
        if (result == null) // 如果用户取消
        {
            return; // 返回
        }

        var baseInfo = AssetFileInfo.Create( // 使用用户输入创建 AssetFileInfo（基础信息）
            result.File.file, result.PathId, result.TypeId, result.ScriptIndex,
            Workspace.Manager.ClassDatabase, false
        );
        var info = new AssetInst(result.File, baseInfo); // 使用创建的 AssetFileInfo 构造 AssetInst
        var baseField = ValueBuilder.DefaultValueFieldFromTemplate(result.TempField); // 从模板字段构建默认的 BaseField

        result.File.file.Metadata.AddAssetInfo(info); // 将新 AssetInst 添加到文件的 Metadata.AssetInfos 中
        info.UpdateAssetDataAndRow(Workspace, baseField); // 使用 baseField 更新资产数据并在 UI 表格中添加行
    }

    public async void RemoveAsset() // 公共异步方法：移除选中资产（RemoveAsset）
    {
        if (SelectedItems.Count == 0) // 如果没有选中项
            return; // 返回

        var singPlurStr = SelectedItems.Count > 1 // 根据数量选择单复数描述（用于提示文本）
            ? "these assets"
            : "this asset";

        var dialogRes = await MessageBoxUtil.ShowDialog( // 弹出确认对话框询问是否删除（英文原文）
            "Remove asset",
            $"Are you sure you want to remove {singPlurStr}? Remaining references to {singPlurStr} will not be fixed.",
            MessageBoxType.YesNo
        );
        if (dialogRes == MessageBoxResult.No) // 如果用户选择否
            return; // 返回

        var modifiedFileInsts = new HashSet<AssetsFileInstance>(); // 集合：记录被修改的文件实例
        foreach (var selectedItem in SelectedItems) // 遍历每个选中项
        {
            var assetFileInst = selectedItem.FileInstance; // 获取所属文件实例
            assetFileInst.file.Metadata.RemoveAssetInfo(selectedItem); // 从文件的 Metadata 中移除该 AssetInfo
            modifiedFileInsts.Add(assetFileInst); // 将文件实例加入修改集合
        }

        foreach (var modifiedFileInst in modifiedFileInsts) // 遍历所有被修改的文件实例
        {
            var wsItem = Workspace.FindWorkspaceItemByInstance(modifiedFileInst); // 查找对应的 WorkspaceItem
            if (wsItem is not null) // 如果找到了
            {
                Workspace.Dirty(wsItem); // 标记该 WorkspaceItem 为已修改
            }
        }
    }

    public async void TransferAnimationClip() // 公共异步方法：将选中的动画剪辑转移到目标文件（TransferAnimationClip）
    {
        // Filter selected items to only AnimationClips
        var animClips = SelectedItems
            .Where(a => a.Type == AssetsTools.NET.Extra.AssetClassID.AnimationClip)
            .ToList();

        if (animClips.Count == 0)
        {
            await MessageBoxUtil.ShowDialog(
                "转移动画（Transfer AnimationClip）",
                "请先选择一个或多个 AnimationClip 资产。\n(Please select one or more AnimationClip assets first.)",
                MessageBoxType.OK
            );
            return;
        }

        // Get the source file instance from the first selected clip
        var sourceFileInst = animClips[0].FileInstance;

        // Walk the ENTIRE Workspace tree to find ALL AssetsFileInstance objects
        var allFileInsts = new List<AssetsFileInstance>();
        var queue = new Queue<WorkspaceItem>(Workspace.RootItems);
        while (queue.Count > 0)
        {
            var item = queue.Dequeue();
            if (item.Object is AssetsFileInstance fileInst)
            {
                allFileInsts.Add(fileInst);
            }
            foreach (var child in item.Children)
            {
                queue.Enqueue(child);
            }
        }

        // Filter out the source file
        var otherFiles = allFileInsts.Where(f => f != sourceFileInst).ToList();

        if (otherFiles.Count == 0)
        {
            await MessageBoxUtil.ShowDialog(
                "转移动画（Transfer AnimationClip）",
                "没有其他已打开的文件可作为目标。请先打开目标 VRCA 文件。\n(No other open files available as target. Please open the target VRCA file first.)",
                MessageBoxType.OK
            );
            return;
        }

        // Show selection window
        AssetsFileInstance? targetFileInst = null;

        var selWindow = new Avalonia.Controls.Window
        {
            Title = "选择目标文件（Select Target File）",
            Width = 500,
            Height = 350,
            WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
        };

        var stackPanel = new Avalonia.Controls.StackPanel { Margin = new Avalonia.Thickness(10) };
        stackPanel.Children.Add(new Avalonia.Controls.TextBlock
        {
            Text = $"请选择要将 {animClips.Count} 个动画转移到的目标文件：\n(Select target file to transfer {animClips.Count} animation(s) to:)",
            Margin = new Avalonia.Thickness(0, 0, 0, 10),
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        });

        var listBox = new Avalonia.Controls.ListBox
        {
            Height = 200,
            ItemsSource = otherFiles.Select(f => f.name ?? f.path ?? "(unknown)").ToList()
        };
        listBox.SelectedIndex = 0;
        stackPanel.Children.Add(listBox);

        var btnPanel = new Avalonia.Controls.StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            Margin = new Avalonia.Thickness(0, 10, 0, 0),
            Spacing = 8
        };

        var okBtn = new Avalonia.Controls.Button { Content = "确定 (OK)", Width = 80 };
        var cancelBtn = new Avalonia.Controls.Button { Content = "取消 (Cancel)", Width = 80 };

        okBtn.Click += (s, e) =>
        {
            int idx = listBox.SelectedIndex;
            if (idx >= 0 && idx < otherFiles.Count)
                targetFileInst = otherFiles[idx];
            selWindow.Close();
        };
        cancelBtn.Click += (s, e) => selWindow.Close();

        btnPanel.Children.Add(okBtn);
        btnPanel.Children.Add(cancelBtn);
        stackPanel.Children.Add(btnPanel);
        selWindow.Content = stackPanel;

        var mainWindow = Avalonia.Application.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow : null;

        if (mainWindow != null)
            await selWindow.ShowDialog(mainWindow);
        else
            await selWindow.ShowDialog(selWindow); // fallback

        if (targetFileInst == null)
            return;

        // Perform the transfer
        try
        {
            var copier = new Logic.Hierarchy.AnimationClipCopier(Workspace, sourceFileInst, targetFileInst);
            var clipPathIds = animClips.Select(a => a.PathId).ToList();
            var result = copier.TransferAnimationClips(clipPathIds);

            // Build detailed diagnostic report
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== CRC-32 对比报告 (CRC-32 Comparison Report) ===");
            sb.AppendLine($"源文件 (Source A) 路径数: {result.SourcePathCount}");
            sb.AppendLine($"目标文件 (Target B) 路径数: {result.TargetPathCount}");
            sb.AppendLine($"动画引用的总绑定哈希数: {result.TotalBindingHashes}");
            sb.AppendLine();

            sb.AppendLine($"--- ✅ B文件已存在 (Matched in B): {result.MatchedInTarget.Count} ---");
            foreach (var m in result.MatchedInTarget)
                sb.AppendLine($"  {m}");
            sb.AppendLine();

            sb.AppendLine($"--- ❌ B文件缺失 (Missing from B): {result.MissingFromTarget.Count} ---");
            foreach (var m in result.MissingFromTarget)
                sb.AppendLine($"  {m}");
            sb.AppendLine();

            sb.AppendLine($"--- ➕ 已创建到B (Created in B): {result.CreatedInTarget.Count} ---");
            foreach (var c in result.CreatedInTarget)
                sb.AppendLine($"  {c}");
            sb.AppendLine();

            if (result.NotFoundInSource.Count > 0)
            {
                sb.AppendLine($"--- ⚠ A和B都没有 (Not in A or B): {result.NotFoundInSource.Count} ---");
                foreach (var n in result.NotFoundInSource)
                    sb.AppendLine($"  {n}");
                sb.AppendLine();
            }

            if (result.Errors.Count > 0)
            {
                sb.AppendLine($"--- 🔴 错误 (Errors): {result.Errors.Count} ---");
                foreach (var e in result.Errors)
                    sb.AppendLine($"  {e}");
                sb.AppendLine();
            }

            sb.AppendLine($"=== 结果: 转移了 {result.ClipsTransferred} 个动画剪辑 ===");

            // Show scrollable diagnostic window
            var diagWindow = new Avalonia.Controls.Window
            {
                Title = $"转移报告 (Transfer Report) - {result.ClipsTransferred} clips",
                Width = 700,
                Height = 500,
                WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
            };
            var textBox = new Avalonia.Controls.TextBox
            {
                Text = sb.ToString(),
                IsReadOnly = true,
                AcceptsReturn = true,
                TextWrapping = Avalonia.Media.TextWrapping.NoWrap,
                FontFamily = new Avalonia.Media.FontFamily("Consolas, Courier New, monospace"),
                FontSize = 12,
                Margin = new Avalonia.Thickness(5)
            };
            var scroll = new Avalonia.Controls.ScrollViewer
            {
                Content = textBox,
                HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto
            };
            diagWindow.Content = scroll;

            var diagMainWindow = Avalonia.Application.Current?.ApplicationLifetime is
                Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime d2
                ? d2.MainWindow : null;
            if (diagMainWindow != null)
                await diagWindow.ShowDialog(diagMainWindow);
            else
                await diagWindow.ShowDialog(diagWindow);
        }
        catch (Exception ex)
        {
            await MessageBoxUtil.ShowDialog(
                "转移失败（Transfer Failed）",
                $"转移过程中出错：{ex.Message}\n(Error during transfer: {ex.Message})",
                MessageBoxType.OK
            );
        }
    }

    public async void SetTypeFilter() // 公共异步方法：设置类型过滤器（SetTypeFilter）
    {
        var dialogService = Ioc.Default.GetRequiredService<IDialogService>(); // 获取对话框服务

        // if not generated already, find all unique types to filter on
        // 注释：如果尚未生成过滤类型列表，则从 Workspace 与 Items 中生成
        _filterTypes ??= SelectTypeFilterViewModel.MakeTypeFilterTypes(Workspace, Items); // 懒加载 _filterTypes

        var result = await dialogService.ShowDialog(new SelectTypeFilterViewModel(_filterTypes)); // 弹出类型过滤选择对话框并等待结果
        if (result == null) // 如果用户取消
            return; // 返回

        // set filtered list
        _filterTypesFiltered = result.ToHashSet(); // 将用户选择的过滤类型转换为 HashSet 存储

        // also generate a list of file instance + script index -> actual script for quick lookup
        // 注释：如果类型引用查找表为空，则为每个文件构建脚本引用数组以便快速查找
        if (_typeRefLookup.Count == 0)
        {
            foreach (var fileInst in FileInsts) // 遍历当前文件实例列表
            {
                var scriptTypes = fileInst.file.Metadata.ScriptTypes; // 获取文件的 ScriptTypes 列表
                var scriptRefArray = new AssetTypeReference?[scriptTypes.Count]; // 为该文件分配脚本引用数组

                for (int i = 0; i < scriptTypes.Count; i++) // 遍历每个脚本类型索引
                {
                    AssetTypeReference typeRef = AssetHelper.GetAssetsFileScriptInfo(Workspace.Manager, fileInst, i); // 获取脚本引用信息
                    if (typeRef == null) // 如果未找到
                    {
                        scriptRefArray[i] = null; // 设为 null
                        continue; // 继续下一个
                    }

                    scriptRefArray[i] = typeRef; // 否则保存引用
                }

                _typeRefLookup[fileInst] = scriptRefArray; // 将该文件的脚本引用数组加入查找表
            }
        }

        // reload filter
        CollectionView.Filter = SetDataGridFilter(SearchText); // 重新设置 CollectionView 的过滤器以应用新的类型过滤
    }

    public void SetSelectedItems(List<AssetInst> assets)
    {
        if (assets.Count > 0 && !string.IsNullOrEmpty(SearchText))
        {
            SearchText = ""; // Use property to avoid MVVMTK0034 error/warning
            CollectionView.Filter = SetDataGridFilter(""); // Update filter immediately so assets are visible
        }
        SetSelectedItemsAction?.Invoke(assets);
    }

    public void OnAssetOpened(List<AssetInst> assets) // 公共方法：当资产在新标签打开时调用（OnAssetOpened）
    {
        if (assets.Count > 0) // 如果至少有一个资产
        {
            WeakReferenceMessenger.Default.Send(new AssetsSelectedMessage([assets[0]])); // 发送 AssetsSelectedMessage，选中第一个资产（消息总线）
        }

        SelectedItems = assets; // 将 SelectedItems 设置为传入的资产列表

        IsAnimationClipSelected = assets.Any(a => a.Type == AssetClassID.AnimationClip);
        IsSMRSelected = assets.Count == 1 && assets[0].Type == AssetClassID.SkinnedMeshRenderer;
    }

    [RelayCommand]
    public async Task SMRReferenceSearch()
    {
        if (SelectedItems.Count != 1 || SelectedItems[0].Type != AssetClassID.SkinnedMeshRenderer)
            return;

        var dialogService = Ioc.Default.GetRequiredService<IDialogService>();
        await dialogService.ShowDialog(new SMRReferenceSearchViewModel(Workspace, SelectedItems[0]));
    }

    public void ResendSelectedAssetsSelected() // 公共方法：重新发送当前选中资产的消息（ResendSelectedAssetsSelected）
    {
        if (SelectedItems.Count > 0) // 如果有选中项
        {
            WeakReferenceMessenger.Default.Send(new AssetsSelectedMessage([SelectedItems[0]])); // 重新发送 AssetsSelectedMessage（第一个选中项）
        }
    }

    private async Task OnWorkspaceClosing(object recipient, WorkspaceClosingMessage message) // 私有异步方法：处理工作区关闭消息（OnWorkspaceClosing）
    {
        await Load([]); // 在工作区关闭时清空当前文档（调用 Load 传入空列表）
    }

    // ====== Folder View (Unity-Style) ======

    [ObservableProperty]
    public bool _isFolderViewMode = false; // 是否处于文件夹视图模式

    [ObservableProperty]
    public ObservableCollection<TypeFolderItem> _typeFolders = new(); // 类型文件夹列表

    [ObservableProperty]
    public string? _currentFolderType = null; // 当前打开的文件夹类型名

    [ObservableProperty]
    public ObservableCollection<AssetInst> _currentFolderAssets = new(); // 当前文件夹内的资产列表

    partial void OnIsFolderViewModeChanged(bool value) // 当文件夹视图模式变化时触发
    {
        if (value)
        {
            CurrentFolderType = null;
            BuildTypeFolders();
        }
    }

    public void OpenFolder(string typeName) // 打开指定类型的文件夹
    {
        CurrentFolderType = typeName;
        CurrentFolderAssets.Clear();
        foreach (var item in Items)
        {
            var typeStr = ClassIdToString.TryGetValue(item.Type, out var name) ? name : item.Type.ToString();
            if (typeStr == typeName)
                CurrentFolderAssets.Add(item);
        }
    }

    public void GoBackToFolders() // 返回文件夹总览
    {
        CurrentFolderType = null;
        CurrentFolderAssets.Clear();
    }

    private void BuildTypeFolders() // 构建类型文件夹列表
    {
        TypeFolders.Clear();
        var groupCounts = new Dictionary<string, int>();
        foreach (var item in Items)
        {
            var typeStr = ClassIdToString.TryGetValue(item.Type, out var name) ? name : item.Type.ToString();
            if (!groupCounts.ContainsKey(typeStr))
                groupCounts[typeStr] = 0;
            groupCounts[typeStr]++;
        }
        foreach (var kv in groupCounts.OrderBy(k => k.Key))
        {
            TypeFolders.Add(new TypeFolderItem { TypeName = kv.Key, Count = kv.Value });
        }
    }
} // 类 AssetDocumentViewModel 结束

// todo: move all classes to new namespace
// 注释：TODO 提示，建议将下面的类移动到新的命名空间

public class PluginItemInfo // 公共类：表示插件菜单项信息（PluginItemInfo）
{
    public string Name { get; } // 只读属性：插件项显示名称（Name）

    private IUavPluginOption? _option; // 私有字段：对应的插件选项接口（可能为 null）
    private AssetDocumentViewModel _docViewModel; // 私有字段：持有父文档视图模型引用以便回调

    public PluginItemInfo(string name, IUavPluginOption? option, AssetDocumentViewModel docViewModel) // 构造函数：初始化 PluginItemInfo
    {
        Name = name; // 设置 Name
        _option = option; // 保存插件选项引用
        _docViewModel = docViewModel; // 保存文档视图模型引用
    }

    public async Task Execute(object selectedItems) // 异步方法：执行插件操作（Execute）
    {
        if (_option != null) // 如果有插件选项
        {
            var workspace = _docViewModel.Workspace; // 获取文档的 Workspace
            var res = await _option.Execute(workspace, new UavPluginFunctions(), _option.Options, (List<AssetInst>)selectedItems); // 调用插件的 Execute 方法并等待结果
            if (res) // 如果插件执行成功（返回 true）
            {
                _docViewModel.ResendSelectedAssetsSelected(); // 重新发送选中资产消息以刷新 UI 或状态
            }
        }
    }

    public override string ToString() // 重写 ToString 以便在 UI 中显示 Name
    {
        return Name; // 返回 Name 字符串
    }
}

public enum AssetTextSearchKind // 枚举：表示文本搜索的两种模式（AssetTextSearchKind）
{
    PlainSearch, // 普通文本搜索（PlainSearch）
    RegexSearch, // 正则表达式搜索（RegexSearch）
    PathId, // Path ID 搜索（PathId）
    Date // Date数据搜索（Date Data Search）
}

public class TypeFolderItem // 类型文件夹项（用于 Unity 风格文件夹视图）
{
    public string TypeName { get; set; } = "";
    public int Count { get; set; }
}
