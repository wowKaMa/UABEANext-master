using Avalonia.Collections; // 引用 Avalonia 的集合类型（用于 AvaloniaList 等 UI 绑定集合）

using CommunityToolkit.Mvvm.ComponentModel; // 引用 CommunityToolkit.Mvvm 的组件模型（提供 ObservableProperty、ObservableObject 等）

using CommunityToolkit.Mvvm.DependencyInjection; // 引用 CommunityToolkit 的依赖注入支持（Ioc.Default）

using CommunityToolkit.Mvvm.Messaging; // 引用 CommunityToolkit 的消息总线（WeakReferenceMessenger）

using Dock.Model.Mvvm.Controls; // 引用 Dock.Model 的 MVVM 控件基类（Tool 等）

using System; // 引用基础系统命名空间（提供基本类型和工具）

using System.Collections.Generic; // 引用泛型集合命名空间（List、Dictionary 等）

using UABEANext4.AssetWorkspace; // 引用项目的资产工作区命名空间（Workspace、WorkspaceItem 等）

using UABEANext4.Logic; // 引用项目逻辑层命名空间（包含 WorkspaceItemType 等）

using UABEANext4.Services; // 引用项目服务层命名空间（IDialogService、StorageService 等）

using UABEANext4.ViewModels.Dialogs; // 引用对话框视图模型命名空间（RenameFileViewModel 等）

namespace UABEANext4.ViewModels.Tools // 定义命名空间 UABEANext4.ViewModels.Tools，用于组织工具类的视图模型
{
    public partial class WorkspaceExplorerToolViewModel : Tool // 定义部分类 WorkspaceExplorerToolViewModel，继承自 Tool（保持类名不变）
    {
        const string TOOL_TITLE = "工作区浏览器 (Workspace Explorer)"; // 常量：工具标题，显示为中文并在括号中保留原始英文名称（TOOL_TITLE）

        public delegate void SelectedWorkspaceItemChangedEvent(List<WorkspaceItem> workspaceItems); // 定义委托类型：当选中的工作区项改变时使用（SelectedWorkspaceItemChangedEvent）

        public Workspace Workspace { get; } // 只读属性：保存传入或创建的 Workspace 实例（Workspace）

        [ObservableProperty] // 特性：由 CommunityToolkit 自动生成属性与通知（会生成 SelectedItems 属性的封装）
        public AvaloniaList<object> _selectedItems; // 字段：用于绑定的选中项集合（底层字段名为 _selectedItems，生成的属性为 SelectedItems）

        [Obsolete("This constructor is for the designer only and should not be used directly.", true)] // 标记：此构造函数仅供设计器使用，不应直接调用（保留原始英文说明）
        public WorkspaceExplorerToolViewModel() // 无参构造函数（仅供设计器）
        {
            Workspace = new(); // 创建一个新的 Workspace 实例并赋值给 Workspace（用于设计器预览）

            SelectedItems = new(); // 初始化生成的 SelectedItems 属性为一个空的 AvaloniaList（便于绑定）

            Id = TOOL_TITLE.Replace(" ", ""); // 设置工具 Id（将 TOOL_TITLE 中的空格移除以生成 Id，保留原始英文/中文混合文本）
            Title = TOOL_TITLE; // 设置工具标题（Title），显示为中文并保留英文原名
        }

        public WorkspaceExplorerToolViewModel(Workspace workspace) // 带 Workspace 参数的构造函数（用于运行时注入 Workspace）
        {
            Workspace = workspace; // 将传入的 workspace 赋值给属性 Workspace（使用外部工作区实例）

            SelectedItems = new(); // 初始化 SelectedItems 为一个空的 AvaloniaList（用于 UI 绑定）

            Id = TOOL_TITLE.Replace(" ", ""); // 设置工具 Id（从 TOOL_TITLE 生成）
            Title = TOOL_TITLE; // 设置工具标题（显示中文并保留英文原名）
        }

        public void SelectedItemsChanged(List<WorkspaceItem> value) // 方法：当选中项改变时调用（SelectedItemsChanged）
        {
            WeakReferenceMessenger.Default.Send(new SelectedWorkspaceItemChangedMessage(value)); // 使用弱引用消息总线发送 SelectedWorkspaceItemChangedMessage，通知其他组件选中项已改变
        }

        public async void RenameItem() // 异步方法（返回 void）：触发重命名选中项的流程（RenameItem）
        {
            var wsItem = (WorkspaceItem)SelectedItems[0]; // 从 SelectedItems 中取第一个项并强制转换为 WorkspaceItem（假定至少有一项被选中）

            if (!IsItemRenamable(wsItem)) // 检查该项是否可重命名（IsItemRenamable）
            {
                return; // 如果不可重命名则直接返回
            }

            var dialogService = Ioc.Default.GetRequiredService<IDialogService>(); // 通过依赖注入获取 IDialogService（对话框服务）
            var newName = await dialogService.ShowDialog(new RenameFileViewModel(wsItem.Name)); // 弹出重命名对话框（RenameFileViewModel），并等待用户输入新名称

            if (newName == null) // 如果用户取消或未提供新名称（返回 null）
            {
                return; // 直接返回，不执行重命名
            }

            Workspace.RenameFile(wsItem, newName); // 调用 Workspace 的 RenameFile 方法将选中项重命名为 newName
        }

        public void LoadAll() // 方法：加载工作区中所有可加载的资产文件并将它们作为选中项（LoadAll）
        {
            var itemsToLoad = new List<WorkspaceItem>(); // 创建一个列表用于收集要加载的 WorkspaceItem（itemsToLoad）

            foreach (var rootItem in Workspace.RootItems) // 遍历 Workspace 的根项集合（RootItems）
                GatherWorkspaceItemsRecursive(rootItem, itemsToLoad); // 递归收集每个根项下的所有 AssetsFile 类型项

            SelectedItemsChanged(itemsToLoad); // 将收集到的项作为“选中项”发送消息（SelectedItemsChanged）
        }

        private bool IsItemRenamable(WorkspaceItem wsItem) // 私有方法：判断某个 WorkspaceItem 是否可以被重命名（IsItemRenamable）
        {
            return wsItem.Parent != null && wsItem.Parent.ObjectType == WorkspaceItemType.BundleFile; // 只有当该项有父项且父项类型为 BundleFile 时才允许重命名
        }

        private void GatherWorkspaceItemsRecursive(WorkspaceItem thisItem, List<WorkspaceItem> allItems) // 私有递归方法：收集指定节点下的所有 AssetsFile 类型项（GatherWorkspaceItemsRecursive）
        {
            if (thisItem.ObjectType == WorkspaceItemType.AssetsFile) // 如果当前项的类型是 AssetsFile
                allItems.Add(thisItem); // 将当前项加入结果列表（allItems）

            foreach (var childItem in thisItem.Children) // 遍历当前项的所有子项（Children）
            {
                GatherWorkspaceItemsRecursive(childItem, allItems); // 对每个子项递归调用自身以收集更深层的 AssetsFile 项
            }
        }
    }
}
