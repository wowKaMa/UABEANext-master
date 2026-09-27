using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using UABEANext4.AssetWorkspace;
using System.Collections.Generic;
using UABEANext4.Logic;
using System;
using System.Linq;
using Avalonia.Threading;
using UABEANext4.Views.Tools;
using System.Threading.Tasks;
using UABEANext4.ViewModels.Dialogs;

namespace UABEANext4.ViewModels.Tools
{
    public class VrcExpMenuParamInfo
    {
        public string Name { get; set; } = "";
        public int Type { get; set; }
        public string TypeName => Type switch
        {
            0 => "Int",
            1 => "Float",
            2 => "Bool",
            _ => "Unknown"
        };
        public string DisplayName => string.IsNullOrEmpty(Name) ? "" : $"{Name}, {TypeName}";
    }

    public class ExpMenuSelectionItem
    {
        public AssetInst? Asset { get; }
        public string DisplayName { get; }
        public string FileName { get; }

        public ExpMenuSelectionItem(AssetInst? asset)
        {
            Asset = asset;
            if (asset == null)
            {
                DisplayName = "None (VRC Expressions Menu)";
                FileName = "";
            }
            else
            {
                DisplayName = asset.AssetName ?? "Unnamed Menu";
                FileName = asset.FileInstance.name;
            }
        }
    }

    public partial class VrcExpMenuViewModel : ObservableObject
    {
        private Workspace _workspace;
        private AssetInst _asset;

        [ObservableProperty] private string _assetName;
        [ObservableProperty] private ObservableCollection<VrcExpMenuItem> _controls = new();
        [ObservableProperty] private VrcExpMenuItem? _selectedControl;

        [ObservableProperty] private long _parametersPathId;
        [ObservableProperty] private string _parametersName = "None (VRCExpressionParameters)";
        [ObservableProperty] private ObservableCollection<VrcExpMenuParamInfo> _availableParameters = new() { new VrcExpMenuParamInfo { Name = "", Type = -1 } };
        private Dictionary<string, int> _paramTypeMap = new(); // name -> type (0=Int, 1=Float, 2=Bool)

        public string SelectionCountText => $"{(SelectedControl == null ? 0 : Controls.IndexOf(SelectedControl) + 1)} / {Controls.Count}";

        public List<VrcExpMenuControlType> ControlTypes { get; } = new()
        {
            new VrcExpMenuControlType(101, "Button"),
            new VrcExpMenuControlType(102, "Toggle"),
            new VrcExpMenuControlType(103, "SubMenu"),
            new VrcExpMenuControlType(201, "TwoAxisPuppet"),
            new VrcExpMenuControlType(202, "FourAxisPuppet"),
            new VrcExpMenuControlType(203, "RadialPuppet")
        };

        public VrcExpMenuViewModel(Workspace workspace, AssetInst asset)
        {
            _workspace = workspace;
            _asset = asset;
            AssetName = asset.AssetName ?? "Unknown";
            LoadMenu(workspace, asset);
        }

        private void LoadMenu(Workspace workspace, AssetInst asset)
        {
            var baseField = workspace.GetBaseField(asset);
            if (baseField == null) return;

            ParametersPathId = baseField["Parameters"]["m_PathID"].AsLong;
            LoadParameterTypes();

            var dControls = baseField["controls"]["Array"];
            if (dControls.IsDummy || dControls.Children == null) return;

            foreach (var c in dControls.Children)
            {
                var item = new VrcExpMenuItem(this)
                {
                    Name = c["name"].AsString,
                    IconPathId = c["icon"]["m_PathID"].AsLong,
                    Parameter = c["parameter"]["name"].AsString,
                    Value = c["value"].AsFloat,
                    Style = c["style"].AsInt,
                    SubMenuPathId = c["subMenu"]["m_PathID"].AsLong,
                    Type = c["type"].AsInt
                };

                var subParams = c["subParameters"]["Array"];
                if (!subParams.IsDummy && subParams.Children != null)
                {
                    item.SubParameters.Clear();
                    foreach (var sp in subParams.Children)
                        item.SubParameters.Add(new VrcExpMenuSubParam { Name = sp["name"].AsString });
                }

                var labels = c["labels"]["Array"];
                if (!labels.IsDummy && labels.Children != null)
                {
                    item.Labels.Clear();
                    foreach (var l in labels.Children)
                    {
                        item.Labels.Add(new VrcExpMenuLabel(this)
                        {
                            Name = l["name"].AsString,
                            IconPathId = l["icon"]["m_PathID"].AsLong
                        });
                    }
                }

                Controls.Add(item);
            }
        }

        private void LoadParameterTypes()
        {
            _paramTypeMap.Clear();
            AvailableParameters.Clear();
            AvailableParameters.Add(new VrcExpMenuParamInfo { Name = "", Type = -1 });

            if (ParametersPathId == 0)
            {
                ParametersName = "None (VRCExpressionParameters)";
                return;
            }

            var paramAsset = _workspace.GetAssetInst(_asset.FileInstance, 0, ParametersPathId);
            if (paramAsset != null)
            {
                ParametersName = $"{paramAsset.AssetName} (VRCExpressionParameters)";
                var baseField = _workspace.GetBaseField(paramAsset);
                if (baseField != null)
                {
                    var pArray = baseField["parameters"]["Array"];
                    if (!pArray.IsDummy && pArray.Children != null)
                    {
                        foreach (var p in pArray.Children)
                        {
                            string name = p["name"].AsString;
                            int type = p["valueType"].AsInt; 
                            _paramTypeMap[name] = type;
                            AvailableParameters.Add(new VrcExpMenuParamInfo { Name = name, Type = type });
                        }
                    }
                }
            }
            else
            {
                ParametersName = $"{ParametersPathId} (VRCExpressionParameters)";
            }
        }

        public int GetParamType(string name)
        {
            if (string.IsNullOrEmpty(name)) return -1;
            if (_paramTypeMap.TryGetValue(name, out int type)) return type;
            return -1;
        }

        public AssetInst? GetAssetInst(long pathId)
        {
            if (pathId == 0) return null;
            return _workspace.GetAssetInst(_asset.FileInstance, 0, pathId);
        }

        [RelayCommand]
        public void AddControl()
        {
            var newItem = new VrcExpMenuItem(this)
            {
                Name = "New Control",
                Type = 101, // 101 is Button
                Parameter = "",
                Value = 1.0f, // Must default to 1 so Button/Toggle triggers TRUE
                SubMenuPathId = 0
            };
            Controls.Add(newItem);
            SelectedControl = newItem;
            OnPropertyChanged(nameof(SelectionCountText));
        }

        [RelayCommand(CanExecute = nameof(CanDeleteControl))]
        public void DeleteControl()
        {
            if (SelectedControl != null)
            {
                int index = Controls.IndexOf(SelectedControl);
                if (index != -1)
                {
                    Controls.RemoveAt(index);
                    if (Controls.Count > 0)
                        SelectedControl = Controls[Math.Min(index, Controls.Count - 1)];
                    else
                        SelectedControl = null;
                    
                    OnPropertyChanged(nameof(SelectionCountText));
                }
            }
        }

        private bool CanDeleteControl => SelectedControl != null;

        [RelayCommand(CanExecute = nameof(CanMoveUp))]
        public void MoveUp()
        {
            if (SelectedControl == null) return;
            int index = Controls.IndexOf(SelectedControl);
            if (index > 0)
            {
                Controls.Move(index, index - 1);
                OnPropertyChanged(nameof(SelectionCountText));
            }
        }

        private bool CanMoveUp => SelectedControl != null && Controls.IndexOf(SelectedControl) > 0;

        [RelayCommand(CanExecute = nameof(CanMoveDown))]
        public void MoveDown()
        {
            if (SelectedControl == null) return;
            int index = Controls.IndexOf(SelectedControl);
            if (index < Controls.Count - 1)
            {
                Controls.Move(index, index + 1);
                OnPropertyChanged(nameof(SelectionCountText));
            }
        }

        private bool CanMoveDown => SelectedControl != null && Controls.IndexOf(SelectedControl) < Controls.Count - 1;

        partial void OnSelectedControlChanged(VrcExpMenuItem? value)
        {
            DeleteControlCommand.NotifyCanExecuteChanged();
            MoveUpCommand.NotifyCanExecuteChanged();
            MoveDownCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(SelectionCountText));
        }

        [RelayCommand]
        public void OpenParameters()
        {
            if (ParametersPathId == 0) return;
            var paramAsset = _workspace.GetAssetInst(_asset.FileInstance, 0, ParametersPathId);
            if (paramAsset != null)
            {
                Dispatcher.UIThread.Post(() => {
                    var win = new VrcExpParamsWindow(_workspace, paramAsset);
                    win.Show();
                });
            }
        }

        [RelayCommand]
        public void OpenSubMenu()
        {
            if (SelectedControl == null || SelectedControl.SubMenuPathId == 0) return;
            var subAsset = _workspace.GetAssetInst(_asset.FileInstance, 0, SelectedControl.SubMenuPathId);
            if (subAsset != null)
            {
                Dispatcher.UIThread.Post(() => {
                    var win = new VrcExpMenuWindow(_workspace, subAsset);
                    win.Show();
                });
            }
        }

        private async Task<List<AssetInst>> ScanTextures()
        {
            var textures = new List<AssetInst>();
            await Task.Run(() =>
            {
                var assetsFiles = WorkspaceItem.GetAssetsFileWorkspaceItems(_workspace.RootItems);
                foreach (var item in assetsFiles)
                {
                    if (item.Object is not AssetsFileInstance afi) continue;
                    foreach (var info in afi.file.AssetInfos)
                    {
                        if (info.TypeId == (int)AssetClassID.Texture2D || info.TypeId == (int)AssetClassID.Cubemap)
                        {
                            var asset = _workspace.GetAssetInst(afi, 0, info.PathId);
                            if (asset != null) textures.Add(asset);
                        }
                    }
                }
            });
            return textures;
        }

        [RelayCommand]
        public async Task SelectParameters()
        {
            var paramsList = new List<ExpMenuSelectionItem> { new ExpMenuSelectionItem(null) };
            
            await Task.Run(() => {
                var assetFiles = WorkspaceItem.GetAssetsFileWorkspaceItems(_workspace.RootItems);
                foreach (var afItem in assetFiles)
                {
                    if (afItem.Object is AssetsFileInstance afi)
                    {
                        foreach (var info in afi.file.Metadata.AssetInfos)
                        {
                            if (info.TypeId == (int)AssetClassID.MonoBehaviour)
                            {
                                var asset = (info is AssetInst ai) ? ai : new AssetInst(afi, info);
                                string scriptName = _workspace.Namer.GetMonoBehaviourNameFast(asset);
                                if (scriptName == "VRCExpressionParameters")
                                {
                                    paramsList.Add(new ExpMenuSelectionItem(asset));
                                }
                            }
                        }
                    }
                }
            });

            var dialogService = CommunityToolkit.Mvvm.DependencyInjection.Ioc.Default.GetRequiredService<UABEANext4.Services.IDialogService>();
            var paramItems = paramsList.Select(m => new ExpParamSelectionItem(_workspace, m.Asset, null)).ToList();
            var vm = new SelectExpParamsViewModel(_workspace, _asset, paramItems, false);
            var result = await dialogService.ShowDialog(vm);

            if (result != null)
            {
                ParametersPathId = result.Asset?.PathId ?? 0;
                LoadParameterTypes();
                OnPropertyChanged(nameof(ParametersName));
                foreach (var item in Controls)
                {
                    item.NotifyTypeBooleans(); // Refresh parameter types in UI
                }
            }
        }

        [RelayCommand]
        public async Task SelectSubMenu()
        {
            if (SelectedControl == null) return;

            var menus = new List<ExpMenuSelectionItem> { new ExpMenuSelectionItem(null) };
            
            await Task.Run(() => {
                var assetFiles = WorkspaceItem.GetAssetsFileWorkspaceItems(_workspace.RootItems);
                foreach (var afItem in assetFiles)
                {
                    if (afItem.Object is AssetsFileInstance afi)
                    {
                        foreach (var info in afi.file.Metadata.AssetInfos)
                        {
                            if (info.TypeId == (int)AssetClassID.MonoBehaviour)
                            {
                                var asset = (info is AssetInst ai) ? ai : new AssetInst(afi, info);
                                string scriptName = _workspace.Namer.GetMonoBehaviourNameFast(asset);
                                if (scriptName == "VRCExpressionsMenu")
                                {
                                    menus.Add(new ExpMenuSelectionItem(asset));
                                }
                            }
                        }
                    }
                }
            });

            var dialogService = CommunityToolkit.Mvvm.DependencyInjection.Ioc.Default.GetRequiredService<UABEANext4.Services.IDialogService>();
            var paramItems = menus.Select(m => new ExpParamSelectionItem(_workspace, m.Asset, null)).ToList();
            var vm = new SelectExpParamsViewModel(_workspace, _asset, paramItems, true);
            var result = await dialogService.ShowDialog(vm);

            if (result != null)
            {
                SelectedControl.SubMenuPathId = result.Asset?.PathId ?? 0;
                OnPropertyChanged(nameof(SelectedControl)); // Refresh UI
                SelectedControl.NotifySubMenuNameChanged();
            }
        }

        [RelayCommand]
        public async Task SelectIcon()
        {
            if (SelectedControl == null) return;
            var textures = await ScanTextures();
            var dialogService = CommunityToolkit.Mvvm.DependencyInjection.Ioc.Default.GetRequiredService<UABEANext4.Services.IDialogService>();
            var vm = new SelectTextureViewModel(_workspace, textures);
            var result = await dialogService.ShowDialog(vm);

            if (result != null)
            {
                SelectedControl.IconPathId = result.PathId;
            }
        }

        public async Task SelectIconForLabel(VrcExpMenuLabel label)
        {
            var textures = await ScanTextures();
            var dialogService = CommunityToolkit.Mvvm.DependencyInjection.Ioc.Default.GetRequiredService<UABEANext4.Services.IDialogService>();
            var vm = new SelectTextureViewModel(_workspace, textures);
            var result = await dialogService.ShowDialog(vm);

            if (result != null)
            {
                label.IconPathId = result.PathId;
            }
        }

        [RelayCommand]
        public void ApplyChanges()
        {
            var baseField = _workspace.GetBaseField(_asset);
            if (baseField == null) return;

            baseField["Parameters"]["m_PathID"].AsLong = ParametersPathId;

            var controlsParent = baseField["controls"];
            var controlsField = controlsParent["Array"];
            if (controlsField.IsDummy || controlsField.Children == null) return;

            if (controlsField.Children.Count != Controls.Count)
            {
                while(controlsField.Children.Count > Controls.Count) 
                    controlsField.Children.RemoveAt(controlsField.Children.Count - 1);
                while(controlsField.Children.Count < Controls.Count)
                    controlsField.Children.Add(ValueBuilder.DefaultValueFieldFromTemplate(controlsField.TemplateField.Children[1]));
                
                var sizeField = controlsParent["size"];
                if (!sizeField.IsDummy)
                    sizeField.AsInt = Controls.Count;
            }

            for (int i = 0; i < Controls.Count; i++)
            {
                var item = Controls[i];
                var c = controlsField.Children[i];
                c["name"].AsString = item.Name;
                c["icon"]["m_FileID"].AsInt = 0;
                c["icon"]["m_PathID"].AsLong = item.IconPathId;
                c["type"].AsInt = item.Type;
                c["parameter"]["name"].AsString = item.Parameter;
                c["value"].AsFloat = item.Value;
                c["style"].AsInt = item.Style;
                c["subMenu"]["m_FileID"].AsInt = 0;
                c["subMenu"]["m_PathID"].AsLong = item.SubMenuPathId;

                // Sync SubParameters
                var subParamsParent = c["subParameters"];
                var subParamsArr = subParamsParent["Array"];
                if (subParamsArr.Children == null) subParamsArr.Children = new List<AssetTypeValueField>();
                if (subParamsArr.Children.Count != item.SubParameters.Count)
                {
                    while (subParamsArr.Children.Count > item.SubParameters.Count)
                        subParamsArr.Children.RemoveAt(subParamsArr.Children.Count - 1);
                    while (subParamsArr.Children.Count < item.SubParameters.Count)
                        subParamsArr.Children.Add(ValueBuilder.DefaultValueFieldFromTemplate(subParamsArr.TemplateField.Children[1]));
                    
                    var subSize = subParamsParent["size"];
                    if (!subSize.IsDummy) subSize.AsInt = item.SubParameters.Count;
                }
                for (int j = 0; j < item.SubParameters.Count; j++)
                    subParamsArr.Children[j]["name"].AsString = item.SubParameters[j].Name;

                // Sync Labels
                var labelsParent = c["labels"];
                var labelsArr = labelsParent["Array"];
                if (labelsArr.Children == null) labelsArr.Children = new List<AssetTypeValueField>();
                if (labelsArr.Children.Count != item.Labels.Count)
                {
                    while (labelsArr.Children.Count > item.Labels.Count)
                        labelsArr.Children.RemoveAt(labelsArr.Children.Count - 1);
                    while (labelsArr.Children.Count < item.Labels.Count)
                        labelsArr.Children.Add(ValueBuilder.DefaultValueFieldFromTemplate(labelsArr.TemplateField.Children[1]));
                    
                    var labelSize = labelsParent["size"];
                    if (!labelSize.IsDummy) labelSize.AsInt = item.Labels.Count;
                }
                for (int j = 0; j < item.Labels.Count; j++)
                {
                    labelsArr.Children[j]["name"].AsString = item.Labels[j].Name;
                    labelsArr.Children[j]["icon"]["m_FileID"].AsInt = 0;
                    labelsArr.Children[j]["icon"]["m_PathID"].AsLong = item.Labels[j].IconPathId;
                }
            }

            _asset.UpdateAssetDataAndRow(_workspace, baseField);
            WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(_asset));
        }
    }

    public class VrcExpMenuControlType
    {
        public int Id { get; }
        public string Name { get; }
        public VrcExpMenuControlType(int id, string name) { Id = id; Name = name; }
    }

    public partial class VrcExpMenuSubParam : ObservableObject
    {
        [ObservableProperty] private string _name = string.Empty;
    }

    public partial class VrcExpMenuLabel : ObservableObject
    {
        private VrcExpMenuViewModel _parent;
        public VrcExpMenuLabel(VrcExpMenuViewModel parent) { _parent = parent; }

        [ObservableProperty] private string _name = string.Empty;
        
        [ObservableProperty] 
        [NotifyPropertyChangedFor(nameof(IconName))]
        private long _iconPathId;

        public string IconName
        {
            get
            {
                if (IconPathId == 0) return "None (Texture 2D)";
                var asset = _parent.GetAssetInst(IconPathId);
                if (asset != null) return $"{asset.AssetName}";
                return $"{IconPathId}";
            }
        }

        [RelayCommand]
        public async Task SelectIcon()
        {
            await _parent.SelectIconForLabel(this);
        }
    }

    public partial class VrcExpMenuItem : ObservableObject
    {
        private VrcExpMenuViewModel _parent;
        public VrcExpMenuItem(VrcExpMenuViewModel parent) { _parent = parent; }

        [ObservableProperty] private string _name = string.Empty;
        [ObservableProperty] private long _iconPathId;
        [ObservableProperty] private int _type;
        [ObservableProperty] private string _parameter = string.Empty;
        [ObservableProperty] private float _value;
        [ObservableProperty] private int _style;
        [ObservableProperty] private long _subMenuPathId;

        public void NotifySubMenuNameChanged()
        {
            OnPropertyChanged(nameof(SubMenuName));
        }

        public void NotifyIconNameChanged()
        {
            OnPropertyChanged(nameof(IconName));
        }

        [ObservableProperty] private ObservableCollection<VrcExpMenuSubParam> _subParameters = new();
        [ObservableProperty] private ObservableCollection<VrcExpMenuLabel> _labels = new();

        public float IntValue { get => Value; set { if (IsInt) Value = value; } }
        public float FloatValue { get => Value; set { if (IsFloat) Value = value; } }
        public bool IsChecked { get => Value != 0; set { if (IsBool) Value = value ? 1f : 0f; } }
        
        public bool IsSubMenu => Type == 103;
        public bool IsTwoAxisPuppet => Type == 201;
        public bool IsFourAxisPuppet => Type == 202;
        public bool IsRadialPuppet => Type == 203;

        public string SubMenuName
        {
            get
            {
                if (SubMenuPathId == 0) return "None (VRC Expressions Menu)";
                var asset = _parent.GetAssetInst(SubMenuPathId);
                if (asset != null) return $"{asset.AssetName} (VRC Expressions Menu)";
                return $"{SubMenuPathId} (VRC Expressions Menu)";
            }
        }

        public string IconName
        {
            get
            {
                if (IconPathId == 0) return "None (Texture 2D)";
                var asset = _parent.GetAssetInst(IconPathId);
                if (asset != null) return $"{asset.AssetName}"; // User asked for name only to increase readability
                return $"{IconPathId}";
            }
        }

        public string DisplayType => Type switch
        {
            101 => "Button",
            102 => "Toggle",
            103 => "SubMenu",
            201 => "TwoAxisPuppet",
            202 => "FourAxisPuppet",
            203 => "RadialPuppet",
            _ => $"Unknown ({Type})"
        };

        public int ParamType => _parent.GetParamType(Parameter);

        public bool IsInt => ParamType == 0;
        public bool IsFloat => ParamType == 1;
        public bool IsBool => ParamType == 2;
        public bool IsUnknown => ParamType == -1 || string.IsNullOrEmpty(Parameter);

        public double ValueMin => IsFloat ? -1.0 : 0.0;
        public double ValueMax => IsFloat ? 1.0 : (IsInt ? 255.0 : 1000000.0);
        public double ValueStep => IsInt ? 1.0 : (IsFloat ? 0.01 : 1.0);
        public bool IsSnapEnabled => IsInt;

        public ObservableCollection<VrcExpMenuParamInfo> AvailableParameters => _parent.AvailableParameters;

        // UI Helpers for Puppet types
        public string HorizontalParam { get => GetSubParam(0); set => SetSubParam(0, value); }
        public string VerticalParam { get => GetSubParam(1); set => SetSubParam(1, value); }
        
        public string UpParam { get => GetSubParam(0); set => SetSubParam(0, value); }
        public string RightParam { get => GetSubParam(1); set => SetSubParam(1, value); }
        public string DownParam { get => GetSubParam(2); set => SetSubParam(2, value); }
        public string LeftParam { get => GetSubParam(3); set => SetSubParam(3, value); }

        public string RadialParam { get => GetSubParam(0); set => SetSubParam(0, value); }

        public VrcExpMenuLabel? UpLabel => GetLabel(0);
        public VrcExpMenuLabel? RightLabel => GetLabel(1);
        public VrcExpMenuLabel? DownLabel => GetLabel(2);
        public VrcExpMenuLabel? LeftLabel => GetLabel(3);

        private string GetSubParam(int idx)
        {
            if (idx < SubParameters.Count) return SubParameters[idx].Name;
            return "";
        }

        private void SetSubParam(int idx, string val)
        {
            while (SubParameters.Count <= idx) SubParameters.Add(new VrcExpMenuSubParam());
            SubParameters[idx].Name = val;
            OnPropertyChanged(nameof(HorizontalParam));
            OnPropertyChanged(nameof(VerticalParam));
            OnPropertyChanged(nameof(UpParam));
            OnPropertyChanged(nameof(RightParam));
            OnPropertyChanged(nameof(DownParam));
            OnPropertyChanged(nameof(LeftParam));
            OnPropertyChanged(nameof(RadialParam));
        }

        partial void OnValueChanged(float value)
        {
            if (IsInt)
            {
                float rounded = MathF.Round(value);
                if (rounded != value)
                {
                    Value = rounded;
                }
            }
            OnPropertyChanged(nameof(IntValue));
            OnPropertyChanged(nameof(FloatValue));
            OnPropertyChanged(nameof(IsChecked));
        }

        private VrcExpMenuLabel? GetLabel(int idx)
        {
            if (idx < Labels.Count) return Labels[idx];
            return null;
        }

        partial void OnParameterChanged(string value)
        {
            OnPropertyChanged(nameof(ParamType));
            NotifyTypeBooleans();
        }

        public void NotifyTypeBooleans()
        {
            OnPropertyChanged(nameof(IsInt));
            OnPropertyChanged(nameof(IsFloat));
            OnPropertyChanged(nameof(IsBool));
            OnPropertyChanged(nameof(IsUnknown));
            OnPropertyChanged(nameof(ValueMin));
            OnPropertyChanged(nameof(ValueMax));
            OnPropertyChanged(nameof(ValueStep));
            OnPropertyChanged(nameof(IsSnapEnabled));
            OnPropertyChanged(nameof(IntValue));
            OnPropertyChanged(nameof(FloatValue));
            OnPropertyChanged(nameof(IsChecked));
        }
        
        partial void OnTypeChanged(int value)
        {
            // Sync counts based on type
            int targetSubParams = value switch { 201 => 2, 202 => 4, 203 => 1, _ => 0 };
            while (SubParameters.Count < targetSubParams) SubParameters.Add(new VrcExpMenuSubParam());
            while (SubParameters.Count > targetSubParams) SubParameters.RemoveAt(SubParameters.Count - 1);
            
            // Defaulting logic for new puppets
            if (value == 201 && string.IsNullOrEmpty(HorizontalParam)) HorizontalParam = Parameter;
            if (value == 202 && string.IsNullOrEmpty(UpParam)) UpParam = Parameter;
            if (value == 203 && string.IsNullOrEmpty(RadialParam)) RadialParam = Parameter;

            int targetLabels = value switch { 201 => 4, 202 => 4, _ => 0 };
            while (Labels.Count < targetLabels) Labels.Add(new VrcExpMenuLabel(_parent));
            while (Labels.Count > targetLabels) Labels.RemoveAt(Labels.Count - 1);
            
            OnPropertyChanged(nameof(DisplayType));
            OnPropertyChanged(nameof(IsSubMenu));
            OnPropertyChanged(nameof(IsTwoAxisPuppet));
            OnPropertyChanged(nameof(IsFourAxisPuppet));
            OnPropertyChanged(nameof(IsRadialPuppet));
            OnPropertyChanged(nameof(SubMenuName));

            OnPropertyChanged(nameof(UpLabel));
            OnPropertyChanged(nameof(RightLabel));
            OnPropertyChanged(nameof(DownLabel));
            OnPropertyChanged(nameof(LeftLabel));
        }
    }
}
