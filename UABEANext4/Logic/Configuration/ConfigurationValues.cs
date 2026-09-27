using Avalonia;
// 引用 Avalonia 框架的根命名空间，用于访问应用级别类型（原名：Avalonia）

using Avalonia.Styling;
// 引用 Avalonia 的样式相关命名空间，用于 ThemeVariant 等样式/主题类型（原名：Avalonia.Styling）

using CommunityToolkit.Mvvm.ComponentModel;
// 引用 CommunityToolkit MVVM 的组件模型命名空间，提供 ObservableObject 与属性生成特性（原名：CommunityToolkit.Mvvm.ComponentModel）

using System;
// 引用基础系统命名空间，提供常用类型（如 Action、Attribute 等）（原名：System）

using System.ComponentModel;
// 引用组件模型命名空间，提供 PropertyChangedEventArgs 等（原名：System.ComponentModel）

using UABEANext4.Util;
// 引用项目内的工具/实用程序命名空间（原名：UABEANext4.Util）

namespace UABEANext4.Logic.Configuration;
// 定义命名空间 UABEANext4.Logic.Configuration，用于组织配置相关逻辑（原名：UABEANext4.Logic.Configuration）

public partial class ConfigurationValues : ObservableObject
// 定义部分类 ConfigurationValues，继承自 ObservableObject（用于属性变更通知）（原名：ConfigurationValues / ObservableObject）
{
    // 类体开始（ConfigurationValues）

    [ObservableProperty]
    // 特性（Attribute）：由 CommunityToolkit 自动生成对应的属性与通知代码（原名：ObservableProperty）

    [property: ConfigTitle("主题类型 (Theme Type)")]
    // 特性（Attribute）：为该配置项提供标题元数据（原名：ConfigTitle，显示文本 "主题类型 (Theme Type)"）

    [property: ConfigDesc("要使用的界面主题。 (The theme to use.)")]
    // 特性（Attribute）：为该配置项提供描述元数据（原名：ConfigDesc，显示文本 "要使用的界面主题。 (The theme to use.)"）

    private ConfigurationThemeType _themeType = ConfigurationThemeType.Auto;
    // 私有字段（由 ObservableProperty 生成公开属性 ThemeType）：表示主题类型（类型：ConfigurationThemeType，默认值 Auto）
    // 说明：外部通过生成的 ThemeType 属性访问；字段名为 _themeType（原名：_themeType / ConfigurationThemeType / Auto）

    [ObservableProperty]
    // 特性：为下一个字段生成属性与通知（原名：ObservableProperty）

    [property: ConfigTitle("优先使用 Managed 代码而非 IL2CPP (Use Managed over IL2CPP)")]
    // 特性：为该配置项提供标题（原名：ConfigTitle，显示文本 "优先使用 Managed 代码而非 IL2CPP (Use Managed over IL2CPP)"）

    [property: ConfigDesc("如果存在 Managed 文件夹则优先使用，而不是使用 CPP2IL。 (Use the Managed folder if it exists, rather than use CPP2IL.)")]
    // 特性：为该配置项提供描述（原名：ConfigDesc，显示文本 "如果存在 Managed 文件夹则优先使用，而不是使用 CPP2IL。 (Use the Managed folder if it exists, rather than use CPP2IL.)"）

    private bool _useManagedOverIl2cpp = false;
    // 私有布尔字段（由 ObservableProperty 生成公开属性 UseManagedOverIl2cpp）：表示是否优先使用 Managed 文件夹（字段名：_useManagedOverIl2cpp，默认 false）

    [ObservableProperty]
    // 特性：为下一个字段生成属性与通知（原名：ObservableProperty）

    [property: ConfigTitle("列表文件名长度限制 (Listing Filename Length Limit)")]
    // 特性：为该配置项提供标题（原名：ConfigTitle，显示文本 "列表文件名长度限制 (Listing Filename Length Limit)"）

    [property: ConfigDesc("生成资产列表时资产名称的最大长度。 (Maximum length for the asset name when generating asset list.)")]
    // 特性：为该配置项提供描述（原名：ConfigDesc，显示文本 "生成资产列表时资产名称的最大长度。 (Maximum length for the asset name when generating asset list.)"）

    [property: ConfigRange(0, int.MaxValue)]
    // 特性：为该配置项提供取值范围（原名：ConfigRange，最小 0，最大 int.MaxValue）

    private int _listingNameLength = 300;
    // 私有整数字段（由 ObservableProperty 生成公开属性 ListingNameLength）：表示生成资产列表时名称的最大长度（字段名：_listingNameLength，默认 300）

    [ObservableProperty]
    // 特性：为下一个字段生成属性与通知（原名：ObservableProperty）

    [property: ConfigTitle("导出文件名长度限制 (Export Filename Length Limit)")]
    // 特性：为该配置项提供标题（原名：ConfigTitle，显示文本 "导出文件名长度限制 (Export Filename Length Limit)"）

    [property: ConfigDesc("导出资产时文件名的最大长度。 (Maximum length for the asset name when exporting assets.)")]
    // 特性：为该配置项提供描述（原名：ConfigDesc，显示文本 "导出资产时文件名的最大长度。 (Maximum length for the asset name when exporting assets.)"）

    [property: ConfigRange(0, int.MaxValue)]
    // 特性：为该配置项提供取值范围（原名：ConfigRange，最小 0，最大 int.MaxValue）

    private int _exportNameLength = 150;
    // 私有整数字段（由 ObservableProperty 生成公开属性 ExportNameLength）：表示导出资产时文件名的最大长度（字段名：_exportNameLength，默认 150）

    private readonly Action<int> _saveDebounceFunc = DebounceUtils.Debounce(
        (int _) => ConfigurationManager.SaveConfig(), 500);
    // 私有只读字段 _saveDebounceFunc：保存配置的防抖（debounce）函数（类型 Action<int>）
    // 说明：使用 DebounceUtils.Debounce 创建一个延迟 500ms 的调用包装，实际回调为 ConfigurationManager.SaveConfig()（原名：DebounceUtils / Debounce / ConfigurationManager.SaveConfig）

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    // 重写 ObservableObject 的 OnPropertyChanged 方法（在任何生成的属性改变时被调用）（原名：OnPropertyChanged / PropertyChangedEventArgs）
    {
        // 方法体开始（OnPropertyChanged）

        base.OnPropertyChanged(e);
        // 调用基类实现以触发标准的属性变更通知流程（原名：base.OnPropertyChanged）

        // special case: theme updates immediately
        // 注释：特殊处理：主题更改需要立即生效（保留英文原注释）

        if (e.PropertyName == nameof(ThemeType) && Application.Current is not null)
        // 如果触发的属性是 ThemeType（由 ObservableProperty 生成的公开属性名）且当前有运行中的 Application 实例
        {
            // 条件块开始

            Application.Current.RequestedThemeVariant = ThemeType switch
            {
                ConfigurationThemeType.Auto => ThemeVariant.Default,
                ConfigurationThemeType.Light => ThemeVariant.Light,
                ConfigurationThemeType.Dark => ThemeVariant.Dark,
                _ => ThemeVariant.Default // shouldn't happen
            };
            // 根据 ThemeType 的值立即设置 Application.Current.RequestedThemeVariant（将自定义枚举映射到 Avalonia 的 ThemeVariant）
            // 说明：ConfigurationThemeType（Auto/Light/Dark）映射到 ThemeVariant（Default/Light/Dark）；默认分支不应发生
        }
        // 条件块结束

        _saveDebounceFunc(0);
        // 调用防抖保存函数（传入任意 int 参数，这里传 0），以延迟方式保存配置，避免频繁磁盘写入（原名：_saveDebounceFunc）
    }
    // 方法体结束（OnPropertyChanged）

}
// 类体结束（ConfigurationValues）

public class ConfigTitle(string title) : Attribute
// 定义特性类 ConfigTitle，用于为配置项提供标题元数据（构造参数 title 保留英文原名）
{
    public string Title { get; } = title;
    // 只读属性 Title：返回构造时传入的标题字符串（原名：Title）
}

public class ConfigDesc(string description) : Attribute
// 定义特性类 ConfigDesc，用于为配置项提供描述元数据（构造参数 description 保留英文原名）
{
    public string Description { get; } = description;
    // 只读属性 Description：返回构造时传入的描述字符串（原名：Description）
}

public class ConfigRange(int min, int max) : Attribute
// 定义特性类 ConfigRange，用于为配置项提供取值范围（构造参数 min、max 保留英文原名）
{
    public int Minimum { get; } = min;
    // 只读属性 Minimum：返回最小值（原名：Minimum）

    public int Maximum { get; } = max;
    // 只读属性 Maximum：返回最大值（原名：Maximum）
}
