using Avalonia.Controls;
using System.ComponentModel;
using UABEANext4.ViewModels.Dialogs;

namespace UABEANext4.Views.Dialogs;

public partial class CompressionSelectionWindow : Window
{
    public CompressionSelectionWindow()
    {
        InitializeComponent();
        
        DataContext = new CompressionSelectionViewModel();
    }
}
