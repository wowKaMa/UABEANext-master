using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using UABEANext4.ViewModels.Tools;
using System;
using System.Linq;

namespace UABEANext4.Views.Tools
{
    public partial class PasswordCrackerWindow : Window
    {
        private bool _isDragging;
        private Point _lastPointerPosition;
        private Point _lastCanvasClickPos;

        public PasswordCrackerWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            if (DataContext is PasswordCrackerViewModel vm)
            {
                vm.RequestCenterView += () =>
                {
                    if (vm.States.Count == 0) return;

                    // 1. Calculate Bounds
                    double minX = vm.States.Min(s => s.X);
                    double maxX = vm.States.Max(s => s.X) + 160; // Include width
                    double minY = vm.States.Min(s => s.Y);
                    double maxY = vm.States.Max(s => s.Y) + 42;  // Include height

                    double centerX = (minX + maxX) / 2;
                    double centerY = (minY + maxY) / 2;

                    // 2. Auto Resize Window
                    double contentWidth = maxX - minX;
                    double contentHeight = maxY - minY;

                    // Apply reasonable padding and min/max constraints
                    this.Width = Math.Clamp(contentWidth + 400, 800, 1600);
                    this.Height = Math.Clamp(contentHeight + 200, 600, 1000);

                    // 3. Center ScrollViewer
                    var scroll = this.FindControl<ScrollViewer>("GraphScrollViewer");
                    if (scroll != null)
                    {
                        // Schedule after layout pass
                        Avalonia.Threading.Dispatcher.UIThread.Post(() => {
                             double viewportWidth = scroll.Viewport.Width;
                             double viewportHeight = scroll.Viewport.Height;
                             
                             scroll.Offset = new Vector(
                                 Math.Max(0, centerX - viewportWidth / 2),
                                 Math.Max(0, centerY - viewportHeight / 2)
                             );
                        }, Avalonia.Threading.DispatcherPriority.Loaded);
                    }
                };
            }
        }

        private void OnStatePointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (DataContext is PasswordCrackerViewModel vm && vm.IsMakingTransition) return;

            if (sender is Control control && control.DataContext is StateItem state)
            {
                var pointerProperties = e.GetCurrentPoint(this).Properties;
                if (pointerProperties.IsLeftButtonPressed)
                {
                    _isDragging = true;
                    _lastPointerPosition = e.GetPosition(this);
                    e.Pointer.Capture(control);
                    e.Handled = true;
                }
            }
        }

        private void OnStatePointerMoved(object? sender, PointerEventArgs e)
        {
            if (_isDragging && sender is Control control && control.DataContext is StateItem state)
            {
                var currentPointerPosition = e.GetPosition(this);
                var diff = currentPointerPosition - _lastPointerPosition;

                state.X += diff.X;
                state.Y += diff.Y;

                _lastPointerPosition = currentPointerPosition;
                e.Handled = true;
            }
        }

        private bool _justFinalizedTransition = false;

        private void OnStatePointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                e.Pointer.Capture(null);
                e.Handled = true;
                return;
            }

            // Handle transition finalization on PointerReleased for better reliability than Tapped
            if (sender is Control control && control.DataContext is StateItem state && DataContext is PasswordCrackerViewModel vm)
            {
                if (vm.IsMakingTransition)
                {
                    _justFinalizedTransition = true;
                    vm.FinalizeTransitionCommand.Execute(state);
                    e.Handled = true;
                    // Reset flag after a short delay or in Tapped
                }
            }
        }

        private void OnStateTapped(object? sender, TappedEventArgs e)
        {
            if (_justFinalizedTransition)
            {
                _justFinalizedTransition = false;
                e.Handled = true;
                return;
            }

            if (sender is Control control && control.DataContext is StateItem state && DataContext is PasswordCrackerViewModel vm)
            {
                if (!vm.IsMakingTransition)
                {
                    vm.SelectedState = state;
                    e.Handled = true;
                }
            }
        }
        private void OnMotionListBoxTapped(object? sender, TappedEventArgs e)
        {
            this.FindControl<Button>("MotionButton")?.Flyout?.Hide();
        }

        private void OnMotionFlyoutOpened(object? sender, EventArgs e)
        {
            this.FindControl<TextBox>("MotionSearchBox")?.Focus();
        }

        private void OnCanvasPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
            {
                var canvas = this.FindControl<Canvas>("GraphCanvas");
                if (canvas != null)
                {
                    _lastCanvasClickPos = e.GetPosition(canvas);
                }
            }
        }

        private void OnCanvasPointerMoved(object? sender, PointerEventArgs e)
        {
            if (DataContext is PasswordCrackerViewModel vm && vm.IsMakingTransition)
            {
                var canvas = sender as Canvas ?? this.FindControl<Canvas>("GraphCanvas");
                if (canvas != null)
                {
                    var pos = e.GetPosition(canvas);
                    vm.UpdatePendingTransitionCommand.Execute(pos);
                }
            }
        }

        private void OnCreateStateClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is PasswordCrackerViewModel vm && vm.SelectedLayer != null)
            {
                vm.CreateStateAt(vm.SelectedLayer, _lastCanvasClickPos.X, _lastCanvasClickPos.Y);
            }
        }
    }
}
