using Avalonia.Controls; // 引用 Avalonia 的控件命名空间，提供 Window、UserControl 等 UI 控件类型（保留英文原名 Avalonia.Controls）

using System; // 引用基础系统命名空间，提供 Exception 等基础类型（保留英文原名 System）

using System.Threading.Tasks; // 引用异步任务支持命名空间，提供 Task、async/await 等（保留英文原名 System.Threading.Tasks）

using UABEANext4.Interfaces; // 引用项目内接口命名空间，包含 IDialogService、IDialogAware 等接口定义（保留英文原名 UABEANext4.Interfaces）

namespace UABEANext4.Services; // 定义命名空间 UABEANext4.Services，用于组织服务相关类型（保留英文原名 UABEANext4.Services）

public class DialogService(Window mainWindow, ViewLocator viewLocator) : IDialogService // 定义公共类 DialogService，使用主构造参数 mainWindow 与 viewLocator（保留英文原名）；实现 IDialogService 接口
{ // 类体开始

    public async Task ShowDialog(IDialogAware viewModel) // 公共异步方法 ShowDialog：接收实现 IDialogAware 的 viewModel 并以模态对话框方式显示（保留英文原名 ShowDialog 与 IDialogAware）
    { // 方法体开始
        var window = CreateWindow(viewModel); // 调用私有方法 CreateWindow 创建一个 Window 实例并将 viewModel 绑定到其内容上

        await window.ShowDialog(mainWindow); // 异步显示模态窗口，指定 mainWindow 为拥有者（等待窗口关闭）
    } // 方法体结束

    public void Show(IDialogAware viewModel) // 公共同步方法 Show：接收 IDialogAware viewModel 并以非模态方式显示窗口（保留英文原名 Show）
    { // 方法体开始
        var window = CreateWindow(viewModel); // 创建窗口
        window.Show(mainWindow); // 以非模态方式显示窗口，并将 mainWindow 作为拥有者（立即返回，不等待关闭）
    } // 方法体结束

    public void Show<TResult>(IDialogAware<TResult> viewModel) // 泛型同步方法 Show<TResult>：显示期望返回 TResult 的对话框（保留英文原名 Show<TResult> 与 IDialogAware<TResult>）
    { // 方法体开始
        var window = CreateWindow(viewModel); // 创建窗口

        void eventHandler(TResult? result) // 局部方法：定义事件处理器，当 viewModel 请求关闭时由其触发并传回结果
        { // 局部方法体开始
            window.Close(result); // 关闭窗口并传递结果给 Show/Close 的调用者
            viewModel.RequestClose -= eventHandler; // 注销事件处理器，避免重复调用或内存泄漏
        } // 局部方法体结束

        viewModel.RequestClose += eventHandler; // 将事件处理器订阅到 viewModel 的 RequestClose 事件上（当 VM 请求关闭时触发）
        window.Show(mainWindow); // 以非模态方式显示窗口（主窗口为拥有者）
    } // 方法体结束

    public async Task<TResult?> ShowDialog<TResult>(IDialogAware<TResult> viewModel) // 泛型异步方法 ShowDialog<TResult>：以模态方式显示对话框并等待 TResult 结果（保留英文原名 ShowDialog<TResult>）
    { // 方法体开始
        var window = CreateWindow(viewModel); // 创建窗口

        void eventHandler(TResult? result) => window.Close(result); // 局部单行方法：当 viewModel 请求关闭时关闭窗口并传递结果

        viewModel.RequestClose += eventHandler; // 订阅 viewModel 的 RequestClose 事件以便在 VM 请求关闭时关闭窗口
        var result = await window.ShowDialog<TResult?>(mainWindow); // 异步显示模态窗口并等待返回的 TResult? 结果
        viewModel.RequestClose -= eventHandler; // 在窗口关闭后取消订阅事件处理器

        return result; // 返回对话框结果（可能为 null）
    } // 方法体结束

    private Window CreateWindow(IDialogAware viewModel) // 私有方法 CreateWindow：根据 viewModel 构建并配置一个 Window 实例（保留英文原名 CreateWindow）
    { // 方法体开始
        var view = viewLocator.Build(viewModel); // 使用 viewLocator 构建与 viewModel 对应的视图（通常返回 UserControl 或类似控件）
        view.DataContext = viewModel; // 将 viewModel 设为视图的 DataContext，以便绑定数据与命令

        if (view is not UserControl uc) // 检查构建得到的 view 是否为 UserControl；如果不是则无法作为窗口内容使用
        { // 条件块开始
            throw new Exception("View is not a UserControl"); // 抛出异常提示视图类型不符合预期（保留英文异常消息）
        } // 条件块结束

        return new Window // 创建并返回一个新的 Window 实例，使用对象初始化器设置其属性
        { // 对象初始化器开始
            Content = uc, // 将视图（UserControl）设置为窗口内容
            Icon = mainWindow.Icon, // 继承主窗口的图标
            WindowStartupLocation = WindowStartupLocation.CenterOwner, // 启动位置设为相对于拥有者居中
            Title = viewModel.Title, // 将窗口标题设置为 viewModel 提供的 Title
            Width = viewModel.Width, // 将窗口宽度设置为 viewModel 提供的 Width
            Height = viewModel.Height, // 将窗口高度设置为 viewModel 提供的 Height
        }; // 对象初始化器结束
    } // 方法体结束

} // 类体结束
