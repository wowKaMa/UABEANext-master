using AssetsTools.NET.Extra; // 引用 AssetsTools.NET.Extra 库，提供与 Unity 资产文件相关的扩展类型（保留英文原名 AssetsTools.NET.Extra）

using System.IO; // 引用 System.IO 命名空间，用于路径与文件操作（保留英文原名 System.IO）

namespace UABEANext4.Util; // 定义命名空间 UABEANext4.Util，用于放置工具类（保留英文原名 UABEANext4.Util）

public static class PathUtils // 定义公共静态类 PathUtils，封装路径相关的实用方法（保留英文原名 PathUtils）
{
    // https://stackoverflow.com/a/23182807
    // 注释：来源链接，说明 ReplaceInvalidPathChars 的实现参考自该 StackOverflow 回答（保留英文原文链接）

    public static string ReplaceInvalidPathChars(string filename) // 方法：ReplaceInvalidPathChars，参数 filename（要处理的文件名），返回处理后的字符串
    {
        return string.Join("_", filename.Split(Path.GetInvalidFileNameChars())); // 将 filename 按操作系统不合法的文件名字符拆分，然后用下划线 "_" 连接各段，返回替换后的文件名
    }

    public static string GetFilePathWithoutExtension(string path) // 方法：GetFilePathWithoutExtension，参数 path（包含路径的文件名），返回不带扩展名的完整路径
    {
        string? directoryName = Path.GetDirectoryName(path); // 获取 path 的目录部分（可能为 null），保存到 directoryName

        if (directoryName != null) // 如果目录存在（非 null）
        {
            return Path.Combine(directoryName, Path.GetFileNameWithoutExtension(path)); // 返回由目录与不带扩展名的文件名组合而成的路径
        }

        return string.Empty; // 如果无法获取目录（例如 path 为 null 或无目录），返回空字符串
    }

    public static string GetAssetsFileDirectory(AssetsFileInstance fileInst) // 方法：GetAssetsFileDirectory，参数 fileInst（AssetsFileInstance），返回该资产文件所在的目录路径
    {
        if (fileInst.parentBundle != null) // 如果该文件属于一个 bundle（parentBundle 非 null）
        {
            string dir = Path.GetDirectoryName(fileInst.parentBundle.path)!; // 取 parentBundle 的路径的目录作为初始目录（使用 ! 表示断言非 null）

            // addressables
            // 注释：下面的逻辑用于处理 Addressables 打包结构的特殊目录层级

            string? upDir = Path.GetDirectoryName(dir); // 获取 dir 的上一级目录
            string? upDir2 = Path.GetDirectoryName(upDir ?? string.Empty); // 获取上两级目录（若 upDir 为 null 则传空字符串以避免异常）
            if (upDir != null && upDir2 != null) // 如果上一级与上两级目录都存在
            {
                if (Path.GetFileName(upDir) == "aa" && Path.GetFileName(upDir2) == "StreamingAssets") // 如果上一级目录名为 "aa" 且上两级目录名为 "StreamingAssets"
                {
                    dir = Path.GetDirectoryName(upDir2)!; // 则将 dir 指向上两级目录的上一级（即跳过 aa 与 StreamingAssets 层），用于处理 addressables 的目录结构
                }
            }

            return dir; // 返回处理后的目录路径
        }
        else // 如果 fileInst 没有 parentBundle（即不是 bundle 内的文件）
        {
            string dir = Path.GetDirectoryName(fileInst.path)!; // 获取 fileInst.path 的目录部分并断言非 null

            if (fileInst.name == "unity default resources" || fileInst.name == "unity_builtin_extra") // 如果文件名是 Unity 的内置资源文件名之一
            {
                dir = Path.GetDirectoryName(dir)!; // 则返回上一级目录（这些内置文件通常位于特殊子目录，向上一级才是项目资源目录）
            }

            return dir; // 返回目录路径
        }
    }
}
