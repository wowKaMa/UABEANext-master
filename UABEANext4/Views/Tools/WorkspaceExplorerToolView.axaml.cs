using Avalonia.Controls; // 引用 Avalonia 控件库，用于使用 Control、TextBlock、TreeView 等控件（Avalonia.Controls）
using Avalonia.Input; // 引用输入相关类型，用于处理 Tapped 等输入事件（Avalonia.Input）
using Avalonia.Interactivity; // 引用交互事件类型，用于 RoutedEventArgs 等（Avalonia.Interactivity）
using System.Collections.Generic; // 引用集合类型，用于 List<T> 等（System.Collections.Generic）
using UABEANext4.AssetWorkspace; // 引用项目的资产工作区命名空间，包含 WorkspaceItem 等类型（UABEANext4.AssetWorkspace）
using UABEANext4.Util; // 引用项目工具类命名空间，包含 DebounceUtils 等实用工具（UABEANext4.Util）
using UABEANext4.ViewModels.Tools; // 引用工具视图模型命名空间，包含 WorkspaceExplorerToolViewModel（UABEANext4.ViewModels.Tools）

namespace UABEANext4.Views.Tools; // 定义此类所属的命名空间（UABEANext4.Views.Tools）

public partial class WorkspaceExplorerToolView : UserControl // 定义部分类 WorkspaceExplorerToolView，继承自 UserControl（界面控件）
{
    public WorkspaceExplorerToolView() // 构造函数：当界面实例被创建时调用（WorkspaceExplorerToolView 构造器）
    {
        InitializeComponent(); // 初始化 XAML 组件并加载界面布局（自动生成的方法）

        var selectionChanged = DebounceUtils.Debounce<object>(SolutionTreeView_OnSelectionChanged, 300); // 使用防抖工具包装选择变更处理函数，延迟 300 毫秒以减少频繁触发

        SolutionTreeView.SelectionChanged += (sender, e) => selectionChanged(e); // 将防抖后的处理函数绑定到 TreeView 的 SelectionChanged 事件

        SolutionTreeView.Tapped += SolutionTreeView_Tapped; // 将点击（Tapped）事件绑定到 SolutionTreeView_Tapped 方法，处理用户点击行为
    }

    private void SolutionTreeView_OnSelectionChanged(object e) // 处理树视图选择变化的回调（接收事件参数 object e）
    {
        if (DataContext == null || DataContext is not WorkspaceExplorerToolViewModel viewModel) // 检查 DataContext 是否存在并且是期望的 ViewModel 类型
            return; // 如果不是期望的 ViewModel，则不处理并返回

        if (e is not SelectionChangedEventArgs selectionEventArgs) // 检查传入的事件参数是否为 SelectionChangedEventArgs
            return; // 如果不是，则返回（避免类型转换异常）

        var selectionActuallyChanged = selectionEventArgs.AddedItems.Count > 0 || selectionEventArgs.RemovedItems.Count > 0; // 判断是否有实际的选择变更（新增或移除项）

        if (selectionActuallyChanged) // 如果选择确实发生了变化
        {
            var wsItems = new List<WorkspaceItem>(); // 创建一个新的列表用于收集选中的 WorkspaceItem

            foreach (var item in SolutionTreeView.SelectedItems) // 遍历 TreeView 的 SelectedItems 集合
            {
                if (item is WorkspaceItem wsItem) // 如果当前项是 WorkspaceItem 类型
                {
                    wsItems.Add(wsItem); // 将其添加到收集列表中
                }
            }

            viewModel.SelectedItemsChanged(wsItems); // 调用 ViewModel 的 SelectedItemsChanged 方法，通知后端选中项已更新
        }
    }

    private void SolutionTreeView_Tapped(object? sender, TappedEventArgs e) // 处理 TreeView 项被点击（Tapped）事件
    {
        if (SolutionTreeView.SelectedItem == null) // 如果当前没有选中任何项
            return; // 则不做任何处理并返回

        var treeViewItem = (TreeViewItem?)SolutionTreeView.TreeContainerFromItem(SolutionTreeView.SelectedItem); // 根据选中的数据项获取对应的 TreeViewItem 容器

        if (treeViewItem != null) // 如果找到了对应的容器
        {
            treeViewItem.IsExpanded = true; // 将该 TreeViewItem 展开（IsExpanded = true），以显示其子节点
        }
    }

    private void ExpandAll(object? sender, RoutedEventArgs e) // 响应“全部展开”操作的方法（ExpandAll）
    {
        foreach (var treeViewObj in SolutionTreeView.Items) // 遍历 TreeView 的根级 Items 集合
        {
            if (treeViewObj is null) // 如果当前项为空
                continue; // 跳过该项

            var treeViewItem = (TreeViewItem?)SolutionTreeView.TreeContainerFromItem(treeViewObj); // 获取该数据项对应的 TreeViewItem 容器

            if (treeViewItem is null) // 如果容器不存在
                continue; // 跳过

            SolutionTreeView.ExpandSubTree(treeViewItem); // 调用 TreeView 的 ExpandSubTree 方法展开该节点及其子树
        }
    }

    private void CollapseAll(object? sender, RoutedEventArgs e) // 响应“全部折叠”操作的方法（CollapseAll）
    {
        foreach (var treeViewObj in SolutionTreeView.Items) // 遍历 TreeView 的根级 Items 集合
        {
            if (treeViewObj is null) // 如果当前项为空
                continue; // 跳过该项

            var treeViewItem = (TreeViewItem?)SolutionTreeView.TreeContainerFromItem(treeViewObj); // 获取该数据项对应的 TreeViewItem 容器

            if (treeViewItem is null) // 如果容器不存在
                continue; // 跳过

            SolutionTreeView.CollapseSubTree(treeViewItem); // 调用 TreeView 的 CollapseSubTree 方法折叠该节点及其子树
        }
    }
}
