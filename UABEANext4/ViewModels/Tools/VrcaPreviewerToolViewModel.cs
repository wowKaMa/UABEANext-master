using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Dock.Model.Mvvm.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using UABEANext4.AssetWorkspace;
using UABEANext4.Logic;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using UABEANext4.Logic.Mesh;
using System.IO;
using System.Linq;
using System.Collections.Specialized;
using System.Diagnostics;
using Avalonia.Threading;
using System.Numerics;

namespace UABEANext4.ViewModels.Tools
{
    public partial class VrcaPreviewerToolViewModel : Tool
    {
        private const string ST_IDLE = "准备就绪 (Ready)";
        private const string ST_LOADING = "正在加载层级... (Loading Hierarchy...)";
        private const string ST_EXTRACTING = "正在提取模型... (Extracting Models...)";

        public Workspace Workspace { get; }

        [ObservableProperty] private ObservableCollection<UnityTreeHelper.HierarchyNode> _hierarchyRoots = new();
        [ObservableProperty] private UnityTreeHelper.HierarchyNode? _selectedNode;
        [ObservableProperty] private List<MeshObj> _activeMeshes = new();

        [ObservableProperty] private string _statusText = ST_IDLE;
        [ObservableProperty] private int _totalMeshes;
        [ObservableProperty] private int _totalVertices;
        [ObservableProperty] private bool _isWorking;
        
        [ObservableProperty] private string _debugData = "No data";
        [ObservableProperty] private string _glInfo = "Initializing...";
        [ObservableProperty] private int _viewportWidth, _viewportHeight;
        [ObservableProperty] private string _keysStatus = "None";
        [ObservableProperty] private string _focusStatus = "N/A";
        [ObservableProperty] private string _shaderStatus = "All OK";
        [ObservableProperty] private bool _isUnityMode = false;
        [ObservableProperty] private string _currentVrcaPath = string.Empty; // Added for binding
        
        public bool IsOpenGLMode => !IsUnityMode;

        partial void OnIsUnityModeChanged(bool value)
        {
             OnPropertyChanged(nameof(IsOpenGLMode));
             // Removed OpenInUnityViewer() call to prevent standalone popup
             UpdateStats();
             
             // Sync state to the active viewer
             if (value) // If switching to Unity Mode
             {
                 OnNodeChecked();
             }
        }

        private AssetsFileInstance? _loadedFile;

        public VrcaPreviewerToolViewModel(Workspace workspace)
        {
            Workspace = workspace;
            Id = "VrcaPreviewerTool";
            Title = "VRCA 浏览器 (VRCA Explorer)";
            
            this.PropertyChanged += OnPropertyChanged;
            LoadFromWorkspace();
            
            Workspace.RootItems.CollectionChanged += RootItems_CollectionChanged;
            WeakReferenceMessenger.Default.Register<AssetsSelectedMessage>(this, (r, m) => OnAssetsSelected(m));
        }

        private void RootItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Reset)
                LoadFromWorkspace();
        }

        private void OnAssetsSelected(AssetsSelectedMessage message)
        {
            if (message.Value == null || message.Value.Count == 0 || IsWorking) return;
            var asset = message.Value[0];
            if (asset.FileInstance == null) return;

            if (asset.TypeId == (int)AssetClassID.Mesh)
            {
                IsWorking = true;
                StatusText = ST_EXTRACTING;
                Task.Run(() => {
                    try {
                        var meshBaseField = Workspace.Manager.GetBaseField(asset.FileInstance, (AssetFileInfo)asset);
                        if (meshBaseField != null)
                        {
                            var mesh = new MeshObj(asset.FileInstance, meshBaseField, new UnityVersion(asset.FileInstance.file.Metadata.UnityVersion));
                            Dispatcher.UIThread.Post(() => {
                                ActiveMeshes = [mesh];
                                UpdateStats();
                                IsWorking = false;
                                StatusText = ST_IDLE;
                            });
                        }
                    } catch {
                        Dispatcher.UIThread.Post(() => IsWorking = false);
                    }
                });
            }
        }

        private async void LoadFromWorkspace()
        {
            var targetFile = FindMainAssetsFile();
            if (targetFile != null && targetFile != _loadedFile)
            {
                _loadedFile = targetFile;
                HierarchyRoots.Clear();
                StatusText = ST_LOADING;
                IsWorking = true;
                try {
                    // Get original bundle path for Unity viewer (same logic as OpenInUnityViewer)
                    CurrentVrcaPath = GetOriginalBundlePath();
                    var roots = await Task.Run(() => UnityTreeHelper.BuildHierarchy(Workspace.Manager, _loadedFile));
                    foreach (var root in roots) 
                    {
                        HierarchyRoots.Add(root);
                        SubscribeHelpers(root); // Subscribe to changes
                    }
                } finally {
                    IsWorking = false;
                    StatusText = ST_IDLE;
                }
            }
        }

        private void SubscribeHelpers(UnityTreeHelper.HierarchyNode node)
        {
            node.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(UnityTreeHelper.HierarchyNode.IsChecked))
                    OnNodeChecked();
            };
            foreach (var child in node.Children) SubscribeHelpers(child);
        }

        private void OnNodeChecked()
        {
            var checkedNodes = new List<UnityTreeHelper.HierarchyNode>();
            foreach (var root in HierarchyRoots) CollectCheckedNodes(root, checkedNodes);

            if (IsUnityMode)
            {
                if (checkedNodes.Count > 0)
                {
                    // Send multi-select command
                    string payload = string.Join("|", checkedNodes.Select(n => n.Name));
                    UnityViewerInteractor.SendCommand($"SHOW:{payload}");
                }
                else
                {
                    UnityViewerInteractor.SendCommand("SHOW:");
                }
            }
            else
            {
                // OpenGL Mode: Load meshes from all checked nodes
                if (checkedNodes.Count > 0)
                {
                    LoadMeshesFromAssetsAsync(checkedNodes.Select(n => n.Asset).ToList());
                }
                else
                {
                    // If nothing checked, show nothing (or clear)
                     ActiveMeshes = [];
                     UpdateStats();
                }
            }
        }

        private void CollectCheckedNodes(UnityTreeHelper.HierarchyNode node, List<UnityTreeHelper.HierarchyNode> list)
        {
            if (node.IsChecked) list.Add(node);
            foreach (var child in node.Children) CollectCheckedNodes(child, list);
        }

        private void CollectChecked(UnityTreeHelper.HierarchyNode node, List<string> list)
        {
             if (node.IsChecked) list.Add(node.Name);
             foreach (var child in node.Children) CollectChecked(child, list);
        }

        private string GetOriginalBundlePath()
        {
            // Find the original bundle file (same logic as OpenInUnityViewer button)
            var mainFile = Workspace.RootItems.FirstOrDefault(i => 
                i.ObjectType == WorkspaceItemType.BundleFile || i.ObjectType == WorkspaceItemType.AssetsFile);
            if (mainFile == null) return string.Empty;

            if (mainFile.Object is AssetsFileInstance afi) return afi.path;
            if (mainFile.Object is BundleFileInstance bfi) return bfi.path;
            return string.Empty;
        }
        
        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedNode) && SelectedNode != null && _loadedFile != null && !IsWorking)
            {
                if (IsUnityMode)
                {
                    UnityViewerInteractor.HighlightObject(SelectedNode.Name);
                }
                else
                {
                    // Single selection: Load just this one
                    LoadMeshesFromAssetsAsync([SelectedNode.Asset]);
                }
            }
        }

        private async void LoadMeshesFromAssetsAsync(List<AssetExternal> roots)
        {
            // If any root is null/invalid basefield, skip it
            var validRoots = roots.Where(r => r.baseField != null).ToList();
            if (validRoots.Count == 0) return;

            IsWorking = true;
            StatusText = ST_EXTRACTING;
            try 
            {
                var manager = Workspace.Manager;
                // Use the file from the first valid root, assume they are from same file/bundle context
                var inst = _loadedFile ?? validRoots[0].file;
                
                var meshAssets = new List<AssetExternal>();
                
                await Task.Run(() => {
                    foreach(var root in validRoots)
                    {
                        FindMeshesRecursive(root, manager, inst, meshAssets);
                    }
                });
                
                // Deduplicate meshes if same mesh is selected multiple times (e.g. parent and child)
                meshAssets = meshAssets.DistinctBy(m => m.info.PathId).ToList();

                var meshObjs = new List<MeshObj>();
                if (meshAssets.Count > 0)
                {
                    await Task.Run(() => {
                        var version = new UnityVersion(inst.file.Metadata.UnityVersion);
                        foreach (var mAsset in meshAssets) {
                            try { 
                                var mObj = new MeshObj(inst, mAsset.baseField, version);
                                if (mObj.Vertices.Length > 0) meshObjs.Add(mObj);
                            } catch { }
                        }
                    });
                }
                
                Dispatcher.UIThread.Post(() => {
                    ActiveMeshes = meshObjs;
                    UpdateStats();
                });
            }
            finally { IsWorking = false; StatusText = ST_IDLE; }
        }

        private void UpdateStats()
        {
            TotalMeshes = ActiveMeshes.Count;
            TotalVertices = ActiveMeshes.Sum(m => m.Vertices.Length / 3);
            
            if (ActiveMeshes.Count > 0)
            {
                var min = new Vector3(float.MaxValue);
                var max = new Vector3(float.MinValue);
                foreach (var m in ActiveMeshes)
                {
                    min = Vector3.Min(min, m.MinBounds);
                    max = Vector3.Max(max, m.MaxBounds);
                }
                var center = (min + max) / 2f;
                var size = max - min;
                DebugData = $"Bounds Min: {min:F2}\nBounds Max: {max:F2}\nCenter: {center:F2}\nSize: {size:F2}\nTotal Verts: {TotalVertices}\n\nUnity Status:\n{UnityViewerInteractor.LastLog}";
            }
            else
            {
                DebugData = $"No meshes loaded.\n\nUnity Status:\n{UnityViewerInteractor.LastLog}";
            }

            Debug.WriteLine($"[VRCA Preview] Stats Updated: Meshes={TotalMeshes}, Vertices={TotalVertices}");
        }

        [RelayCommand] private void ClearPreview() { ActiveMeshes = []; UpdateStats(); }

        [RelayCommand]
        private void OpenInUnityViewer()
        {
            var mainFile = Workspace.RootItems.FirstOrDefault(i => i.ObjectType == WorkspaceItemType.BundleFile || i.ObjectType == WorkspaceItemType.AssetsFile);
            if (mainFile == null) return;

            string path = "";
            if (mainFile.Object is AssetsFileInstance afi) path = afi.path;
            else if (mainFile.Object is BundleFileInstance bfi) path = bfi.path;

            if (!string.IsNullOrEmpty(path))
            {
                UnityViewerInteractor.LaunchViewer(path);
            }
        }

        private void FindMeshesRecursive(AssetExternal goExt, AssetsManager manager, AssetsFileInstance inst, List<AssetExternal> meshes)
        {
            if (goExt.baseField == null) return;

            var mAsset = GetMeshFromGameObject(goExt, manager, inst);
            if (mAsset.baseField != null) meshes.Add(mAsset);

            try {
                var comps = goExt.baseField["m_Component.Array"];
                if (comps.Children.Count > 0) {
                    var transExt = manager.GetExtAsset(inst, comps[0].GetLastChild());
                    if (transExt.baseField != null) {
                        var children = transExt.baseField["m_Children.Array"];
                        foreach (var cRef in children.Children) {
                            var ctExt = manager.GetExtAsset(inst, cRef);
                            if (ctExt.baseField != null) {
                                var cgoRef = ctExt.baseField["m_GameObject"];
                                var cgoExt = manager.GetExtAsset(inst, cgoRef);
                                FindMeshesRecursive(cgoExt, manager, inst, meshes);
                            }
                        }
                    }
                }
            } catch { }
        }

        private AssetExternal GetMeshFromGameObject(AssetExternal goExt, AssetsManager manager, AssetsFileInstance inst)
        {
            try {
                var comps = goExt.baseField["m_Component.Array"];
                foreach (var cPtrWrapper in comps.Children) {
                    var cExt = manager.GetExtAsset(inst, cPtrWrapper.GetLastChild());
                    if (cExt.baseField == null) continue;
                    
                    int tid = cExt.info.TypeId;
                    if (tid == (int)AssetClassID.MeshFilter || tid == (int)AssetClassID.SkinnedMeshRenderer) {
                        var mPtr = cExt.baseField["m_Mesh"];
                        if (mPtr.IsDummy) continue;
                        var actualMPtr = mPtr.GetLastChild();
                        if (actualMPtr.AsLong != 0) return manager.GetExtAsset(inst, mPtr);
                    }
                }
            } catch { }
            return default;
        }

        private AssetsFileInstance? FindMainAssetsFile()
        {
            var queue = new Queue<WorkspaceItem>(Workspace.RootItems);
            while (queue.Count > 0) {
                var item = queue.Dequeue();
                if (item.Object is AssetsFileInstance fileInst && fileInst.file.GetAssetsOfType(AssetClassID.GameObject).Count > 0) return fileInst;
                if (item.Children != null) foreach (var child in item.Children) queue.Enqueue(child);
            }
            return null;
        }
    }
}
