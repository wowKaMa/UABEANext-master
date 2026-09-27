using Avalonia.Controls; // 引用 Avalonia 控件库，用于使用 Window、UserControl、ContextMenu、MenuItem 等控件（Avalonia.Controls）

using Avalonia.Controls.Primitives; // 引用控件原语，包含 FlyoutBase、MenuItemToggleType 等（Avalonia.Controls.Primitives）

using Avalonia.Interactivity; // 引用交互事件类型，包含 RoutedEventArgs、SelectionChangedEventArgs 等（Avalonia.Interactivity）

using Avalonia.VisualTree; // 引用可视树扩展方法，用于查找子孙控件（FindDescendantOfType 等）（Avalonia.VisualTree）

using System; // 引用系统基础命名空间，包含 EventArgs、Exception 等（System）

using System.Collections.Generic; // 引用泛型集合命名空间，用于 List<T>、Dictionary 等（System.Collections.Generic）

using System.Linq; // 引用 LINQ 扩展方法，用于 Cast、ToList、Where 等（System.Linq）

using UABEANext4.AssetWorkspace; // 引用项目的资产工作区命名空间，包含 AssetInst 等类型（UABEANext4.AssetWorkspace）

using UABEANext4.ViewModels.Documents; // 引用文档视图模型命名空间，包含 AssetDocumentViewModel（UABEANext4.ViewModels.Documents）

namespace UABEANext4.Views.Documents; // 定义此类所属的命名空间（UABEANext4.Views.Documents）

public partial class AssetDocumentView : UserControl // 定义部分类 AssetDocumentView，继承自 UserControl（资产文档视图）
{
    public AssetDocumentView() // 构造函数：当 AssetDocumentView 被实例化时调用（constructor）
    {
        InitializeComponent(); // 初始化组件：加载并解析与此控件关联的 .axaml（界面布局）

        Initialized += AssetDocumentView_Initialized; // 注册 Initialized 事件处理器，窗口/控件初始化完成时调用 AssetDocumentView_Initialized
        dataGrid.Loaded += DataGrid_Loaded; // 注册 dataGrid 的 Loaded 事件处理器，dataGrid 加载完成时调用 DataGrid_Loaded
    }

    private void AssetDocumentView_Initialized(object? sender, EventArgs e) // 初始化完成时的回调（AssetDocumentView_Initialized）
    {
        if (DataContext is AssetDocumentViewModel docVm) // 检查 DataContext 是否为期望的 AssetDocumentViewModel，并将其赋值给 docVm
        {
            docVm.ShowPluginsContextMenuAction += ShowPluginsContextMenu; // 将 ViewModel 的 ShowPluginsContextMenuAction 绑定到本地方法 ShowPluginsContextMenu（用于显示插件菜单）
            docVm.SetSelectedItemsAction += SetSelectedItems; // 将 ViewModel 的 SetSelectedItemsAction 绑定到本地方法 SetSelectedItems（用于从 VM 设置选中项）
        }
    }

    // doing this from codebehind because it allows the UI to be
    // the source of truth rather than the VM. this comes with the
    // side effect that this won't save if the dock rebuilds the
    // UI, so this may need to be binded to the VM later on...
    // 上面注释保留英文原文并说明：此处在 code-behind 中处理列可见性以让 UI 成为事实来源（source of truth），但在某些情况下可能需要绑定到 VM

    private void DataGrid_Loaded(object? sender, RoutedEventArgs e) // dataGrid 加载完成时调用（DataGrid_Loaded）
    {
        var contextMenu = new ContextMenu(); // 创建一个新的上下文菜单（ContextMenu），用于列头右键菜单
        for (int i = 0; i < dataGrid.Columns.Count; i++) // 遍历 dataGrid 的所有列（按索引）
        {
            var column = dataGrid.Columns[i]; // 获取当前索引对应的列（DataGridColumn）

            // skip columns with no header
            // 跳过没有标题的列（skip columns with no header）
            var columnHeader = column.Header.ToString()?.Trim(); // 获取列的 Header 文本并去除首尾空白
            if (string.IsNullOrEmpty(columnHeader)) // 如果列标题为空或仅空白
                continue; // 跳过该列，不为其创建菜单项

            var contextMenuMenuItem = new MenuItem // 创建一个新的菜单项（MenuItem）用于控制列的可见性
            {
                Header = columnHeader, // 菜单项显示文本为列标题（Header）
                ToggleType = MenuItemToggleType.CheckBox, // 将菜单项设置为复选框类型（可切换）
                IsChecked = column.IsVisible, // 菜单项的初始选中状态与列的可见性同步
                Tag = column // 将列对象存入 Tag，便于点击时获取对应列
            };

            contextMenuMenuItem.Click += MenuItem_Click; // 为菜单项注册 Click 事件处理器（MenuItem_Click）
            contextMenu.Items.Add(contextMenuMenuItem); // 将菜单项添加到上下文菜单中
        }

        if (dataGrid.FindDescendantOfType<DataGridColumnHeadersPresenter>() is { } columnHeadersPresenter) // 在 dataGrid 的可视树中查找列头呈现器（DataGridColumnHeadersPresenter）
        {
            columnHeadersPresenter.ContextMenu = contextMenu; // 将构建好的上下文菜单赋给列头呈现器，使列头显示右键菜单
        }
    }

    private void ShowPluginsContextMenu() // 显示插件上下文菜单的方法（由 ViewModel 调用）
    {
        FlyoutBase.ShowAttachedFlyout(showPluginsBtn); // 使用 FlyoutBase 显示与 showPluginsBtn 关联的附加弹出（Flyout）
    }

    // necessary since SelectedItems isn't bindable
    // 由于 SelectedItems 无法绑定（不是 bindable），因此需要在 code-behind 中手动同步选中项
    private void DataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e) // dataGrid 选择改变时触发（DataGrid_SelectionChanged）
    {
        if (DataContext is AssetDocumentViewModel docVm) // 检查 DataContext 是否为 AssetDocumentViewModel
        {
            var selectedAssetInsts = dataGrid.SelectedItems.Cast<AssetInst>().ToList(); // 将 SelectedItems 强制转换为 AssetInst 列表
            docVm.OnAssetOpened(selectedAssetInsts); // 调用 ViewModel 的 OnAssetOpened，通知后端哪些资产被打开/选中
        }
    }

    private void MenuItem_Click(object? sender, RoutedEventArgs e) // 菜单项点击事件处理（MenuItem_Click）
    {
        if (sender is not MenuItem menuItem) // 如果事件源不是 MenuItem，则直接返回
            return; // 退出方法

        if (menuItem.Tag is not DataGridColumn dgColumn) // 从菜单项的 Tag 中获取对应的列对象，若不是 DataGridColumn 则返回
            return; // 退出方法

        dgColumn.IsVisible = menuItem.IsChecked; // 根据菜单项的选中状态设置列的可见性（同步 UI 与菜单）
    }

    // probably not great for many items but fine for a few
    // 对于大量项此方法可能效率不高，但用于少量项时可以接受
    private void SetSelectedItems(List<AssetInst> assets) // 由 ViewModel 调用以设置 UI 的选中项（SetSelectedItems）
    {
        if (DataContext is AssetDocumentViewModel docVm) // 确保 DataContext 是 AssetDocumentViewModel
        {
            dataGrid.SelectedItems.Clear(); // 先清空当前选中项集合
            foreach (var asset in assets) // 遍历传入的资产列表
            {
                if (docVm.Items.Contains(asset)) // 仅当 ViewModel 的 Items 包含该资产时才添加（避免无效项）
                    dataGrid.SelectedItems.Add(asset); // 将资产添加到 dataGrid 的 SelectedItems 中
            }

            if (assets.Count > 0)
            {
                // Use Post to ensure the DataGrid has updated its internal state (e.g. after a filter change)
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    dataGrid.ScrollIntoView(assets[0], null);
                });
            }
        }
    }

    // ====== Folder View Event Handlers ======

    private void FolderItem_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (sender is Border border && border.Tag is string typeName)
        {
            if (DataContext is AssetDocumentViewModel vm)
            {
                vm.OpenFolder(typeName);
            }
        }
    }

    private void GoBack_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AssetDocumentViewModel vm)
        {
            vm.GoBackToFolders();
        }
    }

    private void FolderDataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is AssetDocumentViewModel docVm)
        {
            var selectedAssetInsts = folderDataGrid.SelectedItems.Cast<AssetInst>().ToList();
            docVm.OnAssetOpened(selectedAssetInsts);
        }
    }
}
