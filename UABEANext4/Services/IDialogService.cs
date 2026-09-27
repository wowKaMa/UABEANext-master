using System.Threading.Tasks; // 引用异步任务支持命名空间 (System.Threading.Tasks)，提供 Task、async/await 等异步功能

using UABEANext4.Interfaces; // 引用项目内的接口命名空间 (UABEANext4.Interfaces)，用于访问 IDialogAware 等接口定义

namespace UABEANext4.Services; // 定义命名空间 UABEANext4.Services (UABEANext4.Services)，用于组织服务相关类型

public interface IDialogService // 定义公共接口 IDialogService (IDialogService)，声明对话框服务应实现的方法
{ // 接口体开始

    Task ShowDialog(IDialogAware viewModel); // 异步方法声明 ShowDialog (ShowDialog)：接收一个实现 IDialogAware (IDialogAware) 的 viewModel 并以异步方式显示对话框，返回一个 Task 表示操作完成

    void Show(IDialogAware viewModel); // 同步方法声明 Show (Show)：接收一个实现 IDialogAware (IDialogAware) 的 viewModel 并立即显示对话框（不等待结果）

    void Show<TResult>(IDialogAware<TResult> viewModel); // 泛型方法声明 Show<TResult> (Show<TResult>)：接收一个实现 IDialogAware<TResult> (IDialogAware<TResult>) 的 viewModel 并显示对话框，适用于期望结果类型但以同步方式调用的场景

    Task<TResult?> ShowDialog<TResult>(IDialogAware<TResult> viewModel); // 泛型异步方法声明 ShowDialog<TResult> (ShowDialog<TResult>)：接收一个实现 IDialogAware<TResult> (IDialogAware<TResult>) 的 viewModel 并以异步方式显示对话框，返回可能为 null 的 TResult 结果封装在 Task 中

} // 接口体结束
