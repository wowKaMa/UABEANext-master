using AssetsTools.NET;
using AssetsTools.NET.Extra;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UABEANext4.AssetWorkspace;
using UABEANext4.Util;
using System;
using Avalonia.Collections;
using Avalonia.Threading;

namespace UABEANext4.ViewModels.Documents;

public partial class StreamFileDocumentViewModel : Document
{
    private readonly Workspace _workspace;
    private readonly Action<string> _setFilterDb;
    
    [ObservableProperty]
    private ObservableCollection<ResourceEntry> _entries = new();

    [ObservableProperty]
    private DataGridCollectionView _collectionView;

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private bool _isBusy;

    public StreamFileDocumentViewModel(Workspace workspace)
    {
        _workspace = workspace;
        CollectionView = new DataGridCollectionView(Entries);
        _setFilterDb = DebounceUtils.Debounce<string>(value => {
            Dispatcher.UIThread.Post(() => CollectionView.Filter = SetFilter(value));
        }, 200);
    }

    partial void OnSearchTextChanged(string value) => _setFilterDb(value);

    private Func<object, bool> SetFilter(string search)
    {
        if (string.IsNullOrWhiteSpace(search)) return _ => true;
        return (obj) =>
        {
            if (obj is not ResourceEntry entry) return false;
            return entry.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                   entry.Type.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                   entry.FileName.Contains(search, StringComparison.OrdinalIgnoreCase);
        };
    }

    public async Task Load(string resourceFileName)
    {
        Title = resourceFileName;
        Id = resourceFileName;

        // Check cache first
        if (_workspace.ResourceCache.TryGetValue(resourceFileName, out var cachedEntries))
        {
            Dispatcher.UIThread.Post(() => {
                Entries.Clear();
                foreach (var entry in cachedEntries)
                    Entries.Add(entry);
                CollectionView.Refresh();
                IsBusy = false;
            });
            return;
        }

        IsBusy = true;
        
        await Task.Run(() => {
            var newEntries = new ConcurrentBag<ResourceEntry>();
            var assetsFiles = WorkspaceItem.GetAssetsFileWorkspaceItems(_workspace.RootItems);
            
            Parallel.ForEach(assetsFiles, item => 
            {
                if (item.Object is not AssetsFileInstance asInst) return;

                foreach (var info in asInst.file.AssetInfos)
                {
                    // Optimization: Skip types that are extremely unlikely to have resource links
                    var classId = (AssetClassID)info.TypeId;
                    if (UnlikelyTypes.Contains(classId)) continue;

                    AssetTypeValueField baseField;
                    try 
                    {
                        baseField = _workspace.Manager.GetBaseField(asInst, info);
                    }
                    catch
                    {
                        continue;
                    }

                    if (baseField == null) continue;

                    string typeName = classId.ToString();
                    
                    string? assetName = _workspace.Namer.GetAssetName(new AssetInst(asInst, info), false, 100);
                    assetName = AssetNamer.GetFallbackName(new AssetInst(asInst, info), assetName);
                    
                    DeepProbe(baseField, assetName, typeName, info.PathId, asInst.name, resourceFileName, newEntries);
                }
            });

            var sortedEntries = newEntries.OrderBy(e => e.Offset).ToList();
            
            // Store results in cache
            _workspace.ResourceCache[resourceFileName] = sortedEntries;

            Dispatcher.UIThread.Post(() => {
                Entries.Clear();
                foreach (var entry in sortedEntries)
                    Entries.Add(entry);
                CollectionView.Refresh();
                IsBusy = false;
            });
        });
    }

    private static readonly HashSet<AssetClassID> UnlikelyTypes = new() 
    {
        AssetClassID.Transform, AssetClassID.GameObject, AssetClassID.RectTransform,
        AssetClassID.Camera, AssetClassID.Light, AssetClassID.MeshFilter,
        AssetClassID.Canvas, AssetClassID.CanvasRenderer, AssetClassID.CanvasGroup,
        AssetClassID.ParticleSystem, AssetClassID.ParticleSystemRenderer,
        AssetClassID.MonoScript, AssetClassID.TextAsset
    };

    private void DeepProbe(AssetTypeValueField field, string assetName, string typeName, long pathId, string fileName, string targetResName, ConcurrentBag<ResourceEntry> report)
    {
        if (field == null || field.IsDummy) return;

        // Specific logic matching 2.txt for common types
        if (typeName == "AudioClip")
        {
            var res = field.Get("m_Resource");
            if (!res.IsDummy)
            {
                CheckAndAdd(res.Get("m_Source"), res.Get("m_Offset"), res.Get("m_Size"), assetName, typeName, pathId, fileName, targetResName, report);
                return;
            }
        }
        else if (typeName == "Texture2D" || typeName == "Mesh" || typeName == "VideoClip")
        {
            var stream = field.Get("m_StreamData");
            if (!stream.IsDummy)
            {
                CheckAndAdd(stream.Get("path"), stream.Get("offset"), stream.Get("size"), assetName, typeName, pathId, fileName, targetResName, report);
                return;
            }
        }
        
        // Fallback: generic recursive probe for ALL types that might contain resource paths
        RecursiveProbe(field, assetName, typeName, pathId, fileName, targetResName, report);
    }

    private void CheckAndAdd(AssetTypeValueField sourceF, AssetTypeValueField offsetF, AssetTypeValueField sizeF, string assetName, string typeName, long pathId, string fileName, string targetResName, ConcurrentBag<ResourceEntry> report)
    {
        if (sourceF.IsDummy || offsetF.IsDummy || sizeF.IsDummy) return;

        string rawPath = sourceF.AsString;
        if (string.IsNullOrEmpty(rawPath)) return;

        // Logic to match 2.txt: check if the path refers to our target resource file
        string fileNameOnly = Path.GetFileName(rawPath);
        
        bool isMatch = string.Equals(fileNameOnly, targetResName, StringComparison.OrdinalIgnoreCase) ||
                       rawPath.EndsWith("/" + targetResName, StringComparison.OrdinalIgnoreCase) ||
                       rawPath.Contains("archive:/" + targetResName);

        if (isMatch)
        {
            report.Add(new ResourceEntry {
                Name = assetName,
                Type = typeName,
                Size = sizeF.AsLong,
                Offset = offsetF.AsLong,
                PathId = pathId,
                FileName = fileName
            });
        }
    }

    private void RecursiveProbe(AssetTypeValueField field, string assetName, string typeName, long pathId, string fileName, string targetResName, ConcurrentBag<ResourceEntry> report)
    {
        if (field == null || field.IsDummy) return;

        var sourceF = field.Get("m_Source"); if (sourceF.IsDummy) sourceF = field.Get("path");
        var offsetF = field.Get("m_Offset"); if (offsetF.IsDummy) offsetF = field.Get("offset");
        var sizeF = field.Get("m_Size"); if (sizeF.IsDummy) sizeF = field.Get("size");

        if (!offsetF.IsDummy && !sizeF.IsDummy && sizeF.Value != null && sizeF.AsLong > 0)
        {
            CheckAndAdd(sourceF, offsetF, sizeF, assetName, typeName, pathId, fileName, targetResName, report);
        }

        if (field.Children != null)
        {
            foreach (var child in field.Children) 
                RecursiveProbe(child, assetName, typeName, pathId, fileName, targetResName, report);
        }
    }

    #region Placeholder Commands for UI parity
    public void ViewScene() { }
    public void Export() { }
    public void Import() { }
    public void EditDump() { }
    public void ShowPlugins() { }
    public void AddAsset() { }
    public void RemoveAsset() { }
    public void SetTypeFilter() { }
    public List<PluginItemInfo> PluginsItems { get; } = new();
    public bool LoadContainers => false;
    public int SearchKind { get; set; } = 0;
    public bool IsSearchCaseSensitive { get; set; } = false;
    #endregion
}
