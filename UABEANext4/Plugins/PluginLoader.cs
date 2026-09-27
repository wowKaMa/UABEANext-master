using System;
// 引用基础系统命名空间，提供常用类型与功能（原名：System）

using System.Collections.Generic;
// 引用泛型集合命名空间，提供 List<T> 等集合类型（原名：System.Collections.Generic）

using System.IO;
// 引用文件与路径操作命名空间，提供 Path、Directory 等 IO 功能（原名：System.IO）

using UABEANext4.AssetWorkspace;
// 引用项目内的资产工作区命名空间，用于访问 Workspace、AssetInst 等类型（原名：UABEANext4.AssetWorkspace）

using UABEANext4.Util;
// 引用项目内的工具命名空间，包含辅助方法（如 GetUniqueFlags、PathUtils 等）（原名：UABEANext4.Util）

namespace UABEANext4.Plugins;
// 定义命名空间 UABEANext4.Plugins，用于组织插件相关类型（原名：UABEANext4.Plugins）

public class PluginLoader
// 定义公共类 PluginLoader，负责加载插件并管理插件选项与预览器（原名：PluginLoader）
{
    // 类体开始（保留原名：PluginLoader）

    private readonly List<IUavPluginOption> _pluginOptions = [];
    // 私有只读字段 _pluginOptions：用于存储加载到的插件选项实例（类型 List<IUavPluginOption>，原名：_pluginOptions）
    // 初始化为空列表（保留 IUavPluginOption 原名）

    private readonly List<IUavPluginPreviewer> _pluginPreviewers = [];
    // 私有只读字段 _pluginPreviewers：用于存储加载到的插件预览器实例（类型 List<IUavPluginPreviewer>，原名：_pluginPreviewers）
    // 初始化为空列表（保留 IUavPluginPreviewer 原名）

    public bool LoadPlugin(string path)
    // 公共方法 LoadPlugin：接收插件文件路径（string path），尝试加载该插件并注册其类型，返回是否成功（原名：LoadPlugin）
    {
        // 方法体开始（原名：LoadPlugin）

        try
        // 使用 try/catch 捕获加载过程中的异常，避免程序崩溃（原名：try）
        {
            // try 块开始

            var fullPath = Path.GetFullPath(path);
            // 将传入的相对或绝对路径规范化为完整路径（使用 Path.GetFullPath，原名：fullPath）

            var plugLoadCtx = new PluginLoadContext(fullPath);
            // 创建一个 PluginLoadContext 实例用于隔离加载的程序集（原名：PluginLoadContext，参数为 fullPath）

            var asm = plugLoadCtx.LoadAssemblyByPath(fullPath);
            // 使用自定义的加载上下文按路径加载程序集并返回 Assembly（原名：LoadAssemblyByPath，结果存为 asm）

            foreach (Type type in asm.GetTypes())
            // 遍历程序集中的所有类型（Assembly.GetTypes 返回类型数组，原名：type）
            {
                // foreach 循环体开始

                if (typeof(IUavPluginOption).IsAssignableFrom(type))
                // 如果当前类型实现或继承自 IUavPluginOption（表示这是一个插件选项类型）
                {
                    // 条件块开始

                    object? typeInst = Activator.CreateInstance(type);
                    // 使用反射创建该类型的实例（Activator.CreateInstance），结果可能为 null（原名：typeInst）

                    if (typeInst == null)
                        return false;
                    // 如果实例化失败则返回 false（表示加载失败）

                    if (typeInst is not IUavPluginOption plugInst)
                        return false;
                    // 如果实例不是 IUavPluginOption（类型不匹配）则返回 false；否则将其实例转换为 plugInst（原名：plugInst）

                    _pluginOptions.Add(plugInst);
                    // 将转换后的插件选项实例添加到 _pluginOptions 列表中（原名：_pluginOptions）
                }
                // 条件块结束
                else if (typeof(IUavPluginPreviewer).IsAssignableFrom(type))
                // 否则如果当前类型实现或继承自 IUavPluginPreviewer（表示这是一个插件预览器类型）
                {
                    // 条件块开始

                    object? typeInst = Activator.CreateInstance(type);
                    // 使用反射创建该类型的实例（可能为 null）（原名：typeInst）

                    if (typeInst == null)
                        return false;
                    // 如果实例化失败则返回 false

                    if (typeInst is not IUavPluginPreviewer plugInst)
                        return false;
                    // 如果实例不是 IUavPluginPreviewer 则返回 false；否则转换为 plugInst（原名：plugInst）

                    _pluginPreviewers.Add(plugInst);
                    // 将预览器实例添加到 _pluginPreviewers 列表中（原名：_pluginPreviewers）
                }
                // else if 块结束
            }
            // foreach 循环结束
        }
        // try 块结束
        catch
        // 捕获任何异常（不暴露异常细节），并在失败时返回 false（原名：catch）
        {
            return false;
            // 如果加载过程中发生异常则返回 false（表示加载失败）
        }
        return false;
        // 注意：方法末尾返回 false（当前实现始终返回 false，即使没有异常也不会返回 true；可能为逻辑错误或占位实现）
    }
    // 方法体结束

    public void LoadPluginsInDirectory(string directory)
    // 公共方法 LoadPluginsInDirectory：在指定目录中查找并加载所有符合模式的插件文件（原名：LoadPluginsInDirectory）
    {
        // 方法体开始

        Directory.CreateDirectory(directory);
        // 确保目录存在（如果不存在则创建），使用 Directory.CreateDirectory（原名：directory）

        foreach (string file in Directory.EnumerateFiles(directory, "*.dll"))
        // 遍历目录中所有扩展名为 .dll 的文件（使用 Directory.EnumerateFiles）
        {
            // foreach 循环体开始

            LoadPlugin(file);
            // 对每个找到的 dll 文件调用 LoadPlugin 进行加载（原名：LoadPlugin）
        }
        // foreach 循环结束
    }
    // 方法体结束

    public List<PluginOptionModePair> GetOptionsThatSupport(Workspace workspace, List<AssetInst> assets, UavPluginMode mode)
    // 公共方法 GetOptionsThatSupport：根据传入的 Workspace、选中资产列表与所需模式（UavPluginMode），返回支持该选择的插件选项列表（原名：GetOptionsThatSupport）
    {
        // 方法体开始

        var options = new List<PluginOptionModePair>();
        // 创建结果列表 options，用于收集符合条件的 PluginOptionModePair（原名：options）

        foreach (var option in _pluginOptions)
        // 遍历已加载的每个插件选项（来自 _pluginOptions 列表）
        {
            // foreach 循环体开始

            var bothOpt = mode & option.Options;
            // 计算传入模式与插件选项支持模式的按位与（得到两者共有的模式位，原名：bothOpt）

            foreach (var flag in bothOpt.GetUniqueFlags())
            // 使用扩展方法 GetUniqueFlags 将按位组合拆分为单独的标志并遍历（原名：GetUniqueFlags）
            {
                // 内层 foreach 开始

                if (flag == UavPluginMode.All)
                    continue;
                // 如果标志为 All（表示全部），跳过（不将 All 作为单独选项处理）

                var supported = option.SupportsSelection(workspace, flag, assets);
                // 调用插件选项的 SupportsSelection 方法判断该选项是否支持在给定 workspace、flag 与 assets 下的操作（原名：SupportsSelection）

                if (supported)
                    options.Add(new PluginOptionModePair(option, flag));
                // 如果支持，则将该选项与对应的 flag 封装为 PluginOptionModePair 并加入结果列表（原名：PluginOptionModePair）
            }
            // 内层 foreach 结束
        }
        // 外层 foreach 结束

        return options;
        // 返回收集到的支持项列表（原名：options）
    }
    // 方法体结束

    public List<PluginPreviewerTypePair> GetPreviewersThatSupport(Workspace workspace, AssetInst asset)
    // 公共方法 GetPreviewersThatSupport：根据 workspace 与单个 asset 返回支持该资产预览的预览器列表（原名：GetPreviewersThatSupport）
    {
        // 方法体开始

        var previewers = new List<PluginPreviewerTypePair>();
        // 创建结果列表 previewers（原名：previewers）

        foreach (var previewer in _pluginPreviewers)
        // 遍历已加载的每个插件预览器（来自 _pluginPreviewers 列表）
        {
            // foreach 循环体开始

            var previewType = previewer.SupportsPreview(workspace, asset);
            // 调用预览器的 SupportsPreview 方法以确定其支持的预览类型（返回 UavPluginPreviewerType，原名：SupportsPreview）

            if (previewType != UavPluginPreviewerType.None)
                previewers.Add(new PluginPreviewerTypePair(previewer, previewType));
            // 如果预览类型不是 None，则将该预览器与其类型封装为 PluginPreviewerTypePair 并加入结果列表（原名：PluginPreviewerTypePair）
        }
        // foreach 循环结束

        return previewers;
        // 返回收集到的预览器列表（原名：previewers）
    }
    // 方法体结束
}
// 类体结束（PluginLoader）
