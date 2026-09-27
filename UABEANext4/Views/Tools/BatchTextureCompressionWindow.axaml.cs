using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using UABEANext4.ViewModels.Tools;

namespace UABEANext4.Views.Tools;

public partial class BatchTextureCompressionWindow : Window
{
    public BatchTextureCompressionWindow()
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

    protected override void OnOpened(System.EventArgs e)
    {
        base.OnOpened(e);
        if (DataContext is BatchTextureCompressionViewModel vm)
        {
            // Inject StorageProvider if needed (ViewModel uses it)
            vm.StorageProvider = StorageProvider;
            vm.RequestClose = () => Close();
        }
    }
}
