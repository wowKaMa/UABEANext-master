using AssetsTools.NET;
using AssetsTools.NET.Extra;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UABEANext4.AssetWorkspace;
using UABEANext4.Logic;
using UABEANext4.ViewModels.Dialogs;
using UABEANext4.Services;
using UABEANext4.Views.Tools;
using CommunityToolkit.Mvvm.DependencyInjection;
using Avalonia.Threading;
using Avalonia;
using System.IO;
using System.Text.RegularExpressions;

namespace UABEANext4.ViewModels.Tools
{
    public class MotionSelectionItem
    {
        public string Name { get; set; } = "";
        public long PathId { get; set; }
        public string FileName { get; set; } = "";
        public string DisplayName => string.IsNullOrEmpty(FileName) ? Name : $"{Name} ({FileName})";
        public AssetInst Asset { get; set; } = null!;
    }

    public partial class PasswordCrackerViewModel : ViewModelBase
    {
        private readonly Workspace _workspace;

        [ObservableProperty] private ObservableCollection<ControllerItem> _controllers = new();
        [ObservableProperty] private ControllerItem? _selectedController;
        [ObservableProperty] private bool _isControllerSelected;
        private bool _isRuntime;

        private Dictionary<long, string> _vrcRoleMap = new();
        private const long VRC_AVATAR_DESCRIPTOR_ID = -1553566539822497163L;
        private const long VRC_EXP_PARAMS_ID = -8074938933423860320L;
        private const long VRC_EXP_MENU_ID = -2403287076911967329L;
        private const long VRC_EXP_MENU_ID_NEW = 8268836203660554979L;

        [ObservableProperty] private StateItem? _selectedState;

        [ObservableProperty] private ObservableCollection<LayerItem> _layers = new();
        [ObservableProperty] private LayerItem? _selectedLayer;

        public IEnumerable<LayerItem> BaseLayers => Layers.Where(l => !l.IsSynced);
        public void RefreshBaseLayers() => OnPropertyChanged(nameof(BaseLayers));
        public void RefreshGraph() => OnSelectedLayerChanged(SelectedLayer);

        [ObservableProperty] private ObservableCollection<ParameterItem> _parameterItems = new();
        [ObservableProperty] private ParameterItem? _selectedParameter;

        private AssetTypeValueField? _controllerRoot;

        [RelayCommand]
        public void AddParameter(string type)
        {
            if (SelectedController == null || _workspace == null || _controllerRoot == null) return;
            var asset = SelectedController.Asset;
            var baseField = _controllerRoot;

            string defName = GetUniqueParamName($"New {type}");
            int typeId = type switch { "Float" => 1, "Int" => 3, "Bool" => 4, "Trigger" => 9, _ => 1 };

            // Editor format
            var paramsField = FieldHelper.FindFieldRecursive(baseField, "m_Parameters");
            if (paramsField != null && !paramsField.IsDummy)
            {
                var array = FieldHelper.GetField(paramsField, "Array");
                var actualArray = !array.IsDummy ? array : paramsField;
                if (actualArray.TemplateField?.Children?.Count > 1)
                {
                    var newParam = ValueBuilder.DefaultValueFieldFromTemplate(actualArray.TemplateField.Children[1]);
                    if (newParam != null)
                    {
                        newParam["m_Name"].AsString = defName;
                        newParam["m_Type"].AsInt = typeId;
                        if (actualArray.Children == null) actualArray.Children = new List<AssetTypeValueField>();
                        actualArray.Children.Add(newParam);
                        FieldHelper.SyncArraySize(paramsField);
                        asset.UpdateAssetDataAndRow(_workspace, baseField);
                        ParameterItems.Add(new ParameterItem(newParam, _workspace, asset, baseField));
                        WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(asset));
                        return;
                    }
                }
            }

            // Runtime format
            var controller = FieldHelper.GetField(baseField, "m_Controller");
            var valuesVal = !controller.IsDummy ? FieldHelper.GetField(controller, "m_Values") : FieldHelper.GetField(baseField, "m_Values");
            var values = FieldHelper.GetField(valuesVal, "data").IsDummy ? valuesVal : FieldHelper.GetField(valuesVal, "data");
            
            var defaultsVal = !controller.IsDummy ? FieldHelper.GetField(controller, "m_DefaultValues") : FieldHelper.GetField(baseField, "m_DefaultValues");
            var defaults = FieldHelper.GetField(defaultsVal, "data").IsDummy ? defaultsVal : FieldHelper.GetField(defaultsVal, "data");

            if (!values.IsDummy)
            {
                var paramArr = FieldHelper.GetField(values, "m_ParameterArray");
                var paramArray = paramArr.IsDummy ? FieldHelper.GetField(values, "m_ValueArray") : paramArr;
                var actualArray = !paramArray["Array"].IsDummy ? paramArray["Array"] : paramArray;

                if (actualArray.TemplateField?.Children?.Count > 1)
                {
                    var newParam = ValueBuilder.DefaultValueFieldFromTemplate(actualArray.TemplateField.Children[1]);
                    if (newParam != null)
                    {
                        var data = FieldHelper.GetField(newParam, "data").IsDummy ? newParam : FieldHelper.GetField(newParam, "data");
                        uint hash = AnimatorHash.GetHash(defName);
                        
                        var idField = data["m_NameID"];
                        if (idField.IsDummy) idField = data["m_ID"];
                        
                        if (!idField.IsDummy)
                        {
                            idField.AsLong = (long)hash;
                        }

                        data["m_Type"].AsInt = typeId;
                        
                        // Handle indices and default value arrays
                        if (!defaults.IsDummy)
                        {
                            string arrayName = type switch { "Float" => "m_FloatValues", "Int" => "m_IntValues", "Bool" => "m_BoolValues" , "Trigger" => "m_BoolValues", _ => "" };
                            var dataF = FieldHelper.GetField(defaults, "data").IsDummy ? defaults : FieldHelper.GetField(defaults, "data");
                            var valArrayField = FieldHelper.GetField(dataF, arrayName);
                            var valArray = !valArrayField["Array"].IsDummy ? valArrayField["Array"] : valArrayField;
                            
                            if (valArray.Children != null)
                            {
                                data["m_Index"].AsInt = valArray.Children.Count;
                                if (valArray.TemplateField?.Children?.Count > 1)
                                {
                                    var newVal = ValueBuilder.DefaultValueFieldFromTemplate(valArray.TemplateField.Children[1]);
                                    if (newVal != null)
                                    {
                                        if (valArray.Children == null) valArray.Children = new List<AssetTypeValueField>();
                                        valArray.Children.Add(newVal);
                                        FieldHelper.SyncArraySize(valArrayField);
                                    }
                                }
                            }
                        }

                        if (actualArray.Children == null) actualArray.Children = new List<AssetTypeValueField>();
                        actualArray.Children.Add(newParam);
                        FieldHelper.SyncArraySize(paramArray);
                        
                        // Update TOS
                        FieldHelper.UpdateTOS(baseField, hash, defName);
                        if (SelectedController != null)
                        {
                            if (!SelectedController.StringTable.ContainsKey(hash))
                                SelectedController.StringTable[hash] = defName;
                        }
                        
                        asset.UpdateAssetDataAndRow(_workspace, baseField);
                        ParameterItems.Add(new ParameterItem(name: defName, id: hash, field: data, defaults: defaultsVal, workspace: _workspace, asset: asset, root: baseField, stringTable: SelectedController?.StringTable));
                        WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(asset));
                    }
                }
            }
        }

        private string GetUniqueParamName(string baseName)
        {
            string name = baseName;
            int counter = 1;
            while (ParameterItems.Any(p => p.Name == name))
            {
                name = $"{baseName} {counter++}";
            }
            return name;
        }

        [RelayCommand]
        public void RemoveParameter(ParameterItem? item)
        {
            if (SelectedController == null || _controllerRoot == null) return;
            item ??= SelectedParameter;
            if (item == null) return;
            var asset = SelectedController.Asset;
            var baseField = _controllerRoot;

            var paramsField = FieldHelper.FindFieldRecursive(baseField, "m_Parameters");
            if (paramsField != null && !paramsField.IsDummy)
            {
                var array = FieldHelper.GetField(paramsField, "Array");
                var actualArray = !array.IsDummy ? array : paramsField;
                var toRemove = actualArray.Children?.FirstOrDefault(p => FieldHelper.GetString(p, "m_Name") == item.Name);
                if (toRemove != null && actualArray.Children != null)
                {
                    actualArray.Children.Remove(toRemove);
                    FieldHelper.SyncArraySize(paramsField);
                    asset.UpdateAssetDataAndRow(_workspace, baseField);
                    ParameterItems.Remove(item);
                    WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(asset));
                    return;
                }
            }

            // Runtime format
            var controller = FieldHelper.GetField(baseField, "m_Controller");
            var valuesVal = !controller.IsDummy ? FieldHelper.GetField(controller, "m_Values") : FieldHelper.GetField(baseField, "m_Values");
            var values = FieldHelper.GetField(valuesVal, "data").IsDummy ? valuesVal : FieldHelper.GetField(valuesVal, "data");

            if (!values.IsDummy)
            {
                var paramArr = FieldHelper.GetField(values, "m_ParameterArray");
                var paramArray = paramArr.IsDummy ? FieldHelper.GetField(values, "m_ValueArray") : paramArr;
                var actualArray = !paramArray["Array"].IsDummy ? paramArray["Array"] : paramArray;

                var toRemove = actualArray.Children?.FirstOrDefault(p => {
                    var data = FieldHelper.GetField(p, "data").IsDummy ? p : FieldHelper.GetField(p, "data");
                    var idF = data["m_NameID"];
                    if (idF.IsDummy) idF = data["m_ID"];
                    if (idF.IsDummy) return false;
                    return (uint)idF.AsLong == item.ParamId;
                });

                if (toRemove != null && actualArray.Children != null)
                {
                    int index = actualArray.Children.IndexOf(toRemove);
                    actualArray.Children.RemoveAt(index);
                    
                    FieldHelper.SyncArraySize(paramArray);
                    
                    ParameterItems.Remove(item);
                    asset.UpdateAssetDataAndRow(_workspace, baseField);
                    WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(asset));
                    return;
                }
                System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] RemoveParameter: Could not find parameter in asset. ID: {item.ParamId}, Name: {item.Name}");
            }
        }

        [ObservableProperty] private ObservableCollection<StateItem> _states = new();
        [ObservableProperty] private ObservableCollection<TransitionLine> _transitionLines = new();
        
        [ObservableProperty] private ObservableCollection<MotionSelectionItem> _availableMotions = new();
        [ObservableProperty] private ObservableCollection<MotionSelectionItem> _filteredMotions = new();
        [ObservableProperty] private string _motionSearchText = "";
        
        [ObservableProperty] private bool _isMakingTransition;
        [ObservableProperty] private StateItem? _transitionSourceState;
        [ObservableProperty] private string _pendingTransitionLineData = "";

        partial void OnMotionSearchTextChanged(string value) => RefreshFilteredMotions();

        private void RefreshFilteredMotions()
        {
            var noneItem = new MotionSelectionItem { Name = "None", PathId = 0, FileName = "" };
            
            if (string.IsNullOrWhiteSpace(MotionSearchText))
            {
                var list = new List<MotionSelectionItem> { noneItem };
                list.AddRange(AvailableMotions);
                FilteredMotions = new ObservableCollection<MotionSelectionItem>(list);
            }
            else
            {
                // Fuzzy matching: split by space and ensure all terms match (AND logic)
                var searchTerms = MotionSearchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                
                var filtered = AvailableMotions.Where(m => 
                {
                    return searchTerms.All(term => 
                        (m.Name != null && m.Name.Contains(term, StringComparison.OrdinalIgnoreCase)) || 
                        (m.FileName != null && m.FileName.Contains(term, StringComparison.OrdinalIgnoreCase))
                    );
                }).ToList();
                
                var list = new List<MotionSelectionItem>();
                // Only add "None" if it matches the search terms
                if (searchTerms.All(term => "None".Contains(term, StringComparison.OrdinalIgnoreCase)))
                    list.Add(noneItem);
                    
                list.AddRange(filtered);
                FilteredMotions = new ObservableCollection<MotionSelectionItem>(list);
            }
        }

        private Dictionary<long, string> _animationMap = new();

        public event Action? RequestCenterView;

        public PasswordCrackerViewModel(Workspace workspace)
        {
            _workspace = workspace;
            ScanForVrcDescriptor();
            LoadControllers();
            RefreshAvailableMotions();
        }

        [RelayCommand]
        public void OpenExpParams()
        {
            // Find all assets with script PathID: VRC_EXP_PARAMS_ID
            var results = new List<ExpParamSelectionItem>();
            var assetsFiles = WorkspaceItem.GetAssetsFileWorkspaceItems(_workspace.RootItems);
            foreach (var wsItem in assetsFiles)
            {
                if (wsItem.Object is not AssetsFileInstance asInst) continue;
                foreach (var info in asInst.file.AssetInfos)
                {
                    if (info.TypeId == (int)AssetClassID.MonoBehaviour)
                    {
                        var asset = _workspace.GetAssetInst(asInst, 0, info.PathId);
                        if (asset == null) continue;
                        
                        var baseField = _workspace.GetBaseField(asset);
                        if (baseField == null) continue;
                        
                        var script = baseField["m_Script"];
                        if (script != null && !script.IsDummy)
                        {
                            long scriptPathId = script["m_PathID"].AsLong;
                            if (scriptPathId == VRC_EXP_PARAMS_ID)
                            {
                                _vrcRoleMap.TryGetValue(asset.PathId, out string? role);
                                results.Add(new ExpParamSelectionItem(_workspace, asset, role));
                            }
                        }
                    }
                }
            }

            if (results.Count == 0)
            {
                Dispatcher.UIThread.Post(async () => {
                    await Util.MessageBoxUtil.ShowDialog("提醒 (Notice)", "在当前工作区中未找到任何菜单参数资产 (VRCExpressionParameters)。\n(No VRCExpressionParameters assets found in the current workspace.)");
                });
                return;
            }

            if (results.Count == 1)
            {
                OpenParamViewer(results[0].Asset);
            }
            else
            {
                Dispatcher.UIThread.Post(async () => {
                    var dialogService = Ioc.Default.GetRequiredService<IDialogService>();
                    var vm = new SelectExpParamsViewModel(_workspace, results[0].Asset, results, false);
                    var selected = await dialogService.ShowDialog(vm);
                    if (selected != null)
                    {
                        OpenParamViewer(selected.Asset);
                    }
                });
            }
        }

        private void OpenParamViewer(AssetInst asset)
        {
            Dispatcher.UIThread.Post(() => {
                var win = new VrcExpParamsWindow(_workspace, asset);
                win.Show();
            });
        }

        [RelayCommand]
        public void OpenMenu()
        {
            var results = new List<ExpParamSelectionItem>();
            var assetsFiles = WorkspaceItem.GetAssetsFileWorkspaceItems(_workspace.RootItems);
            foreach (var wsItem in assetsFiles)
            {
                if (wsItem.Object is not AssetsFileInstance asInst) continue;
                foreach (var info in asInst.file.AssetInfos)
                {
                    if (info.TypeId == (int)AssetClassID.MonoBehaviour)
                    {
                        var asset = _workspace.GetAssetInst(asInst, 0, info.PathId);
                        if (asset == null) continue;
                        
                        var baseField = _workspace.GetBaseField(asset);
                        if (baseField == null) continue;
                        
                        var script = baseField["m_Script"];
                        if (script != null && !script.IsDummy)
                        {
                            long scriptPathId = script["m_PathID"].AsLong;
                            if (scriptPathId == VRC_EXP_MENU_ID || scriptPathId == VRC_EXP_MENU_ID_NEW)
                            {
                                _vrcRoleMap.TryGetValue(asset.PathId, out string? role);
                                results.Add(new ExpParamSelectionItem(_workspace, asset, role));
                            }
                        }
                    }
                }
            }

            if (results.Count == 0)
            {
                Dispatcher.UIThread.Post(async () => {
                    await Util.MessageBoxUtil.ShowDialog("提醒 (Notice)", "在当前工作区中未找到任何菜单资产 (VRCExpressionMenu)。\n(No VRCExpressionMenu assets found in the current workspace.)");
                });
                return;
            }

            if (results.Count == 1)
            {
                OpenMenuViewer(results[0].Asset);
            }
            else
            {
                Dispatcher.UIThread.Post(async () => {
                    var dialogService = Ioc.Default.GetRequiredService<IDialogService>();
                    var vm = new SelectExpParamsViewModel(_workspace, results[0].Asset, results, true);
                    var selected = await dialogService.ShowDialog(vm);
                    if (selected != null)
                    {
                        OpenMenuViewer(selected.Asset);
                    }
                });
            }
        }

        private void OpenMenuViewer(AssetInst asset)
        {
            Dispatcher.UIThread.Post(() => {
                var window = new VrcExpMenuWindow(_workspace, asset);
                window.Show();
            });
        }

        private void RefreshAvailableMotions()
        {
            AvailableMotions.Clear();
            var assetsFiles = WorkspaceItem.GetAssetsFileWorkspaceItems(_workspace.RootItems);
            foreach (var wsItem in assetsFiles)
            {
                if (wsItem.Object is not AssetsFileInstance asInst) continue;
                foreach (var info in asInst.file.AssetInfos)
                {
                    if (info.TypeId == (int)AssetClassID.AnimationClip)
                    {
                        var animInst = _workspace.GetAssetInst(asInst, 0, info.PathId);
                        if (animInst != null && !string.IsNullOrEmpty(animInst.AssetName))
                        {
                            AvailableMotions.Add(new MotionSelectionItem
                            {
                                Name = animInst.AssetName,
                                PathId = animInst.PathId,
                                FileName = asInst.name,
                                Asset = animInst
                            });
                        }
                    }
                }
            }
            RefreshFilteredMotions();
        }

        // DESIGN: Empty constructor for XAML designer
        public PasswordCrackerViewModel()
        {
             _workspace = new Workspace();
        }

        private void ScanForVrcDescriptor()
        {
            _vrcRoleMap.Clear();
            var assetsFiles = WorkspaceItem.GetAssetsFileWorkspaceItems(_workspace.RootItems);
            foreach (var wsItem in assetsFiles)
            {
                if (wsItem.Object is not AssetsFileInstance asInst) continue;
                foreach (var info in asInst.file.AssetInfos)
                {
                    if (info.TypeId == (int)AssetClassID.MonoBehaviour)
                    {
                        var asset = _workspace.GetAssetInst(asInst, 0, info.PathId);
                        if (asset == null) continue;
                        
                        var baseField = _workspace.GetBaseField(asset);
                        if (baseField == null) continue;
                        
                        var script = baseField["m_Script"];
                        if (script != null && !script.IsDummy)
                        {
                            long scriptPathId = script["m_PathID"].AsLong;
                            if (scriptPathId == VRC_AVATAR_DESCRIPTOR_ID)
                            {
                                // Parse Base Layers
                                var baseLayers = baseField["baseAnimationLayers"]["Array"];
                                if (baseLayers != null && !baseLayers.IsDummy)
                                {
                                    for (int i = 0; i < baseLayers.Children.Count; i++)
                                    {
                                        var layer = baseLayers.Children[i];
                                        long ctrlPid = layer["animatorController"]["m_PathID"].AsLong;
                                        int type = layer["type"].AsInt;
                                        string? role = type switch
                                        {
                                            0 => "Base",
                                            2 => "Additive",
                                            3 => "Gesture",
                                            4 => "Action",
                                            5 => "FX",
                                            _ => null
                                        };
                                        if (ctrlPid != 0 && role != null)
                                            _vrcRoleMap[ctrlPid] = role;
                                    }
                                }

                                // Parse Special Layers
                                var specialLayers = baseField["specialAnimationLayers"]["Array"];
                                string[] specialRoles = { "Sitting", "TPose", "IKPose" }; // types 6, 7, 8
                                if (specialLayers != null && !specialLayers.IsDummy)
                                {
                                    for (int i = 0; i < specialLayers.Children.Count; i++)
                                    {
                                        var layer = specialLayers.Children[i];
                                        long ctrlPid = layer["animatorController"]["m_PathID"].AsLong;
                                        int type = layer["type"].AsInt;
                                        int roleIdx = type - 6;
                                        if (ctrlPid != 0 && roleIdx >= 0 && roleIdx < specialRoles.Length)
                                            _vrcRoleMap[ctrlPid] = specialRoles[roleIdx];
                                    }
                                }

                                // Parse Menu and Parameters
                                long menuPid = baseField["expressionsMenu"]["m_PathID"].AsLong;
                                if (menuPid != 0) _vrcRoleMap[menuPid] = "Menu";

                                long paramsPid = baseField["expressionParameters"]["m_PathID"].AsLong;
                                if (paramsPid != 0) _vrcRoleMap[paramsPid] = "Parameters";
                                
                                return; // Assuming one descriptor per workspace for now
                            }
                        }
                    }
                }
            }
        }

        private void LoadControllers()
        {
            var assetsFiles = WorkspaceItem.GetAssetsFileWorkspaceItems(_workspace.RootItems);
            foreach (var item in assetsFiles)
            {
                if (item.Object is not AssetsFileInstance asInst) continue;
                foreach (var info in asInst.file.AssetInfos)
                {
                    if (info.TypeId == (int)AssetClassID.AnimatorController)
                    {
                        var asset = _workspace.GetAssetInst(asInst, 0, info.PathId);
                        if (asset != null)
                        {
                            _vrcRoleMap.TryGetValue(asset.PathId, out string? role);
                            Controllers.Add(new ControllerItem(asset, _workspace, role));
                        }
                    }
                }
            }
            if (Controllers.Count > 0) SelectedController = Controllers[0];

            WeakReferenceMessenger.Default.Register<ParameterRenamedMessage>(this, (r, m) =>
            {
                SyncLoadedConditions(m.OldHash, m.NewHash, m.NewName);
            });

            WeakReferenceMessenger.Default.Register<DeleteTransitionRequest>(this, (r, m) =>
            {
                DeleteTransition(m.Transition);
            });

            WeakReferenceMessenger.Default.Register<TransitionModifiedMessage>(this, (r, m) =>
            {
                // Soft refresh: find twin transitions (sharing same asset data) and reload their display
                foreach (var state in States)
                {
                    foreach (var trans in state.Transitions)
                    {
                        if (trans != m.Transition && trans.GetField() == m.Transition.GetField())
                        {
                            trans.ReloadConditions();
                        }
                    }
                }
                
                // Also update canvas lines
                if (SelectedLayer != null)
                {
                    UpdateTransitionLines(SelectedLayer);
                }
            });
        }
        
        private void SyncLoadedConditions(uint oldHash, uint newHash, string newName)
        {
            foreach (var state in States)
            {
                foreach (var trans in state.Transitions)
                {
                    foreach (var cond in trans.ConditionList)
                    {
                        var data = cond.GetField();
                        long eid = _isRuntime ? FieldHelper.GetLong(data, "m_EventID") : 0;
                        if (_isRuntime)
                        {
                            if (eid == (long)newHash)
                            {
                                // The hash in asset was already updated by ParameterItem
                                cond.ParamName = newName;
                            }
                        }
                        else 
                        {
                            // In editor format, it matches by string name
                            // But here we might not need to do anything since it's already updated in asset
                            // and the UI might re-bind. But for safety:
                            if (cond.ParamName == newName) cond.NotifyParamChanged(); 
                        }
                    }
                }
            }
        }

        partial void OnSelectedControllerChanged(ControllerItem? value)
        {
            Layers.Clear();
            ParameterItems.Clear();
            SelectedParameter = null;
            IsControllerSelected = value != null;
            _controllerRoot = null;

            if (value != null)
            {
                _controllerRoot = _workspace.GetBaseField(value.Asset);
                var baseField = _controllerRoot;
                _isRuntime = baseField != null && FieldHelper.GetField(baseField, "m_Parameters").IsDummy;

                LoadAnimationMap(value.Asset);
                
                var parameters = value.GetParameters(this, baseField);
                foreach (var param in parameters) ParameterItems.Add(param);

                var layers = value.GetLayers(this, baseField);
                foreach (var layer in layers) Layers.Add(layer);
                if (Layers.Count > 0) SelectedLayer = Layers[0];
                RefreshBaseLayers();
            }
        }

        [RelayCommand]
        public void StartTransition(StateItem source)
        {
            if (source == null) return;
            IsMakingTransition = true;
            TransitionSourceState = source;
            PendingTransitionLineData = ""; // Clear
        }

        [RelayCommand]
        public void CancelTransition()
        {
            IsMakingTransition = false;
            TransitionSourceState = null;
            PendingTransitionLineData = "";
        }

        [RelayCommand]
        public void UpdatePendingTransition(Point canvasPos)
        {
            if (!IsMakingTransition || TransitionSourceState == null) return;
            
            // Generate SVG line from source to point
            double startX = TransitionSourceState.X + 80; // 160 width / 2
            double startY = TransitionSourceState.Y + 21; // 42 height / 2
            
            double endX = canvasPos.X;
            double endY = canvasPos.Y;
            
            // Draw a simple line with an arrow at the end
            double dx = endX - startX;
            double dy = endY - startY;
            double angle = Math.Atan2(dy, dx);
            
            double arrowSize = 8;
            double x1 = endX - arrowSize * Math.Cos(angle - Math.PI / 6);
            double y1 = endY - arrowSize * Math.Sin(angle - Math.PI / 6);
            double x2 = endX - arrowSize * Math.Cos(angle + Math.PI / 6);
            double y2 = endY - arrowSize * Math.Sin(angle + Math.PI / 6);
            
            PendingTransitionLineData = $"M {startX:F1},{startY:F1} L {endX:F1},{endY:F1} M {endX:F1},{endY:F1} L {x1:F1},{y1:F1} M {endX:F1},{endY:F1} L {x2:F1},{y2:F1}";
        }

        [RelayCommand]
        public void FinalizeTransition(StateItem target)
        {
            if (!IsMakingTransition || TransitionSourceState == null || target == null) return;
            if (TransitionSourceState == target) { CancelTransition(); return; }

            // Add the transition to the source state
            TransitionSourceState.AddTransitionTo(target);
            
            CancelTransition();
            
            // Refresh transition lines
            if (SelectedLayer != null) 
            {
                RefreshTransitions();
            }
        }

        private void RefreshTransitions()
        {
            if (SelectedLayer == null || SelectedController == null || _workspace == null) return;

            try
            {
                var paramTypes = ParameterItems.ToDictionary(p => p.Name, p => p.Type);
                var allStates = States.ToList();
                var entryNode = States.FirstOrDefault(s => s.Name == "Entry");

                // Get SM asset for virtual nodes in Editor
                AssetTypeValueField? smField = null;
                if (!_isRuntime)
                {
                    var smPtr = SelectedLayer.GetField()["m_StateMachine"];
                    if (!smPtr.IsDummy && smPtr.Value != null)
                    {
                        var smAsset = _workspace.Manager.GetExtAsset(SelectedController.Asset.FileInstance, smPtr);
                        smField = smAsset.baseField;
                    }
                }
                else
                {
                    // In Runtime, virtual nodes source is the SM root (smField)
                    smField = entryNode?._sourceField; 
                }

                foreach (var s in States)
                {
                    if (s._sourceField == null) continue;

                    if (!_isRuntime)
                    {
                        // Editor re-parse
                        if (s.Name == "Any State" || s.Name == "Entry")
                        {
                            if (smField != null)
                            {
                                var arrayName = (s.Name == "Any State") ? "m_AnyStateTransitions" : "m_EntryTransitions";
                                var transField = FieldHelper.GetField(smField, arrayName);
                                s.Transitions.Clear();
                                var parsed = SelectedLayer.ParseTransitions(transField, false, paramTypes, ParameterItems, s.Name == "Entry");
                                foreach (var t in parsed) s.Transitions.Add(t);
                            }
                        }
                        else
                        {
                            var transField = FieldHelper.GetField(s._sourceField, "m_Transitions");
                            s.Transitions.Clear();
                            var parsed = SelectedLayer.ParseTransitions(transField, false, paramTypes, ParameterItems, false);
                            foreach (var t in parsed) s.Transitions.Add(t);
                        }
                    }
                    else
                    {
                        // Runtime re-parse
                        if (s.Name == "Any State")
                        {
                            s.ParseRuntimeTransitions(FieldHelper.GetField(s._sourceField, "m_AnyStateTransitionConstantArray"), SelectedController.StringTable, paramTypes, ParameterItems, false);
                        }
                        else if (s.Name == "Entry")
                        {
                            // Entry virtual node transitions come from the entry selector
                            // Handled by the selector sync below
                        }
                        else
                        {
                            s.ParseRuntimeTransitions(FieldHelper.GetField(s._sourceField, "m_TransitionConstantArray"), SelectedController.StringTable, paramTypes, ParameterItems, s.IsEntrySelector);
                        }
                    }
                }

                // Extra sync for Runtime Entry/Selector
                if (_isRuntime && smField != null && entryNode != null)
                {
                    var selectorArray = FieldHelper.GetField(smField, "m_SelectorStateConstantArray");
                    var actualSelector = !FieldHelper.GetField(selectorArray, "Array").IsDummy ? FieldHelper.GetField(selectorArray, "Array") : selectorArray;
                    if (actualSelector.Children != null)
                    {
                        foreach (var selector in actualSelector.Children)
                        {
                            var data = FieldHelper.GetField(selector, "data").IsDummy ? selector : FieldHelper.GetField(selector, "data");
                            if (FieldHelper.GetField(data, "m_IsEntry").AsBool)
                            {
                                entryNode.ParseRuntimeTransitions(FieldHelper.GetField(data, "m_TransitionConstantArray"), SelectedController.StringTable, paramTypes, ParameterItems, true);
                                break;
                            }
                        }
                    }
                }

                foreach (var s in States) s.ResolveTransitionNames(allStates);
                UpdateTransitionLines(SelectedLayer);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] RefreshTransitions Error: {ex.Message}");
            }
        }

        [RelayCommand]
        public void SetAsDefaultState(StateItem? state)
        {
            if (SelectedController == null || _controllerRoot == null || state == null || SelectedLayer == null) return;
            if (state.Name == "Any State" || state.Name == "Entry" || state.Name == "Exit") return;
            if (state.PathId < 0 && !_isRuntime) return; // Virtual nodes check for editor

            try
            {
                if (!_isRuntime)
                {
                    // Editor Format
                    var smPtr = SelectedLayer.GetField()["m_StateMachine"];
                    if (smPtr.IsDummy || smPtr.Value == null) return;
                    var smAsset = _workspace.Manager.GetExtAsset(SelectedController.Asset.FileInstance, smPtr);
                    if (smAsset.baseField == null) return;

                    var smField = smAsset.baseField;
                    var defStateField = FieldHelper.GetField(smField, "m_DefaultState");
                    if (!defStateField.IsDummy && defStateField.Children != null && defStateField.Children.Count > 0)
                    {
                        defStateField.Children[defStateField.Children.Count - 1].AsLong = state.PathId;
                    }

                    // Update Entry Transitions
                    var entryTransField = FieldHelper.GetField(smField, "m_EntryTransitions");
                    var entryTransArray = !FieldHelper.GetField(entryTransField, "Array").IsDummy ? FieldHelper.GetField(entryTransField, "Array") : entryTransField;

                    if (entryTransArray.Children != null)
                    {
                        AssetTypeValueField? targetTrans = null;
                        foreach (var transData in entryTransArray.Children)
                        {
                            var data = FieldHelper.GetField(transData, "data").IsDummy ? transData : FieldHelper.GetField(transData, "data");
                            var conds = FieldHelper.GetField(data, "m_Conditions");
                            var condArray = !FieldHelper.GetField(conds, "Array").IsDummy ? FieldHelper.GetField(conds, "Array") : conds;

                            // Identify existing default link: unconditional transition
                            if (condArray.Children == null || condArray.Children.Count == 0)
                            {
                                targetTrans = data;
                                break;
                            }
                        }

                        if (targetTrans != null)
                        {
                            // Update existing
                            var dstState = FieldHelper.GetField(targetTrans, "m_DstState");
                            if (!dstState.IsDummy && dstState.Children != null && dstState.Children.Count > 0)
                            {
                                dstState.Children[dstState.Children.Count - 1].AsLong = state.PathId;
                            }
                        }
                        else if (entryTransArray.TemplateField?.Children?.Count > 1)
                        {
                            // Create new if none found
                            var newTrans = ValueBuilder.DefaultValueFieldFromTemplate(entryTransArray.TemplateField.Children[1]);
                            if (newTrans != null)
                            {
                                var data = FieldHelper.GetField(newTrans, "data").IsDummy ? newTrans : FieldHelper.GetField(newTrans, "data");
                                var dstState = FieldHelper.GetField(data, "m_DstState");
                                if (!dstState.IsDummy && dstState.Children != null && dstState.Children.Count > 0)
                                {
                                    dstState.Children[dstState.Children.Count - 1].AsLong = state.PathId;
                                }
                                entryTransArray.Children.Insert(0, newTrans);
                            }
                        }
                        FieldHelper.SyncArraySize(entryTransField);
                    }

                    // Save StateMachine asset
                    var assetInst = _workspace.GetAssetInst(smAsset.file, 0, smAsset.info.PathId);
                    if (assetInst != null)
                    {
                        assetInst.UpdateAssetDataAndRow(_workspace, smField);
                        WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(assetInst));
                    }
                }
                else
                {
                    // Runtime Format
                    var controller = FieldHelper.GetField(_controllerRoot, "m_Controller");
                    var smArray = !controller.IsDummy ? FieldHelper.GetField(controller, "m_StateMachineArray") : FieldHelper.GetField(_controllerRoot, "m_StateMachineArray");
                    var smArr = !FieldHelper.GetField(smArray, "Array").IsDummy ? FieldHelper.GetField(smArray, "Array") : smArray;

                    int layerIdx = Layers.IndexOf(SelectedLayer);
                    if (smArr.Children == null || layerIdx < 0 || layerIdx >= smArr.Children.Count) return;

                    var smChild = smArr.Children[layerIdx];
                    var smField = FieldHelper.GetField(smChild, "data").IsDummy ? smChild : FieldHelper.GetField(smChild, "data");

                    // Update Default State Index/PathId
                    smField["m_DefaultState"].AsLong = state.PathId;

                    // Update Entry Selector in m_SelectorStateConstantArray
                    var selectorArray = FieldHelper.GetField(smField, "m_SelectorStateConstantArray");
                    var actualSelector = !FieldHelper.GetField(selectorArray, "Array").IsDummy ? FieldHelper.GetField(selectorArray, "Array") : selectorArray;
                    if (actualSelector.Children != null)
                    {
                        foreach (var selector in actualSelector.Children)
                        {
                            var data = FieldHelper.GetField(selector, "data").IsDummy ? selector : FieldHelper.GetField(selector, "data");
                            if (FieldHelper.GetField(data, "m_IsEntry").AsBool)
                            {
                                var transArrayField = FieldHelper.GetField(data, "m_TransitionConstantArray");
                                var transArray = !FieldHelper.GetField(transArrayField, "Array").IsDummy ? FieldHelper.GetField(transArrayField, "Array") : transArrayField;
                                if (transArray.Children != null)
                                {
                                    AssetTypeValueField? targetTrans = null;
                                    foreach (var transField in transArray.Children)
                                    {
                                        var tData = FieldHelper.GetField(transField, "data").IsDummy ? transField : FieldHelper.GetField(transField, "data");
                                        var conds = FieldHelper.GetField(tData, "m_ConditionConstantArray");
                                        var condArray = !FieldHelper.GetField(conds, "Array").IsDummy ? FieldHelper.GetField(conds, "Array") : conds;

                                        if (condArray.Children == null || condArray.Children.Count == 0)
                                        {
                                            targetTrans = tData;
                                            break;
                                        }
                                    }

                                    if (targetTrans != null)
                                    {
                                        // Update existing
                                        var dstField = FieldHelper.GetField(targetTrans, "m_Destination");
                                        if (!dstField.IsDummy) dstField.AsLong = state.PathId;
                                    }
                                    else if (transArray.TemplateField?.Children?.Count > 1)
                                    {
                                        // Create new
                                        var newTrans = ValueBuilder.DefaultValueFieldFromTemplate(transArray.TemplateField.Children[1]);
                                        if (newTrans != null)
                                        {
                                            var tData = FieldHelper.GetField(newTrans, "data").IsDummy ? newTrans : FieldHelper.GetField(newTrans, "data");
                                            var dstField = FieldHelper.GetField(tData, "m_Destination");
                                            if (!dstField.IsDummy) dstField.AsLong = state.PathId;

                                            transArray.Children.Insert(0, newTrans);
                                        }
                                    }
                                    FieldHelper.SyncArraySize(transArrayField);
                                }
                            }
                        }
                    }

                    // Save Controller asset
                    SelectedController.Asset.UpdateAssetDataAndRow(_workspace, _controllerRoot);
                    WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(SelectedController.Asset));
                }

                // Update UI State
                foreach (var s in States) s.IsDefault = (s == state);

                RefreshTransitions();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] SetAsDefaultState Error: {ex.Message}");
            }
        }

        [RelayCommand]
        public void DeleteTransition(TransitionItem? transition)
        {
            if (SelectedController == null || transition == null || SelectedLayer == null || _workspace == null) return;

            try
            {
                var targetField = transition.GetField();
                bool foundInAsset = false;

                if (!_isRuntime)
                {
                    // Editor Format
                    // 1. Check State Machine root (for AnyState and Entry transitions)
                    var smPtr = SelectedLayer.GetField()["m_StateMachine"];
                    if (!smPtr.IsDummy && smPtr.Value != null)
                    {
                        var smAsset = _workspace.Manager.GetExtAsset(SelectedController.Asset.FileInstance, smPtr);
                        if (smAsset.baseField != null)
                        {
                            var smField = smAsset.baseField;
                            string[] smArrays = { "m_AnyStateTransitions", "m_EntryTransitions" };
                            foreach (var arrayName in smArrays)
                            {
                                var container = FieldHelper.GetField(smField, arrayName);
                                if (RemoveFromFieldArray(container, targetField))
                                {
                                    var smAssetInst = _workspace.GetAssetInst(smAsset.file, 0, smAsset.info.PathId);
                                    smAssetInst?.UpdateAssetDataAndRow(_workspace, smField);
                                    foundInAsset = true;
                                    break;
                                }
                            }
                        }
                    }

                    // 2. Check individual states (for normal transitions)
                    if (!foundInAsset)
                    {
                        foreach (var state in States)
                        {
                            if (state._sourceField == null) continue;
                            var container = FieldHelper.GetField(state._sourceField, "m_Transitions");
                            if (RemoveFromFieldArray(container, targetField))
                            {
                                state._sourceAsset?.UpdateAssetDataAndRow(_workspace, state._sourceField);
                                foundInAsset = true;
                                break;
                            }
                        }
                    }

                    if (foundInAsset)
                    {
                        SelectedController.Asset.UpdateAssetDataAndRow(_workspace, _controllerRoot!);
                        WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(SelectedController.Asset));
                    }
                }
                else
                {
                    // Runtime Format
                    foreach (var state in States)
                    {
                        if (state._sourceField == null) continue;
                        
                        // Try both state arrays and SM-level AnyState/Entry arrays
                        if (RemoveFromFieldArray(FieldHelper.GetField(state._sourceField, "m_TransitionConstantArray"), targetField) ||
                            RemoveFromFieldArray(FieldHelper.GetField(state._sourceField, "m_AnyStateTransitionConstantArray"), targetField))
                        {
                            foundInAsset = true;
                            break;
                        }
                    }

                    if (foundInAsset)
                    {
                        SelectedController.Asset.UpdateAssetDataAndRow(_workspace, _controllerRoot!);
                        WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(SelectedController.Asset));
                    }
                }

                RefreshTransitions();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] DeleteTransition Error: {ex.Message}");
            }
        }

        private bool RemoveFromFieldArray(AssetTypeValueField arrayField, AssetTypeValueField targetData)
        {
            if (arrayField.IsDummy) return false;
            var arr = !FieldHelper.GetField(arrayField, "Array").IsDummy ? FieldHelper.GetField(arrayField, "Array") : arrayField;
            if (arr.Children == null) return false;

            for (int i = 0; i < arr.Children.Count; i++)
            {
                var data = (FieldHelper.GetField(arr.Children[i], "data").IsDummy ? arr.Children[i] : FieldHelper.GetField(arr.Children[i], "data"));
                if (data == targetData)
                {
                    arr.Children.RemoveAt(i);
                    FieldHelper.SyncArraySize(arrayField);
                    return true;
                }
            }
            return false;
        }

        [RelayCommand]
        public void DeleteState(StateItem? state)
        {
            if (SelectedController == null || _controllerRoot == null || state == null || SelectedLayer == null) return;
            if (state.Name == "Any State" || state.Name == "Entry" || state.Name == "Exit") return;

            try
            {
                if (!_isRuntime)
                {
                    // Editor Format
                    var smPtr = SelectedLayer.GetField()["m_StateMachine"];
                    if (smPtr.IsDummy || smPtr.Value == null) return;
                    var smAsset = _workspace.Manager.GetExtAsset(SelectedController.Asset.FileInstance, smPtr);
                    if (smAsset.baseField == null) return;
                    var smField = smAsset.baseField;

                    // 1. Remove state from m_States list
                    var statesField = FieldHelper.GetField(smField, "m_States");
                    var statesArr = !FieldHelper.GetField(statesField, "Array").IsDummy ? FieldHelper.GetField(statesField, "Array") : statesField;
                    if (statesArr.Children != null)
                    {
                        var toRemove = statesArr.Children.FirstOrDefault(s => FieldHelper.GetField(s, "m_State")["m_PathID"].AsLong == state.PathId);
                        if (toRemove != null)
                        {
                            statesArr.Children.Remove(toRemove);
                            FieldHelper.SyncArraySize(statesField);
                        }
                    }

                    // 2. Remove any transitions pointing to this state from ALL nodes in the layer
                    foreach (var s in States)
                    {
                        var sourceSmField = s._sourceField;
                        if (sourceSmField == null) continue;
                        
                        // Normal states have transitions in their own asset. Virtual nodes have them in the SM root.
                        AssetTypeValueField transField;
                        if (s.Name == "Any State" || s.Name == "Entry")
                            transField = FieldHelper.GetField(smField, (s.Name == "Any State") ? "m_AnyStateTransitions" : "m_EntryTransitions");
                        else
                            transField = FieldHelper.GetField(sourceSmField, "m_Transitions");
                        
                        var transArr = !FieldHelper.GetField(transField, "Array").IsDummy ? FieldHelper.GetField(transField, "Array") : transField;
                        if (transArr.Children != null)
                        {
                            var deadTransitions = transArr.Children.Where(t => 
                            {
                                var data = FieldHelper.GetField(t, "data").IsDummy ? t : FieldHelper.GetField(t, "data");
                                return data["m_DstState"]["m_PathID"].AsLong == state.PathId;
                            }).ToList();
                            
                            foreach (var dt in deadTransitions) transArr.Children.Remove(dt);
                            if (deadTransitions.Count > 0) FieldHelper.SyncArraySize(transField);
                        }

                        // Save if it's a normal state (not a virtual node on SM root)
                        if (s.Name != "Any State" && s.Name != "Entry" && s.Name != "Exit")
                        {
                            s._sourceAsset?.UpdateAssetDataAndRow(_workspace, sourceSmField);
                        }
                    }

                    // 3. Save StateMachine asset
                    var assetInst = _workspace.GetAssetInst(smAsset.file, 0, smAsset.info.PathId);
                    if (assetInst != null)
                    {
                        assetInst.UpdateAssetDataAndRow(_workspace, smField);
                        WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(assetInst));
                    }
                }
                else
                {
                    // Runtime Format
                    var controller = FieldHelper.GetField(_controllerRoot, "m_Controller");
                    var smArray = !controller.IsDummy ? FieldHelper.GetField(controller, "m_StateMachineArray") : FieldHelper.GetField(_controllerRoot, "m_StateMachineArray");
                    var smArr = !FieldHelper.GetField(smArray, "Array").IsDummy ? FieldHelper.GetField(smArray, "Array") : smArray;
                    
                    int layerIdx = Layers.IndexOf(SelectedLayer);
                    var smChild = smArr.Children[layerIdx];
                    var smField = FieldHelper.GetField(smChild, "data").IsDummy ? smChild : FieldHelper.GetField(smChild, "data");
                    
                    var stateArrField = FieldHelper.GetField(smField, "m_StateConstantArray");
                    var stateArr = !FieldHelper.GetField(stateArrField, "Array").IsDummy ? FieldHelper.GetField(stateArrField, "Array") : stateArrField;
                    
                    int deletedIdx = (int)state.PathId;
                    if (stateArr.Children != null && deletedIdx >= 0 && deletedIdx < stateArr.Children.Count)
                    {
                        stateArr.Children.RemoveAt(deletedIdx);
                        FieldHelper.SyncArraySize(stateArrField);
                    }

                    // Shift indices and remove dead transitions
                    Action<AssetTypeValueField> cleanupTransitions = (container) =>
                    {
                        var arr = !FieldHelper.GetField(container, "Array").IsDummy ? FieldHelper.GetField(container, "Array") : container;
                        if (arr.Children == null) return;
                        
                        var toRemove = new List<AssetTypeValueField>();
                        foreach (var t in arr.Children)
                        {
                            var data = FieldHelper.GetField(t, "data").IsDummy ? t : FieldHelper.GetField(t, "data");
                            var dstField = FieldHelper.GetField(data, "m_DestinationState");
                            if (dstField.IsDummy) dstField = FieldHelper.GetField(data, "m_Destination"); // Selector case
                            
                            if (!dstField.IsDummy)
                            {
                                int currentDst = (int)dstField.AsLong;
                                if (currentDst == deletedIdx) toRemove.Add(t);
                                else if (currentDst > deletedIdx && currentDst < 30000) dstField.AsLong = currentDst - 1;
                            }
                        }
                        foreach (var tr in toRemove) arr.Children.Remove(tr);
                        if (toRemove.Count > 0) FieldHelper.SyncArraySize(container);
                    };

                    // Any State
                    cleanupTransitions(FieldHelper.GetField(smField, "m_AnyStateTransitionConstantArray"));
                    
                    // Entry (Selectors)
                    var selectorArray = FieldHelper.GetField(smField, "m_SelectorStateConstantArray");
                    var actualSelector = !FieldHelper.GetField(selectorArray, "Array").IsDummy ? FieldHelper.GetField(selectorArray, "Array") : selectorArray;
                    if (actualSelector.Children != null)
                    {
                        foreach (var selector in actualSelector.Children)
                        {
                            var data = FieldHelper.GetField(selector, "data").IsDummy ? selector : FieldHelper.GetField(selector, "data");
                            cleanupTransitions(FieldHelper.GetField(data, "m_TransitionConstantArray"));
                        }
                    }

                    // All other states
                    if (stateArr.Children != null)
                    {
                        foreach (var sConst in stateArr.Children)
                        {
                            var sData = FieldHelper.GetField(sConst, "data").IsDummy ? sConst : FieldHelper.GetField(sConst, "data");
                            cleanupTransitions(FieldHelper.GetField(sData, "m_TransitionConstantArray"));
                        }
                    }

                    // Update Default State Index
                    long defIdx = FieldHelper.GetLong(smField, "m_DefaultState");
                    if (defIdx == deletedIdx) smField["m_DefaultState"].AsLong = 0;
                    else if (defIdx > deletedIdx && defIdx < 30000) smField["m_DefaultState"].AsLong = defIdx - 1;

                    SelectedController.Asset.UpdateAssetDataAndRow(_workspace, _controllerRoot);
                    WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(SelectedController.Asset));
                }

                // UI Cleanup: Force reload layer to ensure all indices and transitions are fresh
                if (SelectedLayer != null)
                {
                    var layer = SelectedLayer;
                    SelectedLayer = null;
                    SelectedLayer = layer;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] DeleteState Error: {ex.Message}");
            }
        }

        private void UpdateTransitionLines(LayerItem layer)
        {
            TransitionLines.Clear();
            var states = States.ToList();
            var groupedTransitions = states
                .SelectMany(s => s.Transitions.Select(t => new { From = s, Trans = t }))
                .GroupBy(x => new { x.From, x.Trans.DstPathId });

            foreach (var group in groupedTransitions)
            {
                var target = states.FirstOrDefault(x => x.PathId == group.Key.DstPathId);
                if (target != null)
                {
                    var transList = group.Select(x => x.Trans).ToList();
                    var defaultLink = transList.FirstOrDefault(t => t.IsDefaultLink);
                    var line = new TransitionLine
                    {
                        FromState = group.Key.From,
                        ToState = target,
                        IsMultiple = group.Count() > 1,
                        SourceTransition = defaultLink ?? transList.First() 
                    };
                    foreach (var t in group)
                    {
                        line.Transitions.Add(t.Trans);
                        t.Trans.PropertyChanged += (s, e) => { 
                            if (e.PropertyName == "Conditions" || e.PropertyName == "IsDefaultLink") 
                                line.RefreshSummary(); 
                        };
                    }
                    line.RefreshSummary();
                    TransitionLines.Add(line);
                }
            }

            // Detect mutual transitions and update points
            foreach (var line in TransitionLines)
            {
                // For mutual detection, compare IDs or names
                line.IsMutual = TransitionLines.Any(l => l != line && 
                    (l.FromState?.PathId == line.ToState?.PathId && l.ToState?.PathId == line.FromState?.PathId) &&
                    (l.FromState?.Name == line.ToState?.Name && l.ToState?.Name == line.FromState?.Name));
                line.UpdatePoints();
            }
        }

        public void LogToFile(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] {message}");
        }

        private void LoadAnimationMap(AssetInst controllerAsset)
        {
            _animationMap.Clear();
            LogToFile($"--- Loading Animation Map (Workspace-based) for {controllerAsset.AssetName} ---");
            try 
            {
                var assetsFiles = WorkspaceItem.GetAssetsFileWorkspaceItems(_workspace.RootItems);
                foreach (var wsItem in assetsFiles)
                {
                    if (wsItem.Object is not AssetsFileInstance asInst) continue;
                    
                    foreach (var info in asInst.file.AssetInfos)
                    {
                        if (info.TypeId == (int)AssetClassID.AnimationClip)
                        {
                            // In UABEANext, AssetInst holds the pre-resolved name
                            if (info is AssetInst animInst)
                            {
                                if (!string.IsNullOrEmpty(animInst.AssetName))
                                {
                                    _animationMap[animInst.PathId] = animInst.AssetName;
                                }
                            }
                            else
                            {
                                var inst = _workspace.GetAssetInst(asInst, 0, info.PathId);
                                if (inst != null && !string.IsNullOrEmpty(inst.AssetName))
                                {
                                    _animationMap[inst.PathId] = inst.AssetName;
                                }
                            }
                        }
                    }
                }
                LogToFile($"Map populated with {_animationMap.Count} workspace animation entries.");
            }
            catch (Exception ex)
            {
                LogToFile($"Error loading workspace animation map: {ex.Message}");
            }
        }

        partial void OnSelectedLayerChanged(LayerItem? value)
        {
            States.Clear();
            TransitionLines.Clear();
            if (value != null)
            {
                var paramTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var p in ParameterItems)
                {
                    if (!string.IsNullOrEmpty(p.Name))
                        paramTypes[p.Name.Trim()] = p.Type;
                }
                var states = value.GetStates(_animationMap, paramTypes, ParameterItems, LogToFile);
                
                // --- Coordinate Normalization ---
                if (states.Count > 0)
                {
                    double minX = states.Min(s => s.X);
                    double minY = states.Min(s => s.Y);
                    
                    // Only normalize if there are negative coordinates (to keep them in view)
                    // or if everything is far from the origin.
                    if (minX < 50 || minY < 50 || minX > 500 || minY > 500)
                    {
                        double offsetX = 100.0 - minX;
                        double offsetY = 100.0 - minY;
                        
                        foreach (var s in states)
                        {
                            s.X += offsetX;
                            s.Y += offsetY;
                        }
                    }
                }
                // --------------------------------

                foreach (var s in states)
                {
                    s.CurrentMotion = AvailableMotions.FirstOrDefault(m => m.PathId == s.MotionPathId);
                    s.PositionChanged += HandleStatePositionChanged;
                    s.NameChanged += HandleStateNameChanged;
                    States.Add(s);
                }

                // Build transition lines
                UpdateTransitionLines(value);

                RequestCenterView?.Invoke();
            }
        }

        [RelayCommand]
        public void AddLayer()
        {
            if (_workspace == null || SelectedController == null) return;
            
            try
            {
                var controllerRoot = SelectedController.GetRoot();
                if (controllerRoot == null) return;

                // 1. Get Arrays
                var layerArrayField = FieldHelper.FindFieldRecursive(controllerRoot, "m_LayerArray");
                if (layerArrayField == null || layerArrayField.IsDummy) return;

                var layerArr = !FieldHelper.GetField(layerArrayField, "Array").IsDummy 
                    ? FieldHelper.GetField(layerArrayField, "Array") 
                    : layerArrayField;

                var smArrayField = FieldHelper.FindFieldRecursive(controllerRoot, "m_StateMachineArray");
                if (smArrayField == null || smArrayField.IsDummy) return;

                var smArr = !FieldHelper.GetField(smArrayField, "Array").IsDummy 
                    ? FieldHelper.GetField(smArrayField, "Array") 
                    : smArrayField;

                // 2. Create New State Machine
                if (smArr.TemplateField == null || smArr.TemplateField.Children.Count < 2) return;
                var smTemplate = smArr.TemplateField.Children[1];
                var newSm = ValueBuilder.DefaultValueFieldFromTemplate(smTemplate);
                var smData = FieldHelper.GetField(newSm, "data").IsDummy ? newSm : FieldHelper.GetField(newSm, "data");

                // Initialize State Machine Data (Based on user template)
                smData["m_DefaultState"].AsInt = 0;
                smData["m_SynchronizedLayerCount"].AsInt = 1; // Unsure why 1, but user json has 1. Usually 0 for base?
                
                // Init Arrays in SM
                var stateArr = FieldHelper.GetField(smData, "m_StateConstantArray");
                var stateArrInfo = !FieldHelper.GetField(stateArr, "Array").IsDummy ? FieldHelper.GetField(stateArr, "Array") : stateArr;
                if (stateArrInfo.Children == null) stateArrInfo.Children = new List<AssetTypeValueField>();

                var anyTrans = FieldHelper.GetField(smData, "m_AnyStateTransitionConstantArray");
                var anyTransArr = !FieldHelper.GetField(anyTrans, "Array").IsDummy ? FieldHelper.GetField(anyTrans, "Array") : anyTrans;
                if (anyTransArr.Children == null) anyTransArr.Children = new List<AssetTypeValueField>();

                var selectorArr = FieldHelper.GetField(smData, "m_SelectorStateConstantArray");
                var selectorArrInfo = !FieldHelper.GetField(selectorArr, "Array").IsDummy ? FieldHelper.GetField(selectorArr, "Array") : selectorArr;
                if (selectorArrInfo.Children == null) selectorArrInfo.Children = new List<AssetTypeValueField>();

                // Add Entry/Exit selectors (Typical Runtime Structure)
                // Entry
                 var selectorTemplate = selectorArrInfo.TemplateField.Children[1];
                 var entrySel = ValueBuilder.DefaultValueFieldFromTemplate(selectorTemplate);
                 var entryData = FieldHelper.GetField(entrySel, "data").IsDummy ? entrySel : FieldHelper.GetField(entrySel, "data");
                 entryData["m_FullPathID"].AsUInt = AnimatorHash.GetHash("Entry");
                 entryData["m_IsEntry"].AsBool = true;
                 selectorArrInfo.Children.Add(entrySel);

                 // Exit
                 var exitSel = ValueBuilder.DefaultValueFieldFromTemplate(selectorTemplate);
                 var exitData = FieldHelper.GetField(exitSel, "data").IsDummy ? exitSel : FieldHelper.GetField(exitSel, "data");
                 exitData["m_FullPathID"].AsUInt = AnimatorHash.GetHash("Exit");
                 exitData["m_IsEntry"].AsBool = false; 
                 // Note: Exit usually implies not entry, but check structure. User json shows 2 selectors.
                 selectorArrInfo.Children.Add(exitSel);


                if (smArr.Children == null) smArr.Children = new List<AssetTypeValueField>();
                smArr.Children.Add(newSm);
                int newSmIndex = smArr.Children.Count - 1;


                // 3. Create New Layer
                if (layerArr.TemplateField == null || layerArr.TemplateField.Children.Count < 2) return;
                var layerTemplate = layerArr.TemplateField.Children[1];
                var newLayer = ValueBuilder.DefaultValueFieldFromTemplate(layerTemplate);
                var layerData = FieldHelper.GetField(newLayer, "data").IsDummy ? newLayer : FieldHelper.GetField(newLayer, "data");

                layerData["m_StateMachineIndex"].AsInt = newSmIndex;
                layerData["m_StateMachineSynchronizedLayerIndex"].AsInt = 0;
                layerData["m_DefaultWeight"].AsFloat = 1.0f;
                layerData["m_StateMachineIndex"].AsInt = newSmIndex;
                layerData["m_StateMachineSynchronizedLayerIndex"].AsInt = 0;
                layerData["m_DefaultWeight"].AsFloat = 1.0f;

                // Initialize body mask to all-on (all body parts enabled)
                var bodyMask = layerData["m_BodyMask"];
                if (!bodyMask.IsDummy)
                {
                    if (!bodyMask["word0"].IsDummy) bodyMask["word0"].AsUInt = 0xFFFFFFFF;
                    if (!bodyMask["word1"].IsDummy) bodyMask["word1"].AsUInt = 0xFFFFFFFF;
                    if (!bodyMask["word2"].IsDummy) bodyMask["word2"].AsUInt = 0x7FFFF;
                }
                
                // Unique Name Generation
                string baseName = "New Layer";
                string newLayerName = baseName;
                int suffix = 0;
                var existingNames = new HashSet<string>(SelectedController.GetLayers(this).Select(l => l.Name));
                
                while (existingNames.Contains(newLayerName))
                {
                    newLayerName = $"{baseName} {suffix}";
                    suffix++;
                }
                
                layerData["m_Binding"].AsUInt = AnimatorHash.GetHash(newLayerName);

                if (layerArr.Children == null) layerArr.Children = new List<AssetTypeValueField>();
                layerArr.Children.Add(newLayer);

                // 4. Update TOS
                var tosField = FieldHelper.FindFieldRecursive(controllerRoot, "m_TOS");
                if (tosField == null || tosField.IsDummy) tosField = FieldHelper.FindFieldRecursive(controllerRoot, "m_TOC");
                
                if (tosField != null && !tosField.IsDummy)
                {
                    FieldHelper.UpdateTOS(controllerRoot, layerData["m_Binding"].AsUInt, newLayerName);
                }

                // 5. Sync and Save
                FieldHelper.SyncArraySize(layerArrayField);
                FieldHelper.SyncArraySize(smArrayField);

                SelectedController.Asset.UpdateAssetDataAndRow(_workspace, controllerRoot);
                WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(SelectedController.Asset));

                // 6. Refresh
                SelectedController.Refresh(); // Refresh string table
                var newLayersList = SelectedController.GetLayers(this);
                Layers.Clear();
                foreach (var layer in newLayersList)
                {
                    Layers.Add(layer);
                }
                SelectedLayer = Layers.LastOrDefault();
                RefreshBaseLayers();
            }
            catch (Exception ex)
            {
                LogToFile($"AddLayer Error: {ex.Message}");
            }
        }

        [RelayCommand]
        public void RemoveLayer()
        {
            if (_workspace == null || SelectedController == null || SelectedLayer == null) return;
            
            try
            {
                var controllerRoot = SelectedController.GetRoot();
                if (controllerRoot == null) return;

                var layerArrayField = FieldHelper.FindFieldRecursive(controllerRoot, "m_LayerArray");
                var layerArr = !FieldHelper.GetField(layerArrayField, "Array").IsDummy ? FieldHelper.GetField(layerArrayField, "Array") : layerArrayField;

                var smArrayField = FieldHelper.FindFieldRecursive(controllerRoot, "m_StateMachineArray");
                var smArr = !FieldHelper.GetField(smArrayField, "Array").IsDummy ? FieldHelper.GetField(smArrayField, "Array") : smArrayField;

                // Find index of selected layer
                int index = Layers.IndexOf(SelectedLayer);
                if (index == -1 || index >= layerArr.Children.Count) return;

                // Get SM index before removing
                var layerData = FieldHelper.GetField(layerArr.Children[index], "data").IsDummy ? layerArr.Children[index] : FieldHelper.GetField(layerArr.Children[index], "data");
                int smIndex = FieldHelper.GetInt(layerData, "m_StateMachineIndex");

                // Remove Layer
                layerArr.Children.RemoveAt(index);

                // Remove StateMachine (if valid index)
                if (smIndex >= 0 && smIndex < smArr.Children.Count)
                {
                    smArr.Children.RemoveAt(smIndex);
                    
                    // Shift indices for subsequent layers
                    for (int i = 0; i < layerArr.Children.Count; i++)
                    {
                        var lData = FieldHelper.GetField(layerArr.Children[i], "data").IsDummy ? layerArr.Children[i] : FieldHelper.GetField(layerArr.Children[i], "data");
                        int currentSmIdx = FieldHelper.GetInt(lData, "m_StateMachineIndex");
                        if (currentSmIdx > smIndex)
                        {
                            lData["m_StateMachineIndex"].AsInt = currentSmIdx - 1;
                        }
                    }
                }

                // Sync and Save
                if (layerArrayField != null) FieldHelper.SyncArraySize(layerArrayField);
                if (smArrayField != null) FieldHelper.SyncArraySize(smArrayField);
                
                SelectedController.Asset.UpdateAssetDataAndRow(_workspace, controllerRoot);
                WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(SelectedController.Asset));

                // Refresh
                SelectedLayer = null;
                var newLayersList = SelectedController.GetLayers(this);
                Layers.Clear();
                foreach (var layer in newLayersList)
                {
                    Layers.Add(layer);
                }
                RefreshBaseLayers();
            }
            catch (Exception ex)
            {
                LogToFile($"RemoveLayer Error: {ex.Message}");
            }
        }

        private void LoadControllersAsync()
        {
             // Helper wrapping the original Sync logic if needed, 
             // but for now we just reuse existing methods or refresh manually.
             LoadControllers();
        }

        public void HandleStatePositionChanged(StateItem state, double x, double y)
        {
            state.X = x;
            state.Y = y;
        }

        public void CreateStateAt(LayerItem layer, double x, double y)
        {
            if (_workspace == null) return;
            
            var newState = layer.CreateState(x, y, _animationMap, ParameterItems, LogToFile);
            if (newState != null)
            {
                newState.CurrentMotion = AvailableMotions.FirstOrDefault(m => m.PathId == newState.MotionPathId);
                newState.PositionChanged += HandleStatePositionChanged;
                newState.NameChanged += HandleStateNameChanged;
                States.Add(newState);
                SelectedState = newState;
            }
        }

        private void HandleStateNameChanged(StateItem state)
        {
            // Update destination names for transitions pointing to this state
            foreach (var s in States)
            {
                foreach (var t in s.Transitions)
                {
                    if (t.DstPathId == state.PathId) t.DestinationName = state.Name;
                }
            }
        }

        private void HandleStatePositionChanged(StateItem state)
        {
            foreach (var line in TransitionLines)
            {
                if (line.FromState == state || line.ToState == state)
                {
                    line.UpdatePoints();
                }
            }
        }
    }

    public class ControllerItem
    {
        public AssetInst Asset { get; }
        public string Name { get; }
        private readonly Workspace _workspace;
        
        private Dictionary<uint, string>? _stringTable;
        public Dictionary<uint, string> StringTable
        {
            get
            {
                if (_stringTable == null)
                {
                    var baseField = _workspace.GetBaseField(Asset);
                    _stringTable = baseField != null ? BuildStringTable(baseField) : new Dictionary<uint, string>();
                }
                return _stringTable!;
            }
        }

        public ControllerItem(AssetInst asset, Workspace workspace, string? role = null)
        {
            Asset = asset;
            _workspace = workspace;
            string baseName = asset.AssetName ?? "Unnamed Controller";
            string fileName = asset.FileInstance.name;
            if (role != null)
                Name = $"{baseName} ({role})";
            else
                Name = $"{baseName} ({fileName})";
        }

        public AssetTypeValueField? GetRoot()
        {
            return _workspace.GetBaseField(Asset);
        }

        public void Refresh()
        {
            _stringTable = null;
        }

        public List<LayerItem> GetLayers(UABEANext4.ViewModels.Tools.PasswordCrackerViewModel parent, AssetTypeValueField? sharedRoot = null)
        {
            var list = new List<LayerItem>();
            var baseField = sharedRoot ?? _workspace.GetBaseField(Asset);
            if (baseField == null) return list;

            var stringTable = this.StringTable;

            // Editor format: m_Layers
            var layersField = FieldHelper.FindFieldRecursive(baseField, "m_Layers");
            if (layersField != null && !layersField.IsDummy)
            {
                var array = FieldHelper.GetField(layersField, "Array");
                var actualArray = !array.IsDummy ? array : layersField;
                if (actualArray.Children != null)
                {
                    foreach (var layerField in actualArray.Children)
                    {
                        list.Add(new LayerItem(layerField, _workspace, Asset.FileInstance, Asset, baseField) { Parent = parent });
                    }
                    return list;
                }
            }

            // Runtime format: m_Controller/m_LayerArray
            var controllerField = FieldHelper.GetField(baseField, "m_Controller");
            if (!controllerField.IsDummy)
            {
                var layerArray = FieldHelper.GetField(controllerField, "m_LayerArray");
                if (!layerArray.IsDummy)
                {
                    var array = FieldHelper.GetField(layerArray, "Array");
                    var actualArray = !array.IsDummy ? array : layerArray;
                    if (actualArray.Children != null)
                    {
                        var smIndexToFirstLayer = new Dictionary<int, int>();
                        for (int i = 0; i < actualArray.Children.Count; i++)
                        {
                            var layerField = actualArray.Children[i];
                            var dataField = FieldHelper.GetField(layerField, "data").IsDummy ? layerField : FieldHelper.GetField(layerField, "data");
                            uint bindingId = (uint)FieldHelper.GetLong(dataField, "m_Binding");
                            int smIndex = FieldHelper.GetInt(dataField, "m_StateMachineIndex");
                            string name = stringTable.TryGetValue(bindingId, out var n) ? n : $"Layer_{bindingId}";
                            
                            // Determine Sync Status based on SM Index Reuse
                            int syncedIndex = -1;
                            if (smIndexToFirstLayer.ContainsKey(smIndex))
                            {
                                syncedIndex = smIndexToFirstLayer[smIndex];
                            }
                            else
                            {
                                smIndexToFirstLayer[smIndex] = i;
                            }

                            list.Add(new LayerItem(name, dataField, _workspace, Asset.FileInstance, smIndex, stringTable, baseField, Asset, syncedIndex) { Parent = parent });
                        }
                        return list;
                    }
                }
            }

            return list;
        }

        public List<ParameterItem> GetParameters(UABEANext4.ViewModels.Tools.PasswordCrackerViewModel parent, AssetTypeValueField? sharedRoot = null)
        {
            var list = new List<ParameterItem>();
            var baseField = sharedRoot ?? _workspace.GetBaseField(Asset);
            if (baseField == null) return list;

            var stringTable = this.StringTable;

            // Editor format: m_Parameters
            var paramsField = FieldHelper.FindFieldRecursive(baseField, "m_Parameters");
            
            if (paramsField != null && !paramsField.IsDummy)
            {
                var array = FieldHelper.GetField(paramsField, "Array");
                var actualArray = !array.IsDummy ? array : paramsField;
                if (actualArray.Children != null)
                {
                    foreach (var paramField in actualArray.Children)
                    {
                        list.Add(new ParameterItem(paramField, _workspace, Asset, baseField));
                    }
                    return list;
                }
            }

            // Runtime format
            var controller = FieldHelper.GetField(baseField, "m_Controller");
            var valuesVal = !controller.IsDummy ? FieldHelper.GetField(controller, "m_Values") : FieldHelper.GetField(baseField, "m_Values");
            var values = FieldHelper.GetField(valuesVal, "data").IsDummy ? valuesVal : FieldHelper.GetField(valuesVal, "data");
            
            var defaultsVal = !controller.IsDummy ? FieldHelper.GetField(controller, "m_DefaultValues") : FieldHelper.GetField(baseField, "m_DefaultValues");
            var defaults = FieldHelper.GetField(defaultsVal, "data").IsDummy ? defaultsVal : FieldHelper.GetField(defaultsVal, "data");

            if (!values.IsDummy)
            {
                var paramArr = FieldHelper.GetField(values, "m_ParameterArray");
                var paramArray = paramArr.IsDummy ? FieldHelper.GetField(values, "m_ValueArray") : paramArr;
                if (!paramArray.IsDummy)
                {
                    var array = FieldHelper.GetField(paramArray, "Array");
                    var actualArray = !array.IsDummy ? array : paramArray;
                    if (actualArray.Children != null)
                    {
                        foreach (var paramField in actualArray.Children)
                        {
                            var data = FieldHelper.GetField(paramField, "data").IsDummy ? paramField : FieldHelper.GetField(paramField, "data");
                            uint nameId = (uint)(!FieldHelper.GetField(data, "m_NameID").IsDummy ? FieldHelper.GetLong(data, "m_NameID") : FieldHelper.GetLong(data, "m_ID"));
                            string name = stringTable.TryGetValue(nameId, out var n) ? n : $"Hash_{nameId:X8}";
                            list.Add(new ParameterItem(name: name, id: nameId, field: data, defaults: defaultsVal, workspace: _workspace, asset: Asset, root: baseField, stringTable: stringTable));
                        }
                    }
                }
            }
            
            return list;
        }

        private Dictionary<uint, string> BuildStringTable(AssetTypeValueField baseField)
        {
            var table = new Dictionary<uint, string>();
            
            // Search for m_TOC, m_TOS or any field that looks like a mapping table
            var tocField = FieldHelper.FindFieldRecursive(baseField, "m_TOC");
            if (tocField == null || tocField.IsDummy)
            {
                tocField = FieldHelper.FindFieldRecursive(baseField, "m_TOS");
            }
            if (tocField == null || tocField.IsDummy)
            {
                // Try searching for an array of pairs
                tocField = FindTableOfContents(baseField);
            }

            if (tocField != null && !tocField.IsDummy)
            {
                var actualArray = !tocField["Array"].IsDummy ? tocField["Array"] : tocField;
                if (actualArray.Children != null)
                {
                    foreach (var entry in actualArray.Children)
                    {
                        var data = FieldHelper.GetField(entry, "data").IsDummy ? entry : FieldHelper.GetField(entry, "data");
                        uint id = (uint)FieldHelper.GetLong(data, "first");
                        string name = FieldHelper.GetString(data, "second");
                        if (!string.IsNullOrEmpty(name))
                            table[id] = name;
                    }
                }
            }
            return table;
        }

        private AssetTypeValueField? FindTableOfContents(AssetTypeValueField field)
        {
            if (field.Children == null) return null;
            
            // Look for a field that is an array and whose children have "first" and "second"
            var actualArray = !field["Array"].IsDummy ? field["Array"] : field;
            if (actualArray.Children != null && actualArray.Children.Count > 0)
            {
                var firstChild = actualArray.Children[0];
                var data = FieldHelper.GetField(firstChild, "data").IsDummy ? firstChild : FieldHelper.GetField(firstChild, "data");
                if (!FieldHelper.GetField(data, "first").IsDummy && !FieldHelper.GetField(data, "second").IsDummy)
                    return field;
            }

            foreach (var child in field.Children)
            {
                var result = FindTableOfContents(child);
                if (result != null) return result;
            }
            return null;
        }



        public void UpdateTOS(AssetTypeValueField baseField, uint hash, string name)
        {
            FieldHelper.UpdateTOS(baseField, hash, name);
        }
    }

    public partial class ParameterItem : ObservableObject
    {
        [ObservableProperty] private string _name = "";
        public uint ParamId { get; private set; }
        public string Type { get; private set; } = "";
        [ObservableProperty] private string _defaultValue = "";
        
        public bool IsFloat => Type == "Float";
        public bool IsInt => Type == "Int";
        public bool IsBool => Type == "Bool";
        public bool IsTrigger => Type == "Trigger";
        public bool IsNumeric => IsFloat || IsInt;

        public bool BoolValue
        {
            get => bool.TryParse(DefaultValue, out bool b) && b;
            set => DefaultValue = value.ToString();
        }

        private readonly AssetTypeValueField _field;
        private readonly AssetTypeValueField? _defaults;
        private readonly Workspace? _workspace;
        private readonly AssetInst? _asset;
        private readonly AssetTypeValueField? _root;
        private readonly bool _isRuntime;
        private readonly Dictionary<uint, string>? _stringTable;

        public ParameterItem(AssetTypeValueField field, Workspace? workspace = null, AssetInst? asset = null, AssetTypeValueField? root = null)
        {
            _field = field;
            _workspace = workspace;
            _asset = asset;
            _root = root;
            _isRuntime = false;

            _name = FieldHelper.GetString(field, "m_Name");
            ParamId = (uint)(!FieldHelper.GetField(field, "m_NameID").IsDummy ? FieldHelper.GetLong(field, "m_NameID") : FieldHelper.GetLong(field, "m_ID"));
            int type = FieldHelper.GetInt(field, "m_Type");
            ParseTypeAndValue(field, type, null);
        }

        public ParameterItem(string name, uint id, AssetTypeValueField field, AssetTypeValueField? defaults, 
            Workspace? workspace = null, AssetInst? asset = null, AssetTypeValueField? root = null, Dictionary<uint, string>? stringTable = null)
        {
            _field = field;
            _defaults = defaults;
            _workspace = workspace;
            _asset = asset;
            _root = root;
            _isRuntime = true;
            _stringTable = stringTable;

            _name = name;
            ParamId = id;
            int type = FieldHelper.GetInt(field, "m_Type");
            ParseTypeAndValue(field, type, defaults);
        }

        partial void OnNameChanged(string value)
        {
            if (string.IsNullOrEmpty(value) || _field.IsDummy) return;
            
            string oldName = _name;
            if (!_isRuntime)
            {
                _field["m_Name"].AsString = value;
                // GLOBAL SYNC (Editor): Update all transitions that use this parameter name
                UpdateAllConditionsName(oldName, value);
                WeakReferenceMessenger.Default.Send(new ParameterRenamedMessage(0, 0, value, oldName));
            }
            else
            {
                // Update NameID/Hash
                uint oldHash = ParamId;
                uint newHash = AnimatorHash.GetHash(value);
                var idField = _field["m_NameID"];
                if (idField.IsDummy) idField = _field["m_ID"];
                if (!idField.IsDummy) idField.AsLong = (long)newHash;
                ParamId = newHash;

                // Sync with TOS
                bool tosUpdated = false;
                if (_root != null)
                {
                    var tosField = FieldHelper.FindFieldRecursive(_root, "m_TOS") ?? FieldHelper.GetField(_root, "m_TOS");
                    var tosArray = !FieldHelper.GetField(tosField, "Array").IsDummy ? FieldHelper.GetField(tosField, "Array") : tosField;
                    if (tosArray.Children != null)
                    {
                        foreach (var entry in tosArray.Children)
                        {
                            var data = FieldHelper.GetField(entry, "data").IsDummy ? entry : FieldHelper.GetField(entry, "data");
                            if (FieldHelper.GetLong(data, "first") == (long)oldHash)
                            {
                                var fField = data["first"];
                                if (!fField.IsDummy) fField.AsLong = (long)newHash;
                                
                                var sField = data["second"];
                                if (!sField.IsDummy) sField.AsString = value;
                                tosUpdated = true;
                                break;
                            }
                        }
                    }
                }
                
                // Update live string table dictionary
                if (_stringTable != null)
                {
                    if (oldHash != newHash) _stringTable.Remove(oldHash);
                    _stringTable[newHash] = value;
                }
                
                // If not found in TOS, we need to add a new entry to ensure it's saved by name
                if (!tosUpdated && _root != null)
                {
                    FieldHelper.UpdateTOS(_root, newHash, value);
                }

                // GLOBAL SYNC (Runtime): Update all transitions that use this parameter hash
                if (_root != null)
                {
                    UpdateAllConditionsHash(oldHash, newHash, value);
                    // Send message to update UI conditions in labels/viewmodels
                    WeakReferenceMessenger.Default.Send(new ParameterRenamedMessage(oldHash, newHash, value, oldName));
                }
            }
            _name = value; // Update backing field
            NotifyDirty();
        }

        private void UpdateAllConditionsName(string oldName, string newName)
        {
            if (_root == null || _workspace == null || _asset == null) return;
            
            var layersField = FieldHelper.GetField(_root, "m_Layers");
            var layersArray = !FieldHelper.GetField(layersField, "Array").IsDummy ? FieldHelper.GetField(layersField, "Array") : layersField;
            if (layersArray.Children == null) return;

            foreach (var layerWrapper in layersArray.Children)
            {
                var smPtr = FieldHelper.GetField(layerWrapper, "m_StateMachine");
                if (smPtr.IsDummy) continue;

                if (_workspace.Manager == null) continue;
                var smAsset = _workspace.Manager.GetExtAsset(_asset.FileInstance, smPtr);
                if (smAsset.baseField == null) continue;

                UpdateStateMachineConditions(smAsset, oldName, newName);
            }
        }

        private void UpdateStateMachineConditions(AssetExternal smAsset, string oldName, string newName)
        {
             if (_workspace?.Manager == null) return;
             // Traverse child states
             var statesField = FieldHelper.GetField(smAsset.baseField, "m_States");
             var statesArray = !FieldHelper.GetField(statesField, "Array").IsDummy ? FieldHelper.GetField(statesField, "Array") : statesField;
             if (statesArray.Children != null)
             {
                 foreach (var stateWrapper in statesArray.Children)
                 {
                     var statePtr = FieldHelper.GetField(stateWrapper, "m_State");
                     var stateAsset = _workspace.Manager.GetExtAsset(smAsset.file, statePtr);
                     if (stateAsset.baseField != null)
                     {
                         UpdateStateTransitions(stateAsset, oldName, newName);
                     }
                 }
             }
             
             // Traverse transitions in the state machine itself (Any State, Entry, etc.)
             UpdateTransitionArray(smAsset.file, FieldHelper.GetField(smAsset.baseField, "m_AnyStateTransitions"), oldName, newName);
             UpdateTransitionArray(smAsset.file, FieldHelper.GetField(smAsset.baseField, "m_EntryTransitions"), oldName, newName);
        }
        
        private void UpdateStateTransitions(AssetExternal stateAsset, string oldName, string newName)
        {
            if (_workspace?.Manager == null) return;
            var transitionsField = FieldHelper.GetField(stateAsset.baseField, "m_Transitions");
            UpdateTransitionArray(stateAsset.file, transitionsField, oldName, newName);
        }

        private void UpdateTransitionArray(AssetsFileInstance file, AssetTypeValueField transArrayField, string oldName, string newName)
        {
            var actualArray = !FieldHelper.GetField(transArrayField, "Array").IsDummy ? FieldHelper.GetField(transArrayField, "Array") : transArrayField;
            if (actualArray.Children == null) return;

            foreach (var transPtr in actualArray.Children)
            {
                if (_workspace.Manager == null) continue;
                var transAsset = _workspace.Manager.GetExtAsset(file, transPtr);
                if (transAsset.baseField != null)
                {
                    var condsField = FieldHelper.GetField(transAsset.baseField, "m_Conditions");
                    var condsArray = !FieldHelper.GetField(condsField, "Array").IsDummy ? FieldHelper.GetField(condsField, "Array") : condsField;
                    if (condsArray.Children != null)
                    {
                        bool changed = false;
                        foreach (var cond in condsArray.Children)
                        {
                            var data = FieldHelper.GetField(cond, "data").IsDummy ? cond : FieldHelper.GetField(cond, "data");
                            if (FieldHelper.GetString(data, "m_ConditionParam") == oldName)
                            {
                                data["m_ConditionParam"].AsString = newName;
                                changed = true;
                            }
                        }
                        if (changed)
                        {
                            var inst = _workspace.GetAssetInst(transAsset.file, 0, transAsset.info.PathId);
                            if (inst != null)
                            {
                                inst.UpdateAssetDataAndRow(_workspace, transAsset.baseField);
                                WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(inst));
                            }
                        }
                    }
                }
            }
        }

        private void UpdateAllConditionsHash(uint oldHash, uint newHash, string newName)
        {
            if (_root == null) return;
            // Scan all layers/states/transitions in the root field
            var controller = FieldHelper.GetField(_root, "m_Controller");
            if (controller.IsDummy) return;

            var smArray = FieldHelper.GetField(controller, "m_StateMachineArray");
            var smActual = !smArray["Array"].IsDummy ? smArray["Array"] : smArray;
            if (smActual.Children == null) return;

            foreach (var sm in smActual.Children)
            {
                var smData = FieldHelper.GetField(sm, "data").IsDummy ? sm : FieldHelper.GetField(sm, "data");
                var stArray = FieldHelper.GetField(smData, "m_StateConstantArray");
                var stActual = !stArray["Array"].IsDummy ? stArray["Array"] : stArray;
                if (stActual.Children == null) continue;

                foreach (var st in stActual.Children)
                {
                    var stData = FieldHelper.GetField(st, "data").IsDummy ? st : FieldHelper.GetField(st, "data");
                    var trArray = FieldHelper.GetField(stData, "m_TransitionConstantArray");
                    var trActual = !trArray["Array"].IsDummy ? trArray["Array"] : trArray;
                    if (trActual.Children == null) continue;

                    foreach (var tr in trActual.Children)
                    {
                        var trData = FieldHelper.GetField(tr, "data").IsDummy ? tr : FieldHelper.GetField(tr, "data");
                        var condArray = FieldHelper.GetField(trData, "m_ConditionConstantArray");
                        var condActual = !condArray["Array"].IsDummy ? condArray["Array"] : condArray;
                        if (condActual.Children == null) continue;

                        foreach (var cond in condActual.Children)
                        {
                            var condData = FieldHelper.GetField(cond, "data").IsDummy ? cond : FieldHelper.GetField(cond, "data");
                            if (FieldHelper.GetLong(condData, "m_EventID") == (long)oldHash)
                            {
                                var eidField = condData["m_EventID"];
                                if (!eidField.IsDummy) eidField.AsLong = (long)newHash;
                            }
                        }
                    }
                }
            }
        }

        partial void OnDefaultValueChanged(string value)
        {
            OnPropertyChanged(nameof(BoolValue));
            if (_field.IsDummy) return;

            if (!_isRuntime)
            {
                switch (Type)
                {
                    case "Float": if (float.TryParse(value, out float f)) _field["m_DefaultFloat"].AsFloat = f; break;
                    case "Int": if (int.TryParse(value, out int i)) _field["m_DefaultInt"].AsInt = i; break;
                    case "Bool":
                    case "Trigger": if (bool.TryParse(value, out bool b)) _field["m_DefaultBool"].AsBool = b; break;
                }
            }
            else if (_defaults != null)
            {
                int index = FieldHelper.GetInt(_field, "m_Index");
                var dataField = FieldHelper.GetField(_defaults, "data").IsDummy ? _defaults : FieldHelper.GetField(_defaults, "data");
                
                switch (Type)
                {
                    case "Float": 
                        if (float.TryParse(value, out float f))
                        {
                            var arr = FieldHelper.GetField(dataField, "m_FloatValues");
                            var valField = FieldHelper.GetArrayValue(arr, index);
                            if (valField != null) valField.AsFloat = f;
                        }
                        break;
                    case "Int":
                        if (int.TryParse(value, out int i))
                        {
                            var arr = FieldHelper.GetField(dataField, "m_IntValues");
                            var valField = FieldHelper.GetArrayValue(arr, index);
                            if (valField != null) valField.AsInt = i;
                        }
                        break;
                    case "Bool":
                    case "Trigger":
                        if (bool.TryParse(value, out bool b))
                        {
                            var arr = FieldHelper.GetField(dataField, "m_BoolValues");
                            var valField = FieldHelper.GetArrayValue(arr, index);
                            if (valField != null) valField.AsBool = b;
                        }
                        break;
                }
            }
            NotifyDirty();
        }

        private void NotifyDirty()
        {
            if (_asset != null && _workspace != null && _root != null)
            {
                _asset.UpdateAssetDataAndRow(_workspace, _root);
                WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(_asset));
            }
        }

        private void ParseTypeAndValue(AssetTypeValueField field, int type, AssetTypeValueField? defaults)
        {
            Type = type switch
            {
                1 => "Float",
                3 => "Int",
                4 => "Bool",
                9 => "Trigger",
                _ => $"Type {type}"
            };

            if (defaults != null && !defaults.IsDummy)
            {
                int index = FieldHelper.GetInt(field, "m_Index");
                var dataField = FieldHelper.GetField(defaults, "data").IsDummy ? defaults : FieldHelper.GetField(defaults, "data");
                
                DefaultValue = type switch
                {
                    1 => FieldHelper.GetArrayValue(FieldHelper.GetField(dataField, "m_FloatValues"), index)?.AsFloat.ToString("F3") ?? "-",
                    3 => FieldHelper.GetArrayValue(FieldHelper.GetField(dataField, "m_IntValues"), index)?.AsInt.ToString() ?? "-",
                    4 or 9 => FieldHelper.GetArrayValue(FieldHelper.GetField(dataField, "m_BoolValues"), index)?.AsBool.ToString() ?? "-",
                    _ => "-"
                };
            }
            else
            {
                DefaultValue = type switch
                {
                    1 => FieldHelper.GetFloat(field, "m_DefaultFloat").ToString("F3"),
                    3 => FieldHelper.GetInt(field, "m_DefaultInt").ToString(),
                    4 or 9 => FieldHelper.GetBool(field, "m_DefaultBool").ToString(),
                    _ => "-"
                };
            }
        }
    }

    internal static class FieldHelper
    {
        public static AssetTypeValueField? GetArrayValue(AssetTypeValueField field, int index)
        {
            if (field.IsDummy) return null;
            var array = field["Array"];
            var actualArray = !array.IsDummy ? array : field;
            if (actualArray.Children == null || index < 0 || index >= actualArray.Children.Count) return null;
            return actualArray.Children[index];
        }

        public static AssetTypeValueField GetField(AssetTypeValueField? field, string name)
        {
            if (field == null || field.IsDummy) return new AssetTypeValueField(); // Return a dummy
            return field[name];
        }

        public static long GetLong(AssetTypeValueField field, string name, long defaultValue = 0)
        {
            var f = GetField(field, name);
            return (f.IsDummy || f.Value == null) ? defaultValue : f.AsLong;
        }

        public static int GetInt(AssetTypeValueField field, string name, int defaultValue = 0)
        {
            var f = GetField(field, name);
            return (f.IsDummy || f.Value == null) ? defaultValue : f.AsInt;
        }

        public static float GetFloat(AssetTypeValueField field, string name, float defaultValue = 0)
        {
            var f = GetField(field, name);
            return (f.IsDummy || f.Value == null) ? defaultValue : f.AsFloat;
        }

        public static double GetDouble(AssetTypeValueField field, string name, double defaultValue = 0)
        {
            var f = GetField(field, name);
            // AssetTypeValueField usually has AsFloat, but might not have AsDouble depending on version. 
            // Most Unity positions are floats anyway.
            return (f.IsDummy || f.Value == null) ? defaultValue : (double)f.AsFloat;
        }

        public static bool GetBool(AssetTypeValueField field, string name, bool defaultValue = false)
        {
            var f = GetField(field, name);
            return (f.IsDummy || f.Value == null) ? defaultValue : f.AsBool;
        }

        public static string GetString(AssetTypeValueField field, string name, string defaultValue = "")
        {
            var f = GetField(field, name);
            return (f.IsDummy || f.Value == null) ? defaultValue : f.AsString;
        }

        public static AssetTypeValueField? FindFieldRecursive(AssetTypeValueField field, string name)
        {
            if (field.FieldName == name) return field;
            if (field.Children == null) return null;
            foreach (var child in field.Children)
            {
                var result = FindFieldRecursive(child, name);
                if (result != null) return result;
            }
            return null;
        }

        public static void UpdateTOS(AssetTypeValueField baseField, uint hash, string name)
        {
            if (baseField == null || string.IsNullOrEmpty(name)) return;
            
            var tosField = FindFieldRecursive(baseField, "m_TOS");
            if (tosField == null || tosField.IsDummy)
            {
                tosField = FindFieldRecursive(baseField, "m_TOC");
            }
            if (tosField == null || tosField.IsDummy) return;

            var tosArray = !GetField(tosField, "Array").IsDummy ? GetField(tosField, "Array") : tosField;
            if (tosArray.IsDummy) return;
            
            if (tosArray.Children == null) tosArray.Children = new List<AssetTypeValueField>();
            
            // Check if exists
            foreach (var entry in tosArray.Children)
            {
                var data = GetField(entry, "data").IsDummy ? entry : GetField(entry, "data");
                if (GetLong(data, "first") == (long)hash)
                {
                    var secondField = data["second"];
                    if (!secondField.IsDummy) secondField.AsString = name;
                    return;
                }
            }
            
            // Add new using template
            try
            {
                if (tosArray.TemplateField != null && tosArray.TemplateField.Children != null && tosArray.TemplateField.Children.Count > 1)
                {
                    var template = tosArray.TemplateField.Children[1];
                    var newEntry = ValueBuilder.DefaultValueFieldFromTemplate(template);
                    var entryData = (newEntry == null) ? null : (GetField(newEntry, "data").IsDummy ? newEntry : GetField(newEntry, "data"));
                    if (entryData != null)
                    {
                        var firstField = entryData["first"];
                        if (!firstField.IsDummy) firstField.AsLong = (long)hash;
                        
                        var secondField = entryData["second"];
                        if (!secondField.IsDummy) secondField.AsString = name;
                        
                        tosArray.Children.Add(newEntry);
                        SyncArraySize(tosField);
                    }
                }
            }
            catch { }
        }

        public static void SyncArraySize(AssetTypeValueField arrayField)
        {
            if (arrayField == null || arrayField.IsDummy) return;
            var actualArray = !arrayField["Array"].IsDummy ? arrayField["Array"] : arrayField;
            var sizeField = arrayField["size"];
            if (sizeField.IsDummy) sizeField = actualArray["size"];
            
            if (!sizeField.IsDummy && actualArray.Children != null)
            {
                sizeField.AsInt = actualArray.Children.Count;
            }
        }
    }


    public partial class BodyMaskItem : ObservableObject
    {
        public string PartName { get; }
        public string DisplayName { get; }
        public int Index { get; }
        public uint Word0Mask { get; }
        public uint Word1Mask { get; }
        public uint Word2Mask { get; }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (SetProperty(ref _isEnabled, value))
                {
                    IsEnabledChanged?.Invoke(this);
                }
            }
        }

        public string FillColor => IsEnabled ? "#4CAF50" : "#F44336";

        public event Action<BodyMaskItem>? IsEnabledChanged;

        public BodyMaskItem(string partName, string displayName, int index, uint w0, uint w1, uint w2, bool enabled)
        {
            PartName = partName;
            DisplayName = displayName;
            Index = index;
            Word0Mask = w0;
            Word1Mask = w1;
            Word2Mask = w2;
            _isEnabled = enabled;
        }

        [RelayCommand]
        public void Toggle()
        {
            IsEnabled = !IsEnabled;
            OnPropertyChanged(nameof(FillColor));
        }

        public void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;
            OnPropertyChanged(nameof(IsEnabled));
            OnPropertyChanged(nameof(FillColor));
        }

        // Static definitions for all 13 body parts
        public static readonly (string Name, string Display, uint W0, uint W1, uint W2)[] Definitions = new[]
        {
            ("Root",        "圆盘 (Root)",         0x00000001u, 0x00000000u, 0x00000u),  // 0
            ("Head",        "头部 (Head)",          0x003FFC00u, 0x00000000u, 0x00006u),  // 1
            ("LeftArm",     "左臂 (Left Arm)",      0x00000000u, 0x00FF8000u, 0x78000u),  // 2
            ("Body",        "身体 (Body)",          0x000003FEu, 0xC0000000u, 0x00001u),  // 3
            ("RightArm",    "右臂 (Right Arm)",     0x00000000u, 0x00007FC0u, 0x07800u),  // 4
            ("LeftHand",    "左手 (Left Hand)",     0x00000000u, 0x20000000u, 0x00000u),  // 5
            ("RightHand",   "右手 (Right Hand)",    0x00000000u, 0x10000000u, 0x00000u),  // 6
            ("LeftHandIK",  "左手IK",               0x00000000u, 0x08000000u, 0x00000u),  // 7
            ("RightHandIK", "右手IK",               0x00000000u, 0x04000000u, 0x00000u),  // 8
            ("LeftLeg",     "左腿 (Left Leg)",      0xC0000000u, 0x0000003Fu, 0x00780u),  // 9
            ("RightLeg",    "右腿 (Right Leg)",     0x3FC00000u, 0x00000000u, 0x00078u),  // 10
            ("LeftLegIK",   "左腿IK",               0x00000000u, 0x02000000u, 0x00000u),  // 11
            ("RightLegIK",  "右腿IK",               0x00000000u, 0x01000000u, 0x00000u),  // 12
        };
    }

    public partial class LayerItem : ObservableObject
    {
        [ObservableProperty] private string _name; // Changed from read-only property to ObservableProperty
        private readonly AssetTypeValueField _field;
        public AssetTypeValueField GetField() => _field;
        private readonly Workspace _workspace;
        private readonly AssetsFileInstance _file;
        private int _runtimeStateMachineIndex; // Changed from readonly to allow updates
        private readonly Dictionary<uint, string> _stringTable;
        private readonly AssetTypeValueField? _controllerRoot;
        private readonly AssetInst? _controllerAsset;

        [ObservableProperty] private float _weight;
        [ObservableProperty] private int _blendingMode;
        [ObservableProperty] private bool _iKPass;
        [ObservableProperty] private int _syncedLayerIndex;
        [ObservableProperty] private bool _syncedLayerAffectsTiming;
        [ObservableProperty] private string _maskName = "无 (None) (Avatar Mask)";
        [ObservableProperty] private bool _hasMask;

        public PasswordCrackerViewModel? Parent { get; set; }

        public LayerItem? SyncSource
        {
            get => (SyncedLayerIndex >= 0 && Parent != null && SyncedLayerIndex < Parent.Layers.Count) ? Parent.Layers[SyncedLayerIndex] : null;
            set
            {
                if (Parent != null)
                {
                    SyncedLayerIndex = value != null ? Parent.Layers.IndexOf(value) : -1;
                }
            }
        }

        public string StatusText
        {
            get
            {
                var parts = new List<string>();
                if (HasMask) parts.Add("M");
                if (BlendingMode == 1) parts.Add("A");
                if (IKPass) parts.Add("IK");
                if (IsSynced)
                {
                    if (SyncedLayerAffectsTiming && IsTimingEnabled)
                        parts.Add("S+T");
                    else
                        parts.Add("S");
                }
                return string.Join(" ", parts);
            }
        }

        public bool IsSynced
        {
            get => SyncedLayerIndex != -1;
            set => SyncedLayerIndex = value ? (SyncedLayerIndex <= 0 ? 0 : SyncedLayerIndex) : -1;
        }

        public bool IsTimingEnabled => IsSynced && BlendingMode != 1; // 1 is Additive

        public ObservableCollection<BodyMaskItem> BodyParts { get; } = new();

        public List<string> BlendingModes { get; } = new() { "覆盖 (Override)", "叠加 (Additive)" };

        public LayerItem(AssetTypeValueField field, Workspace workspace, AssetsFileInstance file, AssetInst? controllerAsset = null, AssetTypeValueField? controllerRoot = null)
            : this(FieldHelper.GetString(field, "m_Name"), field, workspace, file, -1, new Dictionary<uint, string>(), controllerRoot, controllerAsset)
        {
        }

        public LayerItem(string name, AssetTypeValueField field, Workspace workspace, AssetsFileInstance file, int runtimeStateMachineIndex, Dictionary<uint, string> stringTable, AssetTypeValueField? controllerRoot, AssetInst? controllerAsset = null, int runtimeSyncedToIndex = -1)
        {
            _field = field;
            _workspace = workspace;
            _file = file;
            _name = name; // Set backing field directly
            _runtimeStateMachineIndex = runtimeStateMachineIndex;
            _stringTable = stringTable;
            _controllerRoot = controllerRoot;
            _controllerAsset = controllerAsset;

            // Initialize properties from fields
            _weight = FieldHelper.GetFloat(_field, "m_DefaultWeight", 1.0f);
            
            // Blending Mode (Editor: m_BlendingMode, Runtime: m_LayerBlendingMode)
            var blendField = _field["m_BlendingMode"];
            if (blendField.IsDummy) blendField = _field["m_LayerBlendingMode"];
            if (blendField.IsDummy) blendField = _field["(int&)m_LayerBlendingMode"];
            _blendingMode = !blendField.IsDummy ? blendField.AsInt : 0;

            _iKPass = FieldHelper.GetBool(_field, "m_IKPass", false);
            _syncedLayerAffectsTiming = FieldHelper.GetBool(_field, "m_SyncedLayerAffectsTiming", false);

            // Sync Logic
            if (_runtimeStateMachineIndex != -1)
            {
                // Runtime (VRCA): Use injected logic based on SM Index reuse
                _syncedLayerIndex = runtimeSyncedToIndex;
            }
            else
            {
                // Editor: m_SyncedLayerIndex = -1 means not synced.
                _syncedLayerIndex = FieldHelper.GetInt(_field, "m_SyncedLayerIndex", -1);
            }

            // Mask resolution
            var maskPtr = FieldHelper.GetField(_field, "m_Mask");
            if (!maskPtr.IsDummy && maskPtr.Value != null && maskPtr["m_PathID"].AsLong != 0)
            {
                long pathId = maskPtr["m_PathID"].AsLong;
                _maskName = $"遮罩 ({pathId})";
                _hasMask = true;
            }
            else
            {
                _hasMask = false;
            }

            // Initialize body mask
            InitBodyMask();
        }

        private string GetFieldName(string name)
        {
            if (_runtimeStateMachineIndex != -1)
            {
                // Runtime mappings
                if (name == "m_SyncedLayerIndex") return "m_StateMachineSynchronizedLayerIndex";
                if (name == "m_BlendingMode") return "m_LayerBlendingMode";
            }
            return name;
        }

        private void InitBodyMask()
        {
            BodyParts.Clear();

            // Read current word values from m_BodyMask
            var bodyMaskField = FieldHelper.GetField(_field, "m_BodyMask");
            uint w0 = 0xFFFFFFFF, w1 = 0xFFFFFFFF, w2 = 0x7FFFF;

            if (!bodyMaskField.IsDummy)
            {
                var w0f = bodyMaskField["word0"];
                var w1f = bodyMaskField["word1"];
                var w2f = bodyMaskField["word2"];
                if (!w0f.IsDummy) w0 = w0f.AsUInt;
                if (!w1f.IsDummy) w1 = w1f.AsUInt;
                if (!w2f.IsDummy) w2 = w2f.AsUInt;
            }

            for (int i = 0; i < BodyMaskItem.Definitions.Length; i++)
            {
                var def = BodyMaskItem.Definitions[i];
                // A part is enabled if ALL its mask bits are set
                bool enabled = true;
                if (def.W0 != 0) enabled &= (w0 & def.W0) == def.W0;
                if (def.W1 != 0) enabled &= (w1 & def.W1) == def.W1;
                if (def.W2 != 0) enabled &= (w2 & def.W2) == def.W2;

                var item = new BodyMaskItem(def.Name, def.Display, i, def.W0, def.W1, def.W2, enabled);
                item.IsEnabledChanged += OnBodyPartToggled;
                BodyParts.Add(item);
            }
        }

        private void OnBodyPartToggled(BodyMaskItem part)
        {
            // Read current masks
            var bodyMaskField = FieldHelper.GetField(_field, "m_BodyMask");
            if (bodyMaskField.IsDummy) return;

            var w0f = bodyMaskField["word0"];
            var w1f = bodyMaskField["word1"];
            var w2f = bodyMaskField["word2"];

            if (part.IsEnabled)
            {
                // Set bits
                if (!w0f.IsDummy && part.Word0Mask != 0) w0f.AsUInt = w0f.AsUInt | part.Word0Mask;
                if (!w1f.IsDummy && part.Word1Mask != 0) w1f.AsUInt = w1f.AsUInt | part.Word1Mask;
                if (!w2f.IsDummy && part.Word2Mask != 0) w2f.AsUInt = w2f.AsUInt | part.Word2Mask;
            }
            else
            {
                // Clear bits
                if (!w0f.IsDummy && part.Word0Mask != 0) w0f.AsUInt = w0f.AsUInt & ~part.Word0Mask;
                if (!w1f.IsDummy && part.Word1Mask != 0) w1f.AsUInt = w1f.AsUInt & ~part.Word1Mask;
                if (!w2f.IsDummy && part.Word2Mask != 0) w2f.AsUInt = w2f.AsUInt & ~part.Word2Mask;
            }

            // Save
            if (_controllerAsset != null)
            {
                var rootField = _controllerRoot ?? _workspace.GetBaseField(_controllerAsset);
                if (rootField != null)
                {
                    _controllerAsset.UpdateAssetDataAndRow(_workspace, rootField);
                    WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(_controllerAsset));
                }
            }
        }

        partial void OnNameChanged(string value)
        {
             if (_runtimeStateMachineIndex != -1)
             {
                 // Runtime: Update Binding and TOS
                 if (_controllerRoot != null)
                 {
                     uint hash = AnimatorHash.GetHash(value);
                     _field["m_Binding"].AsUInt = hash;
                     FieldHelper.UpdateTOS(_controllerRoot, hash, value);
                     
                     // Update local string table if necessary
                     if (!_stringTable.ContainsKey(hash)) _stringTable[hash] = value;
                 }
             }
             else
             {
                 // Editor: Update m_Name
                 UpdateField("m_Name", value);
             }

             // Trigger asset update if likely runtime (UpdateField handles editor)
             if (_runtimeStateMachineIndex != -1 && _controllerAsset != null && _controllerRoot != null)
             {
                 _controllerAsset.UpdateAssetDataAndRow(_workspace, _controllerRoot);
                 WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(_controllerAsset));
             }
        }

        partial void OnWeightChanged(float value) => UpdateField("m_DefaultWeight", value);

        partial void OnSyncedLayerIndexChanged(int value) 
        { 
            // Runtime Handling: We must Manage StateMachines when Sync changes!
            if (_runtimeStateMachineIndex != -1 && Parent != null && _controllerRoot != null)
            {
                int oldSmIndex = _runtimeStateMachineIndex;
                bool changed = false;

                if (value == -1)
                {
                    // Case: Unsync (Become Base Layer)
                    // 1. Create a NEW StateMachine
                    int newSmIndex = CloneStateMachine();
                    
                    // 2. Update Layer Pointers
                    _runtimeStateMachineIndex = newSmIndex;
                    UpdateField("m_StateMachineIndex", newSmIndex);
                    UpdateField("m_StateMachineSynchronizedLayerIndex", 0);
                    changed = true;
                }
                else
                {
                    // Case: Sync to another layer
                    if (value >= 0 && value < Parent.Layers.Count)
                    {
                        var sourceLayer = Parent.Layers[value];
                        // 1. Point to Source's SM
                        int sourceSmIndex = sourceLayer._runtimeStateMachineIndex;
                        
                        if (sourceSmIndex != _runtimeStateMachineIndex)
                        {
                            _runtimeStateMachineIndex = sourceSmIndex;
                            UpdateField("m_StateMachineIndex", sourceSmIndex);
                            changed = true;
                        }
                        
                        // 2. Calculate correct SynchronizedLayerIndex
                        int count = Parent.Layers.Count(l => l._runtimeStateMachineIndex == sourceSmIndex);
                        UpdateField("m_StateMachineSynchronizedLayerIndex", count - 1);
                    }
                }

                if (changed)
                {
                    CleanupStateMachine(oldSmIndex);
                }
                
                // Update Sync property UI
                OnPropertyChanged(nameof(IsSynced));
                OnPropertyChanged(nameof(SyncSource));
                OnPropertyChanged(nameof(IsTimingEnabled));
                OnPropertyChanged(nameof(StatusText));
                Parent.RefreshBaseLayers();
                if (Parent.SelectedLayer == this) Parent.RefreshGraph();
                return; // Done
            }

            // Editor or Fallback
            int saveValue = value;
            if (_runtimeStateMachineIndex != -1)
            {
                saveValue = value >= 0 ? (value + 1) : 0;
            }
            UpdateField("m_SyncedLayerIndex", saveValue); 
            OnPropertyChanged(nameof(IsSynced)); 
            OnPropertyChanged(nameof(SyncSource));
            OnPropertyChanged(nameof(IsTimingEnabled));
            OnPropertyChanged(nameof(StatusText));
            Parent?.RefreshBaseLayers();
            if (Parent?.SelectedLayer == this) Parent.RefreshGraph();
        }

        private void CleanupStateMachine(int smIndex)
        {
            if (Parent == null || _controllerRoot == null) return;
            
            // Check if anyone else uses this SM
            bool isUsed = Parent.Layers.Any(l => l._runtimeStateMachineIndex == smIndex);
            if (!isUsed)
            {
                // Remove SM
                var smArrayField = FieldHelper.FindFieldRecursive(_controllerRoot, "m_StateMachineArray");
                var smArr = !FieldHelper.GetField(smArrayField, "Array").IsDummy ? FieldHelper.GetField(smArrayField, "Array") : smArrayField;

                if (smIndex >= 0 && smIndex < smArr.Children.Count)
                {
                    smArr.Children.RemoveAt(smIndex);
                    if (smArrayField != null) FieldHelper.SyncArraySize(smArrayField);
                    
                    // Shift indices for all layers pointing to higher SMs
                    foreach (var layer in Parent.Layers)
                    {
                        if (layer._runtimeStateMachineIndex > smIndex)
                        {
                            layer._runtimeStateMachineIndex--;
                            layer.UpdateField("m_StateMachineIndex", layer._runtimeStateMachineIndex);
                        }
                    }
                }
            }
        }

        private int CloneStateMachine()
        {
            if (_controllerRoot == null) return -1;
            var smArrayField = FieldHelper.FindFieldRecursive(_controllerRoot, "m_StateMachineArray");
            var smArr = !FieldHelper.GetField(smArrayField, "Array").IsDummy ? FieldHelper.GetField(smArrayField, "Array") : smArrayField;
            
            if (smArr.Children.Count > 0)
            {
                int newIndex = smArr.Children.Count;
                if (smArr.TemplateField != null && smArr.TemplateField.Children.Count > 1)
                {
                    var newSm = ValueBuilder.DefaultValueFieldFromTemplate(smArr.TemplateField.Children[1]);
                    // Ensure arrays are initialized
                    var data = newSm["data"];
                    
                    // Initialize critical arrays
                    if (data["m_StateConstantArray"]["Array"].Children == null) 
                        data["m_StateConstantArray"]["Array"].Children = new List<AssetTypeValueField>();
                    if (data["m_AnyStateTransitionConstantArray"]["Array"].Children == null)
                        data["m_AnyStateTransitionConstantArray"]["Array"].Children = new List<AssetTypeValueField>();
                        
                    // Initialize Entry State (Selector)
                    var selectorArr = data["m_SelectorStateConstantArray"]["Array"];
                    if (selectorArr.Children == null) selectorArr.Children = new List<AssetTypeValueField>();
                    
                    // We definitely need Entry/Exit nodes in Selector array for it to be valid in Unity
                    // We will COPY them from SM[0]
                    var templateSelector = smArr.Children[0]["data"]["m_SelectorStateConstantArray"]["Array"];
                    if (templateSelector.Children != null)
                    {
                        // We can't deep copy easily. 
                        // Let's just rely on Empty SM for now. Unity usually regenerates Entry/Exit if missing?
                        // Or maybe we can just create a basic one.
                        // For parity: The user wants it to be a specific valid base layer.
                        // Let's Copy the first SM's SelectorArray (it's small, usually 2 items: Entry, Exit).
                        // Since we can't Clone, let's just leave it empty. 
                        // Most tools handle empty SMs by showing nothing.
                    }
                    
                    smArr.Children.Add(newSm);
                    if (smArrayField != null) FieldHelper.SyncArraySize(smArrayField);
                    return newIndex;
                }
            }
            return -1;
        }
        partial void OnBlendingModeChanged(int value)
        {
            UpdateField("m_BlendingMode", value);
            OnPropertyChanged(nameof(IsTimingEnabled));
            OnPropertyChanged(nameof(StatusText));
        }

        partial void OnIKPassChanged(bool value)
        {
            UpdateField("m_IKPass", value);
            OnPropertyChanged(nameof(StatusText));
        }

        partial void OnSyncedLayerAffectsTimingChanged(bool value)
        {
            UpdateField("m_SyncedLayerAffectsTiming", value);
            OnPropertyChanged(nameof(StatusText));
        }

        partial void OnHasMaskChanged(bool value) => OnPropertyChanged(nameof(StatusText));

        private void UpdateField<T>(string name, T value)
        {
            // Removed check for _runtimeStateMachineIndex != -1 to allow editing runtime controllers
            var fieldName = GetFieldName(name);
            var f = _field[fieldName];
            
            // Special handling if runtime blending mode was not found by GetFieldName or if we want to be safe
            if (f.IsDummy && name == "m_BlendingMode")
            {
                // Try fallback logic if default mapping failed (e.g. if GetFieldName didn't cover it or different version)
                f = _field["m_LayerBlendingMode"];
                if (f.IsDummy) f = _field["(int&)m_LayerBlendingMode"];
                if (f.IsDummy) f = _field["m_BlendingMode"];
            }

            if (!f.IsDummy)
            {
                if (value is float fl) f.AsFloat = fl;
                else if (value is int i) f.AsInt = i;
                else if (value is bool b) f.AsBool = b;
                
                if (_controllerAsset != null)
                {
                    var rootField = _controllerRoot ?? _workspace.GetBaseField(_controllerAsset);
                    if (rootField != null)
                    {
                        _controllerAsset.UpdateAssetDataAndRow(_workspace, rootField);
                        WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(_controllerAsset));
                    }
                }
            }
        }
 
         public StateItem? CreateState(double x, double y, Dictionary<long, string> animationMap, ObservableCollection<ParameterItem> availableParameters, Action<string>? logAction = null)
         {
             if (_workspace == null) return null;
 
             if (_runtimeStateMachineIndex != -1 && _controllerRoot != null && _controllerAsset != null)
             {
                 // Runtime Format
                 logAction?.Invoke("Creating new state in runtime controller...");
 
                 var controller = FieldHelper.GetField(_controllerRoot, "m_Controller");
                 var smArray = !controller.IsDummy ? FieldHelper.GetField(controller, "m_StateMachineArray") : FieldHelper.GetField(_controllerRoot, "m_StateMachineArray");
                 var smArr = !FieldHelper.GetField(smArray, "Array").IsDummy ? FieldHelper.GetField(smArray, "Array") : smArray;
                 
                 if (smArr.Children == null || _runtimeStateMachineIndex >= smArr.Children.Count) return null;
                 var smChild = smArr.Children[_runtimeStateMachineIndex];
                 var smField = FieldHelper.GetField(smChild, "data").IsDummy ? smChild : FieldHelper.GetField(smChild, "data");
 
                  // 1. Calculate Hashes
                  string stateName = "New State";
                  uint stateNameHash = AnimatorHash.GetHash(stateName);
                  string pathName = string.IsNullOrEmpty(Name) ? stateName : $"{Name}.{stateName}";
                  uint pathHash = AnimatorHash.GetHash(pathName);

                  // 2. Find TOS and Add Entries
                  var tos = FieldHelper.GetField(_controllerRoot, "m_TOS");
                  var tosArr = !FieldHelper.GetField(tos, "Array").IsDummy ? FieldHelper.GetField(tos, "Array") : tos;
                  
                  Action<uint, string> addToTos = (id, name) => 
                  {
                      if (_stringTable.ContainsKey(id)) return;
                      try {
                          var entryTemplate = tosArr.TemplateField.Children[1];
                          var newEntry = ValueBuilder.DefaultValueFieldFromTemplate(entryTemplate);
                          var entryData = FieldHelper.GetField(newEntry, "data").IsDummy ? newEntry : FieldHelper.GetField(newEntry, "data");
                          entryData["first"].AsLong = id;
                          entryData["second"].AsString = name;
                          if (tosArr.Children == null) tosArr.Children = new List<AssetTypeValueField>();
                          tosArr.Children.Add(newEntry);
                          _stringTable[id] = name;
                      } catch { }
                  };

                  Action<AssetTypeValueField> syncSize = (arr) => FieldHelper.SyncArraySize(arr);

                  addToTos(stateNameHash, stateName);
                  addToTos(pathHash, pathName);

                  // 3. Add to m_StateConstantArray
                  var stateArrField = FieldHelper.GetField(smField, "m_StateConstantArray");
                  var stateArr = !FieldHelper.GetField(stateArrField, "Array").IsDummy ? FieldHelper.GetField(stateArrField, "Array") : stateArrField;
                  
                  try
                  {
                      var stateTemplate = stateArr.TemplateField.Children[1];
                      var newStateConst = ValueBuilder.DefaultValueFieldFromTemplate(stateTemplate);
                      var data = FieldHelper.GetField(newStateConst, "data").IsDummy ? newStateConst : FieldHelper.GetField(newStateConst, "data");
                      
                      var idField = data["m_NameID"];
                      if (idField.IsDummy) idField = data["m_ID"];
                      if (!idField.IsDummy) idField.AsLong = (long)stateNameHash;

                      var pathIdField = data["m_PathID"];
                      if (!pathIdField.IsDummy) pathIdField.AsLong = (long)pathHash;

                      var fullPathIdField = data["m_FullPathID"];
                      if (!fullPathIdField.IsDummy) fullPathIdField.AsLong = (long)pathHash;
                      
                      data["m_Speed"].AsFloat = 1.0f;
                      data["m_IKOnFeet"].AsBool = false;
                      data["m_WriteDefaultValues"].AsBool = true;
                      data["m_Loop"].AsBool = true;
                      data["m_Mirror"].AsBool = false;
                      data["m_CycleOffset"].AsFloat = 0.0f;

                      // Initialize m_TransitionConstantArray
                      var transArrField = FieldHelper.GetField(data, "m_TransitionConstantArray");
                      var transArr = !FieldHelper.GetField(transArrField, "Array").IsDummy ? FieldHelper.GetField(transArrField, "Array") : transArrField;
                      if (transArr.Children == null) transArr.Children = new List<AssetTypeValueField>();

                      // Initialize m_BlendTreeConstantArray
                      var btArrField = FieldHelper.GetField(data, "m_BlendTreeConstantArray");
                      var btArr = !FieldHelper.GetField(btArrField, "Array").IsDummy ? FieldHelper.GetField(btArrField, "Array") : btArrField;
                      if (btArr.Children == null) btArr.Children = new List<AssetTypeValueField>();
                      
                      // Handle m_BlendTreeConstantIndexArray
                      var btIndices = FieldHelper.GetField(data, "m_BlendTreeConstantIndexArray");
                      var btIndicesArr = !FieldHelper.GetField(btIndices, "Array").IsDummy ? FieldHelper.GetField(btIndices, "Array") : btIndices;
                      
                      try
                      {
                          var indexTemplate = btIndicesArr.TemplateField.Children[1];
                          var newIndex = ValueBuilder.DefaultValueFieldFromTemplate(indexTemplate);
                          newIndex.AsInt = -1; // Default to None
                          if (btIndicesArr.Children == null) btIndicesArr.Children = new List<AssetTypeValueField>();
                          btIndicesArr.Children.Add(newIndex);
                      }
                      catch 
                      {
                          logAction?.Invoke("Warning: Could not add index to m_BlendTreeConstantIndexArray.");
                      }

                      if (stateArr.Children == null) stateArr.Children = new List<AssetTypeValueField>();
                      stateArr.Children.Add(newStateConst);
                      syncSize(stateArrField);
                      syncSize(tos);
                      
                      int newIdx = stateArr.Children.Count - 1;
                      
                      // Mark Dirty
                      _controllerAsset.UpdateAssetDataAndRow(_workspace, _controllerRoot);
                      WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(_controllerAsset));

                      var newItem = new StateItem(stateName, x, y, newIdx, _stringTable, availableParameters, Name);
                      newItem.MotionName = "None";
                      newItem.MotionPathId = 0;
                      newItem._workspace = _workspace;
                      newItem._controllerAsset = _controllerAsset;
                      newItem._runtimeControllerRoot = _controllerRoot;
                      newItem._sourceField = data;
                      newItem._runtimeNameId = stateNameHash;
                      newItem._runtimeBlendTreeIndices = btIndices;
                      
                      return newItem;
                  }
                 catch (Exception ex)
                 {
                     logAction?.Invoke($"Failed to add to state array: {ex.Message}");
                     return null;
                 }
             }
 
             return null;
         }
 
         public List<StateItem> GetStates(Dictionary<long, string> animationMap, Dictionary<string, string> paramTypes, ObservableCollection<ParameterItem>? availableParameters = null, Action<string>? logAction = null)
        {
            var list = new List<StateItem>();
            AssetTypeValueField smField;
            long defaultStateId = 0;

            if (_runtimeStateMachineIndex != -1)
            {
                // Runtime format
                if (_controllerRoot == null) return list;

                var controller = FieldHelper.GetField(_controllerRoot, "m_Controller");
                var smArray = !controller.IsDummy ? FieldHelper.GetField(controller, "m_StateMachineArray") : FieldHelper.GetField(_controllerRoot, "m_StateMachineArray");
                
                var arrayField = FieldHelper.GetField(smArray, "Array");
                var actualArray = !arrayField.IsDummy ? arrayField : smArray;
                if (actualArray.IsDummy || actualArray.Children == null || _runtimeStateMachineIndex >= actualArray.Children.Count) return list;

                var smChild = actualArray.Children[_runtimeStateMachineIndex];
                smField = FieldHelper.GetField(smChild, "data").IsDummy ? smChild : FieldHelper.GetField(smChild, "data");
                
                defaultStateId = FieldHelper.GetLong(smField, "m_DefaultState");

                // Virtual nodes (using absolute positions from the constant data if available)
                AddVirtualNodes(list, smField, _stringTable, true, paramTypes, availableParameters, _workspace, _controllerAsset, _controllerRoot);

                // Real states
                var stateArrays = new[] { "m_StateConstantArray", "m_SelectorStateConstantArray" };
                foreach (var arrayName in stateArrays)
                {
                    var statesArr = smField[arrayName];
                    var actualStates = !statesArr["Array"].IsDummy ? statesArr["Array"] : statesArr;
                    if (actualStates.Children != null)
                    {
                        for (int i = 0; i < actualStates.Children.Count; i++)
                        {
                            var stateData = FieldHelper.GetField(actualStates.Children[i], "data").IsDummy ? actualStates.Children[i] : FieldHelper.GetField(actualStates.Children[i], "data");
                            uint nameId = (uint)FieldHelper.GetLong(stateData, "m_NameID");
                            string stateName = _stringTable.TryGetValue(nameId, out var n) ? n : $"State_{i}";
                            if (arrayName == "m_SelectorStateConstantArray") stateName = $"[Sel] {stateName}";
                            
                            // IDs for runtime:
                            // m_StateConstantArray: 0, 1, 2...
                            // m_SelectorStateConstantArray: 40000 + i (just for uniqueness in the list, though transitions usually don't point here directly)
                            long id = arrayName == "m_StateConstantArray" ? i : 40000 + i;

                            // Runtime states don't have positions in the same way. 
                            double x = (list.Count % 5) * 200.0;
                            double y = (list.Count / 5) * 100.0;
                            
                            var item = new StateItem(stateName, x, y, id, _stringTable, availableParameters, Name);
                            item._workspace = _workspace;
                            item._controllerAsset = _controllerAsset;
                            item._runtimeControllerRoot = _controllerRoot;
                            item._sourceField = stateData;
                            item._runtimeNameId = nameId;
                            
                            if (arrayName == "m_SelectorStateConstantArray")
                            {
                                item.IsSelector = true;
                                item.IsEntrySelector = FieldHelper.GetField(stateData, "m_IsEntry").AsBool;
                            }

                            item.ParseRuntimeTransitions(FieldHelper.GetField(stateData, "m_TransitionConstantArray"), _stringTable, paramTypes, availableParameters, item.IsEntrySelector);
                            
                            // Try resolve motion for runtime
                            item.ResolveRuntimeMotion(stateData, _controllerRoot!, animationMap, logAction);
                            
                            // Parse extra properties for runtime (set backing fields to avoid redundant updates)
                            item._speed = (decimal)FieldHelper.GetFloat(stateData, "m_Speed", 1.0f);
                            item._ikOnFeet = FieldHelper.GetBool(stateData, "m_IKOnFeet");
                            item._writeDefaultValues = FieldHelper.GetBool(stateData, "m_WriteDefaultValues");
                            item._loop = FieldHelper.GetBool(stateData, "m_Loop");
                            item._mirror = FieldHelper.GetBool(stateData, "m_Mirror");
                            item._cycleOffset = (decimal)FieldHelper.GetFloat(stateData, "m_CycleOffset");
                            uint tagId = (uint)FieldHelper.GetLong(stateData, "m_TagID");
                            if (tagId != 0 && _stringTable.TryGetValue(tagId, out var tag)) item._tag = tag;

                            if (i == defaultStateId && arrayName == "m_StateConstantArray") item.IsDefault = true;
                            list.Add(item);
                        }
                    }
                }

                // Resolve names for transitions now that all states are in the list
                foreach (var state in list)
                {
                    state.ResolveTransitionNames(list);
                }
            }
            else
            {
                // Editor format (original logic preserved)
// ...
                // Editor format

                var smPtr = FieldHelper.GetField(_field, "m_StateMachine");
                if (smPtr.IsDummy || smPtr.Value == null) return list;
                
                var smAsset = _workspace.Manager.GetExtAsset(_file, smPtr);
                if (smAsset.baseField == null) return list;

                smField = smAsset.baseField;
                var defStateField = FieldHelper.GetField(smField, "m_DefaultState");
                defaultStateId = (defStateField.Children != null && defStateField.Children.Count > 0) ? 
                    defStateField.Children[defStateField.Children.Count - 1].AsLong : 0;

                AddVirtualNodes(list, smField, _stringTable, false, paramTypes, availableParameters);

                // Real states
                var statesField = FieldHelper.GetField(smField, "m_States");
                var statesArr = FieldHelper.GetField(statesField, "m_States.Array");
                if (statesArr.Children != null)
                {
                    foreach (var stateWrapper in statesArr.Children)
                    {
                        var statePtr = FieldHelper.GetField(stateWrapper, "m_State");
                        var pos = FieldHelper.GetField(stateWrapper, "m_Position");
                        var stateExt = _workspace.Manager.GetExtAsset(_file, statePtr);
                        if (stateExt.baseField != null)
                        {
                            double x = FieldHelper.GetDouble(pos, "x");
                            double y = FieldHelper.GetDouble(pos, "y");
                            var item = new StateItem(stateExt, x, y, _workspace, _file, animationMap, paramTypes, availableParameters);
                            item._workspace = _workspace;
                            item._sourceAsset = stateExt.info is AssetInst ai ? ai : _workspace.GetAssetInst(stateExt.file, 0, stateExt.info.PathId);
                            item._sourceField = stateExt.baseField;
                            item._controllerAsset = _controllerAsset;

                            if (item.PathId == defaultStateId) item.IsDefault = true;
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        private void AddVirtualNodes(List<StateItem> list, AssetTypeValueField smField, Dictionary<uint, string> stringTable, bool isRuntime, Dictionary<string, string> paramTypes, ObservableCollection<ParameterItem>? availableParameters = null, Workspace? workspace = null, AssetInst? controllerAsset = null, AssetTypeValueField? controllerRoot = null)
        {
            var anyPos = FieldHelper.GetField(smField, "m_AnyStatePosition");
            double ax = 0, ay = -100;
            if (!anyPos.IsDummy)
            {
                ax = FieldHelper.GetField(anyPos, "x").IsDummy ? ax : (double)FieldHelper.GetField(anyPos, "x").AsFloat;
                ay = FieldHelper.GetField(anyPos, "y").IsDummy ? ay : (double)FieldHelper.GetField(anyPos, "y").AsFloat;
            }
            // ID for Any State in runtime is usually 0xFFFFFFFF (as uint) -> -1
            var anyStateItem = new StateItem("Any State", ax, ay, -1, stringTable, availableParameters);
            anyStateItem._workspace = workspace;
            anyStateItem._controllerAsset = controllerAsset;
            anyStateItem._runtimeControllerRoot = controllerRoot;
            anyStateItem._sourceField = smField; // For Any State, source field is the StateMachine root itself
            list.Add(anyStateItem);

            var entryPos = FieldHelper.GetField(smField, "m_EntryPosition");
            double ex = 0, ey = 0;
            if (!entryPos.IsDummy)
            {
                ex = FieldHelper.GetField(entryPos, "x").IsDummy ? ex : (double)FieldHelper.GetField(entryPos, "x").AsFloat;
                ey = FieldHelper.GetField(entryPos, "y").IsDummy ? ey : (double)FieldHelper.GetField(entryPos, "y").AsFloat;
            }
            // ID for Entry in runtime is usually 30000
            var entryStateItem = new StateItem("Entry", ex, ey, isRuntime ? 30000 : -2, stringTable, availableParameters);
            entryStateItem._workspace = workspace;
            entryStateItem._controllerAsset = controllerAsset;
            entryStateItem._runtimeControllerRoot = controllerRoot;
            entryStateItem._sourceField = smField; // For Entry, source field is the StateMachine root
            list.Add(entryStateItem);

            var exitPos = FieldHelper.GetField(smField, "m_ExitPosition");
            double xx = 400, xy = 0;
            if (!exitPos.IsDummy)
            {
                xx = FieldHelper.GetField(exitPos, "x").IsDummy ? xx : (double)FieldHelper.GetField(exitPos, "x").AsFloat;
                xy = FieldHelper.GetField(exitPos, "y").IsDummy ? xy : (double)FieldHelper.GetField(exitPos, "y").AsFloat;
            }
            // ID for Exit in runtime is usually 30001
            var exitStateItem = new StateItem("Exit", xx, xy, isRuntime ? 30001 : -3, stringTable, availableParameters);
            exitStateItem._workspace = workspace;
            exitStateItem._controllerAsset = controllerAsset;
            exitStateItem._runtimeControllerRoot = controllerRoot;
            list.Add(exitStateItem);

            if (isRuntime)
            {
                anyStateItem.ParseRuntimeTransitions(FieldHelper.GetField(smField, "m_AnyStateTransitionConstantArray"), stringTable, paramTypes, availableParameters);
                // Entry transitions in runtime are often in m_SelectorStateConstantArray
                var selectorArray = FieldHelper.GetField(smField, "m_SelectorStateConstantArray");
                var actualSelector = !FieldHelper.GetField(selectorArray, "Array").IsDummy ? FieldHelper.GetField(selectorArray, "Array") : selectorArray;
                if (actualSelector.Children != null)
                {
                    foreach (var selector in actualSelector.Children)
                    {
                        var data = FieldHelper.GetField(selector, "data");
                        if (FieldHelper.GetField(data, "m_IsEntry").AsBool)
                        {
                            entryStateItem.ParseRuntimeTransitions(FieldHelper.GetField(data, "m_TransitionConstantArray"), stringTable, paramTypes, availableParameters, true);
                        }
                    }
                }
            }
            else
            {
                var anyTransitions = ParseTransitions(FieldHelper.GetField(smField, "m_AnyStateTransitions"), false, paramTypes, availableParameters, false);
                foreach (var t in anyTransitions) anyStateItem.Transitions.Add(t);

                var entryTransitions = ParseTransitions(FieldHelper.GetField(smField, "m_EntryTransitions"), false, paramTypes, availableParameters, true);
                foreach (var t in entryTransitions) entryStateItem.Transitions.Add(t);
            }
        }

        public List<TransitionItem> ParseTransitions(AssetTypeValueField field, bool isRuntime, Dictionary<string, string> paramTypes, ObservableCollection<ParameterItem>? availableParameters = null, bool isEntry = false)
        {
            var list = new List<TransitionItem>();
            var transField = field; // The input 'field' is already the m_Transitions or m_AnyStateTransitions field
            var transArray = !FieldHelper.GetField(transField, "Array").IsDummy ? FieldHelper.GetField(transField, "Array") : transField;
            if (transArray.Children != null)
            {
                foreach (var transData in transArray.Children)
                {
                    var data = FieldHelper.GetField(transData, "data").IsDummy ? transData : FieldHelper.GetField(transData, "data");
                    list.Add(new TransitionItem(data, _stringTable, paramTypes, isRuntime, _workspace, _controllerAsset, _controllerRoot, availableParameters, isEntry));
                }
            }
            return list;
        }
    }

     public partial class StateItem : ObservableObject
     {
         public event Action<StateItem>? NameChanged;
         [ObservableProperty] private string _name = "";
         partial void OnNameChanged(string value)
         {
             foreach (var t in Transitions) t.SourceName = value;
             NameChanged?.Invoke(this);
             if (_workspace == null || _sourceField == null) return;

             try
             {
                 if (_sourceAsset != null)
                 {
                     // Editor Format: AnimatorState asset
                     _sourceField["m_Name"].AsString = value;
                     _sourceAsset.UpdateAssetDataAndRow(_workspace, _sourceField);
                     WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(_sourceAsset));
                 }
                  else if (_runtimeControllerRoot != null && _controllerAsset != null)
                 {
                     // Runtime Format: Update m_TOS in the controller
                     var tosField = FieldHelper.GetField(_runtimeControllerRoot, "m_TOS");
                     var tosArray = !FieldHelper.GetField(tosField, "Array").IsDummy ? FieldHelper.GetField(tosField, "Array") : tosField;

                     uint newHash = AnimatorHash.GetHash(value);

                     if (tosArray.Children != null)
                     {
                         foreach (var tosEntry in tosArray.Children)
                         {
                             var data = FieldHelper.GetField(tosEntry, "data").IsDummy ? tosEntry : FieldHelper.GetField(tosEntry, "data");
                              if (FieldHelper.GetLong(data, "first") == _runtimeNameId)
                              {
                                  var fField = data["first"];
                                  if (!fField.IsDummy) fField.AsLong = (long)newHash;
                                  
                                  var sField = data["second"];
                                  if (!sField.IsDummy) sField.AsString = value;
                                  break;
                              }
                         }
                     }

                     // Update hashes in the state constant too
                      if (_sourceField != null)
                      {
                          var idField = _sourceField["m_NameID"];
                          if (idField.IsDummy) idField = _sourceField["m_ID"];
                          if (!idField.IsDummy) idField.AsLong = (long)newHash;
                         
                         string pathName = string.IsNullOrEmpty(_layerName) ? value : $"{_layerName}.{value}";
                         uint pathHash = AnimatorHash.GetHash(pathName);
                         
                         _sourceField["m_PathID"].AsLong = pathHash;
                         _sourceField["m_FullPathID"].AsLong = pathHash;
                     }

                     _runtimeNameId = newHash;
                     _controllerAsset.UpdateAssetDataAndRow(_workspace, _runtimeControllerRoot);
                     WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(_controllerAsset));
                 }
             }
             catch (Exception ex)
             {
                 System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] OnNameChanged Error: {ex.Message}");
             }
         }
        [ObservableProperty] private double _x;
        [ObservableProperty] private double _y;
        public long PathId { get; }
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BackgroundColor))]
        private bool _isDefault;
        [ObservableProperty] private bool _isSelector;
        [ObservableProperty] private bool _isEntrySelector;

        public string BackgroundColor => Name.ToLower() switch
        {
            "any state" => "#417e79",
            "entry" => "#2d5d30",
            "exit" => "#812d2d",
            _ => IsDefault ? "#996611" : "#3c3c3c"
        };
        public ObservableCollection<TransitionItem> Transitions { get; } = new();

        public void AddTransitionTo(StateItem target)
        {
            if (_workspace == null || _sourceField == null || _controllerAsset == null || _runtimeControllerRoot == null) return;
            
            try
            {
                AssetTypeValueField containerField;
                AssetTypeValueField transArray;
                bool isSelectorSource = false;

                // 1. Find correct transition array based on source type
                if (PathId == -1) // Any State
                {
                    containerField = FieldHelper.GetField(_sourceField, "m_AnyStateTransitionConstantArray");
                    transArray = !FieldHelper.GetField(containerField, "Array").IsDummy ? FieldHelper.GetField(containerField, "Array") : containerField;
                }
                else if (PathId == 30000) // Entry
                {
                    var selectorArray = FieldHelper.GetField(_sourceField, "m_SelectorStateConstantArray");
                    var actualSelector = !FieldHelper.GetField(selectorArray, "Array").IsDummy ? FieldHelper.GetField(selectorArray, "Array") : selectorArray;
                    AssetTypeValueField? entrySelector = null;
                    if (actualSelector.Children != null)
                    {
                        foreach (var selector in actualSelector.Children)
                        {
                            var sData = FieldHelper.GetField(selector, "data").IsDummy ? selector : FieldHelper.GetField(selector, "data");
                            if (FieldHelper.GetField(sData, "m_IsEntry").AsBool) { entrySelector = sData; break; }
                        }
                    }

                    if (entrySelector == null) throw new Exception("Entry selector not found.");
                    
                    containerField = FieldHelper.GetField(entrySelector, "m_TransitionConstantArray");
                    transArray = !FieldHelper.GetField(containerField, "Array").IsDummy ? FieldHelper.GetField(containerField, "Array") : containerField;
                    isSelectorSource = true;
                }
                else // Normal State or [Sel] State
                {
                    containerField = FieldHelper.GetField(_sourceField, "m_TransitionConstantArray");
                    transArray = !FieldHelper.GetField(containerField, "Array").IsDummy ? FieldHelper.GetField(containerField, "Array") : containerField;
                    // Check if the source itself is a selector (starts with [Sel])
                    if (Name.StartsWith("[Sel]")) isSelectorSource = true;
                }

                if (transArray.IsDummy) throw new Exception("Transition array not found.");

                // 2. Determine names and hashes
                string transName = $"{this.Name} -> {target.Name}";
                string fullPathName = string.IsNullOrEmpty(_layerName) ? transName : $"{_layerName}.{this.Name} -> {_layerName}.{target.Name}";
                
                uint id = AnimatorHash.GetHash(transName);
                uint fullPathId = AnimatorHash.GetHash(fullPathName);
                
                // 3. Update Table of Strings (TOS)
                UpdateTOS(id, transName);
                UpdateTOS(fullPathId, fullPathName);

                // 4. Create transition field from template
                if (transArray.TemplateField == null || transArray.TemplateField.Children == null || transArray.TemplateField.Children.Count < 2)
                {
                    throw new Exception("m_TransitionConstantArray template is missing or invalid.");
                }

                var transTemplate = transArray.TemplateField.Children[1];
                var newTrans = ValueBuilder.DefaultValueFieldFromTemplate(transTemplate);
                var data = (newTrans == null) ? null : (FieldHelper.GetField(newTrans, "data").IsDummy ? newTrans : FieldHelper.GetField(newTrans, "data"));
                
                if (data == null) throw new Exception("Failed to create transition data from template.");

                // 5. Set Required IDs and Indices
                if (isSelectorSource)
                {
                    var dstField = FieldHelper.GetField(data, "m_Destination");
                    if (!dstField.IsDummy) dstField.AsLong = target.PathId; 
                }
                else
                {
                    var idField = FieldHelper.GetField(data, "m_ID");
                    if (!idField.IsDummy) idField.AsUInt = id;

                    var fullPathIdField = FieldHelper.GetField(data, "m_FullPathID");
                    if (!fullPathIdField.IsDummy) fullPathIdField.AsUInt = fullPathId;

                    var dstStateField = FieldHelper.GetField(data, "m_DestinationState");
                    if (!dstStateField.IsDummy) dstStateField.AsLong = target.PathId; 

                    var isExitField = FieldHelper.GetField(data, "m_IsExit");
                    if (!isExitField.IsDummy) isExitField.AsBool = target.Name == "Exit" || target.PathId == 30001;
                    
                    // Set Default Behaviors
                    var durField = FieldHelper.GetField(data, "m_TransitionDuration");
                    if (!durField.IsDummy) durField.AsFloat = 0.25f;

                    var offField = FieldHelper.GetField(data, "m_TransitionOffset");
                    if (!offField.IsDummy) offField.AsFloat = 0.0f;

                    var extField = FieldHelper.GetField(data, "m_ExitTime");
                    if (!extField.IsDummy) extField.AsFloat = 0.75f;

                    var hasExtField = FieldHelper.GetField(data, "m_HasExitTime");
                    if (!hasExtField.IsDummy) hasExtField.AsBool = false;

                    var hasFixField = FieldHelper.GetField(data, "m_HasFixedDuration");
                    if (!hasFixField.IsDummy) hasFixField.AsBool = true;

                    var intSrcField = FieldHelper.GetField(data, "m_InterruptionSource");
                    if (!intSrcField.IsDummy) intSrcField.AsInt = 0;

                    var ordIntField = FieldHelper.GetField(data, "m_OrderedInterruption");
                    if (!ordIntField.IsDummy) ordIntField.AsBool = true;

                    var canSelfField = FieldHelper.GetField(data, "m_CanTransitionToSelf");
                    if (!canSelfField.IsDummy) canSelfField.AsBool = true;
                }
                
                // 6. Initialize empty condition array
                var condArrField = FieldHelper.GetField(data, "m_ConditionConstantArray");
                if (!condArrField.IsDummy)
                {
                    var condArr = !FieldHelper.GetField(condArrField, "Array").IsDummy ? FieldHelper.GetField(condArrField, "Array") : condArrField;
                    if (condArr.Children == null) condArr.Children = new List<AssetTypeValueField>();
                }

                if (transArray.Children == null) transArray.Children = new List<AssetTypeValueField>();
                transArray.Children.Add(newTrans);
                
                FieldHelper.SyncArraySize(containerField);

                // 7. Save and Notify
                _controllerAsset.UpdateAssetDataAndRow(_workspace, _runtimeControllerRoot);
                WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(_controllerAsset));

                // 8. Update local UI list
                var item = new TransitionItem(data, _stringTable, new Dictionary<string, string>(), true, _workspace, _controllerAsset, _runtimeControllerRoot, null, IsEntrySelector || Name == "Entry" || isSelectorSource);
                item.SourceName = this.Name;
                item.DestinationName = target.Name;
                Transitions.Add(item);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] AddTransitionTo Error: {ex.Message}");
            }
        }

        private void UpdateTOS(uint hash, string name)
        {
            if (_runtimeControllerRoot == null || string.IsNullOrEmpty(name)) return;
            
            var tosField = FieldHelper.GetField(_runtimeControllerRoot, "m_TOS");
            var tosArray = !FieldHelper.GetField(tosField, "Array").IsDummy ? FieldHelper.GetField(tosField, "Array") : tosField;
            
            if (tosArray.Children == null) tosArray.Children = new List<AssetTypeValueField>();
            
            // Check if exists
            foreach (var entry in tosArray.Children)
            {
                var data = FieldHelper.GetField(entry, "data").IsDummy ? entry : FieldHelper.GetField(entry, "data");
                if (FieldHelper.GetLong(data, "first") == (long)hash) return;
            }
            
            // Add new using template
            try
            {
                if (tosArray.TemplateField != null && tosArray.TemplateField.Children != null && tosArray.TemplateField.Children.Count > 1)
                {
                    var template = tosArray.TemplateField.Children[1];
                    var newEntry = ValueBuilder.DefaultValueFieldFromTemplate(template);
                    var entryData = (newEntry == null) ? null : (FieldHelper.GetField(newEntry, "data").IsDummy ? newEntry : FieldHelper.GetField(newEntry, "data"));
                    if (entryData != null)
                    {
                        entryData["first"].AsLong = (long)hash;
                        entryData["second"].AsString = name;
                        tosArray.Children.Add(newEntry);
                        FieldHelper.SyncArraySize(tosField);
                    }
                }
            }
            catch { }
        }

        public void ResolveTransitionNames(List<StateItem> allStates)
        {
            bool defaultLinkFound = false;
            foreach (var trans in Transitions)
            {
                trans.SourceName = this.Name;
                if (trans.DstPathId == -1) // Any State
                {
                    trans.DestinationName = "Any State";
                    trans.IsDefaultLink = false;
                }
                else if (trans.DstPathId == 30000) // Entry
                {
                    trans.DestinationName = "Entry";
                    trans.IsDefaultLink = false;
                }
                else if (trans.DstPathId == 30001) // Exit
                {
                    trans.DestinationName = "Exit";
                    trans.IsDefaultLink = false;
                }
                else
                {
                    var target = allStates.FirstOrDefault(s => s.PathId == trans.DstPathId);
                    if (target != null)
                    {
                        trans.DestinationName = target.Name;
                        // Only the FIRST unconditional link from an entry-type source to the default state is the "Default Link"
                        if (trans.IsEntry && target.IsDefault && trans.ConditionList.Count == 0 && !defaultLinkFound)
                        {
                            trans.IsDefaultLink = true;
                            defaultLinkFound = true;
                        }
                        else
                        {
                            trans.IsDefaultLink = false;
                        }
                    }
                    else
                    {
                        trans.DestinationName = $"[idx: {trans.DstPathId}]";
                        trans.IsDefaultLink = false;
                    }
                }
            }
        }

        [ObservableProperty] private string _motionName = "None";
        public long MotionPathId { get; set; }

        internal decimal _speed = 1.0m;
        public decimal Speed 
        { 
            get => _speed; 
            set { if (SetProperty(ref _speed, value)) NotifyStateDirty(); } 
        }

        internal string _tag = "";
        public string Tag 
        { 
            get => _tag; 
            set { if (SetProperty(ref _tag, value)) NotifyStateDirty(); } 
        }

        internal bool _mirror;
        public bool Mirror 
        { 
            get => _mirror; 
            set { if (SetProperty(ref _mirror, value)) NotifyStateDirty(); } 
        }

        internal bool _ikOnFeet;
        public bool IKOnFeet 
        { 
            get => _ikOnFeet; 
            set { if (SetProperty(ref _ikOnFeet, value)) NotifyStateDirty(); } 
        }

        internal bool _writeDefaultValues;
        public bool WriteDefaultValues 
        { 
            get => _writeDefaultValues; 
            set { if (SetProperty(ref _writeDefaultValues, value)) NotifyStateDirty(); } 
        }

        internal bool _loop;
        public bool Loop 
        { 
            get => _loop; 
            set { if (SetProperty(ref _loop, value)) NotifyStateDirty(); } 
        }

        internal decimal _cycleOffset;
        public decimal CycleOffset 
        { 
            get => _cycleOffset; 
            set { if (SetProperty(ref _cycleOffset, value)) NotifyStateDirty(); } 
        }

        // --- Data for Editing ---
        internal Workspace? _workspace;
        internal AssetInst? _sourceAsset;      // AnimatorState (Editor)
        internal AssetTypeValueField? _sourceField; // State Field (Editor or Runtime)
        internal AssetInst? _controllerAsset;  // Main Controller asset (required for marking dirty)
        internal AssetTypeValueField? _runtimeControllerRoot; // Controller Root (Runtime)
        internal AssetTypeValueField? _runtimeBlendTreeIndices; // Index array for BlendTrees (Runtime)
        internal uint _runtimeNameId = 0; // ID in TOS (Runtime)
        internal string? _layerName; // Layer Name for hash recalculation
        internal int _runtimeClipInfoIndex = -1; // Index in m_ClipID (Runtime)
        // ------------------------

        public string SpeedParam { get; set; } = "";
        public string MirrorParam { get; set; } = "";
        public string CycleOffsetParam { get; set; } = "";
        public string TimeParam { get; set; } = "";

        private void NotifyStateDirty()
        {
            if (_workspace == null || _sourceField == null) return;
            
            bool isRuntime = _runtimeControllerRoot != null;
            
            _sourceField["m_Speed"].AsFloat = (float)Speed;
            _sourceField["m_IKOnFeet"].AsBool = IKOnFeet;
            _sourceField["m_WriteDefaultValues"].AsBool = WriteDefaultValues;
            _sourceField["m_Mirror"].AsBool = Mirror;
            _sourceField["m_Loop"].AsBool = Loop;
            _sourceField["m_CycleOffset"].AsFloat = (float)CycleOffset;

            if (isRuntime)
            {
                // Tag in runtime is a hash (m_TagID), we can't easily re-hash here,
                // but we can try to update m_Name if it's the state name.
                // For now, let's just update the numeric fields which are most important.
            }
            else
            {
                _sourceField["m_Tag"].AsString = Tag;
            }

            if (_sourceAsset != null)
            {
                _sourceAsset.UpdateAssetDataAndRow(_workspace, _sourceField);
                WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(_sourceAsset));
                System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] Updated Editor State Asset. Bytes: {_sourceField.WriteToByteArray().Length}");
            }
            else if (_controllerAsset != null && _runtimeControllerRoot != null)
            {
                _controllerAsset.UpdateAssetDataAndRow(_workspace, _runtimeControllerRoot);
                WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(_controllerAsset));
                System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] Updated Runtime Controller Asset. Bytes: {_runtimeControllerRoot.WriteToByteArray().Length}");
            }
        }

        private readonly Dictionary<uint, string> _stringTable;

        public event Action<StateItem>? PositionChanged;

        partial void OnXChanged(double value) => PositionChanged?.Invoke(this);
        partial void OnYChanged(double value) => PositionChanged?.Invoke(this);

        public StateItem(string name, double x, double y, long pathId, Dictionary<uint, string> stringTable, ObservableCollection<ParameterItem>? availableParameters = null, string? layerName = null)
        {
            Name = name;
            X = x;
            Y = y;
            PathId = pathId;
            _stringTable = stringTable;
            _layerName = layerName;
        }

        public StateItem(AssetExternal ext, double x, double y, Workspace workspace, AssetsFileInstance file, Dictionary<long, string> animationMap, Dictionary<string, string> paramTypes, ObservableCollection<ParameterItem>? availableParameters = null)
            : this(FieldHelper.GetString(ext.baseField, "m_Name"), x, y, ext.info.PathId, new Dictionary<uint, string>(), availableParameters)
        {
            _workspace = workspace;
            var field = ext.baseField;
            if (field != null)
            {
                Speed = (decimal)FieldHelper.GetFloat(field, "m_Speed", 1.0f);
                Tag = FieldHelper.GetString(field, "m_Tag");
                IKOnFeet = FieldHelper.GetBool(field, "m_IKOnFeet");
                WriteDefaultValues = FieldHelper.GetBool(field, "m_WriteDefaultValues");
                Mirror = FieldHelper.GetBool(field, "m_Mirror");
                Loop = FieldHelper.GetBool(field, "m_Loop");
                CycleOffset = (decimal)FieldHelper.GetFloat(field, "m_CycleOffset");
                
                var motion = FieldHelper.GetField(field, "m_Motion");
                if (!motion.IsDummy)
                {
                    long motionPathId = motion["m_PathID"].AsLong;
                    MotionPathId = motionPathId;
                    if (motionPathId != 0 && animationMap.TryGetValue(motionPathId, out var resolvedName))
                    {
                        MotionName = resolvedName;
                    }
                    else
                    {
                        MotionName = FieldHelper.GetString(motion, "m_Name", "None");
                    }
                }
            }

            var transitionsField = FieldHelper.GetField(field, "m_Transitions");
            var transitions = FieldHelper.GetField(transitionsField, "m_Transitions.Array");
            if (transitions.Children != null)
            {
                foreach (var transPtr in transitions.Children)
                {
                    var transAsset = workspace.Manager.GetExtAsset(file, transPtr);
                    if (transAsset.baseField != null)
                    {
                        var transInst = workspace.GetAssetInst(transAsset.file, 0, transAsset.info.PathId);
                        Transitions.Add(new TransitionItem(transAsset.baseField, _stringTable, paramTypes, false, _workspace, transInst, transAsset.baseField, availableParameters));
                    }
                }
            }
        }

        [ObservableProperty] private MotionSelectionItem? _currentMotion;
        partial void OnCurrentMotionChanged(MotionSelectionItem? value)
        {
            // Use PathId comparison to ensure we catch changes even if fallback names are the same
            if (value != null && (value.PathId != MotionPathId || value.Name != MotionName)) 
                UpdateMotion(value);
        }

        public void UpdateMotion(MotionSelectionItem motion)
        {
            if (_workspace == null) return;
            
            System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] Attempting to update motion for {Name} to {motion.Name} (PathID: {motion.PathId})");

            try
            {
                if (_sourceAsset != null && _sourceField != null)
                {
                    // Editor Format: AnimatorState asset
                    var motionField = _sourceField["m_Motion"];
                    if (!motionField.IsDummy)
                    {
                        // Try to resolve correct FileID if it's an external clip
                        int fileId = 0;
                        if (motion.Asset != null && motion.Asset.FileInstance != _sourceAsset.FileInstance)
                        {
                            var externals = _sourceAsset.FileInstance.file.Metadata.Externals;
                            for (int i = 0; i < externals.Count; i++)
                            {
                                if (externals[i].PathName.Contains(motion.Asset.FileInstance.name, StringComparison.OrdinalIgnoreCase))
                                {
                                    fileId = i + 1;
                                    break;
                                }
                            }
                        }
                        
                        motionField["m_FileID"].AsInt = fileId; 
                        motionField["m_PathID"].AsLong = motion.PathId;
                        
                        _sourceAsset.UpdateAssetDataAndRow(_workspace, _sourceField);
                        WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(_sourceAsset));
                        
                        MotionName = motion.Name;
                        MotionPathId = motion.PathId;
                        System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] Editor state motion updated to {motion.Name} (FileID: {fileId}, PathID: {motion.PathId})");
                    }
                }
                else if (_runtimeControllerRoot != null && _sourceField != null && _controllerAsset != null)
                {
                    // Runtime Format
                    if (motion.Name == "None" && motion.PathId == 0)
                    {
                        if (_runtimeBlendTreeIndices != null && !_runtimeBlendTreeIndices.IsDummy)
                        {
                            var indices = !FieldHelper.GetField(_runtimeBlendTreeIndices, "Array").IsDummy ? FieldHelper.GetField(_runtimeBlendTreeIndices, "Array") : _runtimeBlendTreeIndices;
                            if (indices.Children != null && indices.Children.Count > 0)
                            {
                                indices.Children[0].AsInt = -1;
                                _controllerAsset.UpdateAssetDataAndRow(_workspace, _runtimeControllerRoot);
                                WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(_controllerAsset));
                                MotionName = "None";
                                MotionPathId = 0;
                                return;
                            }
                        }
                    }

                    var clipsField = FieldHelper.GetField(_runtimeControllerRoot, "m_AnimationClips");
                    var clipsArray = !FieldHelper.GetField(clipsField, "Array").IsDummy ? FieldHelper.GetField(clipsField, "Array") : clipsField;
                    
                    if (clipsArray.IsDummy || clipsArray.Children == null)
                    {
                        var mController = FieldHelper.GetField(_runtimeControllerRoot, "m_Controller");
                        clipsField = FieldHelper.GetField(mController, "m_AnimationClips");
                        clipsArray = !FieldHelper.GetField(clipsField, "Array").IsDummy ? FieldHelper.GetField(clipsField, "Array") : clipsField;
                    }

                    int foundIndex = -1;
                    if (clipsArray.Children != null)
                    {
                        for (int i = 0; i < clipsArray.Children.Count; i++)
                        {
                            var clipPtr = clipsArray.Children[i];
                            var clipData = FieldHelper.GetField(clipPtr, "data").IsDummy ? clipPtr : FieldHelper.GetField(clipPtr, "data");
                            if (FieldHelper.GetLong(clipData, "m_PathID") == motion.PathId)
                            {
                                foundIndex = i;
                                break;
                            }
                        }
                    }

                    if (foundIndex == -1 && motion.PathId != 0 && !clipsArray.IsDummy)
                    {
                        // Clip not in controller's list - Try to add it
                        System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] Clip {motion.Name} ({motion.PathId}) not in m_AnimationClips. Adding...");
                        try 
                        {
                            var template = clipsArray.TemplateField.Children[1];
                            var newPtr = ValueBuilder.DefaultValueFieldFromTemplate(template);
                            var data = FieldHelper.GetField(newPtr, "data").IsDummy ? newPtr : FieldHelper.GetField(newPtr, "data");
                            data["m_FileID"].AsInt = 0;
                            data["m_PathID"].AsLong = motion.PathId;
                            
                            if (clipsArray.Children == null) clipsArray.Children = new List<AssetTypeValueField>();
                            clipsArray.Children.Add(newPtr);
                            foundIndex = clipsArray.Children.Count - 1;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] Failed to add clip to m_AnimationClips: {ex.Message}");
                        }
                    }

                    if (foundIndex != -1)
                    {
                        bool updated = false;

                        // Check if we are currently at 'None' (index -1)
                        if (_runtimeBlendTreeIndices != null && !_runtimeBlendTreeIndices.IsDummy)
                        {
                            var indices = !FieldHelper.GetField(_runtimeBlendTreeIndices, "Array").IsDummy ? FieldHelper.GetField(_runtimeBlendTreeIndices, "Array") : _runtimeBlendTreeIndices;
                            if (indices.Children != null && indices.Children.Count > 0 && indices.Children[0].AsInt == -1)
                            {
                                // Restore from None: must set index to 0 and ensure BlendTree exists
                                var btArrayField = FieldHelper.GetField(_sourceField, "m_BlendTreeConstantArray");
                                var btArray = !FieldHelper.GetField(btArrayField, "Array").IsDummy ? FieldHelper.GetField(btArrayField, "Array") : btArrayField;
                                
                                if (btArray.Children == null || btArray.Children.Count == 0)
                                {
                                    // Inject a new BlendTree entry
                                    System.Diagnostics.Debug.WriteLine("[AnimatorAnalysis] Injecting BlendTree to m_BlendTreeConstantArray...");
                                    try
                                    {
                                        var btTemplate = btArray.TemplateField.Children[1];
                                        var newBt = ValueBuilder.DefaultValueFieldFromTemplate(btTemplate);
                                        var btData = FieldHelper.GetField(newBt, "data").IsDummy ? newBt : FieldHelper.GetField(newBt, "data");
                                        
                                        // Ensure m_NodeArray has at least one node
                                        var nodesField = FieldHelper.GetField(btData, "m_NodeArray");
                                        var nodesArray = !FieldHelper.GetField(nodesField, "Array").IsDummy ? FieldHelper.GetField(nodesField, "Array") : nodesField;
                                        
                                        var nodeTemplate = nodesArray.TemplateField.Children[1];
                                        var newNode = ValueBuilder.DefaultValueFieldFromTemplate(nodeTemplate);
                                        var nodeData = FieldHelper.GetField(newNode, "data").IsDummy ? newNode : FieldHelper.GetField(newNode, "data");
                                        nodeData["m_ClipID"].AsInt = foundIndex;
                                        
                                        if (nodesArray.Children == null) nodesArray.Children = new List<AssetTypeValueField>();
                                        nodesArray.Children.Add(newNode);
                                        
                                        if (btArray.Children == null) btArray.Children = new List<AssetTypeValueField>();
                                        btArray.Children.Add(newBt);
                                        
                                        indices.Children[0].AsInt = 0;
                                        updated = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] Failed to inject BlendTree: {ex.Message}");
                                    }
                                }
                                else
                                {
                                    indices.Children[0].AsInt = 0; // Just point back to the first one
                                }
                            }
                        }

                        if (!updated)
                        {
                            var clipIdField = _sourceField["m_ClipID"];
                            if (!clipIdField.IsDummy)
                            {
                                clipIdField.AsInt = foundIndex;
                                updated = true;
                            }

                            if (!updated)
                            {
                                var btArray = FieldHelper.GetField(_sourceField, "m_BlendTreeConstantArray");
                                var btActual = !FieldHelper.GetField(btArray, "Array").IsDummy ? FieldHelper.GetField(btArray, "Array") : btArray;
                                
                                if (btActual.Children != null && btActual.Children.Count > 0)
                                {
                                    var btData = FieldHelper.GetField(btActual.Children[0], "data").IsDummy ? btActual.Children[0] : FieldHelper.GetField(btActual.Children[0], "data");
                                    var nodesField = FieldHelper.GetField(btData, "m_NodeArray");
                                    var nodesArray = !FieldHelper.GetField(nodesField, "Array").IsDummy ? FieldHelper.GetField(nodesField, "Array") : nodesField;

                                    if (nodesArray.Children != null && nodesArray.Children.Count > 0)
                                    {
                                        var nodeData = FieldHelper.GetField(nodesArray.Children[0], "data").IsDummy ? nodesArray.Children[0] : FieldHelper.GetField(nodesArray.Children[0], "data");
                                        nodeData["m_ClipID"].AsInt = foundIndex;
                                        updated = true;
                                    }
                                }
                            }
                        }

                        if (updated)
                        {
                            _controllerAsset.UpdateAssetDataAndRow(_workspace, _runtimeControllerRoot);
                            WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(_controllerAsset));
                            MotionName = motion.Name;
                            MotionPathId = motion.PathId;
                            System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] Runtime state motion updated successfully to {motion.Name}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] UpdateMotion Error: {ex.Message}");
            }
        }

        public void ParseRuntimeTransitions(AssetTypeValueField transArrayField, Dictionary<uint, string> stringTable, Dictionary<string, string> paramTypes, ObservableCollection<ParameterItem>? availableParameters = null, bool isEntry = false)
        {
            var actualArray = !transArrayField.IsDummy && !transArrayField["Array"].IsDummy ? transArrayField["Array"] : transArrayField;
            if (actualArray.IsDummy || actualArray.Children == null) return;
            Transitions.Clear();
            foreach (var transField in actualArray.Children)
            {
                var data = FieldHelper.GetField(transField, "data").IsDummy ? transField : FieldHelper.GetField(transField, "data");
                Transitions.Add(new TransitionItem(data, stringTable, paramTypes, true, _workspace, _controllerAsset, _runtimeControllerRoot, availableParameters, isEntry || this.Name == "Entry" || this.IsEntrySelector || (this.Name != null && this.Name.StartsWith("[Sel]"))));
            }
        }

        public void ResolveRuntimeMotion(AssetTypeValueField stateData, AssetTypeValueField controllerRoot, Dictionary<long, string> animationMap, Action<string>? logAction = null)
        {
            if (stateData.IsDummy || controllerRoot.IsDummy) return;
            
            _runtimeBlendTreeIndices = FieldHelper.GetField(stateData, "m_BlendTreeConstantIndexArray");

            string stateName = "Unknown";
            var nameField = FieldHelper.GetField(stateData, "m_Name");
            if (!nameField.IsDummy) stateName = nameField.AsString;
            logAction?.Invoke($"Resolving motion for state: {stateName}");

            var indexArray = !FieldHelper.GetField(_runtimeBlendTreeIndices, "Array").IsDummy ? FieldHelper.GetField(_runtimeBlendTreeIndices, "Array") : _runtimeBlendTreeIndices;
            if (indexArray.Children != null && indexArray.Children.Count > 0 && indexArray.Children[0].AsInt == -1)
            {
                logAction?.Invoke("State is set to 'None' (index -1)");
                MotionName = "None";
                MotionPathId = 0;
                return;
            }

            var blendTreeArray = FieldHelper.GetField(stateData, "m_BlendTreeConstantArray");
            var blendTreeEntries = FieldHelper.GetField(blendTreeArray, "Array");
            var actualBlendTrees = !blendTreeEntries.IsDummy ? blendTreeEntries : blendTreeArray;

            if (actualBlendTrees.IsDummy || actualBlendTrees.Children == null || actualBlendTrees.Children.Count == 0)
            {
                logAction?.Invoke("No BlendTree nodes found for this state.");
                return;
            }

            // Find m_AnimationClips - It can be at root or inside m_Controller
            var clipsField = FieldHelper.GetField(controllerRoot, "m_AnimationClips");
            var clipsArray = FieldHelper.GetField(clipsField, "Array");
            
            if (clipsField.IsDummy || (clipsArray.IsDummy && (clipsField.Children == null || clipsField.Children.Count == 0)))
            {
                logAction?.Invoke("m_AnimationClips not found at root, checking m_Controller...");
                var mController = FieldHelper.GetField(controllerRoot, "m_Controller");
                clipsField = FieldHelper.GetField(mController, "m_AnimationClips");
                clipsArray = FieldHelper.GetField(clipsField, "Array");
            }

            var actualClips = !clipsArray.IsDummy ? clipsArray : clipsField;
            if (actualClips.IsDummy || actualClips.Children == null)
            {
                logAction?.Invoke("m_AnimationClips array still not found.");
                return;
            }

            logAction?.Invoke($"Scanning {actualClips.Children.Count} clips in m_AnimationClips.");

            // Recursively search all nodes in all BlendTrees
            foreach (var btField in actualBlendTrees.Children)
            {
                var btData = FieldHelper.GetField(btField, "data").IsDummy ? btField : FieldHelper.GetField(btField, "data");
                var nodesField = FieldHelper.GetField(btData, "m_NodeArray");
                var nodesArray = FieldHelper.GetField(nodesField, "Array");
                var actualNodes = !nodesArray.IsDummy ? nodesArray : nodesField;
                
                if (actualNodes.IsDummy || actualNodes.Children == null)
                {
                    logAction?.Invoke("No nodes in this BlendTree entry.");
                    continue;
                }

                foreach (var node in actualNodes.Children)
                {
                    var nodeData = FieldHelper.GetField(node, "data").IsDummy ? node : FieldHelper.GetField(node, "data");
                    int clipId = FieldHelper.GetInt(nodeData, "m_ClipID", -1);
                    logAction?.Invoke($"Checking node with ClipID: {clipId}");

                    if (clipId != -1 && clipId < actualClips.Children.Count)
                    {
                        var clipPointer = actualClips.Children[clipId];
                        var clipData = FieldHelper.GetField(clipPointer, "data").IsDummy ? clipPointer : FieldHelper.GetField(clipPointer, "data");
                        long pathId = FieldHelper.GetLong(clipData, "m_PathID");
                        MotionPathId = pathId;
                        
                        logAction?.Invoke($"ClipID {clipId} -> PathID {pathId}");

                        if (animationMap.TryGetValue(pathId, out var resolvedName))
                        {
                            MotionName = resolvedName;
                            logAction?.Invoke($"Successfully resolved to: {resolvedName}");
                            return; // Found one, stop
                        }
                        else
                        {
                            logAction?.Invoke($"PathID {pathId} NOT in map. Map count: {animationMap.Count}");
                        }
                    }
                }
            }
            logAction?.Invoke("Motion resolution failed.");
        }
    }

    public partial class ConditionInfo : ObservableObject
    {
        [ObservableProperty] private bool _isSelected;
        private readonly AssetTypeValueField _field;
        private readonly Dictionary<string, string> _paramTypes;
        private readonly Action _onChanged;
        private readonly bool _isRuntime;
        private readonly ObservableCollection<ParameterItem>? _availableParameters;
        public ObservableCollection<ParameterItem>? AvailableParameters => _availableParameters;

        private string _paramName = "";
        public string ParamName 
        { 
            get => _paramName; 
            set 
            {
                // UI Guard: Prevent clearing the value when switching states or re-binding
                if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(_paramName)) return;
                
                if (SetProperty(ref _paramName, value)) HandleParamNameChanged(value); 
            } 
        }

        private string _mode = "";
        public string Mode 
        { 
            get => _mode; 
            set { if (SetProperty(ref _mode, value)) HandleModeChanged(value); } 
        }

        private string _threshold = "";
        public string Threshold 
        { 
            get => _threshold; 
            set { if (SetProperty(ref _threshold, value)) HandleThresholdChanged(value); } 
        }

        private string _type = "";
        public string Type 
        { 
            get => _type; 
            set 
            {
                if (SetProperty(ref _type, value))
                {
                    OnPropertyChanged(nameof(AvailableModes));
                    HasMode = value != "Trigger";
                    HasThreshold = value == "Int" || value == "Float" || value == "Unknown";
                }
            }
        }

        private bool _hasMode;
        public bool HasMode { get => _hasMode; set => SetProperty(ref _hasMode, value); }

        private bool _hasThreshold;
        public bool HasThreshold { get => _hasThreshold; set => SetProperty(ref _hasThreshold, value); }

        public List<string> AvailableModes => Type switch
        {
            "Bool" => new List<string> { "true", "false" },
            "Int" => new List<string> { "Greater", "Less", "Equals", "NotEqual" },
            "Float" => new List<string> { "Greater", "Less" },
            _ => new List<string> { "true", "false", "Greater", "Less", "Equals", "NotEqual" }
        };

        public ConditionInfo(AssetTypeValueField field, string paramName, string mode, string threshold, string type, Dictionary<string, string> paramTypes, Action onChanged, bool isRuntime = false, ObservableCollection<ParameterItem>? availableParameters = null)
        {
            _field = field;
            _paramTypes = paramTypes;
            _onChanged = onChanged;
            _isRuntime = isRuntime;
            _availableParameters = availableParameters;
            _paramName = paramName;
            _mode = mode;
            _threshold = threshold;
            _type = type;
            HasMode = type != "Trigger";
            HasThreshold = type == "Int" || type == "Float" || type == "Unknown";
        }

        private void HandleParamNameChanged(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            string cleanName = value.Trim();
            string oldType = Type;
            string newType = "Unknown";
            if (_availableParameters != null)
            {
                var p = _availableParameters.FirstOrDefault(x => x.Name == cleanName);
                if (p != null) newType = p.Type;
            }
            
            if (newType == "Unknown" && _paramTypes.TryGetValue(cleanName, out var pt))
            {
                newType = pt;
            }

            Type = newType;
            
            if (newType != oldType)
            {
                Mode = newType switch
                {
                    "Bool" => "true",
                    "Int" => "Greater",
                    "Float" => "Greater",
                    _ => Mode
                };
            }

            if (_isRuntime)
            {
                if (_availableParameters != null)
                {
                    var param = _availableParameters.FirstOrDefault(p => p.Name == cleanName);
                    if (param != null)
                    {
                    _field["m_EventID"].AsLong = (long)param.ParamId;
                }
            }
        }
        else
        {
            _field["m_ConditionParam"].AsString = cleanName;
        }
        _onChanged?.Invoke();
    }

        private void HandleModeChanged(string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            int unityMode = Type switch
            {
                "Bool" => value == "true" ? 1 : 2,
                "Int" => value switch { "Greater" => 3, "Less" => 4, "Equals" => 6, "NotEqual" => 7, _ => 3 },
                "Float" => value == "Greater" ? 3 : 4,
                _ => value switch { "true" => 1, "false" => 2, "Greater" => 3, "Less" => 4, "Equals" => 6, "NotEqual" => 7, _ => 1 }
            };

            _field["m_ConditionMode"].AsInt = unityMode;
            _onChanged?.Invoke();
        }

        private void HandleThresholdChanged(string value)
        {
            if (float.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var val))
            {
                _field["m_EventThreshold"].AsFloat = val;
                _onChanged?.Invoke();
            }
        }
        public void NotifyParamChanged()
        {
            OnPropertyChanged(nameof(ParamName));
            OnPropertyChanged(nameof(AvailableParameters));
        }

        public AssetTypeValueField GetField() => _field;
    }

    public partial class TransitionItem : ObservableObject
    {
        public long DstPathId { get; }
        [ObservableProperty] 
        [NotifyPropertyChangedFor(nameof(DisplayName))]
        private string _sourceName = "";
        [ObservableProperty] 
        [NotifyPropertyChangedFor(nameof(DisplayName))]
        private string _destinationName = "";
        public string DisplayName => $"{SourceName} -> {DestinationName}";

        public AssetTypeValueField GetField() => _field;

        [RelayCommand]
        public void Remove()
        {
            WeakReferenceMessenger.Default.Send(new DeleteTransitionRequest(this));
        }

        [ObservableProperty] private bool _solo;
        partial void OnSoloChanged(bool value) => NotifyDirty();
        [ObservableProperty] private bool _mute;
        partial void OnMuteChanged(bool value) => NotifyDirty();

        [ObservableProperty] 
        [NotifyPropertyChangedFor(nameof(IsNormalTransition))]
        private bool _isEntry;
        
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNormalTransition))]
        private bool _isDefaultLink;

        public bool IsNormalTransition => !IsEntry || !IsDefaultLink || ConditionList.Count > 0 || (SourceName != "Entry" && !SourceName.StartsWith("[Sel]"));

        private string _conditions = "";
        public string Conditions { get => _conditions; set => SetProperty(ref _conditions, value); }

        public ObservableCollection<ConditionInfo> ConditionList { get; } = new();

        private readonly Workspace? _workspace;
        private readonly AssetInst? _ownerAsset;
        private readonly AssetTypeValueField? _rootField;
        private readonly AssetTypeValueField _field;
        private readonly bool _isRuntime;
        private readonly Dictionary<uint, string> _stringTable;
        private readonly Dictionary<string, string> _paramTypes;
        private readonly ObservableCollection<ParameterItem>? _availableParameters;

        public TransitionItem(AssetTypeValueField field, Dictionary<uint, string> stringTable, Dictionary<string, string> paramTypes, bool isRuntime = false, 
            Workspace? workspace = null, AssetInst? ownerAsset = null, AssetTypeValueField? rootField = null,
            ObservableCollection<ParameterItem>? availableParameters = null, bool isEntry = false)
        {
            _isEntry = isEntry;
            _workspace = workspace;
            _ownerAsset = ownerAsset;
            _rootField = rootField;
            _field = field;
            _isRuntime = isRuntime;
            _stringTable = stringTable ?? new();
            _paramTypes = paramTypes ?? new();
            _availableParameters = availableParameters;

            WeakReferenceMessenger.Default.Register<ParameterRenamedMessage>(this, (r, m) => 
            {
                bool changed = false;
                foreach (var cond in ConditionList)
                {
                    if (_isRuntime)
                    {
                        if (m.OldHash != 0 && AnimatorHash.GetHash(cond.ParamName) == m.OldHash)
                        {
                            cond.ParamName = m.NewName;
                            changed = true;
                        }
                    }
                    else
                    {
                        if (m.OldName != null && cond.ParamName == m.OldName)
                        {
                            cond.ParamName = m.NewName;
                            changed = true;
                        }
                    }
                }
                if (changed) UpdateConditionsString();
            });

            if (field.IsDummy) return;

            if (isRuntime)
            {
                var dstField = FieldHelper.GetField(field, "m_DestinationState");
                if (dstField.IsDummy) dstField = FieldHelper.GetField(field, "m_Destination");
                
                long dst = dstField.AsLong;
                // Handle 0xFFFFFFFF (Any State) correctly
                if (dst == 4294967295L) dst = -1;
                DstPathId = dst;
            }
            else
            {
                var dstState = FieldHelper.GetField(field, "m_DstState");
                if (!dstState.IsDummy && dstState.Children != null && dstState.Children.Count > 0)
                    DstPathId = dstState.Children[dstState.Children.Count - 1].AsLong;
            }

            ReloadConditions();
        }

        public void ReloadConditions()
        {
            if (_field.IsDummy) return;

            Solo = FieldHelper.GetBool(_field, _isRuntime ? "m_Solo" : "m_Solo");
            Mute = FieldHelper.GetBool(_field, _isRuntime ? "m_Mute" : "m_Mute");

            var condsField = FieldHelper.GetField(_field, _isRuntime ? "m_ConditionConstantArray" : "m_Conditions");
            if (condsField.IsDummy) return;

            var condsArray = !FieldHelper.GetField(condsField, "Array").IsDummy ? FieldHelper.GetField(condsField, "Array") : condsField;
            if (condsArray.Children == null) return;

            ConditionList.Clear();
            foreach (var condWrap in condsArray.Children)
            {
                var cond = FieldHelper.GetField(condWrap, "data").IsDummy ? condWrap : FieldHelper.GetField(condWrap, "data");
                
                string paramName = "Unknown";
                if (_isRuntime)
                {
                    uint eventId = (uint)FieldHelper.GetLong(cond, "m_EventID");
                    if (_stringTable != null && _stringTable.TryGetValue(eventId, out var n))
                        paramName = n;
                    else
                        paramName = $"Hash_{eventId:X8}";
                }
                else
                {
                    paramName = FieldHelper.GetString(cond, "m_ConditionParam") ?? "Unknown";
                }
                
                if (paramName == null) paramName = "Unknown";

                string paramType = "Unknown";
                if (_availableParameters != null)
                {
                    var p = _availableParameters.FirstOrDefault(x => x.Name == paramName.Trim());
                    if (p != null) paramType = p.Type;
                }
                
                if (paramType == "Unknown" && _paramTypes != null)
                {
                    if (_paramTypes.TryGetValue(paramName.Trim(), out string? pt) && pt != null)
                        paramType = pt;
                }
                int mode = FieldHelper.GetInt(cond, "m_ConditionMode");
                float threshold = FieldHelper.GetFloat(cond, "m_EventThreshold");

                string pMode = "";
                string pThreshold = "";

                if (paramType == "Bool")
                {
                    pMode = mode == 1 ? "true" : "false";
                }
                else if (paramType == "Int")
                {
                    pMode = mode switch { 3 => "Greater", 4 => "Less", 6 => "Equals", 7 => "NotEqual", _ => mode.ToString() };
                    pThreshold = ((int)threshold).ToString();
                }
                else if (paramType == "Float")
                {
                    pMode = mode switch { 3 => "Greater", 4 => "Less", _ => mode.ToString() };
                    pThreshold = threshold.ToString("F3");
                }
                else if (paramType == "Trigger")
                {
                }
                else
                {
                    pMode = mode switch { 1 => "true", 2 => "false", 3 => "Greater", 4 => "Less", 6 => "Equals", 7 => "NotEqual", _ => mode.ToString() };
                    pThreshold = threshold.ToString();
                }
                
                ConditionList.Add(new ConditionInfo(cond, paramName, pMode, pThreshold, paramType, _paramTypes ?? new(), NotifyDirty, _isRuntime, _availableParameters));
            }
            UpdateConditionsString();
        }

        [RelayCommand]
        public void AddCondition()
        {
            if (_field.IsDummy) return;
            var condsField = FieldHelper.GetField(_field, _isRuntime ? "m_ConditionConstantArray" : "m_Conditions");
            var condsArray = !FieldHelper.GetField(condsField, "Array").IsDummy ? FieldHelper.GetField(condsField, "Array") : condsField;
            
            if (condsArray.Children == null) condsArray.Children = new List<AssetTypeValueField>();
            
            AssetTypeValueField? newCond = null;
            if (condsArray.TemplateField != null && condsArray.TemplateField.Children != null && condsArray.TemplateField.Children.Count > 1)
            {
                newCond = ValueBuilder.DefaultValueFieldFromTemplate(condsArray.TemplateField.Children[1]);
            }

            if (newCond != null)
            {
                condsArray.Children.Add(newCond);
                FieldHelper.SyncArraySize(condsField);
                var data = FieldHelper.GetField(newCond, "data").IsDummy ? newCond : FieldHelper.GetField(newCond, "data");
                
                // Initialize with some default
                string defParam = "Unknown";
                string defType = "Bool";

                if (_availableParameters != null && _availableParameters.Count > 0)
                {
                    defParam = _availableParameters[0].Name;
                    defType = _availableParameters[0].Type;
                }
                else if (_paramTypes.Count > 0)
                {
                    defParam = _paramTypes.Keys.FirstOrDefault() ?? "Unknown";
                    defType = _paramTypes.TryGetValue(defParam, out var t) ? t : "Bool";
                }
                
                if (_isRuntime)
                {
                    var param = _availableParameters?.FirstOrDefault(p => p.Name == defParam);
                    data["m_EventID"].AsLong = (long)(param?.ParamId ?? 0);
                }
                else
                {
                    data["m_ConditionParam"].AsString = defParam;
                }
                
                var info = new ConditionInfo(data, defParam, defType == "Bool" ? "true" : "Greater", "0", defType, _paramTypes, NotifyDirty, _isRuntime, _availableParameters);
                ConditionList.Add(info);
                NotifyDirty();
            }
        }

        [RelayCommand]
        public void RemoveCondition()
        {
            if (_field.IsDummy) return;
            var toRemove = ConditionList.Where(c => c.IsSelected).ToList();
            if (toRemove.Count == 0 && ConditionList.Count > 0) toRemove.Add(ConditionList.Last()); // Remove last if nothing selected

            if (toRemove.Count > 0)
            {
                var condsField = FieldHelper.GetField(_field, _isRuntime ? "m_ConditionConstantArray" : "m_Conditions");
                var condsArray = !FieldHelper.GetField(condsField, "Array").IsDummy ? FieldHelper.GetField(condsField, "Array") : condsField;

                // Sort by index descending to avoid index shift issues
                var itemsWithIndex = toRemove.Select(item => new { Item = item, Index = ConditionList.IndexOf(item) })
                                           .Where(x => x.Index >= 0)
                                           .OrderByDescending(x => x.Index)
                                           .ToList();
                
                foreach (var x in itemsWithIndex)
                {
                    if (x.Index < condsArray.Children.Count)
                    {
                        condsArray.Children.RemoveAt(x.Index);
                        ConditionList.RemoveAt(x.Index);
                    }
                }
                FieldHelper.SyncArraySize(condsField);
                NotifyDirty();
            }
        }

        private void UpdateConditionsString()
        {
            var condList = new List<string>();
            foreach (var cond in ConditionList)
            {
                if (cond.Type == "Bool")
                {
                    condList.Add($"{cond.ParamName} is {cond.Mode}");
                }
                else if (cond.Type == "Trigger")
                {
                    condList.Add(cond.ParamName);
                }
                else if (cond.HasMode || cond.HasThreshold)
                {
                    condList.Add($"{cond.ParamName} {cond.Mode} {cond.Threshold}");
                }
                else
                {
                    condList.Add(cond.ParamName);
                }
            }
            Conditions = string.Join(" && ", condList);
        }

        private void NotifyDirty()
        {
            UpdateConditionsString();
            if (!_field.IsDummy)
            {
                var soloField = FieldHelper.GetField(_field, "m_Solo");
                if (!soloField.IsDummy) soloField.AsBool = Solo;
                var muteField = FieldHelper.GetField(_field, "m_Mute");
                if (!muteField.IsDummy) muteField.AsBool = Mute;
            }

            if (_ownerAsset != null && _workspace != null)
            {
                var root = _rootField ?? _workspace.GetBaseField(_ownerAsset);
                if (root != null)
                {
                    _ownerAsset.UpdateAssetDataAndRow(_workspace, root);
                    WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(_ownerAsset));
                    WeakReferenceMessenger.Default.Send(new TransitionModifiedMessage(this));
                    System.Diagnostics.Debug.WriteLine($"[AnimatorAnalysis] Transition Updated. Owner: {_ownerAsset.DisplayName}, Bytes: {root.WriteToByteArray().Length}");
                }
            }
        }
    }

    public partial class TransitionLine : ObservableObject
    {
        [ObservableProperty] private double _fromX;
        [ObservableProperty] private double _fromY;
        [ObservableProperty] private double _toX;
        [ObservableProperty] private double _toY;
        private string _lineData = "";
        public string LineData { get => _lineData; set => SetProperty(ref _lineData, value); }

        private string _conditionSummary = "";
        public string ConditionSummary { get => _conditionSummary; set => SetProperty(ref _conditionSummary, value); }

        public List<TransitionItem> Transitions { get; } = new();

        public void RefreshSummary()
        {
            ConditionSummary = string.Join(" | ", Transitions.Select(t => t.Conditions).Where(c => !string.IsNullOrEmpty(c)));
            OnPropertyChanged(nameof(Color));
        }

        public StateItem? FromState { get; set; }
        public StateItem? ToState { get; set; }
        public bool IsMutual { get; set; }
        public bool IsMultiple { get; set; }

        public TransitionItem? SourceTransition { get; set; }

        public string Color => Transitions.Any(t => t.IsDefaultLink) ? "#ffcc00" : "#888888";

        public void UpdatePoints()
        {
            if (FromState == null || ToState == null) return;

            // Box dimensions: 160x45
            double fx = FromState.X + 80.0;
            double fy = FromState.Y + 22.5;
            double tx = ToState.X + 80.0;
            double ty = ToState.Y + 22.5;

            double dx = tx - fx;
            double dy = ty - fy;
            double len = Math.Sqrt(dx * dx + dy * dy);

            if (len > 0)
            {
                double ux = dx / len;
                double uy = dy / len;

                // Apply lateral offset for mutual transitions (15px clearance)
                double offset = IsMutual ? 15.0 : 0.0;
                
                fx += -uy * offset;
                fy += ux * offset;
                tx += -uy * offset;
                ty += ux * offset;

                // Position the main arrow at 50% (Centered)
                double px = fx + (tx - fx) * 0.5;
                double py = fy + (ty - fy) * 0.5;

                // Build geometry string
                System.Text.StringBuilder sb = new();
                sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "M {0:F2},{1:F2} L {2:F2},{3:F2} ", fx, fy, tx, ty);

                // Add arrowheads
                int arrowCount = IsMultiple ? 3 : 1;
                double arrowSpacing = 8.0; 
                double clusterOffset = (arrowCount - 1) * arrowSpacing / 2.0;

                for (int i = 0; i < arrowCount; i++)
                {
                    // Starting from px, py (the midpoint), shift so the whole group is centered
                    double shift = (i * arrowSpacing) - clusterOffset;
                    double cpx = px - ux * shift;
                    double cpy = py - uy * shift;

                    double ax1 = cpx - 12.0 * ux + 6.0 * (-uy);
                    double ay1 = cpy - 12.0 * uy + 6.0 * ux;
                    double ax2 = cpx - 12.0 * ux - 6.0 * (-uy);
                    double ay2 = cpy - 12.0 * uy - 6.0 * ux;

                    sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "M {0:F2},{1:F2} L {2:F2},{3:F2} L {4:F2},{5:F2} Z ", cpx, cpy, ax1, ay1, ax2, ay2);
                    // Fixed typo in format args: cpy instead of py for index 1
                }
                
                LineData = sb.ToString();
            }
            else
            {
                LineData = "";
            }
        }
    }

    internal static class AnimatorHash
    {
        public static uint GetHash(string name)
        {
            if (string.IsNullOrEmpty(name)) return 0;
            // Unity's Animator string hash is CRC32
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(name);
            uint crc = 0xFFFFFFFF;
            foreach (byte b in bytes)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 1) != 0) crc = (crc >> 1) ^ 0xEDB88320;
                    else crc >>= 1;
                }
            }
            return ~crc;
        }
    }

    public record ParameterRenamedMessage(uint OldHash, uint NewHash, string NewName, string? OldName = null);
    public record DeleteTransitionRequest(UABEANext4.ViewModels.Tools.TransitionItem Transition);
    public record TransitionModifiedMessage(UABEANext4.ViewModels.Tools.TransitionItem Transition);

}
