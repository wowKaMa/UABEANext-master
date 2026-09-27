using System; // 引用基础系统命名空间（System），提供 Exception、Action 等基础类型

namespace UABEANext4.Util; // 定义命名空间 UABEANext4.Util（保留英文原名），用于组织工具类

public class SimpleObserver<T> : IObserver<T> // 定义公共泛型类 SimpleObserver<T>，实现 IObserver<T> 接口（保留英文原名 SimpleObserver 和 IObserver）
{
    private readonly Action<T> _listener; // 私有只读字段 _listener，类型为 Action<T>，用于保存外部传入的回调（listener）

    public SimpleObserver(Action<T> listener) => _listener = listener; // 构造函数：接收一个 Action<T> 并赋值给 _listener（保留英文原名 listener），使用表达式体语法简洁初始化

    public void OnCompleted() { } // IObserver<T>.OnCompleted 实现（保留英文原名 OnCompleted），当观察序列完成时被调用；此处为空实现（不执行任何操作）

    public void OnError(Exception error) { } // IObserver<T>.OnError 实现（保留英文原名 OnError），当观察序列发生错误时被调用；此处为空实现（不处理错误）

    public void OnNext(T value) => _listener(value); // IObserver<T>.OnNext 实现（保留英文原名 OnNext），当有新值到达时调用保存的回调 _listener 并传入该值
}
