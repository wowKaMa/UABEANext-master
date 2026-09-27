using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于访问 Unity 资产解析相关类型（保留英文原名：AssetsTools.NET）

using AssetsTools.NET.Cpp2IL;
// 引用 AssetsTools.NET 的 Cpp2IL 扩展，用于与 il2cpp/Cpp2IL 相关的功能（保留英文原名：AssetsTools.NET.Cpp2IL）

using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展功能，提供扩展类型与辅助方法（保留英文原名：AssetsTools.NET.Extra）

using CommunityToolkit.Mvvm.ComponentModel;
// 引用 CommunityToolkit MVVM 的组件模型命名空间，提供 ObservableObject 与属性生成特性（保留英文原名：CommunityToolkit.Mvvm.ComponentModel）

using System;
// 引用基础系统命名空间，提供常用类型（保留英文原名：System）

using System.Collections.Generic;
// 引用泛型集合命名空间，提供 List<T>、Dictionary<TKey,TValue> 等集合类型（保留英文原名：System.Collections.Generic）

using System.Collections.ObjectModel;
// 引用可观察集合命名空间，提供 ObservableCollection<T> 等（保留英文原名：System.Collections.ObjectModel）

using System.IO;
// 引用 IO 命名空间，用于文件与路径操作（保留英文原名：System.IO）

using System.Linq;
// 引用 LINQ 扩展方法命名空间，用于集合查询与转换（保留英文原名：System.Linq）

using System.Threading;
// 引用线程与同步命名空间，提供 Mutex、SynchronizationContext 等（保留英文原名：System.Threading）

using UABEANext4.Logic.Configuration;
// 引用项目内的配置管理命名空间（保留英文原名：UABEANext4.Logic.Configuration）

using UABEANext4.Plugins;
// 引用项目内插件加载相关命名空间（保留英文原名：UABEANext4.Plugins）

using UABEANext4.Util;
// 引用项目内的工具/实用程序命名空间（保留英文原名：UABEANext4.Util）

namespace UABEANext4.AssetWorkspace;
// 定义命名空间 UABEANext4.AssetWorkspace，用于组织工作区相关类型（保留英文原名：UABEANext4.AssetWorkspace）

public partial class Workspace : ObservableObject
// 定义部分类 Workspace，继承 ObservableObject（用于 MVVM 通知），此文件实现工作区的核心功能（保留英文原名：Workspace / ObservableObject）
{
    // 类体开始（Workspace）

    public AssetsManager Manager { get; } = new AssetsManager();
    // 公共只读属性 Manager：AssetsManager 实例，负责加载/管理资产文件（保留英文原名：Manager / AssetsManager）

    public PluginLoader Plugins { get; } = new PluginLoader();
    // 公共只读属性 Plugins：PluginLoader 实例，用于加载插件（保留英文原名：Plugins / PluginLoader）

    public AssetNamer Namer { get; }
    // 公共只读属性 Namer：AssetNamer 实例，用于生成/管理资产显示名称（保留英文原名：Namer / AssetNamer）

    public Mutex ModifyMutex { get; } = new Mutex();
    // 公共只读属性 ModifyMutex：互斥锁（Mutex），用于保护并发修改操作（保留英文原名：ModifyMutex / Mutex）

    // this should be its own class
    // 注释：提示：下面的进度相关字段应该独立成类（保留英文原注释）

    [ObservableProperty]
    // 特性：由 CommunityToolkit 自动生成属性与通知（保留英文原名：ObservableProperty）

    public float _progressValue = 0f;
    // 由 ObservableProperty 标注的字段 _progressValue：表示当前进度值（0.0 - 1.0），初始为 0（保留英文原名：_progressValue）

    [ObservableProperty]
    // 特性：由 CommunityToolkit 自动生成属性与通知（保留英文原名：ObservableProperty）

    public string _progressText = "";
    // 由 ObservableProperty 标注的字段 _progressText：表示进度文本描述，初始为空字符串（保留英文原名：_progressText）

    public ObservableCollection<WorkspaceItem> RootItems { get; } = new();
    // 公共只读属性 RootItems：根级 WorkspaceItem 的可观察集合，用于 UI 绑定显示工作区根节点（保留英文原名：RootItems / WorkspaceItem）

    public Dictionary<string, WorkspaceItem> ItemLookup { get; } = new();
    // 公共只读属性 ItemLookup：按名称查找 WorkspaceItem 的字典（保留英文原名：ItemLookup）

    private SynchronizationContext? FileSyncContext { get; } = SynchronizationContext.Current;
    // 私有只读属性 FileSyncContext：保存当前同步上下文（通常为 UI 线程上下文），用于线程安全地向 UI 线程派发操作（保留英文原名：FileSyncContext / SynchronizationContext）

    // items modified and unsaved
    // 注释：下面集合用于记录已修改但未保存的项（保留英文原注释）

    public HashSet<WorkspaceItem> UnsavedItems { get; } = new();
    // 公共只读属性 UnsavedItems：保存已修改但尚未保存的 WorkspaceItem 集合（保留英文原名：UnsavedItems）

    // items modified and saved
    // we track this since the base AssetsFile is still reading from the old file
    // 注释：下面集合用于记录已修改并已保存的项（保留英文原注释）

    public HashSet<WorkspaceItem> ModifiedItems { get; } = new();
    // 公共只读属性 ModifiedItems：保存已修改并已保存的 WorkspaceItem 集合（保留英文原名：ModifiedItems）

    public int NextLoadIndex => RootItems.Count != 0 ? RootItems.Max(i => i.LoadIndex) + 1 : 0;
    // 只读计算属性 NextLoadIndex：计算下一个加载索引（LoadIndex），用于按加载顺序插入（保留英文原名：NextLoadIndex / LoadIndex）

    public Dictionary<string, List<ResourceEntry>> ResourceCache { get; } = new();
    // 公共只读属性 ResourceCache：缓存已扫描的资源引用数据，键为文件名，值为引用项列表

    public delegate void MonoTemplateFailureEvent(string path);
    // 定义委托 MonoTemplateFailureEvent：当 Mono 模板加载失败时使用，参数为路径字符串（保留英文原名：MonoTemplateFailureEvent）

    public event MonoTemplateFailureEvent? MonoTemplateLoadFailed;
    // 事件 MonoTemplateLoadFailed：当设置 Mono 临时生成器失败时触发（保留英文原名：MonoTemplateLoadFailed）

    private bool _setMonoTempGeneratorsYet;
    // 私有字段 _setMonoTempGeneratorsYet：标记是否已尝试设置 Mono 临时生成器（保留英文原名：_setMonoTempGeneratorsYet）

    public Workspace()
    // 构造函数 Workspace：初始化 Manager、插件、缓存设置与 Namer（保留英文原名：Workspace）
    {
        string classDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "classdata.tpk");
        // 计算 classdata.tpk 的路径（保留英文原名：classDataPath / Path.Combine / AppDomain.CurrentDomain.BaseDirectory）

        if (File.Exists(classDataPath))
            Manager.LoadClassPackage(classDataPath);
        // 如果 classdata.tpk 存在，则让 Manager 加载该类数据库包（保留英文原名：File.Exists / Manager.LoadClassPackage）

        string pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
        // 计算 plugins 目录路径（保留英文原名：pluginsPath）

        Plugins.LoadPluginsInDirectory(pluginsPath);
        // 从 plugins 目录加载插件（保留英文原名：Plugins.LoadPluginsInDirectory）

        Manager.UseRefTypeManagerCache = true;
        // 启用 Manager 的引用类型管理缓存以提高性能（保留英文原名：UseRefTypeManagerCache）

        Manager.UseTemplateFieldCache = true;
        // 启用模板字段缓存（保留英文原名：UseTemplateFieldCache）

        Manager.UseQuickLookup = true;
        // 启用快速查找功能（保留英文原名：UseQuickLookup）

        Namer = new AssetNamer(this);
        // 创建 AssetNamer 实例并赋值给 Namer（保留英文原名：AssetNamer / Namer）
    }

    public WorkspaceItem? LoadAnyFile(Stream stream, int loadOrder = -1, string path = "")
    // 公共方法 LoadAnyFile：根据流内容检测文件类型并调用相应加载方法，返回 WorkspaceItem（保留英文原名：LoadAnyFile）
    {
        if (path == "" && stream is FileStream fs)
        {
            path = fs.Name;
        }
        // 如果未提供路径且流是 FileStream，则使用流的文件名作为路径（保留英文原名：FileStream / fs.Name）

        var detectedType = FileTypeDetector.DetectFileType(new AssetsFileReader(stream), 0);
        // 使用 FileTypeDetector 检测文件类型（BundleFile / AssetsFile / resource 等），传入新的 AssetsFileReader（保留英文原名：FileTypeDetector.DetectFileType / AssetsFileReader）

        if (detectedType == DetectedFileType.BundleFile)
        {
            stream.Position = 0;
            return LoadBundle(stream, loadOrder, Path.GetFileName(path), path);
        }
        else if (detectedType == DetectedFileType.AssetsFile)
        {
            stream.Position = 0;
            return LoadAssets(stream, loadOrder, Path.GetFileName(path), path);
        }
        else if (path.EndsWith(".resS") || path.EndsWith(".resource"))
        {
            return LoadResource(stream, loadOrder);
        }
        // 根据检测结果调用 LoadBundle、LoadAssets 或 LoadResource；在读取前将流位置重置为 0（保留英文原名：DetectedFileType / LoadBundle / LoadAssets / LoadResource）

        return null;
    }

    public WorkspaceItem LoadBundle(Stream stream, int loadOrder = -1, string name = "", string fullPath = "")
    // 公共方法 LoadBundle：加载 bundle 文件并返回对应的 WorkspaceItem（保留英文原名：LoadBundle）
    {
        // todo: don't always unpack to memory lol
        // 注释：TODO：目前总是解包到内存，未来可优化（保留英文原注释）

        BundleFileInstance bunInst;
        // 局部变量 bunInst：BundleFileInstance 实例（保留英文原名：BundleFileInstance）

        if (stream is FileStream fs)
        {
            bunInst = Manager.LoadBundleFile(fs);
        }
        else
        {
            bunInst = Manager.LoadBundleFile(stream, name);
        }
        
        if (!string.IsNullOrEmpty(fullPath)) bunInst.path = fullPath;
        // 如果流是文件流则调用 Manager.LoadBundleFile(FileStream)，否则调用带 name 的重载（保留英文原名：Manager.LoadBundleFile）

        TryLoadClassDatabase(bunInst.file);
        // 尝试根据 bundle 文件头信息加载类数据库（保留英文原名：TryLoadClassDatabase）

        var item = new WorkspaceItem(this, bunInst, loadOrder);
        // 使用 bundle 实例创建 WorkspaceItem（保留英文原名：WorkspaceItem）

        AddRootItemThreadSafe(item, bunInst.name);
        // 将该项以线程安全方式添加到根集合（保留英文原名：AddRootItemThreadSafe）

        return item;
    }

    public WorkspaceItem LoadAssets(Stream stream, int loadOrder = -1, string name = "", string fullPath = "")
    // 公共方法 LoadAssets：加载 .assets 文件并返回对应的 WorkspaceItem（保留英文原名：LoadAssets）
    {
        AssetsFileInstance fileInst;
        // 局部变量 fileInst：AssetsFileInstance（保留英文原名：AssetsFileInstance）

        if (stream is FileStream fs)
        {
            fileInst = Manager.LoadAssetsFile(fs);
        }
        else
        {
            fileInst = Manager.LoadAssetsFile(stream, name);
        }
        
        if (!string.IsNullOrEmpty(fullPath)) fileInst.path = fullPath;
        // 根据流类型调用 Manager.LoadAssetsFile 的不同重载（保留英文原名：Manager.LoadAssetsFile）

        TryLoadClassDatabase(fileInst.file);
        // 尝试加载类数据库（保留英文原名：TryLoadClassDatabase）

        FixupAssetsFile(fileInst);
        // 对加载的 AssetsFileInstance 做后处理（Fixup），例如将 AssetInfos 转换为 AssetInst（保留英文原名：FixupAssetsFile）

        var item = new WorkspaceItem(fileInst, loadOrder);
        // 创建 WorkspaceItem（保留英文原名：WorkspaceItem）

        AddRootItemThreadSafe(item, fileInst.name);
        // 将该项以线程安全方式添加到根集合（保留英文原名：AddRootItemThreadSafe）

        return item;
    }

    public WorkspaceItem LoadAssetsFromBundle(BundleFileInstance bunInst, int index)
    // 公共方法 LoadAssetsFromBundle：从 bundle 中按索引加载 .assets 文件并返回 WorkspaceItem（保留英文原名：LoadAssetsFromBundle）
    {
        var dirInf = BundleHelper.GetDirInfo(bunInst.file, index);
        // 获取 bundle 中第 index 个目录信息（dirInf）（保留英文原名：BundleHelper.GetDirInfo）

        var fileInst = Manager.LoadAssetsFileFromBundle(bunInst, index);
        // 使用 Manager 从 bundle 中加载 AssetsFileInstance（保留英文原名：LoadAssetsFileFromBundle）

        TryLoadClassDatabase(fileInst.file);
        // 尝试加载类数据库（保留英文原名：TryLoadClassDatabase）

        FixupAssetsFile(fileInst);
        // 对加载的文件做后处理（保留英文原名：FixupAssetsFile）

        var item = new WorkspaceItem(dirInf.Name, fileInst, -1, WorkspaceItemType.AssetsFile);
        // 创建一个表示 bundle 内部 assets 文件的 WorkspaceItem（保留英文原名：WorkspaceItem / WorkspaceItemType.AssetsFile）

        return item;
    }

    internal void FixupAssetsFile(AssetsFileInstance fileInst)
    // 私有方法 FixupAssetsFile：将 fileInst.file.AssetInfos 转换为 RangeObservableCollection<AssetFileInfo>（若尚未是该类型），并生成快速查找（保留英文原名：FixupAssetsFile）
    {
        if (fileInst.file.AssetInfos is not RangeObservableCollection<AssetFileInfo>)
        {
            var assetInsts = new RangeObservableCollection<AssetFileInfo>();
            var tmp = new List<AssetFileInfo>();
            var maxNameLen = ConfigurationManager.Settings.ListingNameLength;
            foreach (var info in fileInst.file.AssetInfos)
            {
                var asset = new AssetInst(fileInst, info);
                asset.AssetName = Namer.GetAssetName(asset, true, maxNameLen);

                tmp.Add(asset);
            }
            assetInsts.AddRange(tmp);
            fileInst.file.Metadata.AssetInfos = assetInsts;
            fileInst.file.GenerateQuickLookup();
        }
    }
    // 说明：如果 AssetInfos 不是 RangeObservableCollection，则构建新的集合，创建 AssetInst 并设置显示名，替换原集合并调用 GenerateQuickLookup（保留英文原名：RangeObservableCollection / AssetInst / GenerateQuickLookup）

    public void TryLoadClassDatabase(AssetBundleFile file)
    // 公共方法 TryLoadClassDatabase（重载）：根据 AssetBundleFile 的 Header.EngineVersion 尝试加载类数据库（保留英文原名：TryLoadClassDatabase）
    {
        if (Manager.ClassDatabase == null)
        {
            var fileVersion = file.Header.EngineVersion;
            if (fileVersion != "0.0.0")
            {
                Manager.LoadClassDatabaseFromPackage(fileVersion);
            }
        }
    }

    public void TryLoadClassDatabase(AssetsFile file)
    // 公共方法 TryLoadClassDatabase（重载）：根据 AssetsFile 的 Metadata.UnityVersion 尝试加载类数据库（保留英文原名：TryLoadClassDatabase）
    {
        if (Manager.ClassDatabase == null)
        {
            var metadata = file.Metadata;
            var fileVersion = metadata.UnityVersion;
            if (fileVersion != "0.0.0")
            {
                Manager.LoadClassDatabaseFromPackage(fileVersion);
            }
        }
    }

    public WorkspaceItem LoadResource(Stream stream, int loadOrder = -1, string name = "")
    // 公共方法 LoadResource：将资源流包装为 WorkspaceItem 并添加到根（保留英文原名：LoadResource）
    {
        if (name == "" && stream is FileStream fs)
        {
            name = Path.GetFileName(fs.Name);
        }
        // 如果未提供 name 且流是文件流，则使用文件名作为 name（保留英文原名：Path.GetFileName）

        WorkspaceItem item = new WorkspaceItem(name, stream, loadOrder, WorkspaceItemType.ResourceFile);
        // 创建一个类型为 ResourceFile 的 WorkspaceItem（保留英文原名：WorkspaceItemType.ResourceFile）

        AddRootItemThreadSafe(item, name);
        // 将该项以线程安全方式添加到根集合（保留英文原名：AddRootItemThreadSafe）

        return item;
    }

    internal void AddRootItemThreadSafe(WorkspaceItem item, string itemName)
    // 内部方法 AddRootItemThreadSafe：在 FileSyncContext（UI 线程）上安全地将根项插入 RootItems 并更新 ItemLookup（保留英文原名：AddRootItemThreadSafe）
    {
        FileSyncContext?.Post(_ =>
        {
            if (item.LoadIndex != -1)
            {
                int pos = RootItems.BinarySearch(item, (i, j) => i.LoadIndex.CompareTo(j.LoadIndex));
                if (pos < 0)
                {
                    RootItems.Insert(~pos, item);
                }
                else
                {
                    RootItems.Insert(pos, item);
                }
                ItemLookup[itemName] = item;
                return;
            }

            RootItems.Add(item);
            ItemLookup[itemName] = item;
        }, null);
    }
    // 说明：在 UI 线程上下文中执行插入逻辑，若 LoadIndex 有效则按 LoadIndex 排序插入，否则追加（保留英文原名：FileSyncContext.Post / BinarySearch / ItemLookup）

    internal void AddChildItemThreadSafe(WorkspaceItem item, WorkspaceItem parent, string itemName)
    // 内部方法 AddChildItemThreadSafe：在 FileSyncContext 上安全地将子项添加到 parent.Children 并设置 parent 引用与 ItemLookup（保留英文原名：AddChildItemThreadSafe）
    {
        FileSyncContext?.Post(_ =>
        {
            // loadorder ignored here
            parent.Children.Add(item);
            item.Parent = parent;
            ItemLookup[itemName] = item;
        }, null);
    }
    // 说明：在 UI 线程上下文中执行添加子项操作（保留英文原名：Children / Parent / ItemLookup）

    public void SetProgressThreadSafe(float value, string text)
    // 公共方法 SetProgressThreadSafe：以线程安全方式更新进度值与文本（仅在变化显著时派发到 UI 线程）（保留英文原名：SetProgressThreadSafe）
    {
        var roundedValue = (float)Math.Round(value * 20) / 20;
        if (Math.Abs(roundedValue - ProgressValue) >= 0.05f || value == 0f || value == 1f)
        {
            FileSyncContext?.Post(_ =>
            {
                ProgressValue = value;
                ProgressText = text;
            }, null);
        }
    }
    // 说明：将进度值四舍五入到 0.05 的步长，只有在显著变化或到达 0/1 时才更新 UI（保留英文原名：ProgressValue / ProgressText）

    // should be nullable
    // 注释：提示：下面的方法返回值应可空（保留英文原注释）

    public AssetTypeTemplateField GetTemplateField(AssetInst asset, bool skipMonoBehaviourFields = false)
    // 公共方法 GetTemplateField：获取 AssetInst 的模板字段（可选择跳过 MonoBehaviour 字段），通过 Manager 获取（保留英文原名：GetTemplateField）
    {
        AssetReadFlags readFlags = AssetReadFlags.None;
        if (skipMonoBehaviourFields && asset.Type == AssetClassID.MonoBehaviour)
        {
            readFlags |= AssetReadFlags.SkipMonoBehaviourFields | AssetReadFlags.ForceFromCldb;
        }

        return Manager.GetTemplateBaseField(asset.FileInstance, asset, readFlags);
    }

    public AssetTypeTemplateField GetTemplateField(AssetsFileInstance fileInst, AssetFileInfo info, bool skipMonoBehaviourFields = false)
    // 公共方法 GetTemplateField（重载）：根据 fileInst 与 AssetFileInfo 获取模板字段（保留英文原名：GetTemplateField）
    {
        AssetReadFlags readFlags = AssetReadFlags.None;
        if (skipMonoBehaviourFields && info.TypeId == (int)AssetClassID.MonoBehaviour)
        {
            readFlags |= AssetReadFlags.SkipMonoBehaviourFields | AssetReadFlags.ForceFromCldb;
        }

        return Manager.GetTemplateBaseField(fileInst, info, readFlags);
    }

    public void CheckAndSetMonoTempGenerators(AssetsFileInstance fileInst, AssetFileInfo? info)
    // 公共方法 CheckAndSetMonoTempGenerators：检查是否需要并尝试设置 Mono 临时生成器（用于解析 MonoBehaviour），若失败触发事件（保留英文原名：CheckAndSetMonoTempGenerators）
    {
        bool isValidMono = info == null || info.TypeId == (int)AssetClassID.MonoBehaviour || info.TypeId < 0;
        if (isValidMono && !_setMonoTempGeneratorsYet && !fileInst.file.Metadata.TypeTreeEnabled)
        {
            string dataDir = PathUtils.GetAssetsFileDirectory(fileInst);
            bool success = SetMonoTempGenerators(dataDir);
            if (!success)
            {
                MonoTemplateLoadFailed?.Invoke(dataDir);
            }
        }
    }

    private bool SetMonoTempGenerators(string fileDir)
    // 私有方法 SetMonoTempGenerators：尝试根据文件目录查找 Managed 或 il2cpp 信息并设置 Manager.MonoTempGenerator（保留英文原名：SetMonoTempGenerators）
    {
        if (!_setMonoTempGeneratorsYet)
        {
            _setMonoTempGeneratorsYet = true;

            string managedDir = Path.Combine(fileDir, "Managed");
            FindCpp2IlFilesResult il2cppFiles = FindCpp2IlFiles.Find(fileDir);

            bool managedExists = Directory.Exists(managedDir);
            bool il2cppExists = il2cppFiles.success;

            if (managedExists && (!il2cppExists || ConfigurationManager.Settings.UseManagedOverIl2cpp))
            {
                bool hasDll = Directory.GetFiles(managedDir, "*.dll").Length > 0;
                if (hasDll)
                {
                    Manager.MonoTempGenerator = new MonoCecilTempGenerator(managedDir);
                    return true;
                }
            }

            if (il2cppExists)
            {
                Manager.MonoTempGenerator = new Cpp2IlTempGenerator(il2cppFiles.metaPath, il2cppFiles.asmPath);
                return true;
            }
        }
        return false;
    }
    // 说明：优先使用 Managed（dll）目录创建 MonoCecilTempGenerator，若不存在则尝试使用 Cpp2Il 生成器（保留英文原名：MonoCecilTempGenerator / Cpp2IlTempGenerator）

    public AssetFileInfo? GetAssetFileInfo(AssetsFileInstance fileInst, AssetTypeValueField pptrField)
    // 公共方法 GetAssetFileInfo（重载）：从 pptr 字段读取 m_FileID 与 m_PathID 并调用另一重载获取 AssetFileInfo（保留英文原名：GetAssetFileInfo）
    {
        return GetAssetFileInfo(fileInst, pptrField["m_FileID"].AsInt, pptrField["m_PathID"].AsLong);
    }

    public AssetFileInfo? GetAssetFileInfo(AssetsFileInstance fileInst, int fileId, long pathId)
    // 公共方法 GetAssetFileInfo：根据 fileId 与 pathId 获取目标文件的 AssetFileInfo（保留英文原名：GetAssetFileInfo）
    {
        if (fileId != 0)
        {
            fileInst = fileInst.GetDependency(Manager, fileId - 1);
        }
        if (fileInst == null)
        {
            return null;
        }

        return fileInst.file.GetAssetInfo(pathId);
    }

    public AssetInst? GetAssetInst(AssetsFileInstance fileInst, AssetTypeValueField pptrField)
    // 公共方法 GetAssetInst（重载）：从 pptr 字段读取 m_FileID 与 m_PathID 并调用另一重载获取 AssetInst（保留英文原名：GetAssetInst）
    {
        return GetAssetInst(fileInst, pptrField["m_FileID"].AsInt, pptrField["m_PathID"].AsLong);
    }

    public AssetInst? GetAssetInst(AssetsFileInstance fileInst, int fileId, long pathId)
    // 公共方法 GetAssetInst：根据 fileId 与 pathId 获取或构造 AssetInst（保留英文原名：GetAssetInst）
    {
        if (fileId != 0)
        {
            fileInst = fileInst.GetDependency(Manager, fileId - 1);
            fileId = 0;
        }
        AssetFileInfo? info = GetAssetFileInfo(fileInst, fileId, pathId);

        if (info == null)
        {
            return null;
        }
        else if (info is AssetInst inst)
        {
            return inst;
        }
        else
        {
            return new AssetInst(fileInst, info);
        }
    }

    public AssetTypeValueField? GetBaseField(AssetInst asset)
    // 公共方法 GetBaseField（重载）：根据 AssetInst 获取其根 BaseField（保留英文原名：GetBaseField）
    {
        return GetBaseField(asset.FileInstance, asset.PathId);
    }

    public AssetTypeValueField? GetBaseField(AssetsFileInstance fileInst, long pathId)
    // 公共方法 GetBaseField（重载）：根据 fileInst 与 pathId 获取根 BaseField（保留英文原名：GetBaseField）
    {
        return GetBaseField(fileInst, 0, pathId);
    }

    public AssetTypeValueField? GetBaseField(AssetsFileInstance fileInst, AssetTypeValueField pptrField)
    // 公共方法 GetBaseField（重载）：从 pptr 字段读取 m_FileID 与 m_PathID 并调用主重载（保留英文原名：GetBaseField）
    {
        return GetBaseField(fileInst, pptrField["m_FileID"].AsInt, pptrField["m_PathID"].AsLong);
    }

    public AssetTypeValueField? GetBaseField(AssetsFileInstance fileInst, int fileId, long pathId)
    // 公共方法 GetBaseField（主重载）：根据 fileId/pathId 获取目标文件并通过 Manager 获取 BaseField，包含错误处理与 Mono 临时生成器检查（保留英文原名：GetBaseField）
    {
        if (fileId != 0)
        {
            fileInst = fileInst.GetDependency(Manager, fileId - 1);
        }
        if (fileInst == null)
        {
            return null;
        }

        AssetFileInfo? info = fileInst.file.GetAssetInfo(pathId);
        if (info == null)
        {
            return null;
        }

        CheckAndSetMonoTempGenerators(fileInst, info);

        // negative target platform seems to indicate an editor version
        AssetReadFlags readFlags = AssetReadFlags.None;
        if ((int)fileInst.file.Metadata.TargetPlatform < 0)
        {
            readFlags |= AssetReadFlags.PreferEditor;
        }

        try
        {
            return Manager.GetBaseField(fileInst, info, readFlags);
        }
        catch
        {
            return null;
        }
    }

    public void Dirty(WorkspaceItem item)
    // 公共方法 Dirty：标记项为已修改（加入 UnsavedItems 与 ModifiedItems），并递归标记父项（保留英文原名：Dirty）
    {
        UnsavedItems.Add(item);
        ModifiedItems.Add(item);
        if (item.Parent != null)
        {
            Dirty(item.Parent);
        }
    }

    public void CloseAll()
    // 公共方法 CloseAll：关闭所有资源并清理 Manager 与集合（保留英文原名：CloseAll）
    {
        foreach (var item in RootItems)
        {
            if (item.ObjectType == WorkspaceItemType.ResourceFile && item.Loaded)
            {
                var stream = (Stream?)item.Object;
                stream?.Close();
            }
        }
        Manager.UnloadAll();
        Manager.UnloadClassDatabase();
        Manager.MonoTempGenerator = null;
        _setMonoTempGeneratorsYet = false;
        RootItems.Clear();
        ItemLookup.Clear();
        UnsavedItems.Clear();
        ModifiedItems.Clear();
    }

    public void RenameFile(WorkspaceItem wsItem, string newName)
    // 公共方法 RenameFile：重命名 WorkspaceItem，并在必要时更新底层实例的 name、ItemLookup 与标记为脏（保留英文原名：RenameFile）
    {
        var oldName = wsItem.Name;
        if (oldName != newName)
        {
            if (wsItem.Object is AssetsFileInstance fileInst)
            {
                fileInst.name = newName;
            }
            else if (wsItem.Object is BundleFileInstance bunInst)
            {
                bunInst.name = newName;
            }

            wsItem.Name = newName;
            wsItem.Update(nameof(wsItem.Name));
            Dirty(wsItem);
            ItemLookup.Remove(oldName);
            ItemLookup[newName] = wsItem;
        }
    }

    public WorkspaceItem? FindWorkspaceItemByInstance(AssetsFileInstance fileInst)
    // 公共方法 FindWorkspaceItemByInstance（重载）：通过 AssetsFileInstance 查找对应的 WorkspaceItem（保留英文原名：FindWorkspaceItemByInstance）
    {
        // todo: keying needs to be a generic method
        var key = fileInst.name;

        if (ItemLookup.TryGetValue(key, out var wsItem))
            return wsItem;

        // no match? try bfs searching starting at the root
        // we pass the null since this is the last resort option
        return FindWorkspaceItemBfs(i =>
            i.Object is AssetsFileInstance thisFileInst && thisFileInst == fileInst
        );
    }

    public WorkspaceItem? FindWorkspaceItemByInstance(BundleFileInstance bunInst)
    // 公共方法 FindWorkspaceItemByInstance（重载）：通过 BundleFileInstance 查找对应的 WorkspaceItem（保留英文原名：FindWorkspaceItemByInstance）
    {
        // todo: keying needs to be a generic method
        var key = bunInst.name;

        if (ItemLookup.TryGetValue(key, out var wsItem))
            return wsItem;

        return FindWorkspaceItemBfs(i =>
            i.Object is BundleFileInstance thisBunInst && thisBunInst == bunInst
        );
    }

    private WorkspaceItem? FindWorkspaceItemBfs(Func<WorkspaceItem, bool> predicate)
    // 私有方法 FindWorkspaceItemBfs：对 RootItems 做广度优先搜索以满足 predicate 的项（保留英文原名：FindWorkspaceItemBfs）
    {
        var searchQueue = new Queue<WorkspaceItem>(RootItems);
        while (searchQueue.Count > 0)
        {
            var current = searchQueue.Dequeue();

            if (predicate(current))
                return current;

            if (current.Children != null)
            {
                foreach (var child in current.Children)
                    searchQueue.Enqueue(child);
            }
        }

        return null;
    }
    // 方法结束（FindWorkspaceItemBfs）

}
// 类体结束（Workspace）
