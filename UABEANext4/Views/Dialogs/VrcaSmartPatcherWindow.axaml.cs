using Avalonia.Controls;
using UABEANext4.ViewModels.Dialogs;

namespace UABEANext4.Views.Dialogs
{
    public partial class VrcaSmartPatcherWindow : Window
    {
        public VrcaSmartPatcherWindow()
        {
            InitializeComponent();
        }

        public VrcaSmartPatcherWindow(VrcaSmartPatcherViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}
