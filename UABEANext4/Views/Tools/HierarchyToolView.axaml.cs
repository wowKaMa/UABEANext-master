using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Linq;
using UABEANext4.Logic.Hierarchy;
using UABEANext4.ViewModels.Tools;
using System.Reflection;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using Avalonia.Input.Raw;
using Avalonia.Threading;

namespace UABEANext4.Views.Tools;

public partial class HierarchyToolView : UserControl
{
    private HierarchyItem? _draggedItem;
    private Point _dragStartPoint;
    private bool _isDragging;
    private IDisposable? _rawWheelSubscription;
    private static IntPtr _mouseHook = IntPtr.Zero;
    private static LowLevelMouseProc? _mouseProc;
    private static HierarchyToolView? _instance; // For static hook callback

    public HierarchyToolView()
    {
        InitializeComponent();
        _instance = this;

        var treeView = this.FindControl<TreeView>("GameObjectTreeView");
        if (treeView != null)
        {
            treeView.AddHandler(DragDrop.DropEvent, OnDrop);
            treeView.AddHandler(DragDrop.DragOverEvent, OnDragOver);
            
            // Selection preservation: tunnel PointerPressed with handledEventsToo=true
            this.AddHandler(PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel, handledEventsToo: true);
            this.AddHandler(PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel, handledEventsToo: true);
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _rawWheelSubscription?.Dispose();
        _rawWheelSubscription = null;
    }

    #region Native Mouse Hook for Drag-and-Drop Scroll
    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int x; public int y; }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    private const int WH_MOUSE_LL = 14;
    private const int WM_MOUSEWHEEL = 0x020A;

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_MOUSEWHEEL)
        {
            MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
            short delta = (short)((hookStruct.mouseData >> 16) & 0xffff);
            
            if (_instance != null)
            {
                Dispatcher.UIThread.Post(() => {
                    var treeView = _instance.FindControl<TreeView>("GameObjectTreeView");
                    var scrollViewer = treeView?.FindDescendantOfType<ScrollViewer>();
                    if (scrollViewer != null)
                    {
                        var newOffset = scrollViewer.Offset.WithY(scrollViewer.Offset.Y - (delta / 120.0 * 100)); // Standard scroll: 100px per notch
                        scrollViewer.Offset = newOffset;
                    }
                }, DispatcherPriority.Background);
            }
        }
        return CallNextHookEx(_mouseHook, nCode, wParam, lParam);
    }

    private void InstallMouseHook()
    {
        if (_mouseHook != IntPtr.Zero) return;
        _mouseProc = HookCallback;
        using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
        using (var curModule = curProcess.MainModule)
        {
            _mouseHook = SetWindowsHookEx(WH_MOUSE_LL, _mouseProc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private void UninstallMouseHook()
    {
        if (_mouseHook != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_mouseHook);
            _mouseHook = IntPtr.Zero;
            _mouseProc = null;
        }
    }
    #endregion

    private void HierarchyTreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is HierarchyToolViewModel hrVm)
        {
            foreach (var item in e.RemovedItems)
            {
                if (item is HierarchyItem hi) hrVm.SelectedItems.Remove(hi);
            }
            foreach (var item in e.AddedItems)
            {
                if (item is HierarchyItem hi && !hrVm.SelectedItems.Contains(hi)) hrVm.SelectedItems.Add(hi);
            }

            // Update ActiveAssets for the Inspector
            if (hrVm.SelectedItems.Count > 0)
            {
                var lastItem = hrVm.SelectedItems.LastOrDefault() as HierarchyItem;
                if (lastItem?.Asset != null)
                {
                    hrVm.SelectedItemsChanged(lastItem.Asset);
                }
            }
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _isDragging = false;
        _draggedItem = null;

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            // Only initiate drag when clicking on a TextBlock (the item name),
            // NOT on the expand/collapse toggle button or other UI elements
            var source = e.Source as Visual;
            if (source is not Avalonia.Controls.TextBlock && source?.FindAncestorOfType<Avalonia.Controls.TextBlock>() == null)
                return; // Clicked on expand/collapse arrow or other element, skip

            _dragStartPoint = e.GetPosition(this);

            var treeViewItem = source?.FindAncestorOfType<TreeViewItem>();
            if (treeViewItem?.DataContext is HierarchyItem item && item.TransformPathId != 0)
            {
                _draggedItem = item;

                // --- FIX: Preserve selection on drag initiation ---
                // If the item we clicked is ALREADY selected, we Mark it as handled 
                // to prevent the TreeView from resetting the selection to just this one item.
                if (DataContext is HierarchyToolViewModel vm && vm.SelectedItems.Contains(item))
                {
                    e.Handled = true; 
                }
            }
        }
    }

    private async void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_draggedItem == null || _isDragging) return;

        // Verify left button is still held
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _draggedItem = null;
            return;
        }

        var currentPos = e.GetPosition(this);
        var diff = currentPos - _dragStartPoint;

        // Require significant movement (20px) to avoid accidental drags
        if (System.Math.Abs(diff.X) > 20 || System.Math.Abs(diff.Y) > 20)
        {
            _isDragging = true;

            var itemsToDrag = new List<HierarchyItem>();
            if (DataContext is HierarchyToolViewModel vm)
            {
                // If the item we grabbed is part of the selection, drag the whole selection
                var selectedItems = vm.SelectedItems.Cast<HierarchyItem>().ToList();
                if (selectedItems.Contains(_draggedItem))
                {
                    itemsToDrag.AddRange(selectedItems);
                }
                else
                {
                    itemsToDrag.Add(_draggedItem);
                }
            }
            else
            {
                itemsToDrag.Add(_draggedItem);
            }

            var data = new DataObject();
            data.Set("HierarchyItems", itemsToDrag);

            InstallMouseHook();
            try 
            {
                await DragDrop.DoDragDrop(e, data, DragDropEffects.Move);
            }
            finally
            {
                UninstallMouseHook();
            }

            _isDragging = false;
            _draggedItem = null;
        }
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        if (!e.Data.Contains("HierarchyItems"))
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        // Check target
        var target = GetHierarchyItemFromDragEvent(e);
        var sources = e.Data.Get("HierarchyItems") as List<HierarchyItem>;

        if (target == null || sources == null || sources.Contains(target))
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        // Allow drop if target is a GameObject (PathID != 0) OR a File Container (PathID == 0 but has FileInstance)
        if (target.TransformPathId == 0 && target.FileInstance == null)
        {
            e.DragEffects = DragDropEffects.None;
            return;
        }

        e.DragEffects = DragDropEffects.Move;
    }

    private async void OnDrop(object? sender, DragEventArgs e)
    {
        if (!e.Data.Contains("HierarchyItems")) return;

        var sources = e.Data.Get("HierarchyItems") as List<HierarchyItem>;
        var target = GetHierarchyItemFromDragEvent(e);

        if (sources == null || sources.Count == 0 || target == null || sources.Contains(target)) return;
        // Check if target is valid destination
        if (target.TransformPathId == 0 && target.FileInstance == null) return;

        if (DataContext is HierarchyToolViewModel vm)
        {
            await vm.MoveItemsAsync(sources, target);
        }
    }

    private HierarchyItem? GetHierarchyItemFromDragEvent(DragEventArgs e)
    {
        var visual = e.Source as Visual;
        var treeViewItem = visual?.FindAncestorOfType<TreeViewItem>();
        return treeViewItem?.DataContext as HierarchyItem;
    }

    private async void SetupOutfit_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem) return;

        // Get the HierarchyItem from the ContextMenu's DataContext
        HierarchyItem? targetItem = null;

        // The MenuItem is inside a ContextMenu which is on a TextBlock bound to HierarchyItem
        if (menuItem.Parent is ContextMenu contextMenu)
        {
            targetItem = contextMenu.DataContext as HierarchyItem;
        }

        if (targetItem == null) return;

        if (DataContext is HierarchyToolViewModel vm)
        {
            var report = vm.SetupOutfit(targetItem);

            // Show the diagnostic report window
            var dialogService = CommunityToolkit.Mvvm.DependencyInjection.Ioc.Default.GetService<UABEANext4.Services.IDialogService>();
            if (dialogService != null)
            {
                var reportVm = new UABEANext4.ViewModels.Dialogs.SetupOutfitReportViewModel(report);
                dialogService.Show(reportVm);
            }
        }
    }

    private void RemoveNode_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem) return;

        HierarchyItem? targetItem = null;

        if (menuItem.Parent is ContextMenu contextMenu)
        {
            targetItem = contextMenu.DataContext as HierarchyItem;
        }

        if (targetItem == null) return;

        if (DataContext is HierarchyToolViewModel vm)
        {
            vm.RemoveNode(targetItem);
        }
    }
}
