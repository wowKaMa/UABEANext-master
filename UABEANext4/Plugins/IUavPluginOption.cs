using System.Collections.Generic;
// 引用泛型集合命名空间（System.Collections.Generic），提供 List<T>、IList<T> 等集合类型

using System.Threading.Tasks;
// 引用异步任务支持命名空间（System.Threading.Tasks），提供 Task、async/await 等异步功能

using UABEANext4.AssetWorkspace;
// 引用项目内的资产工作区命名空间（UABEANext4.AssetWorkspace），用于访问 Workspace、AssetInst 等类型

namespace UABEANext4.Plugins;
// 定义命名空间 UABEANext4.Plugins（UABEANext4.Plugins），用于组织插件相关接口与类

public interface IUavPluginOption
// 定义公共接口 IUavPluginOption（IUavPluginOption），声明插件选项应实现的成员
{
    // 接口体开始

    string Name { get; }
    // 只读属性 Name（Name）：返回插件选项的可读名称，用于 UI 列表或日志显示

    string Description { get; }
    // 只读属性 Description（Description）：返回插件选项的描述文本，说明该选项的用途或行为

    UavPluginMode Options { get; }
    // 只读属性 Options（Options）：返回该插件选项支持的模式（类型为 UavPluginMode），例如 Import/Export/Console/Create 等

    bool SupportsSelection(Workspace workspace, UavPluginMode mode, IList<AssetInst> selection);
    // 方法 SupportsSelection（SupportsSelection）：判断在给定的 workspace（Workspace）、模式 mode（UavPluginMode）和选中资产列表 selection（IList<AssetInst>）下，该插件选项是否可用
    // 返回值：bool，表示是否支持当前选择与模式

    Task<bool> Execute(Workspace workspace, IUavPluginFunctions funcs, UavPluginMode mode, IList<AssetInst> selection);
    // 异步方法 Execute（Execute）：在给定 workspace（Workspace）、宿主提供的功能 funcs（IUavPluginFunctions）、模式 mode（UavPluginMode）和选中资产 selection（IList<AssetInst>）下执行该插件选项的操作
    // 返回值：Task<bool>，异步完成后返回 bool 表示操作是否成功

}
// 接口体结束
