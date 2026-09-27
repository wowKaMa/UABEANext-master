using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using UABEANext4.AssetWorkspace;
using UABEANext4.Interfaces;
using UABEANext4.Util;
using UABEANext4.ViewModels.Tools;

namespace UABEANext4.ViewModels.Dialogs
{
    public partial class SelectExpMenuViewModel : ObservableObject, IDialogAware<ExpMenuSelectionItem>
    {
        private Workspace _workspace;
        private AssetInst _contextAsset;

        [ObservableProperty] private string _title = "选择菜单 (Select Expressions Menu)";
        public int Width => 500;
        public int Height => 400;

        public event Action<ExpMenuSelectionItem?>? RequestClose;

        private List<ExpMenuSelectionItem> _allAssets;
        [ObservableProperty] private ObservableCollection<ExpMenuSelectionItem> _assets;
        [ObservableProperty] private ExpMenuSelectionItem? _selectedAsset;

        [ObservableProperty] private string _searchText = string.Empty;

        public SelectExpMenuViewModel(Workspace workspace, AssetInst contextAsset, List<ExpMenuSelectionItem> assets)
        {
            _workspace = workspace;
            _contextAsset = contextAsset;
            _allAssets = assets;
            _assets = new ObservableCollection<ExpMenuSelectionItem>(assets);
            if (Assets.Count > 0) SelectedAsset = Assets[0];
        }

        partial void OnSearchTextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Assets = new ObservableCollection<ExpMenuSelectionItem>(_allAssets);
            }
            else
            {
                var filtered = _allAssets.Where(a => 
                    a.DisplayName.Contains(value, StringComparison.OrdinalIgnoreCase) || 
                    a.FileName.Contains(value, StringComparison.OrdinalIgnoreCase)
                ).ToList();
                Assets = new ObservableCollection<ExpMenuSelectionItem>(filtered);
            }
        }

        [RelayCommand]
        private void Confirm()
        {
            RequestClose?.Invoke(SelectedAsset);
        }

        [RelayCommand]
        private void Cancel()
        {
            RequestClose?.Invoke(null);
        }
    }
}
