using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET.Extra（AssetsTools.NET.Extra），提供对 Unity 资产扩展类型的访问，例如 AssetsFileInstance 等

using System.Diagnostics.CodeAnalysis;
// 引用 System.Diagnostics.CodeAnalysis（System.Diagnostics.CodeAnalysis），用于可空性与成员设置相关的注解（例如 SetsRequiredMembers）

namespace UABEANext4.Logic.AssetInfo;
// 定义命名空间 UABEANext4.Logic.AssetInfo（UABEANext4.Logic.AssetInfo），用于组织与资产信息显示相关的类型

public class GeneralInfo
// 定义公共类 GeneralInfo（GeneralInfo），用于封装并展示一个 AssetsFileInstance 的通用元信息
{
    // 类体开始（GeneralInfo）

    public required string MetadataSize { get; set; }
    // 必需属性 MetadataSize（MetadataSize）：表示元数据区大小（字符串形式），使用 C# 11 的 required 表示初始化时必须赋值

    public required string FileSize { get; set; }
    // 必需属性 FileSize（FileSize）：表示文件总大小（字符串形式），required 表示必须在初始化时提供

    public required string Format { get; set; }
    // 必需属性 Format（Format）：表示文件格式/版本（字符串形式），例如 Unity 版本号或文件版本

    public required string FirstFileOffset { get; set; }
    // 必需属性 FirstFileOffset（FirstFileOffset）：表示第一个数据块在文件中的偏移（字符串形式）

    public required string Endianness { get; set; }
    // 必需属性 Endianness（Endianness）：表示字节序（"Big endian" 或 "Little endian"），以字符串形式保存

    public required string EngineVersion { get; set; }
    // 必需属性 EngineVersion（EngineVersion）：表示 Unity 引擎版本（字符串形式），来自文件元数据

    public required string Platform { get; set; }
    // 必需属性 Platform（Platform）：表示目标平台信息（字符串形式），例如 "Android (11)" 或 "Windows" 等

    public required string TypeTreeEnabled { get; set; }
    // 必需属性 TypeTreeEnabled（TypeTreeEnabled）：表示 TypeTree 是否启用（"Enabled" / "Disabled"），字符串形式

    [SetsRequiredMembers]
    // 特性 SetsRequiredMembers（SetsRequiredMembers）：标记构造函数会设置所有 required 成员，避免编译器警告

    public GeneralInfo(AssetsFileInstance file)
    // 构造函数 GeneralInfo(AssetsFileInstance file)：接收一个 AssetsFileInstance（file）并从中提取通用信息初始化属性
    {
        // 构造函数体开始

        var header = file.file.Header;
        // 从传入的 file（AssetsFileInstance）中获取底层的 Header（header），包含文件头信息（MetadataSize、FileSize、Version 等）

        var metadata = file.file.Metadata;
        // 从 file 中获取 Metadata（metadata），包含 Unity 版本、平台、TypeTreeEnabled 等元数据

        MetadataSize = header.MetadataSize.ToString();
        // 将 header.MetadataSize 转为字符串并赋值给 MetadataSize（MetadataSize），用于显示或绑定

        FileSize = header.FileSize.ToString();
        // 将 header.FileSize 转为字符串并赋值给 FileSize（FileSize）

        Format = header.Version.ToString();
        // 将 header.Version（文件格式/版本）转为字符串并赋值给 Format（Format）

        FirstFileOffset = header.DataOffset.ToString();
        // 将 header.DataOffset（第一个数据块偏移）转为字符串并赋值给 FirstFileOffset（FirstFileOffset）

        Endianness = header.Endianness ? "Big endian" : "Little endian";
        // 根据 header.Endianness（布尔）决定 Endianness 字符串：true -> "Big endian"，false -> "Little endian"

        EngineVersion = metadata.UnityVersion;
        // 从 metadata.UnityVersion 读取引擎版本字符串并赋值给 EngineVersion（EngineVersion）

        Platform = $"{(BuildTarget)metadata.TargetPlatform} ({metadata.TargetPlatform})";
        // 将 metadata.TargetPlatform（数值）先转换为枚举 BuildTarget 的名称，再与数值一起格式化为字符串赋给 Platform（Platform）
        // 说明：使用 (BuildTarget) 强制转换以显示可读平台名，保留原始数值以便调试或显示

        TypeTreeEnabled = metadata.TypeTreeEnabled ? "Enabled" : "Disabled";
        // 根据 metadata.TypeTreeEnabled（布尔）设置 TypeTreeEnabled 为 "Enabled" 或 "Disabled"
    }
    // 构造函数体结束

    private GeneralInfo()
    // 私有无参构造函数 GeneralInfo()：用于内部创建空实例（例如 Empty 单例），不从文件初始化
    {
    }
    // 私有构造函数结束

    public static GeneralInfo Empty { get; } = new()
    // 公共静态只读属性 Empty（Empty）：提供一个空的 GeneralInfo 实例作为默认值或占位符
    {
        MetadataSize = string.Empty,
        // 将 Empty.MetadataSize 初始化为空字符串（MetadataSize）

        FileSize = string.Empty,
        // 将 Empty.FileSize 初始化为空字符串（FileSize）

        Format = string.Empty,
        // 将 Empty.Format 初始化为空字符串（Format）

        FirstFileOffset = string.Empty,
        // 将 Empty.FirstFileOffset 初始化为空字符串（FirstFileOffset）

        Endianness = string.Empty,
        // 将 Empty.Endianness 初始化为空字符串（Endianness）

        EngineVersion = string.Empty,
        // 将 Empty.EngineVersion 初始化为空字符串（EngineVersion）

        Platform = string.Empty,
        // 将 Empty.Platform 初始化为空字符串（Platform）

        TypeTreeEnabled = string.Empty,
        // 将 Empty.TypeTreeEnabled 初始化为空字符串（TypeTreeEnabled）
    };
    // Empty 初始化结束

}
// 类体结束（GeneralInfo）
