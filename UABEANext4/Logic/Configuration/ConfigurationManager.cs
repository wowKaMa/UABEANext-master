using System;
// 引用基础系统命名空间（System），提供常用类型与运行时功能（例如 AppDomain、Exception 等）

using System.IO;
// 引用输入/输出命名空间（System.IO），提供文件与路径操作（例如 File、Path、Stream 等）

using System.Text.Json;
// 引用 System.Text.Json 命名空间，用于 JSON 序列化与反序列化（JsonSerializer 等）

using System.Text.Json.Serialization;
// 引用 System.Text.Json 的序列化相关命名空间，提供 JsonStringEnumConverter 等转换器（JsonStringEnumConverter）

using UABEANext4.Util;
// 引用项目内的工具/实用程序命名空间（UABEANext4.Util），用于调用工具函数（例如 DebounceUtils、ConfigurationManager 等）

namespace UABEANext4.Logic.Configuration;
// 定义命名空间 UABEANext4.Logic.Configuration，用于组织配置管理相关类型（ConfigurationManager、ConfigurationValues 等）

public static class ConfigurationManager
// 定义公共静态类 ConfigurationManager（ConfigurationManager），负责加载与保存应用配置（单例式静态管理）
{
    // 类体开始（ConfigurationManager）

    public const string CONFIG_FILENAME = "config.json";
    // 定义公共常量 CONFIG_FILENAME（CONFIG_FILENAME），表示配置文件名 "config.json"

    public static ConfigurationValues Settings { get; }
    // 定义公共静态只读属性 Settings（Settings），类型为 ConfigurationValues，保存当前运行时的配置值

    public static bool IsInitialized { get; }
    // 定义公共静态只读属性 IsInitialized（IsInitialized），表示配置管理器是否已完成初始化

    private static readonly JsonSerializerOptions OPTIONS = new()
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
    // 定义私有静态只读字段 OPTIONS（OPTIONS），用于 JSON 序列化/反序列化的选项
    // 在此选项中添加了 JsonStringEnumConverter（将枚举以驼峰式字符串序列化/反序列化）

    static ConfigurationManager()
    // 静态构造函数（ConfigurationManager 的静态初始化器），在首次访问该类的任何成员前执行一次
    {
        // 静态构造体开始

        var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIG_FILENAME);
        // 计算配置文件的完整路径（configPath）：将应用程序基目录（AppDomain.CurrentDomain.BaseDirectory）与 CONFIG_FILENAME 组合

        if (!File.Exists(configPath))
        {
            Settings = new ConfigurationValues();
            IsInitialized = true;
        }
        else
        {
            var configText = File.ReadAllText(configPath);
            Settings = JsonSerializer.Deserialize<ConfigurationValues>(configText, OPTIONS)
                ?? new ConfigurationValues();

            IsInitialized = true;
        }
        // 如果配置文件不存在，则创建默认的 ConfigurationValues 实例并将 IsInitialized 设为 true
        // 如果配置文件存在，则读取文件内容（configText），使用 JsonSerializer 和前面定义的 OPTIONS 反序列化为 ConfigurationValues 并赋值给 Settings
        // 如果反序列化返回 null，则回退为新的 ConfigurationValues 实例
        // 最后将 IsInitialized 设为 true 表示初始化完成

    }
    // 静态构造体结束

    public static void SaveConfig()
    // 公共静态方法 SaveConfig（SaveConfig）：将当前 Settings 序列化并写回到磁盘上的配置文件
    {
        if (!IsInitialized)
            return;
        // 如果尚未初始化（IsInitialized 为 false），则不执行保存操作并直接返回

        var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIG_FILENAME);
        // 重新计算配置文件路径（与静态构造函数中相同），以确保写入到正确位置

        var configText = JsonSerializer.Serialize(Settings, OPTIONS);
        // 使用 JsonSerializer 将当前 Settings 对象序列化为 JSON 字符串（使用之前定义的 OPTIONS）

        File.WriteAllText(configPath, configText);
        // 将序列化后的 JSON 字符串写入到配置文件（覆盖原文件或创建新文件）
    }
    // SaveConfig 方法结束

}
// 类体结束（ConfigurationManager）
