using AssetsTools.NET; // 引用 AssetsTools.NET 库，用于处理 Unity 资产文件的底层 API（保留原名 AssetsTools.NET）

using CommunityToolkit.Mvvm.ComponentModel; // 引用 CommunityToolkit.Mvvm 的组件模型，提供 ObservableProperty 等 MVVM 特性（保留原名 CommunityToolkit.Mvvm.ComponentModel）

using System; // 引用基础系统命名空间，提供基本类型与委托（保留原名 System）

using System.ComponentModel.DataAnnotations; // 引用数据注解命名空间，用于属性验证（保留原名 System.ComponentModel.DataAnnotations）

using System.Threading.Tasks; // 引用异步任务支持（Task、async/await）（保留原名 System.Threading.Tasks）

using UABEANext4.Interfaces; // 引用项目接口命名空间，包含 IDialogAware 等接口（保留原名 UABEANext4.Interfaces）

using UABEANext4.Util; // 引用项目工具类命名空间（MessageBoxUtil、GUID128 等）（保留原名 UABEANext4.Util）

namespace UABEANext4.ViewModels.Dialogs; // 定义命名空间 UABEANext4.ViewModels.Dialogs，用于组织对话框相关的视图模型类（保留原名）

public partial class AddExternalViewModel : ViewModelBaseValidator, IDialogAware<AssetsFileExternal> // 定义部分类 AddExternalViewModel，继承自 ViewModelBaseValidator 并实现 IDialogAware<AssetsFileExternal>（保留原名）
{
    [ObservableProperty] // 特性：由 CommunityToolkit 自动生成公开属性 FileName 并在变更时通知（保留原名 ObservableProperty）
    [NotifyPropertyChangedFor(nameof(HasOriginalName))] // 特性：当 FileName 改变时也触发 HasOriginalName 的属性变更通知（保留原名 NotifyPropertyChangedFor）
    private string _fileName = ""; // 私有字段：FileName 的后备字段，保存用户输入的路径名（默认空字符串）

    [ObservableProperty] // 特性：自动生成公开属性 OriginalFileName 并在变更时通知
    private string _originalFileName = ""; // 私有字段：OriginalFileName 的后备字段，保存原始路径名（默认空字符串）

    [ObservableProperty] // 特性：自动生成公开属性 ExternalType 并在变更时通知
    [NotifyPropertyChangedFor(nameof(HasGuid))] // 特性：当 ExternalType 改变时也触发 HasGuid 的属性变更通知
    private AssetsFileExternalType _externalType = AssetsFileExternalType.Normal; // 私有字段：ExternalType 的后备字段，表示外部类型（默认 Normal）

    [ObservableProperty] // 特性：自动生成公开属性 GuidString 并在变更时通知
    [CustomValidation(typeof(AddExternalViewModel), nameof(ValidateGuid))] // 特性：对 GuidString 使用自定义验证方法 ValidateGuid
    private string _guidString = "00000000000000000000000000000000"; // 私有字段：GuidString 的后备字段，默认 32 字符 0（GUID 文本）

    public bool HasOriginalName => FileName.StartsWith("Resources/"); // 只读属性：判断 FileName 是否以 "Resources/" 开头（用于决定是否显示 OriginalFileName），保留英文原名 HasOriginalName

    public bool HasGuid => ExternalType != AssetsFileExternalType.Normal; // 只读属性：判断 ExternalType 是否不是 Normal（用于决定是否需要 GUID），保留英文原名 HasGuid

    public string Title => "编辑外部 (Edit External)"; // 对话框标题：中文显示并在括号保留英文原名（Title）

    public int Width => 350; // 对话框宽度（像素）（Width）

    public int Height => 170; // 对话框高度（像素）（Height）

    public event Action<AssetsFileExternal?>? RequestClose; // 事件：请求关闭对话框并返回 AssetsFileExternal 或 null（RequestClose）

    public AddExternalViewModel(AssetsFileExternal? external) // 构造函数：接收可选的现有 AssetsFileExternal 用于编辑
    {
        if (external != null) // 如果传入了外部对象（非 null）
        {
            FileName = external.PathName; // 将外部对象的 PathName 赋给 FileName（用于 UI 显示/编辑）
            OriginalFileName = external.OriginalPathName; // 将外部对象的 OriginalPathName 赋给 OriginalFileName
            ExternalType = external.Type; // 将外部对象的 Type 赋给 ExternalType
            GuidString = external.Guid.ToString(); // 将外部对象的 Guid 转为字符串并赋给 GuidString
        }
    }

    public static ValidationResult? ValidateGuid(string guidString, ValidationContext context) // 静态方法：用于验证 GUID 字符串的有效性（供 CustomValidation 使用）
    {
        if (!GUID128.TryParse(guidString, out GUID128 _)) // 尝试解析 guidString 为 GUID128，解析失败则返回错误
        {
            return new("GUID is invalid"); // 返回验证失败结果（英文消息保留原文）
        }

        return ValidationResult.Success; // 验证通过，返回 Success
    }

    public async void BtnOk_Click() // 异步方法：当用户点击“确定”按钮时调用（BtnOk_Click）
    {
        GUID128 guid; // 局部变量：将保存最终使用的 GUID128 值

        if (HasGuid) // 如果当前外部类型需要 GUID（HasGuid 为 true）
        {
            var guidSuccess = GUID128.TryParse(GuidString, out guid); // 尝试解析用户输入的 GuidString
            if (!guidSuccess) // 如果解析失败
            {
                await ShowInvalidOptionsBox(); // 弹出错误提示对话框（异步）
                return; // 终止方法，不关闭对话框
            }
        }
        else // 如果不需要 GUID
        {
            guid = new GUID128(); // 使用默认构造的空 GUID（或随机 GUID，取决于 GUID128 实现）
        }

        var result = new AssetsFileExternal() // 构造一个新的 AssetsFileExternal 实例作为返回结果
        {
            PathName = FileName, // 设置 PathName 为当前 FileName
            OriginalPathName = HasOriginalName ? OriginalFileName : FileName, // 如果有 OriginalName 则使用 OriginalFileName，否则使用 FileName
            Type = ExternalType, // 设置 Type 为当前 ExternalType
            Guid = guid // 设置 Guid 为上面解析或生成的 guid
        };
        RequestClose?.Invoke(result); // 触发 RequestClose 事件并传回构造好的结果，通知宿主关闭对话框
    }

    private async Task ShowInvalidOptionsBox() // 私有异步方法：显示“无效选项”错误对话框
    {
        await MessageBoxUtil.ShowDialog("错误 (Error)", "提供的选项无效。 (Invalid options provided.)"); // 弹出对话框，中文提示并在括号保留英文原文
    }

    public void BtnCancel_Click() // 方法：当用户点击“取消”按钮时调用（BtnCancel_Click）
    {
        RequestClose?.Invoke(null); // 触发 RequestClose 事件并传回 null，表示取消操作
    }
} // 类 AddExternalViewModel 结束
