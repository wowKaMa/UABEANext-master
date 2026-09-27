using System;

// 引用基础系统命名空间（原名：System），提供常用类型（例如 Action、EventHandler 等）

namespace UABEANext4.Interfaces;

// 定义命名空间 UABEANext4.Interfaces（原名：UABEANext4.Interfaces），用于组织接口类型

public interface IDialogAware

// 定义公共接口 IDialogAware（原名：IDialogAware），表示对话框可感知的契约
{

    // 接口体开始（IDialogAware）

    public string Title { get; }

    // 只读属性 Title（原名：Title）：表示对话框的标题文本，供 UI 显示或绑定使用

    public int Width { get; }

    // 只读属性 Width（原名：Width）：表示对话框的宽度（像素或逻辑单位），供布局或默认值参考

    public int Height { get; }

    // 只读属性 Height（原名：Height）：表示对话框的高度（像素或逻辑单位），供布局或默认值参考

}

// 接口体结束（IDialogAware）

public interface IDialogAware<TResult> : IDialogAware

// 定义泛型接口 IDialogAware<TResult>（原名：IDialogAware<TResult>），继承自 IDialogAware，用于带返回结果的对话框契约
{

    // 接口体开始（IDialogAware<TResult>）

    public event Action<TResult?> RequestClose;

    // 事件 RequestClose（原名：RequestClose）：当对话框请求关闭时触发，携带可空的 TResult 作为返回结果
    // 说明：订阅者（通常是对话框宿主或对话框服务）可以在此事件触发时接收结果并关闭对话框或执行后续逻辑
    // 类型说明：Action<TResult?> 表示一个无返回值的委托，参数为 TResult?（可空），用于传递对话框的返回值或 null 表示无结果

}

// 接口体结束（IDialogAware<TResult>）
