using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using UABEANext4.ViewModels.Tools;
using System;
using System.Collections.Generic;
using UABEANext4.Controls.MeshPreviewer;

namespace UABEANext4.Views.Tools
{
    public partial class VrcaPreviewerToolView : UserControl
    {
        public VrcaPreviewerToolView()
        {
            InitializeComponent();
            var control = this.FindControl<MeshPreviewerControl>("PreviewerControl");
            if (control != null)
            {
                control.RequestNextFrameRendering();
                // Periodic update of GL Info
                Avalonia.Threading.DispatcherTimer.Run(() => {
                    if (DataContext is VrcaPreviewerToolViewModel vm) {
                        vm.GlInfo = $"GL: {control.GlRenderer}\nVER: {control.GlVersionFull}\nVIEW: {control.ViewportWidth}x{control.ViewportHeight}";
                        vm.KeysStatus = control.PressedKeysString;
                        vm.FocusStatus = control.IsControlFocused ? "FOCUSED" : "UNFOCUSED";
                        vm.ShaderStatus = control.LastShaderError;
                    }
                    return true;
                }, TimeSpan.FromSeconds(1));
            }
        }

        public void ForceFocus()
        {
            var control = this.FindControl<MeshPreviewerControl>("PreviewerControl");
            control?.Focus();
        }
    }
}
