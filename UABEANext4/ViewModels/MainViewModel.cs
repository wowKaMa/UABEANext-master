using AssetsTools.NET;
using AssetsTools.NET.Extra; // 引用 AssetsTools.NET.Extra 库，用于处理 Unity 资产文件的扩展功能（保持原名 AssetsTools.NET.Extra）

using Avalonia.Platform; // 引用 Avalonia 平台相关命名空间，提供跨平台 UI 平台抽象（Avalonia.Platform）

using Avalonia.Platform.Storage; // 引用 Avalonia 的存储接口，用于文件/文件夹选择对话框（Avalonia.Platform.Storage）

using CommunityToolkit.Mvvm.ComponentModel; // 引用 CommunityToolkit.Mvvm 的组件模型，提供 ObservableObject 等 MVVM 基类（CommunityToolkit.Mvvm.ComponentModel）

using CommunityToolkit.Mvvm.DependencyInjection; // 引用 CommunityToolkit 的依赖注入支持（Ioc.Default）（CommunityToolkit.Mvvm.DependencyInjection）

using CommunityToolkit.Mvvm.Messaging; // 引用 CommunityToolkit 的消息总线（WeakReferenceMessenger）（CommunityToolkit.Mvvm.Messaging）

using Dock.Model.Controls; // 引用 Dock.Model 控件接口，提供停靠窗口相关类型（Dock.Model.Controls）

using Dock.Model.Core; // 引用 Dock.Model 核心接口（IDockable、IDock 等）（Dock.Model.Core）

using Dock.Model.Core.Events; // 引用 Dock.Model 的事件类型（DockableAddedEventArgs 等）（Dock.Model.Core.Events）

using Dock.Model.Mvvm.Controls; // 引用 Dock.Model 的 MVVM 控件支持（Document、Tool 等）（Dock.Model.Mvvm.Controls）

using DynamicData; // 引用 DynamicData 库，用于响应式集合处理（DynamicData）

using System; // 引用基础系统命名空间，包含基本类型和工具（System）

using System.Collections.Concurrent; // 引用并发集合命名空间，提供 Partitioner 等（System.Collections.Concurrent）

using System.Collections.Generic; // 引用泛型集合命名空间，提供 List、HashSet 等（System.Collections.Generic）

using System.IO; // 引用 IO 操作命名空间，用于文件和目录操作（System.IO）

using System.Linq; // 引用 LINQ 扩展方法，用于集合查询和转换（System.Linq）

using System.Threading; // 引用线程与同步原语（Mutex、Interlocked 等）（System.Threading）

using System.Threading.Tasks; // 引用异步任务支持（Task、async/await）（System.Threading.Tasks）

using UABEANext4.AssetWorkspace; // 引用项目的资产工作区命名空间，包含 Workspace、WorkspaceItem 等（UABEANext4.AssetWorkspace）

using UABEANext4.Logic; // 引用项目逻辑层命名空间（UABEANext4.Logic）

using UABEANext4.Services; // 引用项目服务层命名空间（StorageService、IDialogService 等）（UABEANext4.Services）

using UABEANext4.Util; // 引用项目工具类命名空间（FileDialogUtils、MessageBoxUtil 等）（UABEANext4.Util）

using UABEANext4.ViewModels.Dialogs; // 引用对话框视图模型命名空间（VersionSelectViewModel 等）（UABEANext4.ViewModels.Dialogs）

using UABEANext4.ViewModels.Documents; // 引用文档视图模型命名空间（AssetDocumentViewModel 等）（UABEANext4.ViewModels.Documents）

using UABEANext4.ViewModels.Tools; // 引用工具视图模型命名空间（InspectorToolViewModel 等）（UABEANext4.ViewModels.Tools）

// 定义命名空间 UABEANext4.ViewModels，用于组织视图模型类（namespace UABEANext4.ViewModels）
namespace UABEANext4.ViewModels;

public partial class MainViewModel : ViewModelBase // 定义部分类 MainViewModel，继承自 ViewModelBase（MainViewModel : ViewModelBase），作为应用的主视图模型
{
    [ObservableProperty] // 特性：自动生成属性与通知（CommunityToolkit.Mvvm 的 ObservableProperty）
    public IRootDock? _layout; // 字段：保存 Dock 布局根对象（IRootDock），用于 UI 布局管理（_layout）

    [ObservableProperty] // 特性：自动生成属性与通知
    public bool _dockWorkspaceExplorerVisible = true; // 字段：控制 WorkspaceExplorer 工具是否可见（默认 true）（_dockWorkspaceExplorerVisible）

    [ObservableProperty] // 特性：自动生成属性与通知
    public bool _dockHierarchyVisible = true; // 字段：控制 Hierarchy 工具是否可见（默认 true）（_dockHierarchyVisible）

    [ObservableProperty] // 特性：自动生成属性与通知
    public bool _dockInspectorVisible = true; // 字段：控制 Inspector 工具是否可见（默认 true）（_dockInspectorVisible）

    [ObservableProperty] // 特性：自动生成属性与通知
    public bool _dockPreviewerVisible = true; // 字段：控制 Previewer 工具是否可见（默认 true）（_dockPreviewerVisible）


    [ObservableProperty] // 特性：自动生成属性与通知
    public bool _dockUnityComponentsVisible = true; // 字段：控制 Previewer 工具是否可见（默认 true）（_dockPreviewerVisible）

    [ObservableProperty] // 特性：自动生成属性与通知
    public bool _loadContainers = false; // 字段：控制是否加载容器（Bundle）内容（默认 false）（_loadContainers）

    public Workspace Workspace { get; } // 属性：只读的 Workspace 实例，管理已加载的文件与资产（Workspace）

    public bool UsesChrome => OperatingSystem.IsWindows(); // 只读属性：判断是否在 Windows 平台运行（UsesChrome），用于决定窗口装饰策略

    public ExtendClientAreaChromeHints ChromeHints => UsesChrome // 只读属性：根据 UsesChrome 返回扩展客户端区域的提示（ChromeHints）
        ? ExtendClientAreaChromeHints.PreferSystemChrome // 如果 UsesChrome 为 true，则偏好系统 Chrome（PreferSystemChrome）
        : ExtendClientAreaChromeHints.Default; // 否则使用默认提示（Default）

    private readonly MainDockFactory _factory; // 私有只读字段：MainDockFactory 实例，用于创建和管理 Dock 布局（_factory）

    private List<AssetsFileInstance> _lastLoadedFiles = []; // 私有字段：保存最近加载的 AssetsFileInstance 列表（_lastLoadedFiles）

    public MainViewModel() // 构造函数：初始化 MainViewModel 的实例
    {
        Workspace = new(); // 创建并赋值 Workspace（Workspace = new()）

        _factory = new MainDockFactory(Workspace); // 使用 Workspace 创建 MainDockFactory（_factory = new MainDockFactory(Workspace)）

        Layout = _factory.CreateLayout(); // 使用工厂创建布局并赋值给 Layout（Layout = _factory.CreateLayout()）

        if (Layout is not null) // 检查 Layout 是否为 null（如果不为 null 则初始化）
        {
            _factory.InitLayout(Layout); // 使用工厂初始化布局（_factory.InitLayout(Layout)）
        }

        WeakReferenceMessenger.Default.Register<SelectedWorkspaceItemChangedMessage>(this, (r, h) => _ = OnSelectedWorkspaceItemsChanged(r, h)); // 注册消息：当选中工作区项改变时调用 OnSelectedWorkspaceItemsChanged（WeakReferenceMessenger 注册）

        WeakReferenceMessenger.Default.Register<RequestEditAssetMessage>(this, OnRequestEditAsset); // 注册消息：当请求编辑资产时调用 OnRequestEditAsset

        WeakReferenceMessenger.Default.Register<RequestVisitAssetMessage>(this, (r, h) => _ = OnRequestVisitAsset(r, h)); // 注册消息：当请求访问资产时调用 OnRequestVisitAsset

        _factory.DockableAdded += FactoryDockableAdded; // 订阅工厂事件：当 Dockable 被添加时触发 FactoryDockableAdded

        _factory.DockableClosed += FactoryDockableClosed; // 订阅工厂事件：当 Dockable 被关闭时触发 FactoryDockableClosed

        _factory.FocusedDockableChanged += FactoryDockableFocused; // 订阅工厂事件：当焦点 Dockable 改变时触发 FactoryDockableFocused
    }

    // todo: split out
    // 注释：TODO 提示，表示该类可能需要拆分以减少复杂度

    #region Dockable toggles // 区域开始：与 Dockable 显示/隐藏相关的方法
    private void FactoryDockableAdded(object? sender, DockableAddedEventArgs e) // 方法：处理 Dockable 添加事件（FactoryDockableAdded）
    {
        if (e.Dockable is not IDockable dockable) // 如果事件中的 Dockable 不是 IDockable，则直接返回
            return; // 退出方法

        switch (dockable.Id) // 根据 dockable 的 Id 字符串判断是哪一个工具被添加
        {
            case "WorkspaceExplorer": DockWorkspaceExplorerVisible = true; break; // 如果 Id 为 "WorkspaceExplorer"，设置 DockWorkspaceExplorerVisible 为 true

            case "Hierarchy": DockHierarchyVisible = true; break; // 如果 Id 为 "Hierarchy"，设置 DockHierarchyVisible 为 true

            case "Inspector": DockInspectorVisible = true; break; // 如果 Id 为 "Inspector"，设置 DockInspectorVisible 为 true

            case "Previewer": DockPreviewerVisible = true; break; // 如果 Id 为 "Previewer"，设置 DockPreviewerVisible 为 true

            case "UnityComponents": DockUnityComponentsVisible = true; break; // 如果 Id 为 "UnityComponents"，设置 DockPreviewerVisible 为 true
        }
    }

    private void FactoryDockableClosed(object? sender, DockableClosedEventArgs e) // 方法：处理 Dockable 关闭事件（FactoryDockableClosed）
    {
        if (e.Dockable is not IDockable dockable) // 如果事件中的 Dockable 不是 IDockable，则直接返回
            return; // 退出方法

        switch (dockable.Id) // 根据 dockable 的 Id 判断是哪一个工具被关闭
        {
            case "WorkspaceExplorer": DockWorkspaceExplorerVisible = false; break; // 如果 Id 为 "WorkspaceExplorer"，设置 DockWorkspaceExplorerVisible 为 false

            case "Hierarchy": DockHierarchyVisible = false; break; // 如果 Id 为 "Hierarchy"，设置 DockHierarchyVisible 为 false

            case "Inspector": DockInspectorVisible = false; break; // 如果 Id 为 "Inspector"，设置 DockInspectorVisible 为 false

            case "Previewer": DockPreviewerVisible = false; break; // 如果 Id 为 "Previewer"，设置 DockPreviewerVisible 为 false

            case "UnityComponents": DockUnityComponentsVisible = true; break; // 如果 Id 为 "UnityComponents"，设置 DockPreviewerVisible 为 false
        }
    }

    private void FactoryDockableFocused(object? sender, FocusedDockableChangedEventArgs e) // 方法：处理焦点 Dockable 改变事件（FactoryDockableFocused）
    {
        if (e.Dockable is Document document) // 如果焦点对象是 Document（Dock.Model.Mvvm.Controls.Document）
            _factory.DocMan.LastFocusedDocument = document; // 将工厂的 DocMan.LastFocusedDocument 更新为当前 document
    }

    partial void OnDockWorkspaceExplorerVisibleChanged(bool value) // 部分方法：当 DockWorkspaceExplorerVisible 属性改变时被调用（由 ObservableProperty 生成）
    {
        var explorer = _factory.GetDockable<WorkspaceExplorerToolViewModel>("WorkspaceExplorer"); // 从工厂获取 Id 为 "WorkspaceExplorer" 的工具视图模型

        if (explorer is null || Layout is null) // 如果 explorer 或 Layout 为 null，则不继续
            return; // 退出方法

        ShowHideDockable(explorer, value); // 调用 ShowHideDockable 来显示或隐藏该工具（ShowHideDockable）
    }

    partial void OnDockHierarchyVisibleChanged(bool value) // 部分方法：当 DockHierarchyVisible 属性改变时被调用
    {
        var hierarchy = _factory.GetDockable<HierarchyToolViewModel>("Hierarchy"); // 获取 Id 为 "Hierarchy" 的工具视图模型

        if (hierarchy is null || Layout is null) // 如果 hierarchy 或 Layout 为 null，则返回
            return; // 退出方法

        ShowHideDockable(hierarchy, value); // 显示或隐藏 hierarchy
    }

    partial void OnDockInspectorVisibleChanged(bool value) // 部分方法：当 DockInspectorVisible 属性改变时被调用
    {
        var inspector = _factory.GetDockable<InspectorToolViewModel>("Inspector"); // 获取 Id 为 "Inspector" 的工具视图模型

        if (inspector is null || Layout is null) // 如果 inspector 或 Layout 为 null，则返回
            return; // 退出方法

        ShowHideDockable(inspector, value); // 显示或隐藏 inspector
    }

    partial void OnDockPreviewerVisibleChanged(bool value) // 部分方法：当 DockPreviewerVisible 属性改变时被调用
    {
        var previewer = _factory.GetDockable<PreviewerToolViewModel>("Previewer"); // 获取 Id 为 "Previewer" 的工具视图模型

        if (previewer is null || Layout is null) // 如果 previewer 或 Layout 为 null，则返回
            return; // 退出方法

        ShowHideDockable(previewer, value); // 显示或隐藏 previewer
    }
    partial void OnDockUnityComponentsVisibleChanged(bool value) // 部分方法：当 DockUnityVisible 属性改变时被调用
    {
        var previewer = _factory.GetDockable<UnityToolComponentsViewModel>("UnityComponents"); // 获取 Id 为 "Previewer" 的工具视图模型

        if (previewer is null || Layout is null) // 如果 previewer 或 Layout 为 null，则返回
            return; // 退出方法

        ShowHideDockable(previewer, value); // 显示或隐藏 previewer
    }

    private void ShowHideDockable(IDockable dockable, bool show) // 方法：根据 show 参数显示或隐藏指定的 dockable（ShowHideDockable）
    {
        if (show) // 如果需要显示
        {
            if (dockable.Owner is IDock dock && HasPathToRoot(dock)) // 如果 dockable 有 Owner 且 Owner 是 IDock，并且从该 dock 到根有可见路径
            {
                _factory.AddDockable(dock, dockable); // 将 dockable 添加到该 dock（AddDockable）
                _factory.SetActiveDockable(dockable); // 设置该 dockable 为活动（SetActiveDockable）
                _factory.SetFocusedDockable(dock, dockable); // 设置该 dock 的焦点 dockable（SetFocusedDockable）
            }
            else if (_factory.MainPane is not null) // 否则如果工厂有 MainPane（主面板）
            {
                _factory.AddDockable(_factory.MainPane, dockable); // 将 dockable 添加到 MainPane
                _factory.FloatDockable(dockable); // 将 dockable 设为浮动（FloatDockable），注释：暂时浮动显示
                _factory.SetActiveDockable(dockable); // 设置为活动 dockable
                _factory.SetFocusedDockable(_factory.MainPane, dockable); // 设置 MainPane 的焦点 dockable
            }
        }
        else // 如果需要隐藏
        {
            _factory.CloseDockable(dockable); // 关闭该 dockable（CloseDockable）
        }
    }

    // a very roundabout way to check if a dock is visible all the way to the root
    // 注释：下面的方法用于检查某个 dockable 是否从其所属 dock 一直到根布局都是可见的（HasPathToRoot）
    private bool HasPathToRoot(IDockable baseDockable) // 方法：检查从 baseDockable 到根布局是否存在可见路径（HasPathToRoot）
    {
        IDockable? dockable = baseDockable; // 从传入的 baseDockable 开始
        while (true) // 循环向上遍历 owner 链
        {
            if (dockable is null || dockable.Owner is null) // 如果当前 dockable 或其 Owner 为 null
                return false; // 返回 false，表示没有到根的路径

            if (dockable.Owner is not IDock parentDock) // 如果 Owner 不是 IDock 类型
                return false; // 返回 false

            if (parentDock.VisibleDockables is null || !parentDock.VisibleDockables.Contains(dockable)) // 如果 parentDock 的 VisibleDockables 为 null 或不包含当前 dockable
                return false; // 返回 false

            dockable = dockable.Owner; // 将 dockable 上移到其 Owner
            if (dockable == Layout) // 如果上移后到达根布局（Layout）
                return true; // 返回 true，表示存在到根的可见路径
        }
    }
    #endregion // 区域结束：Dockable toggles

    #region Menu items // 区域开始：菜单项（文件操作等）
    public async Task OpenFiles(IEnumerable<string?> paths) // 异步方法：打开一组路径（文件或文件夹）（OpenFiles）
    {
        var filePaths = new List<string>(); // 创建一个字符串列表用于存放实际要打开的文件路径（filePaths）

        foreach (var path in paths) // 遍历传入的路径集合
        {
            if (File.Exists(path)) // 如果路径是一个文件
                filePaths.Add(path); // 将文件路径加入 filePaths

            if (Directory.Exists(path)) // 如果路径是一个目录
                filePaths.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories)); // 将目录下所有文件（递归）加入 filePaths
        }

        int totalCount = filePaths.Count; // 记录总文件数（totalCount）

        if (totalCount == 0) // 如果没有文件要打开
        {
            return; // 直接返回
        }

        // ===== 两阶段流水线：阶段1=并行IO读取，阶段2=并行CPU解压 =====
        int maxThreads = Math.Max(1, Math.Min(Environment.ProcessorCount, totalCount));

        Workspace.SetProgressThreadSafe(0f, $"[阶段1/2] 正在并行读取 {totalCount} 个文件到内存 ({maxThreads} 线程)...");
        await Task.Run(() =>
        {
            Workspace.ModifyMutex.WaitOne();
            Workspace.ProgressValue = 0;
            int startLoadOrder = Workspace.NextLoadIndex;

            // ---- 阶段1: 并行将所有文件读入内存 (最大化磁盘IO吞吐量) ----
            var fileBuffers = new (string Name, byte[] Data, int Index)[totalCount];
            int readCount = 0;

            var readPartitioner = Partitioner.Create(0, totalCount);
            Parallel.ForEach(readPartitioner, new ParallelOptions { MaxDegreeOfParallelism = maxThreads }, range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var fileName = filePaths[i];
                    try
                    {
                        var data = File.ReadAllBytes(fileName);
                        fileBuffers[i] = (fileName, data, i);
                    }
                    catch
                    {
                        fileBuffers[i] = (fileName, Array.Empty<byte>(), i);
                    }
                    var cnt = Interlocked.Increment(ref readCount);
                    if (cnt % 20 == 0 || cnt == totalCount)
                        Workspace.SetProgressThreadSafe(cnt / (float)totalCount * 0.3f,
                            $"[阶段1/2] 读取文件 ({cnt}/{totalCount}) {Path.GetFileName(fileName)}");
                }
            });

            // ---- 阶段2: 并行解压和解析所有已读入内存的文件 (最大化CPU利用率) ----
            Workspace.SetProgressThreadSafe(0.3f, $"[阶段2/2] 正在并行解压 {totalCount} 个文件 ({maxThreads} 线程)...");
            int processCount = 0;

            var processPartitioner = Partitioner.Create(fileBuffers, EnumerablePartitionerOptions.NoBuffering);
            Parallel.ForEach(processPartitioner, new ParallelOptions { MaxDegreeOfParallelism = maxThreads }, item =>
            {
                if (item.Data.Length == 0)
                {
                    var cnt = Interlocked.Increment(ref processCount);
                    Workspace.SetProgressThreadSafe(0.3f + cnt / (float)totalCount * 0.7f,
                        $"[阶段2/2] 跳过 ({cnt}/{totalCount}) {Path.GetFileName(item.Name)}");
                    return;
                }

                MemoryStream? ms = null;
                try
                {
                    ms = new MemoryStream(item.Data);
                    var file = Workspace.LoadAnyFile(ms, startLoadOrder + item.Index, item.Name);
                    
                    var cnt = Interlocked.Increment(ref processCount);
                    Workspace.SetProgressThreadSafe(0.3f + cnt / (float)totalCount * 0.7f,
                        $"[阶段2/2] 解析完成 ({cnt}/{totalCount}) {Path.GetFileName(item.Name)}");
                }
                catch
                {
                    ms?.Dispose();
                    var cnt = Interlocked.Increment(ref processCount);
                    Workspace.SetProgressThreadSafe(0.3f + cnt / (float)totalCount * 0.7f,
                        $"[阶段2/2] 失败 ({cnt}/{totalCount}) {Path.GetFileName(item.Name)}");
                }
            });

            Workspace.SetProgressThreadSafe(1f, "Done"); // 所有文件处理完成后将进度设为 100% 并显示 "Done"
            Workspace.ModifyMutex.ReleaseMutex(); // 释放互斥锁
        });

        if (Workspace.Manager.ClassDatabase == null) // 如果 Workspace 的 ClassDatabase 为空（未加载类数据库）
        {
            var anySerializedItems = false; // 标记：是否存在序列化的资产（serialized items）

            foreach (var rootItem in Workspace.RootItems) // 遍历 Workspace 的根项（RootItems）
            {
                if (rootItem.ObjectType == WorkspaceItemType.AssetsFile) // 如果根项是 AssetsFile 类型
                {
                    anySerializedItems = true; // 标记为存在序列化项
                    break; // 跳出循环
                }
                else if (rootItem.ObjectType == WorkspaceItemType.BundleFile) // 否则如果根项是 BundleFile（容器）
                {
                    foreach (var childItem in rootItem.Children) // 遍历该容器的子项
                    {
                        if (rootItem.ObjectType == WorkspaceItemType.AssetsFile) // 注意：这里原代码可能有笔误（应检查 childItem.ObjectType），但按原样注释：如果 rootItem.ObjectType 为 AssetsFile
                        {
                            anySerializedItems = true; // 标记为存在序列化项
                            break; // 跳出子循环
                        }
                    }
                }

                if (anySerializedItems) // 如果已发现序列化项
                {
                    break; // 跳出根项循环
                }
            }

            if (anySerializedItems) // 如果发现了序列化项但 ClassDatabase 为空
            {
                var dialogService = Ioc.Default.GetRequiredService<IDialogService>(); // 通过依赖注入获取 IDialogService（对话框服务）
                var version = await dialogService.ShowDialog(new VersionSelectViewModel()); // 弹出版本选择对话框（VersionSelectViewModel），等待用户选择版本
                if (version != null) // 如果用户选择了版本（version 不为 null）
                {
                    Workspace.Manager.LoadClassDatabaseFromPackage(version); // 使用所选版本加载类数据库（LoadClassDatabaseFromPackage）
                }
            }
        }
    }

    public async void FileOpen() // 方法：通过文件选择器打开文件（FileOpen）
    {
        var storageProvider = StorageService.GetStorageProvider(); // 获取平台存储提供者（StorageService.GetStorageProvider）
        if (storageProvider is null) // 如果没有可用的存储提供者
        {
            return; // 返回，不执行打开操作
        }

        var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions // 调用平台的文件选择器（OpenFilePickerAsync），传入选项
        {
            Title = "Open a file", // 对话框标题（"Open a file"），UI 显示英文原文
            FileTypeFilter = [ // 文件类型过滤器（注意：此处语法为原代码风格，表示一个包含 FilePickerFileType 的集合）
                new FilePickerFileType("All files (*.*)") { Patterns = [ "*" ] } // 允许所有文件（"All files (*.*)"），模式为 "*"
            ],
            AllowMultiple = true // 允许多选
        });

        var fileNames = FileDialogUtils.GetOpenFileDialogFiles(result); // 使用工具类解析选择结果并获取文件路径列表（FileDialogUtils.GetOpenFileDialogFiles）
        await OpenFiles(fileNames); // 调用 OpenFiles 打开这些文件
    }

    public async void FileOpenFolder() // 方法：通过文件夹选择器打开文件夹（FileOpenFolder）
    {
        var storageProvider = StorageService.GetStorageProvider(); // 获取存储提供者
        if (storageProvider is null) // 如果没有提供者
        {
            return; // 返回
        }

        var result = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions // 调用平台的文件夹选择器（OpenFolderPickerAsync）
        {
            Title = "Open a folder", // 对话框标题（"Open a folder"）
            AllowMultiple = true // 允许多选文件夹
        });

        var folderNames = FileDialogUtils.GetOpenFolderDialogFolders(result); // 解析选择结果并获取文件夹路径列表（FileDialogUtils.GetOpenFolderDialogFolders）
        await OpenFiles(folderNames); // 将文件夹路径传给 OpenFiles（OpenFiles 会递归获取文件）
    }

    private async Task DoSaveOverwrite(IEnumerable<WorkspaceItem> items, BundleSaveMethod? saveMethod = null) // 私有异步方法：覆盖保存给定的 WorkspaceItem 集合（DoSaveOverwrite）

    {
        Workspace.ModifyMutex.WaitOne(); // 获取 Workspace 的互斥锁，确保保存操作的线程安全
        try // 使用 try/finally 确保最后释放互斥锁
        {
            var rootItems = new HashSet<WorkspaceItem>(); // 创建 HashSet 用于收集每个项的根项（rootItems）

            foreach (var item in items) // 遍历传入的 items
            {
                var rootItem = item; // 从当前项开始向上查找根项
                while (rootItem.Parent != null) // 当存在父项时
                {
                    rootItem = rootItem.Parent; // 将 rootItem 上移到父项
                }
                rootItems.Add(rootItem); // 将找到的根项加入集合（避免重复保存同一根）
            }

            var fileInstsToReload = new HashSet<AssetsFileInstance>(); // 创建集合用于记录需要重新加载的 AssetsFileInstance
            var someFailed = false; // 标记是否有保存失败的情况

            foreach (var item in rootItems) // 遍历每个根项并尝试保存
            {
                var (saved, failed) = await Workspace.Save(item, saveMethod); // 调用 Workspace.Save 返回 (saved, failed) 元组

                if (failed) // 如果保存失败
                {
                    someFailed = true; // 标记有失败
                    continue; // 跳过后续处理
                }
                else if (!saved) // 如果没有保存（例如用户取消）
                {
                    continue; // 跳过
                }

                if (item.Object is AssetsFileInstance fileInst) // 如果根项的 Object 是 AssetsFileInstance
                {
                    fileInstsToReload.Add(fileInst); // 将该文件实例加入需要重载集合
                }
                else if (item.Object is BundleFileInstance) // 否则如果根项是 BundleFileInstance（容器）
                {
                    foreach (var child in item.Children) // 遍历容器的子项
                    {
                        if (child.Object is AssetsFileInstance childInst) // 如果子项的 Object 是 AssetsFileInstance
                        {
                            fileInstsToReload.Add(childInst); // 将子文件实例加入重载集合
                        }
                    }
                }
            }

            if (fileInstsToReload.Count == 0) // 如果没有需要重载的文件
            {
                if (someFailed) // 如果有失败
                    Workspace.SetProgressThreadSafe(1f, "All files failed to save (check if you have write access?)"); // 更新进度并显示失败信息（英文原文）
                else
                    Workspace.SetProgressThreadSafe(1f, "No files open to save"); // 更新进度并显示没有文件可保存的信息（英文原文）
            }
            else // 如果有需要重载的文件
            {
                await ReloadAssetDocuments(fileInstsToReload); // 调用 ReloadAssetDocuments 重新加载受影响的文档
                if (someFailed) // 如果有失败
                    Workspace.SetProgressThreadSafe(1f, "Saved (some failed), with open saved files reloaded"); // 更新进度并显示部分失败信息（英文原文）
                else
                    Workspace.SetProgressThreadSafe(1f, "Saved, with open saved files reloaded"); // 更新进度并显示保存成功并重载的提示（英文原文）
            }
        }
        finally // finally 块确保互斥锁被释放
        {
            Workspace.ModifyMutex.ReleaseMutex(); // 释放互斥锁
        }
    }

    private async Task DoSaveCopy(IEnumerable<WorkspaceItem> items) // 私有异步方法：另存为（DoSaveCopy），对每个根项执行 SaveAs
    {
        Workspace.ModifyMutex.WaitOne(); // 获取互斥锁
        try // 确保释放互斥锁
        {
            var rootItems = new HashSet<WorkspaceItem>(); // 收集根项集合

            foreach (var item in items) // 遍历传入项
            {
                var rootItem = item; // 向上查找根项
                while (rootItem.Parent != null) // 当存在父项
                {
                    rootItem = rootItem.Parent; // 上移
                }
                rootItems.Add(rootItem); // 加入集合
            }

            foreach (var item in rootItems) // 对每个根项调用 SaveAs（另存为）
            {
                await Workspace.SaveAs(item); // 异步调用 Workspace.SaveAs
            }

            Workspace.SetProgressThreadSafe(1f, "Saved"); // 更新进度并显示 "Saved"
        }
        finally
        {
            Workspace.ModifyMutex.ReleaseMutex(); // 释放互斥锁
        }
    }

    public async Task FileSave() // 公共异步方法：保存当前选中的项（FileSave）
    {
        var explorer = _factory.GetDockable<WorkspaceExplorerToolViewModel>("WorkspaceExplorer"); // 获取 WorkspaceExplorer 工具视图模型
        if (explorer == null) // 如果 explorer 为 null
            return; // 返回

        var items = explorer.SelectedItems.Cast<WorkspaceItem>(); // 获取 explorer 的 SelectedItems 并转换为 WorkspaceItem 枚举
        await DoSaveOverwrite(items); // 调用 DoSaveOverwrite 覆盖保存这些项
    }

    // more like "save copy as"
    // 注释：下面的 FileSaveAs 更像是“另存为复制”的行为
    public async Task FileSaveAs() // 公共异步方法：另存为（FileSaveAs）
    {
        var explorer = _factory.GetDockable<WorkspaceExplorerToolViewModel>("WorkspaceExplorer"); // 获取 WorkspaceExplorer
        if (explorer == null) // 如果为 null
            return; // 返回

        var items = explorer.SelectedItems.Cast<WorkspaceItem>(); // 获取选中项
        await DoSaveCopy(items); // 调用 DoSaveCopy 执行另存为
    }

    public async Task FileSaveAll() // 公共异步方法：保存所有打开的文件（FileSaveAll）
    {
        // Check if there are unsaved bundle files
        bool hasUnsavedBundles = Workspace.UnsavedItems.Any(i => i.ObjectType == WorkspaceItemType.BundleFile);
        BundleSaveMethod? saveMethod = null;


        if (hasUnsavedBundles)
        {
            var dialogService = Ioc.Default.GetRequiredService<IDialogService>();
            var vm = new CompressionSelectionViewModel();
            saveMethod = await dialogService.ShowDialog(vm);

            
            // If user closed dialog without selecting (optional: can strictly return here)
            // But let's assume if null, it just cancels the special compression and falls back to default/uncompressed overwrite
            // Or maybe canceling the dialog cancels the save?
            // Usually canceling a modal dialog for an action should cancel the action.
            // But given the simple implementation, if null (closed), we can probably proceed with default (null) behavior
            // or return. Let's return if user specifically closed it without choice to avoid accidental uncompressed save.
            // However, Current implementation of DialogService returns null if closed via window chrome.
            // If the user presses "No Compression", it returns AssetBundleCompressionType.NONE (0).
            // So null means "Cancel".
            
            // Wait, my ViewModel calls RequestClose explicitly.
            // If I close window with X, result is null.
            // Let's treat null as "Cancel Save All".
            // But wait, what if only assets are unsaved? Then we shouldn't show dialog.
        }
        
        if (hasUnsavedBundles && saveMethod == null)

        {
            // User cancelled logic or closed window
            // Return to avoid saving if they closed the compression dialog
            return;
        }

        await DoSaveOverwrite(Workspace.RootItems, saveMethod); // 对 Workspace.RootItems 调用 DoSaveOverwrite（覆盖保存所有根项）

    }

    public async Task FileSaveAllAs() // 公共异步方法：另存为所有（FileSaveAllAs）
    {
        await DoSaveCopy(Workspace.RootItems); // 对 Workspace.RootItems 调用 DoSaveCopy（另存为所有根项）
    }

    public void FileCloseAll() // 公共方法：关闭所有文件与清理（FileCloseAll）
    {
        Workspace.CloseAll(); // 调用 Workspace.CloseAll 关闭所有打开的文件
        WeakReferenceMessenger.Default.Send(new WorkspaceClosingMessage()); // 发送 WorkspaceClosingMessage 通知其他组件工作区正在关闭

        var files = _factory.GetDockable<IDocumentDock>("Files"); // 获取 Id 为 "Files" 的文档停靠（IDocumentDock）
        if (files is not null && files.VisibleDockables != null && files.VisibleDockables.Count > 0) // 如果 files 存在且有可见的 dockables
        {
            // lol you have to pass in a child
            // 注释：需要传入一个子项才能关闭所有停靠项（原作者的幽默注释）
            _factory.CloseAllDockables(files.VisibleDockables[0]); // 关闭 files 的所有停靠项，传入第一个可见子项作为入口

            _factory.DocMan.Clear(); // 清空工厂的文档管理器（DocMan）
        }
    }

    public void ViewDuplicateTab() // 公共方法：复制当前标签页（ViewDuplicateTab）
    {
        var files = _factory.GetDockable<IDocumentDock>("Files"); // 获取 "Files" 文档停靠
        if (Layout is not null && files is not null) // 如果 Layout 和 files 都存在
        {
            if (files.ActiveDockable != null) // 如果有活动的 dockable
            {
                var oldDockable = files.ActiveDockable; // 保存当前活动 dockable
                _factory.AddDockable(files, oldDockable); // 将同一个 dockable 再次添加到 files（实现“复制”标签的效果）
            }
        }
    }

    // todo: should we just replace every assetinst? is that too expensive?
    // would it be better than unselecting everything?
    // 注释：TODO 提示，讨论是否替换所有 assetinst 以避免取消选择的问题

    private async Task ReloadAssetDocuments(HashSet<AssetsFileInstance> fileInst) // 私有异步方法：重新加载受影响的资产文档（ReloadAssetDocuments）
    {
        foreach (var dockable in _factory.DocMan.Documents) // 遍历工厂文档管理器中的所有文档（DocMan.Documents）
        {
            if (dockable is not AssetDocumentViewModel document) // 如果当前 dockable 不是 AssetDocumentViewModel，则跳过
                continue; // 继续下一个

            var matchesAny = document.FileInsts.Intersect(fileInst).Any(); // 检查 document.FileInsts 是否与传入的 fileInst 集合有交集
            if (matchesAny) // 如果有交集（表示该文档受影响）
            {
                await document.Load(document.FileInsts); // 重新加载该文档（调用其 Load 方法）
            }
        }
    }
    public async Task ShowVrcaInspector() // 公共异步方法：打开 VRCA 深度结构审查器 (ShowVrcaInspector)
    {
        var dialogService = Ioc.Default.GetRequiredService<IDialogService>();
        await dialogService.ShowDialog(new VrcaInspectorViewModel());
    }
    #endregion // 区域结束：Menu items


    private async Task OnSelectedWorkspaceItemsChanged(object recipient, SelectedWorkspaceItemChangedMessage message) // 私有异步方法：处理选中工作区项改变的消息（OnSelectedWorkspaceItemsChanged）
    {
        await OpenAssetDocument(message.Value, true); // 调用 OpenAssetDocument 打开对应的资产文档，replaceDock 参数为 true
    }

    public List<AssetsFileInstance> GetSelectedDocFileInsts() // 公共方法：获取当前选中文档对应的 AssetsFileInstance 列表（GetSelectedDocFileInsts）
    {
        List<AssetsFileInstance> fileInsts; // 局部变量：保存结果列表
        var lastFocusedDoc = _factory.DocMan.LastFocusedDocument; // 获取最后聚焦的文档（LastFocusedDocument）

        if (lastFocusedDoc is AssetDocumentViewModel assetDocVm) // 如果最后聚焦的文档是 AssetDocumentViewModel
        {
            fileInsts = assetDocVm.FileInsts; // 直接使用该文档的 FileInsts 作为结果
        }
        else // 否则回退到所有打开的资产文件
        {
            // fallback to all items
            // 注释：回退逻辑，收集 Workspace 中所有 AssetsFile 类型的 WorkspaceItem
            fileInsts = []; // 初始化为空列表（注意：原代码使用 [] 语法，实际 C# 应为 new List<AssetsFileInstance>()，此处按原样注释）
            foreach (var item in WorkspaceItem.GetAssetsFileWorkspaceItems(Workspace.RootItems)) // 遍历 Workspace.RootItems 中所有资产文件类型的 WorkspaceItem
            {
                if (item.Object is AssetsFileInstance fileInst) // 如果 item.Object 是 AssetsFileInstance
                {
                    fileInsts.Add(fileInst); // 将其加入结果列表
                }
            }
        }

        return fileInsts; // 返回结果列表
    }

    private async Task<Document?> OpenAssetDocument(List<WorkspaceItem> workspaceItems, bool replaceDock) // 改为返回 Document? 以支持多种文档类型
    {
        Document? document = null; // 声明基础文档变量

        if (workspaceItems.Count == 1) // 单选逻辑
        {
            var workspaceItem = workspaceItems[0];

            if (workspaceItem.ObjectType == WorkspaceItemType.ResourceFile) // 如果是资源文件 (.resS/.resource)
            {
                var streamDoc = new StreamFileDocumentViewModel(Workspace);
                await streamDoc.Load(workspaceItem.Name); // 加载资源引用数据
                document = streamDoc;
            }
            else if (workspaceItem.ObjectType == WorkspaceItemType.AssetsFile) // 如果是资产文件 (CAB-...)
            {
                if (workspaceItem.Object is AssetsFileInstance mainFileInst)
                {
                    var assetDoc = new AssetDocumentViewModel(Workspace, LoadContainers)
                    {
                        Title = mainFileInst.name,
                        Id = mainFileInst.name
                    };
                    _lastLoadedFiles = [mainFileInst];
                    await assetDoc.Load(_lastLoadedFiles);
                    document = assetDoc;
                }
            }

            if (document == null) return null; // 如果未识别类型则返回 null
        }
        else // 多选逻辑 (目前仅支持 AssetsFile)
        {
            var assetsFileItems = workspaceItems
                .Where(i => i.ObjectType == WorkspaceItemType.AssetsFile)
                .Select(i => (AssetsFileInstance?)i.Object)
                .Where(i => i != null)
                .ToList();

            if (assetsFileItems.Count == 0 || assetsFileItems[0] is not AssetsFileInstance mainFileInst)
                return null;

            var assetDoc = new AssetDocumentViewModel(Workspace, LoadContainers)
            {
                Title = $"{mainFileInst.name} and {assetsFileItems.Count - 1} other files"
            };

            _lastLoadedFiles = assetsFileItems!;
            await assetDoc.Load(_lastLoadedFiles);
            document = assetDoc;
        }

        var files = _factory.GetDockable<IDocumentDock>("Files");
        if (Layout is not null && files is not null)
        {
            if (files.ActiveDockable != null && replaceDock)
            {
                var oldDockable = files.ActiveDockable;
                _factory.AddDockable(files, document);
                _factory.SwapDockable(files, oldDockable, document);
                _factory.CloseDockable(oldDockable);
                _factory.SetActiveDockable(document);
                _factory.SetFocusedDockable(files, document);

                if (oldDockable is Document oldDockableDocument)
                    _factory.DocMan.Documents.Remove(oldDockableDocument);
            }
            else
            {
                _factory.AddDockable(files, document);
                _factory.SetActiveDockable(document);
                _factory.SetFocusedDockable(files, document);
            }

            _factory.DocMan.Documents.Add(document);
            _factory.DocMan.LastFocusedDocument = document;
        }

        return document;
    }

    private void OnRequestEditAsset(object recipient, RequestEditAssetMessage message) // 私有方法：处理请求编辑资产的消息（OnRequestEditAsset）
    {
        _ = ShowEditAssetDialog(message.Value); // 异步调用 ShowEditAssetDialog（不等待结果），传入 message.Value（资产实例）
    }

    private async Task OnRequestVisitAsset(object recipient, RequestVisitAssetMessage message) // 私有异步方法：处理请求访问资产的消息（OnRequestVisitAsset）
    {
        var asset = message.Value; // 获取消息中的资产（AssetInst）
        var lastFocusedDoc = _factory.DocMan.LastFocusedDocument; // 获取最后聚焦的文档
        AssetDocumentViewModel? foundAssetDocVm = null; // 用于保存找到的包含该资产的文档视图模型（可空）

        // best case scenario: last selected document contains this asset
        // 注释：最佳情况是最后聚焦的文档已经包含该资产
        if (lastFocusedDoc is AssetDocumentViewModel assetDocVm
            && assetDocVm.Items.Contains(asset)) // 如果 lastFocusedDoc 是 AssetDocumentViewModel 且其 Items 包含该 asset
        {
            foundAssetDocVm = assetDocVm; // 将其设为找到的文档
            goto finish; // 跳转到 finish 标签处理后续逻辑
        }

        // second best case scenario: the last selected document is
        // a blank document we can open the containing file in.
        // 注释：第二种情况是最后聚焦的是空白文档，可以在其中打开包含该资产的文件
        var wsItem = Workspace.FindWorkspaceItemByInstance(asset.FileInstance); // 在 Workspace 中查找与 asset.FileInstance 对应的 WorkspaceItem
        if (wsItem is not null) // 如果找到了对应的 WorkspaceItem
        {
            var replaceDock = lastFocusedDoc is BlankDocumentViewModel; // 如果最后聚焦的是 BlankDocumentViewModel，则 replaceDock 为 true
            var newDoc = await OpenAssetDocument([wsItem], replaceDock); // 调用 OpenAssetDocument 打开该 WorkspaceItem
            if (newDoc is AssetDocumentViewModel newAssetDocVm) // 模式匹配：如果返回的是 AssetDocumentViewModel
            {
                foundAssetDocVm = newAssetDocVm; // 记录找到的文档
                goto finish; // 跳转到 finish
            }
        }

        // neither of those were the case. hopefully one of the open
        // asset documents contains this asset?
        // 注释：如果以上都不成立，尝试在已打开的资产文档中查找包含该资产的文档
        foreach (var dock in _factory.DocMan.Documents) // 遍历所有打开的文档
        {
            // we already checked this one, skip
            // 注释：如果是最后聚焦的文档，已经检查过，跳过
            if (dock == lastFocusedDoc)
                continue; // 跳过

            if (dock is not AssetDocumentViewModel otherAssetDocVm) // 如果当前 dock 不是 AssetDocumentViewModel，跳过
                continue; // 跳过

            if (otherAssetDocVm.Items.Contains(asset)) // 如果该文档包含 asset
            {
                foundAssetDocVm = otherAssetDocVm; // 记录找到的文档
                goto finish; // 跳转到 finish
            }
        }

        // give up
        // 注释：如果仍未找到，弹出错误对话框并返回
        await MessageBoxUtil.ShowDialog("Error", "Couldn't find asset document to show this asset in."); // 显示错误对话框（英文原文）
        return; // 返回

    finish:
        if (foundAssetDocVm is not null)
        {
            var files = _factory.GetDockable<IDocumentDock>("Files");
            if (Layout is not null && files is not null)
            {
                _factory.SetFocusedDockable(files, foundAssetDocVm);
                _factory.SetActiveDockable(foundAssetDocVm);
            }

            // Set selection AFTER focusing/activating the dockable
            foundAssetDocVm.SetSelectedItems([asset]);
        }
    }

    private async Task ShowEditAssetDialog(AssetInst asset) // 私有异步方法：显示编辑资产对话框并处理返回数据（ShowEditAssetDialog）
    {
        var dialogService = Ioc.Default.GetRequiredService<IDialogService>(); // 通过依赖注入获取 IDialogService（对话框服务）
        var baseField = Workspace.GetBaseField(asset); // 获取资产的基础字段（BaseField）
        if (baseField == null) // 如果无法获取基础字段
        {
            return; // 返回，不显示编辑对话框
        }

        var refMan = Workspace.Manager.GetRefTypeManager(asset.FileInstance); // 获取引用类型管理器（RefTypeManager）用于解析引用类型
        Workspace.CheckAndSetMonoTempGenerators(asset.FileInstance, asset); // 检查并设置 Mono 临时生成器（与 Unity 的 MonoBehaviour 相关）
        var newData = await dialogService.ShowDialog(new EditDataViewModel(baseField, refMan)); // 显示 EditData 对话框并等待用户编辑结果（EditDataViewModel）
        if (newData != null) // 如果用户返回了新的数据
        {
            asset.UpdateAssetDataAndRow(Workspace, newData); // 使用新的数据更新资产并刷新行显示（UpdateAssetDataAndRow）
            WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(asset)); // 发送 AssetsUpdatedMessage 通知其他组件资产已更新
        }
    }

    public void ShowAssetInfoDialog() // 公共方法：显示资产信息对话框（ShowAssetInfoDialog）
    {
        var dialogService = Ioc.Default.GetRequiredService<IDialogService>(); // 获取对话框服务
        var explorer = _factory.GetDockable<WorkspaceExplorerToolViewModel>("WorkspaceExplorer"); // 获取 WorkspaceExplorer 工具视图模型

        if (explorer is null) // 如果 explorer 为 null
        {
            return; // 返回
        }

        var fileInsts = GetSelectedDocFileInsts(); // 获取当前选中文档对应的文件实例列表
        var wsItems = fileInsts // 将文件实例映射为 WorkspaceItem（通过 Workspace.FindWorkspaceItemByInstance）
            .Select(Workspace.FindWorkspaceItemByInstance) // 对每个 fileInst 调用 Workspace.FindWorkspaceItemByInstance
            .Where(i => i is not null) as IEnumerable<WorkspaceItem>; // 过滤掉 null 并转换为 IEnumerable<WorkspaceItem>
        dialogService.Show(new AssetInfoViewModel(Workspace, wsItems)); // 使用对话框服务显示 AssetInfoViewModel（传入 Workspace 和 wsItems）
    }

    public void ShowSearchBytesDialog() // 公共方法：显示按字节搜索对话框（ShowSearchBytesDialog）
    {
        var explorer = _factory.GetDockable<WorkspaceExplorerToolViewModel>("WorkspaceExplorer"); // 获取 WorkspaceExplorer
        if (explorer is null) return;

        var fileInsts = GetSelectedDocFileInsts();
        var dialogService = Ioc.Default.GetRequiredService<IDialogService>();
        dialogService.Show(new AssetDataSearchViewModel(Workspace, fileInsts));
    }

    public void ShowVrcaPatcher()
    {
        var vm = new VrcaSmartPatcherViewModel(Workspace);
        OnRequestShowVrcaPatcher?.Invoke(vm);
    }

    public event Action<VrcaSmartPatcherViewModel>? OnRequestShowVrcaPatcher;

    public void ShowTextureCompression()
    {
        var vm = new TextureCompressionViewModel(Workspace);
        OnRequestShowTextureCompression?.Invoke(vm);
    }

    public event Action<TextureCompressionViewModel>? OnRequestShowTextureCompression;

    public void ShowTextureReplacement()
    {
        OnRequestShowTextureReplacement?.Invoke();
    }

    public event Action? OnRequestShowTextureReplacement;

    public void ShowPasswordCracker()
    {
        OnRequestShowPasswordCracker?.Invoke();
    }

    public event Action? OnRequestShowPasswordCracker;

    public async void ShowOptionsDialog() // 公共方法：显示设置/选项对话框（ShowOptionsDialog）
    {
        var dialogService = Ioc.Default.GetRequiredService<IDialogService>();
        await dialogService.ShowDialog(new SettingsViewModel());
    }

    public void ShowOscTool()
    {
        var win = new Views.Tools.OscToolWindow(Workspace);
        win.Show();
    }

    public void ShowDeformationRepair()
    {
        OnRequestShowDeformationRepair?.Invoke();
    }

    public event Action? OnRequestShowDeformationRepair;
} // 类 MainViewModel 结束
