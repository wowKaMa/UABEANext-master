using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using UABEANext4.ViewModels.Dialogs;

namespace UABEANext4.Views.Dialogs;

public partial class TextureCompressionWindow : Window
{
    public TextureCompressionWindow()
    {
        InitializeComponent();
    }

    public TextureCompressionWindow(TextureCompressionViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void CloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
