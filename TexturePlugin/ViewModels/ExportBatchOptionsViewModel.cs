using AssetsTools.NET.Texture;
// 引用纹理处理相关的库，用于 ImageExportType、TextureFile 等（保留英文原名：AssetsTools.NET.Texture）

using CommunityToolkit.Mvvm.ComponentModel;
// 引用 CommunityToolkit 的 MVVM 特性与基类（例如 ObservableProperty、ObservableObject）（保留英文原名：CommunityToolkit.Mvvm.ComponentModel）

using UABEANext4.Interfaces;
// 引用项目内定义的接口集合（例如 IDialogAware）（保留英文原名：UABEANext4.Interfaces）

using UABEANext4.ViewModels;
// 引用项目内的视图模型基类或相关类型（例如 ViewModelBase）（保留英文原名：UABEANext4.ViewModels）

namespace TexturePlugin.ViewModels;
// 定义命名空间 TexturePlugin.ViewModels，用于组织插件的视图模型（保留英文原名：TexturePlugin.ViewModels）

public partial class ExportBatchOptionsViewModel : ViewModelBase, IDialogAware<ExportBatchOptionsResult?>
// 定义部分类 ExportBatchOptionsViewModel，继承 ViewModelBase 并实现对话框接口 IDialogAware，泛型返回类型为 ExportBatchOptionsResult?（保留英文原名：ExportBatchOptionsViewModel / ViewModelBase / IDialogAware）
{
    // 类体开始（ExportBatchOptionsViewModel）

    [ObservableProperty]
    // 特性：由 CommunityToolkit 自动生成对应的属性（例如生成 SelectedExportType 属性并触发通知）（保留英文原名：ObservableProperty）

    public ImageExportType _selectedExportType = ImageExportType.Png;
    // 字段：默认导出类型为 PNG；ObservableProperty 特性会生成公开属性 SelectedExportType（保留英文原名：_selectedExportType / ImageExportType / ImageExportType.Png）

    [ObservableProperty]
    // 特性：为下一个字段生成可绑定属性（保留英文原名：ObservableProperty）

    public int _quality = 100;
    // 字段：导出质量，默认 100；ObservableProperty 会生成公开属性 Quality（保留英文原名：_quality）

    public List<string> DropdownItems { get; } =
    // 只读属性 DropdownItems：下拉菜单项列表（保留英文原名：DropdownItems）
    [
        "BMP (alpha, uncompressed, lossless)",
         // 下拉项：BMP 描述（保留英文原名："BMP (alpha, uncompressed, lossless)"）

        "PNG (alpha, compressed, lossless)",
         // 下拉项：PNG 描述（保留英文原名："PNG (alpha, compressed, lossless)"）

        "JPG (no alpha, compressed, lossy)",
         // 下拉项：JPG 描述（保留英文原名："JPG (no alpha, compressed, lossy)"）

        "TGA (alpha, compressed, lossless)"
         // 下拉项：TGA 描述（保留英文原名："TGA (alpha, compressed, lossless)"）
    ];

    public string Title => "Texture Batch Export";
    // 只读属性 Title：对话框标题文本（保留英文原名：Title / "Texture Batch Export"）

    public int Width => 300;
    // 只读属性 Width：对话框建议宽度（保留英文原名：Width / 300）

    public int Height => 100;
    // 只读属性 Height：对话框建议高度（保留英文原名：Height / 100）

    public event Action<ExportBatchOptionsResult?>? RequestClose;
    // 事件 RequestClose：对话完成时触发，传回 ExportBatchOptionsResult?（保留英文原名：RequestClose / ExportBatchOptionsResult?）

    public void BtnOk_Click()
    // 方法 BtnOk_Click：当用户点击“确定”按钮时调用（保留英文原名：BtnOk_Click）
    {
        RequestClose?.Invoke(new ExportBatchOptionsResult(SelectedExportType, Quality));
        // 触发 RequestClose 事件并传回用户选择的导出类型与质量（保留英文原名：RequestClose / ExportBatchOptionsResult / SelectedExportType / Quality）
    }

    public void BtnCancel_Click()
    // 方法 BtnCancel_Click：当用户点击“取消”按钮时调用（保留英文原名：BtnCancel_Click）
    {
        RequestClose?.Invoke(null);
        // 触发 RequestClose 事件并传回 null 表示取消（保留英文原名：RequestClose）
    }
}
// ExportBatchOptionsViewModel 类结束（保留英文原名：ExportBatchOptionsViewModel）

public readonly struct ExportBatchOptionsResult
// 定义只读结构 ExportBatchOptionsResult：用于封装对话返回结果（保留英文原名：ExportBatchOptionsResult）
{
    public ImageExportType ImageType { get; }
    // 只读属性 ImageType：用户选择的图像导出类型（保留英文原名：ImageType / ImageExportType）

    public int Quality { get; }
    // 只读属性 Quality：用户选择的导出质量（保留英文原名：Quality）

    public readonly string Extension => ImageType switch
    // 只读属性 Extension：根据 ImageType 返回对应的文件扩展名（保留英文原名：Extension / switch）
    {
        ImageExportType.Bmp => ".bmp",
        // 如果 ImageType 为 Bmp，则扩展名为 .bmp（保留英文原名：ImageExportType.Bmp）

        ImageExportType.Png => ".png",
        // 如果 ImageType 为 Png，则扩展名为 .png（保留英文原名：ImageExportType.Png）

        ImageExportType.Jpg => ".jpg",
        // 如果 ImageType 为 Jpg，则扩展名为 .jpg（保留英文原名：ImageExportType.Jpg）

        ImageExportType.Tga => ".tga",
        // 如果 ImageType 为 Tga，则扩展名为 .tga（保留英文原名：ImageExportType.Tga）

        _ => throw new ArgumentOutOfRangeException(nameof(ImageType))
        // 默认分支：抛出异常，表示遇到未知的 ImageType（保留英文原名：ArgumentOutOfRangeException / nameof(ImageType)）
    };

    public ExportBatchOptionsResult(ImageExportType imageType, int quality)
    // 构造函数：用 imageType 与 quality 初始化结构（保留英文原名：ExportBatchOptionsResult）
    {
        ImageType = imageType;
        // 将 ImageType 设为传入的 imageType（保留英文原名：ImageType）

        Quality = quality;
        // 将 Quality 设为传入的 quality（保留英文原名：Quality）
    }
}
// ExportBatchOptionsResult 结构结束（保留英文原名：ExportBatchOptionsResult）
