using System; // Required for EventArgs
using Avalonia.Controls;
using UABEANext4.ViewModels;
using UABEANext4.ViewModels.Dialogs;

namespace UABEANext4.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is MainViewModel vm)
        {
            vm.OnRequestShowVrcaPatcher += Vm_OnRequestShowVrcaPatcher;
            vm.OnRequestShowTextureCompression += Vm_OnRequestShowTextureCompression;
            vm.OnRequestShowTextureReplacement += Vm_OnRequestShowTextureReplacement;
            vm.OnRequestShowPasswordCracker += Vm_OnRequestShowPasswordCracker;
            vm.OnRequestShowDeformationRepair += Vm_OnRequestShowDeformationRepair;
        }
    }

    private void Vm_OnRequestShowDeformationRepair()
    {
        if (DataContext is MainViewModel vm)
        {
            var viewModel = new ViewModels.Tools.DeformationRepairViewModel(vm.Workspace);
            var window = new Tools.DeformationRepairWindow(viewModel);
            var owner = TopLevel.GetTopLevel(this) as Window;
            if (owner != null)
                window.Show(owner);
            else
                window.Show();
        }
    }

    private void Vm_OnRequestShowTextureReplacement()
    {
        if (DataContext is MainViewModel vm)
        {
            var window = new Tools.BatchTextureReplacementWindow();
            window.DataContext = new ViewModels.Tools.BatchTextureReplacementViewModel(vm.Workspace);
            var owner = TopLevel.GetTopLevel(this) as Window;
            if (owner != null)
                window.Show(owner);
            else
                window.Show();
        }
    }


    private void Vm_OnRequestShowVrcaPatcher(VrcaSmartPatcherViewModel vm)
    {
        var window = new Dialogs.VrcaSmartPatcherWindow(vm);
        var owner = TopLevel.GetTopLevel(this) as Window;
        if (owner != null)
            window.Show(owner);
        else
            window.Show();
    }

    private void Vm_OnRequestShowTextureCompression(TextureCompressionViewModel vm)
    {
        var window = new Dialogs.TextureCompressionWindow(vm);
        var owner = TopLevel.GetTopLevel(this) as Window;
        if (owner != null)
            window.Show(owner);
        else
            window.Show();
    }

    private void Vm_OnRequestShowPasswordCracker()
    {
        if (DataContext is MainViewModel vm)
        {
            var window = new Tools.PasswordCrackerWindow();
            window.DataContext = new ViewModels.Tools.PasswordCrackerViewModel(vm.Workspace);
            var owner = TopLevel.GetTopLevel(this) as Window;
            if (owner != null)
                window.Show(owner);
            else
                window.Show();
        }
    }
}
