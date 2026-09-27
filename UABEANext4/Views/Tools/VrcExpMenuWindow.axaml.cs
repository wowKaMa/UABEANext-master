using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using UABEANext4.ViewModels.Tools;
using UABEANext4.AssetWorkspace;
using AssetsTools.NET;
using Avalonia.Interactivity;

namespace UABEANext4.Views.Tools
{
    public partial class VrcExpMenuWindow : Window
    {
        public VrcExpMenuWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public VrcExpMenuWindow(Workspace workspace, AssetInst asset) : this()
        {
            DataContext = new VrcExpMenuViewModel(workspace, asset);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
