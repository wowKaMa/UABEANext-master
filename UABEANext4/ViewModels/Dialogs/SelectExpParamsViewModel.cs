using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UABEANext4.Interfaces;
using AssetsTools.NET;
using UABEANext4.AssetWorkspace;
using UABEANext4.ViewModels.Tools;
using System.Linq;
using System.Threading.Tasks;
using UABEANext4.Util;
using AssetsTools.NET.Extra;

namespace UABEANext4.ViewModels.Dialogs
{
    public partial class SelectExpParamsViewModel : ObservableObject, IDialogAware<ExpParamSelectionItem>
    {
        private Workspace _workspace;
        private AssetInst _contextAsset;

        public string Title => "选择菜单参数表 (Select Expression Parameters)";
        public int Width => 500;
        public int Height => 400;

        public event Action<ExpParamSelectionItem?>? RequestClose;

        [ObservableProperty] private ObservableCollection<ExpParamSelectionItem> _assets;
        [ObservableProperty] private ExpParamSelectionItem? _selectedAsset;
        [ObservableProperty] private bool _showManagementButtons;

        private List<ExpParamSelectionItem> _allAssets;

        public SelectExpParamsViewModel(Workspace workspace, AssetInst contextAsset, List<ExpParamSelectionItem> assets, bool showManagementButtons = false)
        {
            _workspace = workspace;
            _contextAsset = contextAsset;
            _allAssets = assets;
            _assets = new ObservableCollection<ExpParamSelectionItem>(assets);
            ShowManagementButtons = showManagementButtons;
            if (Assets.Count > 0) SelectedAsset = Assets[0];
        }

        [RelayCommand]
        private async Task AddMenu()
        {
            if (_contextAsset == null) return;
            var fileInst = _contextAsset.FileInstance;

            // 1. Find a unique PathID
            long newPathId = 1;
            var assetInfos = fileInst.file.Metadata.AssetInfos;
            if (assetInfos.Count > 0)
            {
                newPathId = assetInfos.Max(a => a.PathId) + 1;
            }

            // 2. Find avatar descriptor to get parameters
            long paramsPathId = 0;
            foreach (var info in fileInst.file.Metadata.AssetInfos)
            {
                if (info.TypeId == (int)AssetClassID.MonoBehaviour)
                {
                    var asset = (info is AssetInst ai) ? ai : new AssetInst(fileInst, info);
                    var scriptName = _workspace.Namer.GetMonoBehaviourNameFast(asset);
                    if (scriptName == "VRCAvatarDescriptor")
                    {
                        var bf = _workspace.GetBaseField(asset);
                        if (bf != null)
                        {
                            paramsPathId = bf["expressionParameters"]["m_PathID"].AsLong;
                            break;
                        }
                    }
                }
            }

            // 3. Create the new asset
            ushort scriptIdx = ushort.MaxValue;
            var scriptTypes = fileInst.file.Metadata.ScriptTypes;
            for (int i = 0; i < scriptTypes.Count; i++)
            {
                var pptr = scriptTypes[i];
                var scriptBf = _workspace.GetBaseField(fileInst, pptr.FileId, pptr.PathId);
                if (scriptBf != null)
                {
                    if (scriptBf["m_ClassName"].AsString == "VRCExpressionsMenu" && 
                        scriptBf["m_Namespace"].AsString == "VRC.SDK3.Avatars.ScriptableObjects")
                    {
                        scriptIdx = (ushort)i;
                        break;
                    }
                }
            }
            
            if (scriptIdx == ushort.MaxValue)
            {
                await MessageBoxUtil.ShowDialog("Error", "Could not find VRCExpressionsMenu script in asset file.");
                return;
            }

            var baseInfo = AssetFileInfo.Create(
                fileInst.file, newPathId, (int)AssetClassID.MonoBehaviour, 
                scriptIdx, _workspace.Manager.ClassDatabase, false
            );
            
            if (baseInfo == null)
            {
                await MessageBoxUtil.ShowDialog("Error", "Could not create asset info for VRCExpressionsMenu.");
                return;
            }

            var newAsset = new AssetInst(fileInst, baseInfo);
            
            // 4. Create default base field from template
            var tempField = _workspace.GetTemplateField(newAsset);
            if (tempField == null)
            {
                await MessageBoxUtil.ShowDialog("Error", "Could not load template for VRCExpressionsMenu.");
                return;
            }

            var baseField = ValueBuilder.DefaultValueFieldFromTemplate(tempField);
            baseField["m_Enabled"].AsInt = 1;
            baseField["m_Script"]["m_FileID"].AsInt = 0;
            baseField["m_Script"]["m_PathID"].AsLong = 8268836203660554979L;
            baseField["m_Name"].AsString = "NewMenu(Clone)";
            baseField["Parameters"]["m_PathID"].AsLong = paramsPathId;
            baseField["controls"]["Array"].Children = new List<AssetTypeValueField>();

            // 5. Add to file and workspace
            fileInst.file.Metadata.AddAssetInfo(newAsset);
            newAsset.UpdateAssetDataAndRow(_workspace, baseField);

            // 6. Add to preload table if AssetBundle exists
            var preloadAsset = fileInst.file.Metadata.GetAssetInfo(1);
            if (preloadAsset != null && preloadAsset.TypeId == (int)AssetClassID.AssetBundle)
            {
                var preloadInst = (preloadAsset is AssetInst pai) ? pai : new AssetInst(fileInst, preloadAsset);
                var preloadBf = _workspace.GetBaseField(preloadInst);
                if (preloadBf != null)
                {
                    var preloadArray = preloadBf["m_PreloadTable"]["Array"];
                    if (!preloadArray.IsDummy && preloadArray.Children != null)
                    {
                        var newChild = ValueBuilder.DefaultValueFieldFromTemplate(preloadArray.TemplateField.Children[1]);
                        newChild["m_FileID"].AsInt = 0;
                        newChild["m_PathID"].AsLong = newPathId;
                        preloadArray.Children.Add(newChild);
                        
                        var sizeField = preloadBf["m_PreloadTable"]["size"];
                        if (!sizeField.IsDummy) sizeField.AsInt = preloadArray.Children.Count;
                        
                        preloadInst.UpdateAssetDataAndRow(_workspace, preloadBf);
                    }
                }
            }

            var wsItem = _workspace.FindWorkspaceItemByInstance(fileInst);
            if (wsItem != null) _workspace.Dirty(wsItem);

            // Update UI list
            var newItem = new ExpParamSelectionItem(_workspace, newAsset, null);
            _allAssets.Add(newItem);
            if (!Assets.Contains(newItem))
            {
                Assets.Add(newItem);
            }
            SelectedAsset = newItem;
        }

        [RelayCommand]
        private async Task DeleteMenu()
        {
            if (SelectedAsset == null || SelectedAsset.Asset == null) return;

            var dialogRes = await MessageBoxUtil.ShowDialog(
                "Delete Menu",
                $"Are you sure you want to delete {SelectedAsset.DisplayName}?",
                MessageBoxType.YesNo
            );
            if (dialogRes == MessageBoxResult.No) return;

            var asset = SelectedAsset.Asset;
            var fileInst = asset.FileInstance;

            // 1. Remove from preload table if AssetBundle exists
            var preloadAsset = fileInst.file.Metadata.GetAssetInfo(1);
            if (preloadAsset != null && preloadAsset.TypeId == (int)AssetClassID.AssetBundle)
            {
                var preloadInst = (preloadAsset is AssetInst pai) ? pai : new AssetInst(fileInst, preloadAsset);
                var preloadBf = _workspace.GetBaseField(preloadInst);
                if (preloadBf != null)
                {
                    var preloadArray = preloadBf["m_PreloadTable"]["Array"];
                    if (!preloadArray.IsDummy && preloadArray.Children != null)
                    {
                        int removed = preloadArray.Children.RemoveAll(c => c["m_PathID"].AsLong == asset.PathId);
                        if (removed > 0)
                        {
                            var sizeField = preloadBf["m_PreloadTable"]["size"];
                            if (!sizeField.IsDummy) sizeField.AsInt = preloadArray.Children.Count;
                            preloadInst.UpdateAssetDataAndRow(_workspace, preloadBf);
                        }
                    }
                }
            }

            // 2. Remove from file metadata
            fileInst.file.Metadata.RemoveAssetInfo(asset);
            
            // 3. Update Workspace
            var wsItem = _workspace.FindWorkspaceItemByInstance(fileInst);
            if (wsItem != null) _workspace.Dirty(wsItem);

            // 4. Update UI list
            _allAssets.Remove(SelectedAsset);
            Assets.Remove(SelectedAsset);
            if (Assets.Count > 0) SelectedAsset = Assets[0];
            else SelectedAsset = null;
        }

        partial void OnSelectedAssetChanged(ExpParamSelectionItem? value)
        {
        }

        [RelayCommand]
        private void ApplyRename(ExpParamSelectionItem item)
        {
            item?.ApplyNameChange();
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
