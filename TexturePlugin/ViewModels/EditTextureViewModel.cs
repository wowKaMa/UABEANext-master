using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于处理 Unity 资产文件（保留英文原名：AssetsTools.NET）

using AssetsTools.NET.Texture;
// 引用 AssetsTools.NET 的纹理处理命名空间，提供 TextureFile、TextureFormat 等（保留英文原名：AssetsTools.NET.Texture）

using CommunityToolkit.Mvvm.ComponentModel;
// 引用 CommunityToolkit MVVM 的组件模型，提供 ObservableProperty 等特性（保留英文原名：CommunityToolkit.Mvvm.ComponentModel）

using System.ComponentModel.DataAnnotations;
// 引用数据注解命名空间，用于验证属性（保留英文原名：System.ComponentModel.DataAnnotations）

using UABEANext4.AssetWorkspace;
// 引用项目的工作区命名空间，提供 Workspace、AssetInst 等类型（保留英文原名：UABEANext4.AssetWorkspace）

using UABEANext4.Interfaces;
// 引用项目内定义的接口集合（例如 IDialogAware）（保留英文原名：UABEANext4.Interfaces）

using UABEANext4.Util;
// 引用项目的工具/实用程序命名空间（保留英文原名：UABEANext4.Util）

using UABEANext4.ViewModels;
// 引用项目的视图模型基类或相关类型（保留英文原名：UABEANext4.ViewModels）

using ColorSpaceEnm = TexturePlugin.Logic.EditTexture.ColorSpace;
// 为 ColorSpace 枚举创建别名 ColorSpaceEnm（保留英文原名：ColorSpaceEnm = TexturePlugin.Logic.EditTexture.ColorSpace）

using FilterModeEnm = TexturePlugin.Logic.EditTexture.FilterMode;
// 为 FilterMode 枚举创建别名 FilterModeEnm（保留英文原名：FilterModeEnm = TexturePlugin.Logic.EditTexture.FilterMode）

using TextureFormatEnm = AssetsTools.NET.Texture.TextureFormat;
// 为 TextureFormat 枚举创建别名 TextureFormatEnm（保留英文原名：TextureFormatEnm = AssetsTools.NET.Texture.TextureFormat）

using WrapModeEnm = TexturePlugin.Logic.EditTexture.WrapMode;
// 为 WrapMode 枚举创建别名 WrapModeEnm（保留英文原名：WrapModeEnm = TexturePlugin.Logic.EditTexture.WrapMode）

// this could be uh... improved... but it'll work for now :D
// 注释：作者自述：此处实现可以改进，但目前可用（保留英文原注释）

namespace TexturePlugin.ViewModels;
// 定义命名空间 TexturePlugin.ViewModels（保留英文原名：TexturePlugin.ViewModels）

public partial class EditTextureViewModel : ViewModelBaseValidator, IDialogAware<EditTextureResult?>
// 定义部分类 EditTextureViewModel，继承 ViewModelBaseValidator 并实现 IDialogAware，返回类型为 EditTextureResult?（保留英文原名：EditTextureViewModel / ViewModelBaseValidator / IDialogAware）
{
    // 类体开始（EditTextureViewModel）

    [ObservableProperty]
    // 特性：由 CommunityToolkit 自动生成对应的可绑定属性（保留英文原名：ObservableProperty）

    public bool _isSingleTexture;
    // 字段：是否为单个纹理（由 ObservableProperty 生成公开属性 IsSingleTexture）（保留英文原名：_isSingleTexture）

    // field properties
    // 注释：下面是各个字段对应的属性（保留英文原注释）

    [ObservableProperty]
    // 特性：生成属性并触发通知（保留英文原名：ObservableProperty）

    [NotifyPropertyChangedFor(nameof(NameWatermark))]
    // 特性：当此字段变化时也触发 NameWatermark 的属性变更通知（保留英文原名：NotifyPropertyChangedFor）

    public string? _name = "";
    // 字段：纹理名称（可空），默认空字符串；会生成公开属性 Name（保留英文原名：_name）

    [ObservableProperty]
    // 特性：生成属性（保留英文原名：ObservableProperty）

    public TextureFormatEnm? _textureFormat = TextureFormatEnm.RGBA32;
    // 字段：纹理格式（可空），默认 RGBA32；生成属性 TextureFormat（保留英文原名：_textureFormat / TextureFormatEnm.RGBA32）

    [ObservableProperty]
    // 特性：生成属性（保留英文原名：ObservableProperty）

    [NotifyPropertyChangedFor(nameof(UsingMipsThreeOn))]
    // 特性：当此字段变化时也触发 UsingMipsThreeOn 的属性变更通知（保留英文原名：NotifyPropertyChangedFor）

    public bool? _usingMips = false;
    // 字段：是否使用 mipmaps（三态：true/false/null），默认 false；生成属性 UsingMips（保留英文原名：_usingMips）

    [ObservableProperty]
    // 特性：生成属性（保留英文原名：ObservableProperty）

    [NotifyPropertyChangedFor(nameof(IsReadableThreeOn))]
    // 特性：当此字段变化时也触发 IsReadableThreeOn 的属性变更通知（保留英文原名：NotifyPropertyChangedFor）

    public bool? _isReadable = false;
    // 字段：是否可读（IsReadable 三态），默认 false；生成属性 IsReadable（保留英文原名：_isReadable）

    [ObservableProperty]
    // 特性：生成属性（保留英文原名：ObservableProperty）

    public FilterModeEnm? _filterMode = FilterModeEnm.Point;
    // 字段：过滤模式（可空），默认 Point；生成属性 FilterMode（保留英文原名：_filterMode / FilterModeEnm.Point）

    [ObservableProperty]
    // 特性：生成属性（保留英文原名：ObservableProperty）

    [NotifyPropertyChangedFor(nameof(FilteringWatermark))]
    // 特性：当此字段变化时也触发 FilteringWatermark 的属性变更通知（保留英文原名：NotifyPropertyChangedFor）

    [CustomValidation(typeof(EditTextureViewModel), nameof(ValidateInt))]
    // 特性：为该字段添加自定义验证，使用本类的 ValidateInt 方法（保留英文原名：CustomValidation）

    public string? _filteringString = "0";
    // 字段：各向异性过滤（Aniso）以字符串形式表示（可空），默认 "0"；生成属性 FilteringString（保留英文原名：_filteringString）

    [ObservableProperty]
    // 特性：生成属性（保留英文原名：ObservableProperty）

    [NotifyPropertyChangedFor(nameof(MipBiasWatermark))]
    // 特性：当此字段变化时也触发 MipBiasWatermark 的属性变更通知（保留英文原名：NotifyPropertyChangedFor）

    [CustomValidation(typeof(EditTextureViewModel), nameof(ValidateInt))]
    // 特性：为该字段添加自定义验证（保留英文原名：CustomValidation）

    public string? _mipBiasString = "0";
    // 字段：mip bias 以字符串形式表示（可空），默认 "0"；生成属性 MipBiasString（保留英文原名：_mipBiasString）

    [ObservableProperty]
    // 特性：生成属性（保留英文原名：ObservableProperty）

    public WrapModeEnm? _wrapModeU = WrapModeEnm.Repeat;
    // 字段：U 方向的 Wrap 模式（可空），默认 Repeat；生成属性 WrapModeU（保留英文原名：_wrapModeU）

    [ObservableProperty]
    // 特性：生成属性（保留英文原名：ObservableProperty）

    public WrapModeEnm? _wrapModeV = WrapModeEnm.Repeat;
    // 字段：V 方向的 Wrap 模式（可空），默认 Repeat；生成属性 WrapModeV（保留英文原名：_wrapModeV）

    [ObservableProperty]
    // 特性：生成属性（保留英文原名：ObservableProperty）

    [NotifyPropertyChangedFor(nameof(LightMapFormatWatermark))]
    // 特性：当此字段变化时也触发 LightMapFormatWatermark 的属性变更通知（保留英文原名：NotifyPropertyChangedFor）

    [CustomValidation(typeof(EditTextureViewModel), nameof(ValidateInt))]
    // 特性：为该字段添加自定义验证（保留英文原名：CustomValidation）

    public string? _lightMapFormatString = "0";
    // 字段：光照贴图格式以字符串表示（可空），默认 "0"；生成属性 LightMapFormatString（保留英文原名：_lightMapFormatString）

    [ObservableProperty]
    // 特性：生成属性（保留英文原名：ObservableProperty）

    public ColorSpaceEnm? _colorSpace = ColorSpaceEnm.Gamma;
    // 字段：颜色空间（可空），默认 Gamma；生成属性 ColorSpace（保留英文原名：_colorSpace / ColorSpaceEnm.Gamma）

    // default fields
    // 注释：下面是用于保存默认值的私有字段（保留英文原注释）

    private string? _defaultName;
    // 私有字段：保存默认名称（保留英文原名：_defaultName）

    private TextureFormatEnm? _defaultTextureFormat;
    // 私有字段：保存默认纹理格式（保留英文原名：_defaultTextureFormat）

    private bool? _defaultUsingMips;
    // 私有字段：保存默认 UsingMips（保留英文原名：_defaultUsingMips）

    private bool? _defaultIsReadable;
    // 私有字段：保存默认 IsReadable（保留英文原名：_defaultIsReadable）

    private FilterModeEnm? _defaultFilterMode;
    // 私有字段：保存默认 FilterMode（保留英文原名：_defaultFilterMode）

    private string? _defaultFilteringString;
    // 私有字段：保存默认 FilteringString（保留英文原名：_defaultFilteringString）

    private string? _defaultMipBiasString;
    // 私有字段：保存默认 MipBiasString（保留英文原名：_defaultMipBiasString）

    private WrapModeEnm? _defaultWrapModeU;
    // 私有字段：保存默认 WrapModeU（保留英文原名：_defaultWrapModeU）

    private WrapModeEnm? _defaultWrapModeV;
    // 私有字段：保存默认 WrapModeV（保留英文原名：_defaultWrapModeV）

    private string? _defaultLightMapFormatString;
    // 私有字段：保存默认 LightMapFormatString（保留英文原名：_defaultLightMapFormatString）

    private ColorSpaceEnm? _defaultColorSpace;
    // 私有字段：保存默认 ColorSpace（保留英文原名：_defaultColorSpace）

    // textbox watermarks
    // 注释：下面是用于 UI 占位文本（watermark）的只读属性（保留英文原注释）

    public string NameWatermark => Name is null ? "(Multiple values)" : "";
    // 只读属性：当 Name 为 null（多选且值不一致）时显示 "(Multiple values)"，否则为空（保留英文原名：NameWatermark / Name）

    public string FilteringWatermark => FilteringString is null ? "(Multiple values)" : "";
    // 只读属性：当 FilteringString 为 null 时显示占位文本（保留英文原名：FilteringWatermark / FilteringString）

    public string MipBiasWatermark => MipBiasString is null ? "(Multiple values)" : "";
    // 只读属性：当 MipBiasString 为 null 时显示占位文本（保留英文原名：MipBiasWatermark / MipBiasString）

    public string LightMapFormatWatermark => LightMapFormatString is null ? "(Multiple values)" : "";
    // 只读属性：当 LightMapFormatString 为 null 时显示占位文本（保留英文原名：LightMapFormatWatermark / LightMapFormatString）

    // checkbox three-states
    // 注释：下面是用于三态复选框的只读属性（保留英文原注释）

    public bool UsingMipsThreeOn => UsingMips is null;
    // 只读属性：当 UsingMips 为 null（多选且不一致）时返回 true，用于 UI 三态显示（保留英文原名：UsingMipsThreeOn / UsingMips）

    public bool IsReadableThreeOn => IsReadable is null;
    // 只读属性：当 IsReadable 为 null 时返回 true（保留英文原名：IsReadableThreeOn / IsReadable）

    // combobox options
    // 注释：下面是下拉框可选项集合（保留英文原注释）

    public static TextureFormatEnm[] TextureFormats => Enum.GetValues<TextureFormatEnm>();
    // 静态属性：返回所有 TextureFormat 枚举值，用于下拉列表（保留英文原名：TextureFormats / Enum.GetValues）

    public static FilterModeEnm[] FilterModes => Enum.GetValues<FilterModeEnm>();
    // 静态属性：返回所有 FilterMode 枚举值（保留英文原名：FilterModes）

    public static WrapModeEnm[] WrapModes => Enum.GetValues<WrapModeEnm>();
    // 静态属性：返回所有 WrapMode 枚举值（保留英文原名：WrapModes）

    public static ColorSpaceEnm[] ColorSpaces => Enum.GetValues<ColorSpaceEnm>();
    // 静态属性：返回所有 ColorSpace 枚举值（保留英文原名：ColorSpaces）

    public string Title => "Texture Edit";
    // 只读属性：对话框标题（保留英文原名：Title）

    public int Width => 300;
    // 只读属性：对话框建议宽度（保留英文原名：Width）

    public int Height => 360;
    // 只读属性：对话框建议高度（保留英文原名：Height）

    public event Action<EditTextureResult?>? RequestClose;
    // 事件：对话完成时触发，传回 EditTextureResult?（保留英文原名：RequestClose / EditTextureResult?）

    private readonly List<(AssetInst, AssetTypeValueField, TextureFile)> _textures = [];
    // 私有只读字段：保存要编辑的纹理集合，每项为三元组 (AssetInst, AssetTypeValueField, TextureFile)（保留英文原名：_textures）

    public EditTextureViewModel(Workspace workspace, IList<AssetInst> assets)
    // 构造函数：接收 Workspace 与选中的 AssetInst 列表，初始化视图模型（保留英文原名：EditTextureViewModel）
    {
        IsSingleTexture = assets.Count == 1;
        // 设置 IsSingleTexture（是否仅选中一个纹理）（保留英文原名：IsSingleTexture / assets.Count）

        foreach (var asset in assets)
        // 遍历每个选中的资产（保留英文原名：assets / asset）
        {
            var baseField = workspace.GetBaseField(asset);
            // 获取该资产的 BaseField（反序列化后的字段结构）（保留英文原名：workspace.GetBaseField / baseField）

            if (baseField is null)
                continue;
            // 如果无法读取 baseField 则跳过该资产（保留英文原名：baseField）

            var textureFile = TextureFile.ReadTextureFile(baseField);
            // 使用 TextureFile 解析 baseField 得到 TextureFile（保留英文原名：TextureFile.ReadTextureFile）

            _textures.Add((asset, baseField, textureFile));
            // 将三元组 (asset, baseField, textureFile) 添加到 _textures 列表（保留英文原名：_textures）
        }

        SetDefaultValues();
        // 调用 SetDefaultValues 初始化默认值与 UI 显示（保留英文原名：SetDefaultValues）
    }

    public void SetDefaultValues()
    // 公共方法：根据已加载的纹理集合计算并设置默认值（保留英文原名：SetDefaultValues）
    {
        string? name;
        // 局部变量：name（保留英文原名：name）

        TextureFormatEnm? textureFormat;
        // 局部变量：textureFormat（保留英文原名：textureFormat）

        bool? usingMips;
        // 局部变量：usingMips（保留英文原名：usingMips）

        bool? isReadable;
        // 局部变量：isReadable（保留英文原名：isReadable）

        FilterModeEnm? filterMode;
        // 局部变量：filterMode（保留英文原名：filterMode）

        int? filtering;
        // 局部变量：filtering（保留英文原名：filtering）

        float? mipBias;
        // 局部变量：mipBias（保留英文原名：mipBias）

        WrapModeEnm? wrapModeU;
        // 局部变量：wrapModeU（保留英文原名：wrapModeU）

        WrapModeEnm? wrapModeV;
        // 局部变量：wrapModeV（保留英文原名：wrapModeV）

        int? lightMapFormat;
        // 局部变量：lightMapFormat（保留英文原名：lightMapFormat）

        ColorSpaceEnm? colorSpace;
        // 局部变量：colorSpace（保留英文原名：colorSpace）

        // can't do anything
        // 注释：如果没有纹理则无法设置默认值（保留英文原注释）

        if (_textures.Count == 0)
            return;
        // 如果 _textures 为空则直接返回（保留英文原名：_textures）

        var firstTexture = _textures[0];
        // 取第一个纹理三元组作为基准（保留英文原名：firstTexture）

        var firstTextureFile = firstTexture.Item3;
        // 取第一个三元组中的 TextureFile（保留英文原名：firstTextureFile / Item3）

        var firstTextureSettings = firstTextureFile.m_TextureSettings;
        // 取第一个纹理的 TextureSettings（保留英文原名：firstTextureSettings / m_TextureSettings）

        name = firstTextureFile.m_Name;
        // 从第一个纹理读取名称（保留英文原名：m_Name）

        textureFormat = (TextureFormatEnm)firstTextureFile.m_TextureFormat;
        // 从第一个纹理读取纹理格式并转换为枚举（保留英文原名：m_TextureFormat）

        usingMips = firstTextureFile.m_MipMap;
        // 从第一个纹理读取是否使用 mipmap（保留英文原名：m_MipMap）

        isReadable = firstTextureFile.m_IsReadable;
        // 从第一个纹理读取是否可读（保留英文原名：m_IsReadable）

        filterMode = (FilterModeEnm)firstTextureSettings.m_FilterMode;
        // 从第一个纹理设置读取过滤模式并转换为枚举（保留英文原名：m_FilterMode）

        filtering = firstTextureSettings.m_Aniso;
        // 从第一个纹理设置读取各向异性过滤值（保留英文原名：m_Aniso）

        mipBias = firstTextureSettings.m_MipBias;
        // 从第一个纹理设置读取 mip bias（保留英文原名：m_MipBias）

        wrapModeU = (WrapModeEnm)firstTextureSettings.m_WrapU;
        // 从第一个纹理设置读取 U 方向 wrap 模式并转换为枚举（保留英文原名：m_WrapU）

        wrapModeV = (WrapModeEnm)firstTextureSettings.m_WrapV;
        // 从第一个纹理设置读取 V 方向 wrap 模式并转换为枚举（保留英文原名：m_WrapV）

        lightMapFormat = firstTextureFile.m_LightmapFormat;
        // 从第一个纹理读取光照贴图格式（保留英文原名：m_LightmapFormat）

        colorSpace = (ColorSpaceEnm)firstTextureFile.m_ColorSpace;
        // 从第一个纹理读取颜色空间并转换为枚举（保留英文原名：m_ColorSpace）

        if (_textures.Count > 1)
        {
            for (var i = 1; i < _textures.Count; i++)
            {
                var texture = _textures[i];
                var textureFile = texture.Item3;
                var textureSettings = textureFile.m_TextureSettings;

                if (name != textureFile.m_Name)
                    name = null;
                if (textureFormat != (TextureFormatEnm)textureFile.m_TextureFormat)
                    textureFormat = null;
                if (usingMips != textureFile.m_MipMap)
                    usingMips = null;
                if (isReadable != textureFile.m_IsReadable)
                    isReadable = null;
                if (filterMode != (FilterModeEnm)textureSettings.m_FilterMode)
                    filterMode = null;
                if (filtering != textureSettings.m_Aniso)
                    filtering = null;
                if (mipBias != textureSettings.m_MipBias)
                    mipBias = null;
                if (wrapModeU != (WrapModeEnm)textureSettings.m_WrapU)
                    wrapModeU = null;
                if (wrapModeV != (WrapModeEnm)textureSettings.m_WrapV)
                    wrapModeV = null;
                if (lightMapFormat != textureFile.m_LightmapFormat)
                    lightMapFormat = null;
                if (colorSpace != (ColorSpaceEnm)textureFile.m_ColorSpace)
                    colorSpace = null;
            }
        }
        // 如果有多个纹理，遍历其余纹理并比较每个字段；若发现不一致则将对应变量设为 null（表示“多值”状态）（保留英文原名：_textures / textureFile / textureSettings）

        _defaultName = Name = name;
        // 将计算得到的 name 赋给默认值 _defaultName 并设置公开属性 Name（保留英文原名：_defaultName / Name）

        _defaultTextureFormat = TextureFormat = textureFormat;
        // 将 textureFormat 赋给默认值并设置公开属性 TextureFormat（保留英文原名：_defaultTextureFormat / TextureFormat）

        _defaultUsingMips = UsingMips = usingMips;
        // 将 usingMips 赋给默认值并设置公开属性 UsingMips（保留英文原名：_defaultUsingMips / UsingMips）

        _defaultIsReadable = IsReadable = isReadable;
        // 将 isReadable 赋给默认值并设置公开属性 IsReadable（保留英文原名：_defaultIsReadable / IsReadable）

        _defaultFilterMode = FilterMode = filterMode;
        // 将 filterMode 赋给默认值并设置公开属性 FilterMode（保留英文原名：_defaultFilterMode / FilterMode）

        _defaultFilteringString = FilteringString = filtering?.ToString();
        // 将 filtering 转为字符串并赋给默认值与公开属性 FilteringString（保留英文原名：_defaultFilteringString / FilteringString）

        _defaultMipBiasString = MipBiasString = mipBias?.ToString();
        // 将 mipBias 转为字符串并赋给默认值与公开属性 MipBiasString（保留英文原名：_defaultMipBiasString / MipBiasString）

        _defaultWrapModeU = WrapModeU = wrapModeU;
        // 将 wrapModeU 赋给默认值并设置公开属性 WrapModeU（保留英文原名：_defaultWrapModeU / WrapModeU）

        _defaultWrapModeV = WrapModeV = wrapModeV;
        // 将 wrapModeV 赋给默认值并设置公开属性 WrapModeV（保留英文原名：_defaultWrapModeV / WrapModeV）

        _defaultLightMapFormatString = LightMapFormatString = lightMapFormat?.ToString();
        // 将 lightMapFormat 转为字符串并赋给默认值与公开属性 LightMapFormatString（保留英文原名：_defaultLightMapFormatString / LightMapFormatString）

        _defaultColorSpace = ColorSpace = colorSpace;
        // 将 colorSpace 赋给默认值并设置公开属性 ColorSpace（保留英文原名：_defaultColorSpace / ColorSpace）
    }

    public void ResetToDefault(object param)
    // 公共方法：根据传入的索引重置对应字段为默认值（保留英文原名：ResetToDefault）
    {
        if (param is not string paramStr || !int.TryParse(paramStr, out int index))
            return;
        // 验证参数为字符串并能解析为整数索引，否则返回（保留英文原名：paramStr / int.TryParse）

        switch (index)
        {
            case 0: Name = _defaultName; break;
            case 1: TextureFormat = _defaultTextureFormat; break;
            case 2: UsingMips = _defaultUsingMips; break;
            case 3: IsReadable = _defaultIsReadable; break;
            case 4: FilterMode = _defaultFilterMode; break;
            case 5: FilteringString = _defaultFilteringString; break;
            case 6: MipBiasString = _defaultMipBiasString; break;
            case 7: WrapModeU = _defaultWrapModeU; break;
            case 8: WrapModeV = _defaultWrapModeV; break;
            case 9: LightMapFormatString = _defaultLightMapFormatString; break;
            case 10: ColorSpace = _defaultColorSpace; break;
        }
    }
    // 根据索引将对应属性重置为之前保存的默认值（保留英文原名：switch / case）

    public async void SaveChanges()
    // 公共方法：验证输入并在通过后通过 RequestClose 返回 EditTextureResult（保留英文原名：SaveChanges）
    {
        int? filtering;
        // 局部变量：解析后的 filtering（保留英文原名：filtering）

        if (FilteringString is not null)
        {
            if (!int.TryParse(FilteringString, out var filteringTmp))
            {
                await ShowInvalidOptionsBox();
                return;
            }
            filtering = filteringTmp;
        }
        else
        {
            filtering = null;
        }
        // 如果 FilteringString 非 null，则尝试解析为 int；解析失败弹出错误对话并返回；否则将 filtering 设为 null（保留英文原名：FilteringString / int.TryParse）

        int? mipBias;
        // 局部变量：解析后的 mipBias（保留英文原名：mipBias）

        if (MipBiasString is not null)
        {
            if (!int.TryParse(MipBiasString, out var mipBiasTmp))
            {
                await ShowInvalidOptionsBox();
                return;
            }
            mipBias = mipBiasTmp;
        }
        else
        {
            mipBias = null;
        }
        // 同上：解析 MipBiasString（保留英文原名：MipBiasString）

        int? lightMapFormat;
        // 局部变量：解析后的 lightMapFormat（保留英文原名：lightMapFormat）

        if (LightMapFormatString is not null)
        {
            if (!int.TryParse(LightMapFormatString, out var lightMapFormatTmp))
            {
                await ShowInvalidOptionsBox();
                return;
            }
            lightMapFormat = lightMapFormatTmp;
        }
        else
        {
            lightMapFormat = null;
        }
        // 同上：解析 LightMapFormatString（保留英文原名：LightMapFormatString）

        RequestClose?.Invoke(
            new EditTextureResult(
                Name,
                TextureFormat,
                UsingMips,
                IsReadable,
                FilterMode,
                filtering,
                mipBias,
                WrapModeU,
                WrapModeV,
                lightMapFormat,
                ColorSpace
            )
        );
        // 构造 EditTextureResult（包含所有用户设置或 null 表示多值）并通过 RequestClose 事件返回（保留英文原名：RequestClose / EditTextureResult）
    }

    public void Cancel()
    // 公共方法：取消编辑并通过 RequestClose 返回 null（保留英文原名：Cancel）
    {
        RequestClose?.Invoke(null);
    }

    public static ValidationResult? ValidateInt(string intStr, ValidationContext context)
    // 静态方法：用于验证字符串是否为无符号整数（保留英文原名：ValidateInt / ValidationResult）
    {
        if (!string.IsNullOrEmpty(intStr) && !uint.TryParse(intStr, out var _))
        {
            return new("Value must be an int");
        }

        return ValidationResult.Success;
    }
    // 如果字符串非空且无法解析为无符号整数则返回错误，否则返回成功（保留英文原名：uint.TryParse / ValidationResult.Success）

    private async Task ShowInvalidOptionsBox()
    // 私有方法：显示“无效选项”错误对话（保留英文原名：ShowInvalidOptionsBox）
    {
        await MessageBoxUtil.ShowDialog("Error", "Invalid options provided.");
    }
    // 调用 MessageBoxUtil 弹出错误对话（保留英文原名：MessageBoxUtil.ShowDialog）

}
// 类体结束（EditTextureViewModel）

public readonly struct EditTextureResult(
    string? name,
    TextureFormatEnm? textureFormat,
    bool? usingMips,
    bool? isReadable,
    FilterModeEnm? filterMode,
    int? filtering,
    int? mipBias,
    WrapModeEnm? wrapModeU,
    WrapModeEnm? wrapModeV,
    int? lightMapFormat,
    ColorSpaceEnm? colorSpace)
// 定义只读记录结构 EditTextureResult，包含所有编辑结果字段（保留英文原名：EditTextureResult）
{
    public readonly string? Name = name;
    // 只读字段：Name（保留英文原名：Name）

    public readonly TextureFormatEnm? TextureFormat = textureFormat;
    // 只读字段：TextureFormat（保留英文原名：TextureFormat）

    public readonly bool? UsingMips = usingMips;
    // 只读字段：UsingMips（保留英文原名：UsingMips）

    public readonly bool? IsReadable = isReadable;
    // 只读字段：IsReadable（保留英文原名：IsReadable）

    public readonly FilterModeEnm? FilterMode = filterMode;
    // 只读字段：FilterMode（保留英文原名：FilterMode）

    public readonly int? Filtering = filtering;
    // 只读字段：Filtering（保留英文原名：Filtering）

    public readonly int? MipBias = mipBias;
    // 只读字段：MipBias（保留英文原名：MipBias）

    public readonly WrapModeEnm? WrapModeU = wrapModeU;
    // 只读字段：WrapModeU（保留英文原名：WrapModeU）

    public readonly WrapModeEnm? WrapModeV = wrapModeV;
    // 只读字段：WrapModeV（保留英文原名：WrapModeV）

    public readonly int? LightMapFormat = lightMapFormat;
    // 只读字段：LightMapFormat（保留英文原名：LightMapFormat）

    public readonly ColorSpaceEnm? ColorSpace = colorSpace;
    // 只读字段：ColorSpace（保留英文原名：ColorSpace）
}
// 结构体结束（EditTextureResult）
