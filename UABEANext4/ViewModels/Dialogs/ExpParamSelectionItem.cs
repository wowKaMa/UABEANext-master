using CommunityToolkit.Mvvm.ComponentModel;
using UABEANext4.AssetWorkspace;
using AssetsTools.NET;

namespace UABEANext4.ViewModels.Dialogs;

public partial class ExpParamSelectionItem : ObservableObject
{
    private Workspace _workspace;
    public AssetInst? Asset { get; }
    public string? Role { get; }

    [ObservableProperty]
    private string _displayName;

    [ObservableProperty]
    private string _editableName;

    [ObservableProperty]
    private bool _isEditing;

    public ExpParamSelectionItem(Workspace workspace, AssetInst? asset, string? role = null)
    {
        _workspace = workspace;
        Asset = asset;
        Role = role;
        if (asset == null)
        {
            DisplayName = "None";
            EditableName = "";
        }
        else
        {
            string baseName = asset.AssetName ?? "Unnamed Asset";
            EditableName = baseName;
            DisplayName = role != null ? $"{baseName} ({role})" : baseName;
        }
    }

    public void UpdateName(string newName)
    {
        EditableName = newName;
        DisplayName = Role != null ? $"{newName} ({Role})" : newName;
    }

    public void ApplyNameChange()
    {
        if (Asset == null || string.IsNullOrWhiteSpace(EditableName)) return;
        if (EditableName == Asset.AssetName) return;

        var bf = _workspace.GetBaseField(Asset);
        if (bf != null)
        {
            bf["m_Name"].AsString = EditableName;
            Asset.UpdateAssetDataAndRow(_workspace, bf);
            
            var wsItem = _workspace.FindWorkspaceItemByInstance(Asset.FileInstance);
            if (wsItem != null) _workspace.Dirty(wsItem);
            
            DisplayName = Role != null ? $"{EditableName} ({Role})" : EditableName;
        }
    }
}
