namespace UABEANext4.AssetWorkspace;
// 定义命名空间 UABEANext4.AssetWorkspace，用于组织与工作区（Workspace）相关的类型（保留英文原名：UABEANext4.AssetWorkspace）

public enum WorkspaceItemType
// 定义公共枚举 WorkspaceItemType，表示工作区中项的类型（保留英文原名：WorkspaceItemType）
{
    // 枚举体开始（WorkspaceItemType）

    BundleFile,
    // 枚举成员 BundleFile：表示一个 bundle 文件（例如 Unity 的 asset bundle）；用于区分工作区项类型（保留英文原名：BundleFile）

    AssetsFile,
    // 枚举成员 AssetsFile：表示一个 .assets 文件（Unity 的资源文件）；用于区分工作区项类型（保留英文原名：AssetsFile）

    ResourceFile,
    // 枚举成员 ResourceFile：表示资源文件（可能是游戏资源或其他非 .assets 的资源）；用于区分工作区项类型（保留英文原名：ResourceFile）

    OtherFile
    // 枚举成员 OtherFile：表示其他类型的文件（不属于上面三类）；作为通用/回退类型使用（保留英文原名：OtherFile）
}

public enum BundleSaveMethod
{
    None,
    LZMA,
    LZ4,
    ChunkedLZMA
}

// 枚举体结束（WorkspaceItemType）
