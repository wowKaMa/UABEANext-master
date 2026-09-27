using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using UABEANext4.AssetWorkspace;
using System.Collections.Generic;
using UABEANext4.Logic;

namespace UABEANext4.ViewModels.Tools
{
    public partial class VrcExpParamsViewModel : ObservableObject
    {
        private Workspace _workspace;
        private AssetInst _asset;

        [ObservableProperty] private string _assetName;
        [ObservableProperty] private ObservableCollection<VrcExpParamItem> _parameters = new();
        [ObservableProperty] private VrcExpParamItem? _selectedParameter;
        
        public List<VrcExpParamType> ValueTypes { get; } = new()
        {
            new VrcExpParamType(0, "整数 (Int)"),
            new VrcExpParamType(1, "浮点 (Float)"),
            new VrcExpParamType(2, "布尔 (Bool)")
        };

        public VrcExpParamsViewModel(Workspace workspace, AssetInst asset)
        {
            _workspace = workspace;
            _asset = asset;
            AssetName = asset.AssetName ?? "Unknown";
            LoadParameters(workspace, asset);
        }

        private void LoadParameters(Workspace workspace, AssetInst asset)
        {
            var baseField = workspace.GetBaseField(asset);
            if (baseField == null) return;

            var paramsField = baseField["parameters"]["Array"];
            if (paramsField.IsDummy || paramsField.Children == null) return;

            foreach (var p in paramsField.Children)
            {
                var item = new VrcExpParamItem
                {
                    Name = p["name"].AsString,
                    ValueType = (int)p["valueType"].AsLong,
                    Saved = p["saved"].AsInt != 0,
                    DefaultValue = p["defaultValue"].AsFloat,
                    NetworkSynced = p["networkSynced"].AsInt != 0
                };
                Parameters.Add(item);
            }
        }

        [RelayCommand]
        public void AddParameter()
        {
            Parameters.Add(new VrcExpParamItem
            {
                Name = "New Parameter",
                ValueType = 0,
                Saved = true,
                DefaultValue = 0,
                NetworkSynced = true
            });
        }

        [RelayCommand(CanExecute = nameof(CanDeleteParameter))]
        public void DeleteParameter()
        {
            if (SelectedParameter != null)
            {
                Parameters.Remove(SelectedParameter);
            }
        }

        private bool CanDeleteParameter => SelectedParameter != null;

        partial void OnSelectedParameterChanged(VrcExpParamItem? value)
        {
            DeleteParameterCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        public void ApplyChanges()
        {
            var baseField = _workspace.GetBaseField(_asset);
            if (baseField == null) return;

            var paramsParent = baseField["parameters"];
            var paramsField = paramsParent["Array"];
            if (paramsField.IsDummy || paramsField.Children == null) return;

            // Recreate array if size changed
            if (paramsField.Children.Count != Parameters.Count)
            {
                var template = paramsField.TemplateField.Children[1]; // Index 1 is the 'data' template
                var newParams = new List<AssetTypeValueField>();
                for (int i = 0; i < Parameters.Count; i++)
                {
                    newParams.Add(ValueBuilder.DefaultValueFieldFromTemplate(template));
                }
                paramsField.Children = newParams;
                
                // Sync size field
                var sizeField = paramsParent["size"];
                if (!sizeField.IsDummy)
                {
                    sizeField.AsInt = Parameters.Count;
                }
            }

            for (int i = 0; i < Parameters.Count; i++)
            {
                var item = Parameters[i];
                var p = paramsField.Children[i];
                p["name"].AsString = item.Name;
                p["valueType"].AsInt = item.ValueType;
                p["saved"].AsBool = item.Saved;
                p["defaultValue"].AsFloat = item.DefaultValue;
                p["networkSynced"].AsBool = item.NetworkSynced;
            }

            _asset.UpdateAssetDataAndRow(_workspace, baseField);
            WeakReferenceMessenger.Default.Send(new AssetsUpdatedMessage(_asset));
        }
    }

    public class VrcExpParamType
    {
        public int Id { get; }
        public string Name { get; }
        public VrcExpParamType(int id, string name) { Id = id; Name = name; }
    }

    public partial class VrcExpParamItem : ObservableObject
    {
        [ObservableProperty] private string _name = string.Empty;
        [ObservableProperty] private int _valueType;
        [ObservableProperty] private bool _saved;
        [ObservableProperty] private float _defaultValue;
        [ObservableProperty] private bool _networkSynced;
    }
}
