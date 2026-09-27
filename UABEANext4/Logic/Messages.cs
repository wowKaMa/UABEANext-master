using CommunityToolkit.Mvvm.Messaging.Messages;
// 引用 CommunityToolkit 的消息类型命名空间，用于 ValueChangedMessage 等消息基类（保留英文原名 CommunityToolkit.Mvvm.Messaging.Messages）

using System.Collections.Generic;
// 引用泛型集合命名空间，提供 List<T> 等集合类型（保留英文原名 System.Collections.Generic）

using UABEANext4.AssetWorkspace;
using AssetsTools.NET.Extra;
// 引用项目内的资产工作区命名空间，提供 AssetInst、WorkspaceItem 等类型（保留英文原名 UABEANext4.AssetWorkspace）

namespace UABEANext4.Logic;
// 定义命名空间 UABEANext4.Logic，用于组织逻辑层消息类型（保留英文原名 UABEANext4.Logic）

public class AssetsSelectedMessage(List<AssetInst> value)
    // 定义公共消息类 AssetsSelectedMessage（保留英文原名 AssetsSelectedMessage），构造参数为 List<AssetInst> value
    // 作用：表示“选中的资产集合”发生变化的消息载体
    : ValueChangedMessage<List<AssetInst>>(value)
// 继承自 ValueChangedMessage<List<AssetInst>> 并将构造参数 value 传给基类构造函数（保留英文原名 ValueChangedMessage）
{
    // 类体开始（当前为空体，消息类仅用于承载数据）
}

public class RequestEditAssetMessage(AssetInst value)
    // 定义公共消息类 RequestEditAssetMessage（保留英文原名 RequestEditAssetMessage），构造参数为 AssetInst value
    // 作用：请求编辑某个资产的消息载体
    : ValueChangedMessage<AssetInst>(value)
// 继承自 ValueChangedMessage<AssetInst> 并将 value 传给基类构造函数（保留英文原名 ValueChangedMessage）
{
    // 类体开始（空体，消息仅承载要编辑的 AssetInst）
}

public class RequestVisitAssetMessage(AssetInst value)
    // 定义公共消息类 RequestVisitAssetMessage（保留英文原名 RequestVisitAssetMessage），构造参数为 AssetInst value
    // 作用：请求“定位/跳转到”某个资产（例如在资源树中选中或在资源视图中定位）
    : ValueChangedMessage<AssetInst>(value)
// 继承自 ValueChangedMessage<AssetInst> 并传入 value（保留英文原名 ValueChangedMessage）
{
    // 类体开始（空体）
}

public class RequestSceneViewMessage(AssetInst value)
    // 定义公共消息类 RequestSceneViewMessage（保留英文原名 RequestSceneViewMessage），构造参数为 AssetInst value
    // 作用：请求在场景视图中查看或聚焦某个资产（例如场景对象）
    : ValueChangedMessage<AssetInst>(value)
// 继承自 ValueChangedMessage<AssetInst> 并传入 value（保留英文原名 ValueChangedMessage）
{
    // 类体开始（空体）
}

public class AssetsUpdatedMessage(AssetInst value)
    // 定义公共消息类 AssetsUpdatedMessage（保留英文原名 AssetsUpdatedMessage），构造参数为 AssetInst value
    // 作用：表示某个资产已被更新（例如内容或元数据发生变化）
    : ValueChangedMessage<AssetInst>(value)
// 继承自 ValueChangedMessage<AssetInst> 并传入 value（保留英文原名 ValueChangedMessage）
{
    // 类体开始（空体）
}

public class SelectedWorkspaceItemChangedMessage(List<WorkspaceItem> value)
    // 定义公共消息类 SelectedWorkspaceItemChangedMessage（保留英文原名 SelectedWorkspaceItemChangedMessage），构造参数为 List<WorkspaceItem> value
    // 作用：表示工作区中选中的项集合发生变化（例如文件/文件夹/资产项）
    : ValueChangedMessage<List<WorkspaceItem>>(value)
// 继承自 ValueChangedMessage<List<WorkspaceItem>> 并传入 value（保留英文原名 ValueChangedMessage）
{
    // 类体开始（空体）
}

public class WorkspaceClosingMessage()
    // 定义公共消息类 WorkspaceClosingMessage（保留英文原名 WorkspaceClosingMessage），无构造参数
    // 作用：表示工作区正在关闭或重置（用于通知订阅者进行清理）
    : ValueChangedMessage<bool>(false)
// 继承自 ValueChangedMessage<bool> 并传入 false 作为初始值（表示默认状态为 false；保留英文原名 ValueChangedMessage）
{
    // 类体开始（空体）
}
public class AssetFileModifiedMessage(AssetsFileInstance value)
    : ValueChangedMessage<AssetsFileInstance>(value)
{
}
