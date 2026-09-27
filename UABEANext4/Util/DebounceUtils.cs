using Avalonia.Threading;
// 引用 Avalonia 的调度器命名空间，用于在 UI 线程上调度操作（保留原名 Avalonia.Threading）

using System;
// 引用基础系统命名空间，提供 Action、Exception 等基础类型（保留原名 System）

using System.Threading;
// 引用线程与同步原语命名空间，提供 CancellationTokenSource 等（保留原名 System.Threading）

using System.Threading.Tasks;
// 引用异步任务支持命名空间，提供 Task、Task.Delay 等（保留原名 System.Threading.Tasks）

namespace UABEANext4.Util;
// 定义命名空间 UABEANext4.Util，用于组织工具类（保留原名 UABEANext4.Util）

public static class DebounceUtils
// 定义公共静态类 DebounceUtils，封装防抖（debounce）相关的工具方法（保留原名 DebounceUtils）
{
    public static Action<T> Debounce<T>(Action<T> func, int milliseconds = 300)
    // 定义泛型静态方法 Debounce：接收一个带参数的回调 func 和防抖延迟毫秒数 milliseconds（默认 300），返回一个包装后的 Action<T>
    {
        CancellationTokenSource? cancelTokenSource = null;
        // 声明并初始化可空的 CancellationTokenSource，用于在新的调用到来时取消之前的延迟任务

        return arg =>
        // 返回一个闭包（Action<T>），当调用此闭包时会启动/重置防抖计时
        {
            cancelTokenSource?.Cancel();
            // 如果已有未完成的延迟任务，则取消它（通过取消其 CancellationToken）

            cancelTokenSource = new CancellationTokenSource();
            // 为当前调用创建一个新的 CancellationTokenSource，用于控制本次延迟任务

            Task.Delay(milliseconds, cancelTokenSource.Token)
                // 创建一个延迟任务，等待指定的毫秒数，支持通过 cancelTokenSource.Token 取消
                .ContinueWith(t =>
                // 延迟任务完成后继续执行的回调（在默认任务调度器上运行）
                {
                    if (t.IsCompletedSuccessfully)
                    // 如果延迟任务成功完成（未被取消且未发生异常）
                    {
                        Dispatcher.UIThread.Post(() =>
                        // 将实际要执行的 func 调度到 UI 线程（Avalonia 的 UI 线程），以确保对 UI 的安全访问
                        {
                            func(arg);
                            // 在 UI 线程上调用原始回调 func，并传入最初的参数 arg
                        });
                    }
                }, TaskScheduler.Default);
            // 指定 ContinueWith 在默认任务调度器上执行（避免在调用线程上直接执行回调）
        };
    }
}
