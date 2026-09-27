using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using UABEANext4.AssetWorkspace;
using UABEANext4.ViewModels.Tools;

namespace UABEANext4.Views.Tools
{
    public partial class OscToolWindow : Window
    {
        public OscToolWindow(Workspace workspace)
        {
            InitializeComponent();
            DataContext = new OscToolViewModel(workspace);
        }

        public OscToolWindow()
        {
             InitializeComponent();
             DataContext = new OscToolViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
