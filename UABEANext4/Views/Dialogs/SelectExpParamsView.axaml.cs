using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.Threading;
using System.Linq;
using UABEANext4.ViewModels.Dialogs;

namespace UABEANext4.Views.Dialogs
{
    public partial class SelectExpParamsView : UserControl
    {
        public SelectExpParamsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnItemDoubleTapped(object? sender, TappedEventArgs e)
        {
            if (sender is Panel p && p.DataContext is ExpParamSelectionItem item)
            {
                item.IsEditing = true;
                
                // Try to find the TextBox in the Panel children and focus it
                var textBox = p.Children.OfType<TextBox>().FirstOrDefault();
                if (textBox != null)
                {
                    // Use a slight delay to ensure the TextBox is visible before focusing
                    Dispatcher.UIThread.Post(() => {
                        textBox.Focus();
                        textBox.SelectAll();
                    });
                }
            }
        }

        private void OnRenameKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox tb && tb.DataContext is ExpParamSelectionItem item)
                {
                    item.ApplyNameChange();
                    item.IsEditing = false;
                }
            }
            else if (e.Key == Key.Escape)
            {
                if (sender is TextBox tb && tb.DataContext is ExpParamSelectionItem item)
                {
                    item.EditableName = item.Asset?.AssetName ?? ""; // Revert
                    item.IsEditing = false;
                }
            }
        }

        private void OnRenameLostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.DataContext is ExpParamSelectionItem item)
            {
                if (item.IsEditing)
                {
                    item.ApplyNameChange();
                    item.IsEditing = false;
                }
            }
        }
    }
}
