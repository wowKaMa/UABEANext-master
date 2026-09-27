using AssetsTools.NET;
using UABEANext4.AssetWorkspace;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using UABEANext4.Interfaces;

namespace UABEANext4.ViewModels.Dialogs
{
    public partial class CompressionSelectionViewModel : ViewModelBase, IDialogAware<BundleSaveMethod?>

    {
        public string Title => "Save Options (保存选项)";
        public int Width => 400;
        public int Height => 150;

        public event Action<BundleSaveMethod?>? RequestClose;


        [RelayCommand]
        private void SelectLZMA()
        {
            RequestClose?.Invoke(BundleSaveMethod.LZMA);
        }

        [RelayCommand]
        private void SelectLZ4()
        {
            RequestClose?.Invoke(BundleSaveMethod.LZ4);
        }

        [RelayCommand]
        private void SelectNone()
        {
            RequestClose?.Invoke(BundleSaveMethod.None);
        }

        [RelayCommand]
        private void SelectChunkedLZMA()
        {
            RequestClose?.Invoke(BundleSaveMethod.ChunkedLZMA);
        }

    }
}
