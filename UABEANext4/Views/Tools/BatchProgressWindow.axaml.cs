using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using UABEANext4.ViewModels.Tools;

namespace UABEANext4.Views.Tools;

public partial class BatchProgressWindow : Window
{
    public BatchProgressWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
