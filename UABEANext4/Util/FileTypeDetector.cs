using AssetsTools.NET; // 引用 AssetsTools.NET 库，用于读取 Unity 资产文件的低层 API（保留英文原名 AssetsTools.NET）

using System.IO; // 引用 System.IO 命名空间，用于文件流操作（保留英文原名 System.IO）

using System.Text.RegularExpressions; // 引用正则表达式命名空间，用于对版本字符串做模式匹配与过滤（保留英文原名 System.Text.RegularExpressions）

namespace UABEANext4.Util; // 定义命名空间 UABEANext4.Util，用于放置工具类（保留英文原名 UABEANext4.Util）

public static class FileTypeDetector // 定义公共静态类 FileTypeDetector，封装检测文件类型的静态方法（保留英文原名 FileTypeDetector）
{ // 类体开始

    public static DetectedFileType DetectFileType(string filePath) // 公共静态方法：通过文件路径检测文件类型，返回 DetectedFileType 枚举（保留英文原名 DetectFileType）
    { // 方法体开始
        using (FileStream fs = File.OpenRead(filePath)) // 打开指定路径的文件为只读 FileStream，并在 using 块结束时自动关闭流
        using (AssetsFileReader r = new AssetsFileReader(fs)) // 使用 AssetsFileReader 包装 FileStream，以便按 Unity 资产格式读取数据
        { // 嵌套 using 块开始
            return DetectFileType(r, 0); // 调用重载方法 DetectFileType(AssetsFileReader, long) 从文件起始地址 0 开始检测并返回结果
        } // 嵌套 using 块结束（AssetsFileReader 与 FileStream 在此处被释放）
    } // 方法结束

    public static DetectedFileType DetectFileType(AssetsFileReader r, long startAddress) // 公共静态方法：在给定的 AssetsFileReader 和起始地址处检测文件类型（保留英文原名 DetectFileType）
    { // 方法体开始
        string possibleBundleHeader; // 局部变量：可能的 bundle 文件头字符串（例如 "UnityFS"）
        int possibleFormat; // 局部变量：可能的格式号（从文件头读取的整数）
        string emptyVersion, fullVersion; // 局部变量：用于保存过滤后的版本字符串（空字符过滤与非字母数字过滤的两个版本）

        r.BigEndian = true; // 将读取器设置为大端模式以便安全读取头部字段（先假设大端以便读取固定偏移处的字节）

        if (r.BaseStream.Length < 0x20) // 如果流长度小于 0x20（32 字节），文件太小无法判断
        { // 条件块开始
            return DetectedFileType.Unknown; // 返回 Unknown（未知类型）
        } // 条件块结束

        r.Position = startAddress; // 将读取器位置设置到起始地址（通常为 0）
        possibleBundleHeader = r.ReadStringLength(7); // 从当前位置读取长度为 7 的字符串，可能是 bundle 的头部标识（例如 "UnityFS"）
        r.Position = startAddress + 0x08; // 将读取位置移动到起始地址 + 0x08（通常是 format 字段所在位置）
        possibleFormat = r.ReadInt32(); // 读取一个 32 位整数作为可能的格式号（possibleFormat）

        r.Position = startAddress + (possibleFormat >= 0x16 ? 0x30 : 0x14); // 根据 possibleFormat 的值选择不同的偏移来读取版本字符串：如果 format >= 0x16 则偏移 0x30，否则偏移 0x14

        string possibleVersion = ""; // 局部变量：累积读取到的可能版本字符串
        char curChar; // 局部变量：当前读取的字符
        while (r.Position < r.BaseStream.Length && (curChar = (char)r.ReadByte()) != 0x00) // 循环读取字节并转换为字符，直到遇到字符串结束符 0x00 或到达流末尾
        { // 循环体开始
            possibleVersion += curChar; // 将读取到的字符追加到 possibleVersion 字符串中
            if (possibleVersion.Length > 0xFF) // 如果读取的版本字符串长度超过 0xFF（255），认为异常或过长，跳出以避免无限增长
            {
                break; // 跳出循环
            }
        } // 循环体结束

        emptyVersion = Regex.Replace(possibleVersion, "[a-zA-Z0-9\\-\\.\\n]", ""); // 使用正则将字母、数字、连字符、点和换行保留以外的字符移除后得到 emptyVersion（这里保留的字符被替换掉后剩余的即为“非法字符”）
        fullVersion = Regex.Replace(possibleVersion, "[^a-zA-Z0-9\\-\\.\\n]", ""); // 使用正则移除所有非字母数字、连字符、点和换行的字符，得到 fullVersion（只保留合法字符）

        if (possibleBundleHeader == "UnityFS") // 如果读取到的 7 字节头部等于 "UnityFS"
        {
            return DetectedFileType.BundleFile; // 则判定为 BundleFile（Unity 的 bundle 包）
        }
        else if (possibleFormat < 0xFF && emptyVersion.Length == 0 && fullVersion.Length >= 5) // 否则如果 possibleFormat 小于 0xFF，且 emptyVersion 为空（表示没有非法字符），并且 fullVersion 长度至少 5（版本字符串看起来合理）
        {
            return DetectedFileType.AssetsFile; // 则判定为 AssetsFile（Unity 的 assets 文件）
        }
        return DetectedFileType.Unknown; // 其他情况返回 Unknown（未知类型）
    } // 方法结束
} // 类结束

public enum DetectedFileType // 定义枚举 DetectedFileType，表示检测到的文件类型（保留英文原名 DetectedFileType）
{ // 枚举体开始
    Unknown, // 未知（Unknown）
    AssetsFile, // Assets 文件（AssetsFile）
    BundleFile // Bundle 文件（BundleFile）
} // 枚举体结束
