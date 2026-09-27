using System.Threading.Tasks; // 引用异步任务支持命名空间（System.Threading.Tasks），提供 Task、Task.CompletedTask、Task.FromResult 等异步类型和方法

using UABEANext4.Interfaces; // 引用项目内接口命名空间（UABEANext4.Interfaces），用于访问 IDialogAware、IDialogAware<TResult> 等接口定义

namespace UABEANext4.Services; // 定义命名空间 UABEANext4.Services（保留英文原名），用于组织服务相关类型

internal class DummyDialogService : IDialogService // 定义内部类 DummyDialogService（保留英文原名），实现 IDialogService 接口；此类为“占位/空实现”对话框服务
{
    public void Show(IDialogAware viewModel) // 实现 IDialogService 的同步显示方法 Show，参数为实现 IDialogAware 的 viewModel（保留英文原名 Show 与 IDialogAware）
    {
    } // 空实现：不执行任何操作，作为占位或测试用实现

    public void Show<TResult>(IDialogAware<TResult> viewModel) // 实现泛型同步显示方法 Show<TResult>，参数为实现 IDialogAware<TResult> 的 viewModel（保留英文原名 Show<TResult> 与 IDialogAware<TResult>）
    {
    } // 空实现：不执行任何操作

    public Task ShowDialog(IDialogAware viewModel) // 实现异步显示方法 ShowDialog，接收 IDialogAware viewModel 并返回一个 Task（保留英文原名 ShowDialog）
    {
        return Task.CompletedTask; // 返回已完成的任务（Task.CompletedTask），表示异步操作立即完成且无实际对话框交互
    }

    public Task<TResult?> ShowDialog<TResult>(IDialogAware<TResult> viewModel) // 实现泛型异步显示方法 ShowDialog<TResult>，返回可能为 null 的 TResult（保留英文原名 ShowDialog<TResult>）
    {
        return Task.FromResult(default(TResult)); // 返回一个已完成的 Task，结果为默认值（default(TResult)，通常为 null），表示没有实际结果
    }
}
