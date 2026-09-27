using System;
// 引用基础系统命名空间，提供常用类型与功能（原名：System）

using System.IO;
// 引用文件与路径操作命名空间，提供 Path、File 等 IO 功能（原名：System.IO）

using System.Reflection;
// 引用反射相关命名空间，提供 Assembly、AssemblyName 等类型（原名：System.Reflection）

using System.Runtime.Loader;
// 引用运行时装载命名空间，提供 AssemblyLoadContext、AssemblyDependencyResolver 等（原名：System.Runtime.Loader）

namespace UABEANext4.Plugins;
// 定义命名空间 UABEANext4.Plugins，用于组织插件相关类型（原名：UABEANext4.Plugins）

class PluginLoadContext : AssemblyLoadContext
// 定义类 PluginLoadContext（原名：PluginLoadContext），继承自 AssemblyLoadContext（原名：AssemblyLoadContext），用于隔离并加载插件程序集
{
    // 类体开始

    private readonly AssemblyDependencyResolver _resolver;
    // 私有只读字段 _resolver：用于解析程序集及本机库的实际路径（类型：AssemblyDependencyResolver，原名：_resolver）

    public PluginLoadContext(string pluginPath)
    // 构造函数 PluginLoadContext(string pluginPath)：接收插件文件路径（原名：PluginLoadContext）
    {
        // 构造函数体开始

        _resolver = new AssemblyDependencyResolver(pluginPath);
        // 使用传入的插件路径创建 AssemblyDependencyResolver（原名：AssemblyDependencyResolver），用于解析依赖项的磁盘路径
    }
    // 构造函数体结束

    public Assembly LoadAssemblyByName(string name)
    // 公共方法 LoadAssemblyByName(string name)：按程序集名称加载程序集（原名：LoadAssemblyByName）
    {
        // 方法体开始

        return LoadFromAssemblyName(new AssemblyName(name));
        // 调用基类方法 LoadFromAssemblyName（原名：LoadFromAssemblyName）通过 AssemblyName 加载程序集并返回 Assembly
    }
    // 方法体结束

    public Assembly LoadAssemblyByPath(string path)
    // 公共方法 LoadAssemblyByPath(string path)：按文件路径加载程序集（原名：LoadAssemblyByPath）
    {
        // 方法体开始

        return LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
        // 从路径中提取不带扩展名的文件名作为程序集名，创建 AssemblyName 并通过 LoadFromAssemblyName 加载程序集
    }
    // 方法体结束

    protected override Assembly? Load(AssemblyName assemblyName)
    // 重写基类方法 Load(AssemblyName assemblyName)：当运行时需要解析程序集时调用（原名：Load）
    {
        // 方法体开始

        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        // 使用 AssemblyDependencyResolver.ResolveAssemblyToPath（原名）尝试解析给定 AssemblyName 对应的磁盘路径，结果可能为 null

        if (assemblyPath != null)
        // 如果解析成功（assemblyPath 非空）
        {
            // 条件块开始

            return LoadFromAssemblyPath(assemblyPath);
            // 使用 LoadFromAssemblyPath（原名）从解析到的路径加载程序集并返回 Assembly
        }
        // 条件块结束

        return null;
        // 如果无法解析则返回 null，表示由默认加载上下文或其他机制处理
    }
    // 方法体结束

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    // 重写基类方法 LoadUnmanagedDll(string unmanagedDllName)：用于解析并加载非托管（本机）库（原名：LoadUnmanagedDll）
    {
        // 方法体开始

        var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        // 使用 AssemblyDependencyResolver.ResolveUnmanagedDllToPath（原名）尝试解析本机库的磁盘路径，结果可能为 null

        if (libraryPath != null)
        // 如果解析成功（libraryPath 非空）
        {
            // 条件块开始

            return LoadUnmanagedDllFromPath(libraryPath);
            // 使用 LoadUnmanagedDllFromPath（原名）从解析到的路径加载本机库并返回其句柄（IntPtr）
        }
        // 条件块结束

        return IntPtr.Zero;
        // 如果无法解析则返回 IntPtr.Zero，表示加载失败或交由其他机制处理
    }
    // 方法体结束

}
// 类体结束
