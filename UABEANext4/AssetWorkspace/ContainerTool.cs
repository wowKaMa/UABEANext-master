using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于处理 Unity 资产文件（保留英文原名：AssetsTools.NET）

using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展功能，提供额外的类型与辅助方法（保留英文原名：AssetsTools.NET.Extra）

using System;
// 引用基础系统命名空间，提供常用类型（保留英文原名：System）

using System.Collections.Generic;
// 引用泛型集合命名空间，提供 List、Dictionary 等集合类型（保留英文原名：System.Collections.Generic）

using System.Diagnostics.CodeAnalysis;
// 引用可空性注解等诊断辅助特性（保留英文原名：System.Diagnostics.CodeAnalysis）

using System.IO;
// 引用 IO 命名空间，用于文件与路径操作（保留英文原名：System.IO）

using System.Linq;
// 引用 LINQ 扩展方法命名空间，用于集合查询与转换（保留英文原名：System.Linq）

using UABEANext4.Util;
// 引用项目内的工具/实用程序命名空间（保留英文原名：UABEANext4.Util）

namespace UABEANext4.AssetWorkspace;
// 定义命名空间 UABEANext4.AssetWorkspace（保留英文原名：UABEANext4.AssetWorkspace）

// from UABEA. probably needs some editing/cleanup.
// 注释：来源于 UABEA，可能需要清理或修改（保留英文原注释）

public class ContainerTool
// 定义公共类 ContainerTool：用于解析/查询 bundle/resource 中的容器信息（保留英文原名：ContainerTool）
{
    // 类体开始（ContainerTool）

    public List<AssetPPtr> PreloadTable { get; } = [];
    // 公共只读属性 PreloadTable：保存 preload 表（AssetPPtr 列表），初始为空列表（保留英文原名：PreloadTable / AssetPPtr）

    // normally this map is string -> AssetInfo, but we only do path id -> string lookups so this isn't useful
    // 注释：通常映射为 string -> AssetInfo，但此处只做 path id -> string 的查找（保留英文原注释）

    public Dictionary<ContainerAssetInfo, string> AssetMap { get; } = [];
    // 公共只读属性 AssetMap：将 ContainerAssetInfo 映射到路径字符串（保留英文原名：AssetMap / ContainerAssetInfo）

    public static ContainerTool FromAssetBundle(AssetsManager am, AssetsFileInstance fromFile, AssetTypeValueField assetBundleBf)
    // 静态工厂方法 FromAssetBundle：从 AssetBundle 的 BaseField 构建 ContainerTool（保留英文原名：FromAssetBundle）
    {
        ContainerTool ct = new ContainerTool();
        // 创建一个新的 ContainerTool 实例（ct）（保留英文原名：ct）

        AssetTypeValueField m_PreloadTable = assetBundleBf["m_PreloadTable.Array"];
        // 从 assetBundleBf 中读取 m_PreloadTable.Array 字段（保留英文原名：m_PreloadTable）

        foreach (AssetTypeValueField ptr in m_PreloadTable)
        // 遍历 preload 表中的每个条目（ptr）（保留英文原名：ptr）
        {
            AssetPPtr assetPPtr = AssetPPtr.FromField(ptr);
            // 从字段构造 AssetPPtr（保留英文原名：AssetPPtr.FromField）

            assetPPtr.SetFilePathFromFile(am, fromFile);
            // 使用 AssetsManager 与源文件设置 AssetPPtr 的文件路径信息（保留英文原名：SetFilePathFromFile）

            ct.PreloadTable.Add(assetPPtr);
            // 将解析出的 AssetPPtr 添加到 ct.PreloadTable（保留英文原名：PreloadTable）
        }

        AssetTypeValueField m_Container = assetBundleBf["m_Container.Array"];
        // 从 assetBundleBf 中读取 m_Container.Array 字段（保留英文原名：m_Container）

        foreach (AssetTypeValueField container in m_Container)
        // 遍历容器数组中的每个条目（container）（保留英文原名：container）
        {
            string key = container["first"].AsString;
            // 读取容器条目的 key（first 字段，字符串）（保留英文原名：first / key）

            AssetTypeValueField value = container["second"];
            // 读取容器条目的 value（second 字段，通常是一个结构体字段）（保留英文原名：second / value）

            ContainerAssetInfo assetInfo = ContainerAssetInfo.FromField(value);
            // 从 value 字段构造 ContainerAssetInfo（保留英文原名：ContainerAssetInfo.FromField）

            assetInfo.Ptr.SetFilePathFromFile(am, fromFile);
            // 为 assetInfo 中的 Ptr 设置文件路径信息（保留英文原名：Ptr / SetFilePathFromFile）

            if (assetInfo.Ptr.PathId != 0)
            {
                ct.AssetMap[assetInfo] = key;
            }
            // 如果 Ptr 的 PathId 非 0，则将 assetInfo -> key 加入 AssetMap（保留英文原名：PathId / AssetMap）
        }

        return ct;
        // 返回构建好的 ContainerTool 实例（ct）
    }

    public static ContainerTool FromResourceManager(AssetsManager am, AssetsFileInstance fromFile, AssetTypeValueField rsrcManBf)
    // 静态工厂方法 FromResourceManager：从 ResourceManager 的 BaseField 构建 ContainerTool（保留英文原名：FromResourceManager）
    {
        ContainerTool ct = new ContainerTool();
        // 创建新的 ContainerTool 实例（ct）

        AssetTypeValueField m_Container = rsrcManBf["m_Container.Array"];
        // 从 rsrcManBf 中读取 m_Container.Array 字段（保留英文原名：m_Container）

        foreach (AssetTypeValueField container in m_Container)
        // 遍历容器数组中的每个条目（container）
        {
            string key = container["first"].AsString;
            // 读取 key（first 字段）（保留英文原名：first / key）

            AssetTypeValueField value = container["second"];
            // 读取 value（second 字段）（保留英文原名：second / value）

            AssetPPtr assetPPtr = AssetPPtr.FromField(value);
            // 将 value 字段解析为 AssetPPtr（保留英文原名：AssetPPtr.FromField）

            assetPPtr.SetFilePathFromFile(am, fromFile);
            // 设置 AssetPPtr 的文件路径信息（保留英文原名：SetFilePathFromFile）

            ContainerAssetInfo assetInfo = new ContainerAssetInfo(assetPPtr);
            // 使用 AssetPPtr 构造 ContainerAssetInfo（保留英文原名：ContainerAssetInfo）

            if (assetPPtr.PathId != 0)
            {
                ct.AssetMap[assetInfo] = key;
            }
            // 如果 PathId 非 0，则将 assetInfo -> key 加入 AssetMap（保留英文原名：PathId / AssetMap）
        }

        return ct;
        // 返回构建好的 ContainerTool（ct）
    }

    public string? GetContainerPath(AssetsFileInstance fileInst, long pathId)
    // 实例方法 GetContainerPath（重载）：根据 fileInst 与 pathId 查找容器路径（保留英文原名：GetContainerPath）
    {
        return GetContainerPath(new AssetPPtr(fileInst.path, 0, pathId));
        // 将 fileInst.path 与 pathId 包装为 AssetPPtr 并调用另一重载（保留英文原名：AssetPPtr）
    }

    public string? GetContainerPath(AssetPPtr assetPPtr)
    // 实例方法 GetContainerPath：根据 AssetPPtr 在 AssetMap 中查找对应路径（保留英文原名：GetContainerPath）
    {
        ContainerAssetInfo search = new ContainerAssetInfo(assetPPtr);
        // 构造一个临时的 ContainerAssetInfo 用于查找（保留英文原名：search）

        if (AssetMap.TryGetValue(search, out string? path))
        {
            return path;
        }
        // 如果在 AssetMap 中找到对应项则返回路径（path）

        return null;
        // 未找到则返回 null
    }

    public ContainerAssetInfo GetContainerInfo(string path)
    // 实例方法 GetContainerInfo：根据路径字符串查找并返回对应的 ContainerAssetInfo（保留英文原名：GetContainerInfo）
    {
        return AssetMap.FirstOrDefault(i => i.Value.Equals(path, StringComparison.InvariantCultureIgnoreCase)).Key;
        // 在 AssetMap 中按值（路径）查找忽略大小写的匹配，返回匹配项的 Key（ContainerAssetInfo），若无匹配返回默认 Key（null）（保留英文原名：FirstOrDefault）
    }

    // if an assets file, file can be any opened file. if a bundle file, it should be _that_ bundle file.
    // 注释：说明参数 file 的语义：若是 assets 文件可以是任意打开的文件；若是 bundle 文件应为该 bundle（保留英文原注释）

    public static bool TryGetBundleContainerBaseField(
        Workspace workspace, AssetsFileInstance file,
        [MaybeNullWhen(false)] out AssetsFileInstance actualFile,
        [MaybeNullWhen(false)] out AssetTypeValueField baseField
    )
    // 静态方法 TryGetBundleContainerBaseField：尝试在给定文件中找到 AssetBundle 的 base field 并返回实际文件与 baseField（保留英文原名：TryGetBundleContainerBaseField）
    {
        actualFile = null;
        // 初始化 out 参数 actualFile 为 null

        baseField = null;
        // 初始化 out 参数 baseField 为 null

        List<AssetFileInfo> assetBundleInfos = file.file.GetAssetsOfType(AssetClassID.AssetBundle);
        // 在 file 中查找所有类型为 AssetBundle 的 AssetFileInfo（保留英文原名：GetAssetsOfType / AssetClassID.AssetBundle）

        if (assetBundleInfos.Count == 0)
            return false;
        // 如果没有找到 AssetBundle 类型则返回 false

        baseField = workspace.GetBaseField(file, assetBundleInfos[0].PathId);
        // 使用 workspace.GetBaseField 获取第一个 AssetBundle 的 base field（保留英文原名：GetBaseField）

        if (baseField == null)
            return false;
        // 如果无法获取 baseField 则返回 false

        actualFile = file;
        // 将 actualFile 设为传入的 file

        return true;
        // 成功则返回 true
    }

    public static bool TryGetRsrcManContainerBaseField(
        Workspace workspace, AssetsFileInstance file,
        [MaybeNullWhen(false)] out AssetsFileInstance actualFile,
        [MaybeNullWhen(false)] out AssetTypeValueField baseField
    )
    // 静态方法 TryGetRsrcManContainerBaseField：尝试从 globalgamemanagers 中获取 ResourceManager 的 base field（保留英文原名：TryGetRsrcManContainerBaseField）
    {
        actualFile = null;
        // 初始化 actualFile 为 null

        baseField = null;
        // 初始化 baseField 为 null

        string gameDir = PathUtils.GetAssetsFileDirectory(file);
        // 使用 PathUtils 获取 assets 文件所在的游戏目录（保留英文原名：PathUtils.GetAssetsFileDirectory）

        if (gameDir == null)
        {
            return false;
        }
        // 如果无法确定游戏目录则返回 false

        // todo: what about mainData?
        // 注释：TODO：关于 mainData 的处理（保留英文原注释）

        string ggmPath = Path.Combine(gameDir, "globalgamemanagers");
        // 构建 globalgamemanagers 文件的路径（保留英文原名：ggmPath）

        if (!File.Exists(ggmPath))
        {
            return false;
        }
        // 如果 globalgamemanagers 文件不存在则返回 false

        // this intentionally does not add to the workspace file list
        // if the user loads this file themselves, it will reuse the
        // currently open ggm file+stream.
        // 注释：说明接下来加载 ggm 时不会把它加入 Workspace 的文件列表（保留英文原注释）

        AssetsFileInstance ggmInst;
        // 声明局部变量 ggmInst（保留英文原名：ggmInst）

        int ggmIndex = workspace.Manager.Files.FindIndex(f => f.path == ggmPath);
        // 在 Manager.Files 中查找是否已有该路径的文件实例（保留英文原名：FindIndex / Manager.Files）

        if (ggmIndex != -1)
        {
            ggmInst = workspace.Manager.Files[ggmIndex];
        }
        else
        {
            ggmInst = workspace.Manager.LoadAssetsFile(ggmPath, true);
        }
        // 如果已加载则复用已有实例，否则调用 Manager.LoadAssetsFile 加载该文件（保留英文原名：LoadAssetsFile）

        List<AssetFileInfo> resourceManagerInfos = ggmInst.file.GetAssetsOfType(AssetClassID.ResourceManager);
        // 在 ggmInst 中查找 ResourceManager 类型的 AssetFileInfo（保留英文原名：ResourceManager）

        if (resourceManagerInfos.Count == 0)
        {
            return false;
        }
        // 如果没有找到 ResourceManager 则返回 false

        baseField = workspace.GetBaseField(ggmInst, 0, resourceManagerInfos[0].PathId);
        // 使用 workspace.GetBaseField 获取 ResourceManager 的 base field（保留英文原名：GetBaseField）

        if (baseField != null)
        {
            actualFile = ggmInst;
            return true;
        }
        // 如果成功获取 baseField，则设置 actualFile 并返回 true

        return false;
        // 否则返回 false
    }
}
// ContainerTool 类结束

public class ContainerAssetInfo
// 定义公共类 ContainerAssetInfo：表示容器中单个资产的信息（保留英文原名：ContainerAssetInfo）
{
    public int PreloadIndex;
    // 公共字段 PreloadIndex：预加载索引（保留英文原名：PreloadIndex）

    public int PreloadSize;
    // 公共字段 PreloadSize：预加载大小（保留英文原名：PreloadSize）

    public AssetPPtr Ptr;
    // 公共字段 Ptr：指向实际资产的 AssetPPtr（保留英文原名：Ptr / AssetPPtr）

    public object Name;
    // 公共字段 Name：名称或占位对象（类型为 object，具体含义视上下文）（保留英文原名：Name）

    public ContainerAssetInfo(AssetPPtr asset)
    // 构造函数 ContainerAssetInfo(AssetPPtr)：使用 AssetPPtr 初始化（保留英文原名：ContainerAssetInfo）
    {
        PreloadIndex = -1;
        // 将 PreloadIndex 初始化为 -1（表示未知）（保留英文原名：PreloadIndex）

        PreloadSize = -1;
        // 将 PreloadSize 初始化为 -1（表示未知）（保留英文原名：PreloadSize）

        Ptr = asset;
        // 将 Ptr 设为传入的 asset（保留英文原名：Ptr）

        Name = string.Empty;
        // 将 Name 初始化为空字符串（保留英文原名：Name）
    }

    public ContainerAssetInfo(int preloadIndex, int preloadSize, AssetPPtr asset)
    // 构造函数 ContainerAssetInfo(索引, 大小, AssetPPtr)：显式设置 preloadIndex 与 preloadSize（保留英文原名：ContainerAssetInfo）
    {
        PreloadIndex = preloadIndex;
        // 将 PreloadIndex 设为传入值（保留英文原名：PreloadIndex）

        PreloadSize = preloadSize;
        // 将 PreloadSize 设为传入值（保留英文原名：PreloadSize）

        Ptr = asset;
        // 将 Ptr 设为传入的 asset（保留英文原名：Ptr）

        Name = string.Empty;
        // 将 Name 初始化为空字符串（保留英文原名：Name）
    }

    public static ContainerAssetInfo FromField(AssetTypeValueField field)
    // 静态方法 FromField：从 AssetTypeValueField 解析出 ContainerAssetInfo（保留英文原名：FromField）
    {
        int preloadIndex = field["preloadIndex"].AsInt;
        // 从字段读取 preloadIndex（保留英文原名：preloadIndex）

        int preloadSize = field["preloadSize"].AsInt;
        // 从字段读取 preloadSize（保留英文原名：preloadSize）

        AssetPPtr asset = AssetPPtr.FromField(field["asset"]);
        // 从字段读取 asset 并解析为 AssetPPtr（保留英文原名：asset / AssetPPtr.FromField）

        return new ContainerAssetInfo(preloadIndex, preloadSize, asset);
        // 返回新的 ContainerAssetInfo 实例
    }

    public override bool Equals(object? obj)
    // 重写 Equals：用于比较两个 ContainerAssetInfo 是否相等（保留英文原名：Equals）
    {
        if (obj is not ContainerAssetInfo assetInfo)
            return false;
        // 如果 obj 不是 ContainerAssetInfo，则返回 false

        return assetInfo.Ptr.Equals(Ptr);
        // 通过比较 Ptr（AssetPPtr）来判断相等性（保留英文原名：Ptr / Equals）
    }

    public override int GetHashCode()
    // 重写 GetHashCode：返回基于 Ptr 的哈希码（保留英文原名：GetHashCode）
    {
        return Ptr.GetHashCode();
        // 返回 Ptr 的哈希码（保留英文原名：GetHashCode）
    }
}
// ContainerAssetInfo 类结束
