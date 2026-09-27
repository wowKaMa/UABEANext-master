using CommunityToolkit.Mvvm.DependencyInjection; // 引用 CommunityToolkit 的依赖注入工具（Ioc），用于解析服务（保留原名 CommunityToolkit.Mvvm.DependencyInjection）

using System.Linq; // 引用 LINQ 扩展方法（如 ToList），用于数组/集合转换（保留原名 System.Linq）

using System.Threading.Tasks; // 引用异步任务支持（Task、async/await），用于异步对话框调用（保留原名 System.Threading.Tasks）

using UABEANext4.Services; // 引用项目内的服务接口命名空间（例如 IDialogService），用于显示对话框（保留原名 UABEANext4.Services）

using UABEANext4.ViewModels.Dialogs; // 引用对话框视图模型命名空间（例如 MessageBoxViewModel、MessageBoxType、MessageBoxResult），用于构造对话框 VM（保留原名 UABEANext4.ViewModels.Dialogs）

namespace UABEANext4.Util; // 定义命名空间 UABEANext4.Util，放置工具类（保留原名 UABEANext4.Util）

public class MessageBoxUtil // 定义公共类 MessageBoxUtil，封装显示消息框的便捷方法（保留原名 MessageBoxUtil）
{
    public static async Task<MessageBoxResult> ShowDialog(string header, string message) // 公共静态异步方法 ShowDialog：接收标题 header 与消息 message，返回 MessageBoxResult（保留原名 ShowDialog）
    {
        var dialogService = Ioc.Default.GetRequiredService<IDialogService>(); // 通过依赖注入容器 (Ioc.Default) 获取必需的 IDialogService 实例，用于显示对话框（保留原名 IDialogService）

        var messageBoxVm = new MessageBoxViewModel(header, message, MessageBoxType.OK); // 创建 MessageBoxViewModel 实例，传入 header、message，并使用单一确认按钮类型 MessageBoxType.OK（保留原名 MessageBoxViewModel 与 MessageBoxType.OK）

        return await dialogService.ShowDialog(messageBoxVm) ?? MessageBoxResult.Unknown; // 使用 dialogService 显示对话框并等待结果；若返回 null 则使用 MessageBoxResult.Unknown 作为兜底值（保留原名 ShowDialog 与 MessageBoxResult.Unknown）
    }

    public static async Task<MessageBoxResult> ShowDialog(string header, string message, MessageBoxType buttons) // 重载的 ShowDialog：允许调用者指定按钮类型（MessageBoxType），返回 MessageBoxResult
    {
        var dialogService = Ioc.Default.GetRequiredService<IDialogService>(); // 同样通过依赖注入获取 IDialogService 实例

        var messageBoxVm = new MessageBoxViewModel(header, message, buttons); // 创建 MessageBoxViewModel，使用传入的 buttons（MessageBoxType）来决定按钮布局

        return await dialogService.ShowDialog(messageBoxVm) ?? MessageBoxResult.Unknown; // 显示对话框并返回结果，若为 null 则返回 Unknown
    }

    public static async Task<string> ShowDialogCustom(string header, string message, params string[] buttons) // 公共静态异步方法 ShowDialogCustom：用于自定义按钮文本，返回被点击的按钮文本（string）
    {
        var dialogService = Ioc.Default.GetRequiredService<IDialogService>(); // 通过依赖注入获取 IDialogService

        var messageBoxVm = new MessageBoxViewModel(header, message, MessageBoxType.Custom, buttons.ToList()); // 创建 MessageBoxViewModel，类型为 MessageBoxType.Custom，并将按钮文本数组转换为 List 传入（保留 ToList）

        var res = await dialogService.ShowDialog(messageBoxVm) ?? MessageBoxResult.Unknown; // 显示对话框并等待结果，若为 null 则使用 Unknown 作为默认结果

        return res switch // 使用 switch 表达式根据返回的 MessageBoxResult 映射到对应的按钮文本并返回
        {
            MessageBoxResult.CustomButtonA => buttons[0], // 如果结果为 CustomButtonA，则返回第一个自定义按钮文本
            MessageBoxResult.CustomButtonB => buttons[1], // 如果结果为 CustomButtonB，则返回第二个自定义按钮文本
            MessageBoxResult.CustomButtonC => buttons[2], // 如果结果为 CustomButtonC，则返回第三个自定义按钮文本
            _ => string.Empty, // 其他情况返回空字符串（表示未选择或未知）
        };
    }
}
