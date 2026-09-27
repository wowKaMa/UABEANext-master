using Avalonia.Controls; // 引用 Avalonia 的控件命名空间，用于 UI 控件类型（保持原名 Avalonia.Controls）

using Avalonia.Controls.Templates; // 引用 Avalonia 的数据模板接口，用于模板选择器（保持原名 Avalonia.Controls.Templates）

using Avalonia.Metadata; // 引用 Avalonia 的元数据特性（例如 [Content]），用于模板选择器（保持原名 Avalonia.Metadata）

using CommunityToolkit.Mvvm.ComponentModel; // 引用 CommunityToolkit MVVM 的组件模型，提供 ObservableProperty 特性（保持原名 CommunityToolkit.Mvvm.ComponentModel）

using System; // 引用基础系统命名空间，提供 Action、Exception 等类型（保持原名 System）

using System.Collections.Generic; // 引用泛型集合命名空间，提供 Dictionary、List 等（保持原名 System.Collections.Generic）

using UABEANext4.Interfaces; // 引用项目接口命名空间，包含 IDialogAware 等接口（保持原名 UABEANext4.Interfaces）

namespace UABEANext4.ViewModels.Dialogs; // 定义命名空间 UABEANext4.ViewModels.Dialogs，用于组织对话框相关的视图模型类（保持原名）

public partial class MessageBoxViewModel : ViewModelBase, IDialogAware<MessageBoxResult?> // 定义部分类 MessageBoxViewModel，继承 ViewModelBase 并实现 IDialogAware，返回 MessageBoxResult?（类名和接口保留原名）
{
    [ObservableProperty] // 特性：自动生成属性与通知（将生成公开属性 MsgTitle）
    public string _msgTitle = "消息框标题 (Message box title)"; // 字段：消息框标题的后备字段，默认显示中文并在括号中保留英文原名

    [ObservableProperty] // 特性：自动生成属性与通知（将生成公开属性 MsgText）
    public string _msgText = "消息框内容 (Message box text)"; // 字段：消息框文本的后备字段，默认显示中文并在括号中保留英文原名

    [ObservableProperty] // 特性：自动生成属性与通知（将生成公开属性 MsgType）
    public MessageBoxType _msgType = MessageBoxType.OKCancel; // 字段：消息框类型的后备字段，默认使用 OKCancel（枚举保留原名）

    [ObservableProperty] // 特性：自动生成属性与通知（将生成公开属性 ButtonTextA）
    public string _buttonTextA = ""; // 字段：按钮 A 的文本后备字段，默认空字符串

    [ObservableProperty] // 特性：自动生成属性与通知（将生成公开属性 ButtonTextB）
    public string _buttonTextB = ""; // 字段：按钮 B 的文本后备字段，默认空字符串

    [ObservableProperty] // 特性：自动生成属性与通知（将生成公开属性 ButtonTextC）
    public string _buttonTextC = ""; // 字段：按钮 C 的文本后备字段，默认空字符串

    public string Title => MsgTitle; // 属性：对话框窗口标题，直接返回 MsgTitle（MsgTitle 中已包含中文与英文原名）

    public int Width => 400; // 属性：对话框默认宽度（像素）

    public int Height => 160; // 属性：对话框默认高度（像素）

    public event Action<MessageBoxResult?>? RequestClose; // 事件：请求关闭对话框时触发，参数为 MessageBoxResult?（调用者订阅以接收结果）

    public MessageBoxViewModel() // 无参构造函数：默认构造器（用于设计器或无参数创建）
    {
    } // 构造函数结束

    public MessageBoxViewModel(string title, string text, MessageBoxType type) // 构造函数：使用外部传入的标题、文本和类型初始化视图模型
    {
        MsgTitle = title; // 将传入的 title 赋值给 MsgTitle（保留传入原文）
        MsgText = text; // 将传入的 text 赋值给 MsgText（保留传入原文）
        MsgType = type; // 将传入的 type 赋值给 MsgType（保留传入枚举）
    } // 构造函数结束

    public MessageBoxViewModel(string title, string text, MessageBoxType type, List<string> buttonTexts) // 构造函数：带自定义按钮文本的初始化
    {
        MsgTitle = title; // 赋值标题
        MsgText = text; // 赋值文本
        MsgType = type; // 赋值类型
        while (buttonTexts.Count < 3) // 确保 buttonTexts 至少有 3 个元素
        {
            buttonTexts.Add(""); // 不足时补空字符串
        }
        ButtonTextA = buttonTexts[0]; // 将第一个文本赋给按钮 A
        ButtonTextB = buttonTexts[1]; // 将第二个文本赋给按钮 B
        ButtonTextC = buttonTexts[2]; // 将第三个文本赋给按钮 C
    } // 构造函数结束

    public void BtnA_Click() // 方法：当用户点击按钮 A 时调用（根据 MsgType 返回不同结果）
    {
        if (MsgType == MessageBoxType.OK) // 如果类型为 OK
        {
            RequestClose?.Invoke(MessageBoxResult.OK); // 返回 OK
        }
        else if (MsgType == MessageBoxType.OKCancel) // 如果类型为 OKCancel
        {
            RequestClose?.Invoke(MessageBoxResult.OK); // 返回 OK
        }
        else if (MsgType == MessageBoxType.YesNo) // 如果类型为 YesNo
        {
            RequestClose?.Invoke(MessageBoxResult.Yes); // 返回 Yes
        }
        else if (MsgType == MessageBoxType.YesNoCancel) // 如果类型为 YesNoCancel
        {
            RequestClose?.Invoke(MessageBoxResult.Yes); // 返回 Yes
        }
        else if (MsgType == MessageBoxType.Custom) // 如果类型为 Custom（自定义按钮）
        {
            RequestClose?.Invoke(MessageBoxResult.CustomButtonA); // 返回自定义按钮 A 的结果
        }
        RequestClose?.Invoke(MessageBoxResult.Unknown); // 如果以上分支都未触发，则返回 Unknown（作为兜底）
    } // 方法结束

    public void BtnB_Click() // 方法：当用户点击按钮 B 时调用（根据 MsgType 返回不同结果）
    {
        if (MsgType == MessageBoxType.OKCancel) // 如果类型为 OKCancel
        {
            RequestClose?.Invoke(MessageBoxResult.Cancel); // 返回 Cancel
        }
        else if (MsgType == MessageBoxType.YesNo) // 如果类型为 YesNo
        {
            RequestClose?.Invoke(MessageBoxResult.No); // 返回 No
        }
        else if (MsgType == MessageBoxType.YesNoCancel) // 如果类型为 YesNoCancel
        {
            RequestClose?.Invoke(MessageBoxResult.No); // 返回 No
        }
        else if (MsgType == MessageBoxType.Custom) // 如果类型为 Custom（自定义按钮）
        {
            RequestClose?.Invoke(MessageBoxResult.CustomButtonB); // 返回自定义按钮 B 的结果
        }
        RequestClose?.Invoke(MessageBoxResult.Unknown); // 兜底返回 Unknown
    } // 方法结束

    public void BtnC_Click() // 方法：当用户点击按钮 C 时调用（根据 MsgType 返回不同结果）
    {
        if (MsgType == MessageBoxType.YesNoCancel) // 如果类型为 YesNoCancel
        {
            RequestClose?.Invoke(MessageBoxResult.Cancel); // 返回 Cancel
        }
        else if (MsgType == MessageBoxType.Custom) // 如果类型为 Custom（自定义按钮）
        {
            RequestClose?.Invoke(MessageBoxResult.CustomButtonC); // 返回自定义按钮 C 的结果
        }
        RequestClose?.Invoke(MessageBoxResult.Unknown); // 兜底返回 Unknown
    } // 方法结束
} // MessageBoxViewModel 类结束

public class MessageBoxTemplateSelector : IDataTemplate // 定义模板选择器类，用于根据 MessageBoxType 选择不同的模板（实现 IDataTemplate）
{
    [Content] // 特性：标记 AvailableTemplates 为模板内容集合（Avalonia 元数据）
    public Dictionary<string, IDataTemplate> AvailableTemplates { get; } = new(); // 属性：可用模板字典，键为字符串（通常为 MessageBoxType 的 ToString）

    public Control Build(object? param) // IDataTemplate.Build 实现：根据键构建对应模板的控件
    {
        var key = param?.ToString() ?? throw new ArgumentNullException(nameof(param)); // 将 param 转为字符串键，若为 null 则抛出异常
        return AvailableTemplates[key].Build(param)!; // 使用对应模板构建并返回控件（假定存在且非 null）
    } // 方法结束

    public bool Match(object? data) // IDataTemplate.Match 实现：判断给定数据是否匹配此模板选择器
    {
        var key = data?.ToString(); // 将 data 转为字符串键（可能为 null）

        return data is MessageBoxType // 匹配条件：data 必须是 MessageBoxType
                && !string.IsNullOrEmpty(key) // 且键不为空
                && AvailableTemplates.ContainsKey(key); // 且 AvailableTemplates 中包含该键
    } // 方法结束
} // MessageBoxTemplateSelector 类结束

public enum MessageBoxType // 枚举：定义消息框支持的类型（MessageBoxType 保留原名）
{
    OK, // 仅有一个确认按钮（OK）
    OKCancel, // 确认 + 取消（OKCancel）
    YesNo, // 是/否（YesNo）
    YesNoCancel, // 是/否/取消（YesNoCancel）
    Custom // 自定义按钮布局（Custom）
} // 枚举结束

public enum MessageBoxResult // 枚举：表示消息框返回的结果（MessageBoxResult 保留原名）
{
    Unknown, // 未知或未处理的结果（Unknown）
    OK, // 确认（OK）
    Yes, // 是（Yes）
    No, // 否（No）
    Cancel, // 取消（Cancel）
    CustomButtonA, // 自定义按钮 A（CustomButtonA）
    CustomButtonB, // 自定义按钮 B（CustomButtonB）
    CustomButtonC // 自定义按钮 C（CustomButtonC）
} // 枚举结束
