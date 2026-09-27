using Avalonia.Controls;
using UABEANext4.ViewModels.Tools;

namespace UABEANext4.Views.Tools
{
    public partial class DeformationRepairWindow : Window
    {
        public DeformationRepairWindow()
        {
            InitializeComponent();
        }

        public DeformationRepairWindow(DeformationRepairViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}
