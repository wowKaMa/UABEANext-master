using CommunityToolkit.Mvvm.ComponentModel;
using UABEANext4.AssetWorkspace;
using UABEANext4.Logic.Mesh;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace UABEANext4.ViewModels
{
    public partial class MeshTestWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private SceneNode? _rootNode;

        [ObservableProperty]
        private string _status = "Initializing...";

        private readonly Workspace _workspace;
        private AssetsManager _manager => _workspace.Manager;
        
        // Cache for Texture Assets (PathID -> TextureObj)
        private Dictionary<long, TextureObj> _textureCache = new();

        public MeshTestWindowViewModel(Workspace workspace)
        {
            _workspace = workspace;
            LoadSceneTree();
        }

        private void LoadSceneTree()
        {
            try
            {
                Status = "Searching for Avatar in workspace...";
                var nodes = new List<SceneNode>();
                var assetsFiles = GetAllAssetsFiles(_workspace.RootItems);

                foreach (var fileInst in assetsFiles)
                {
                    var transformInfos = fileInst.file.GetAssetsOfType(AssetClassID.Transform);
                    foreach (var info in transformInfos)
                    {
                        var baseField = _manager.GetBaseField(fileInst, info);
                        var father = baseField["m_Father"];
                        if (IsPtrNull(father))
                        {
                            var rootNode = RecursiveBuildNode(fileInst, baseField);
                            nodes.Add(rootNode);
                        }
                    }
                }

                if (nodes.Count > 0)
                {
                    if (nodes.Count == 1)
                    {
                        RootNode = nodes[0];
                        Status = $"Hierarchy Built! Root: {RootNode.Name}";
                    }
                    else
                    {
                        var masterRoot = new SceneNode() { Name = "Scene Root" };
                        foreach (var n in nodes) masterRoot.AddChild(n);
                        RootNode = masterRoot;
                        Status = $"Hierarchy Built! Found {nodes.Count} roots.";
                    }
                }
                else
                {
                    Status = "No Root Transform found in any loaded files.";
                }
            }
            catch (Exception ex)
            {
                Status = $"ERROR: {ex.Message} \n {ex.StackTrace}";
            }
        }

        private List<AssetsFileInstance> GetAllAssetsFiles(System.Collections.ObjectModel.ObservableCollection<AssetWorkspace.WorkspaceItem> items)
        {
            var list = new List<AssetsFileInstance>();
            foreach (var item in items)
            {
                if (item.Object is AssetsFileInstance afi)
                {
                    list.Add(afi);
                }
                else if (item.Object is BundleFileInstance)
                {
                    if (item.Children != null)
                    {
                         foreach(var child in item.Children)
                         {
                             if (child.Object is AssetsFileInstance childAfi)
                                list.Add(childAfi);
                         }
                    }
                }
            }
            return list;
        }
        
        private bool IsPtrNull(AssetTypeValueField ptr)
        {
            return ptr["m_PathID"].AsLong == 0 && ptr["m_FileID"].AsInt == 0;
        }

        private SceneNode RecursiveBuildNode(AssetsFileInstance fileInst, AssetTypeValueField transformBase)
        {
            var node = new SceneNode();
            
            var goPtr = transformBase["m_GameObject"];
            if (!IsPtrNull(goPtr))
            {
                var goBase = _manager.GetExtAsset(fileInst, goPtr).baseField;
                node.Name = goBase["m_Name"].AsString;
                ProcessComponents(fileInst, goBase, node);
            }
            
            var pos = transformBase["m_LocalPosition"];
            var rot = transformBase["m_LocalRotation"];
            var scl = transformBase["m_LocalScale"];
            
            node.LocalPosition = new Vector3(pos["x"].AsFloat, pos["y"].AsFloat, pos["z"].AsFloat);
            node.LocalRotation = new Quaternion(rot["x"].AsFloat, rot["y"].AsFloat, rot["z"].AsFloat, rot["w"].AsFloat);
            node.LocalScale = new Vector3(scl["x"].AsFloat, scl["y"].AsFloat, scl["z"].AsFloat);
            
            var children = transformBase["m_Children.Array"];
            foreach (var childPtr in children)
            {
                var childTransform = _manager.GetExtAsset(fileInst, childPtr).baseField;
                var childNode = RecursiveBuildNode(fileInst, childTransform);
                node.AddChild(childNode);
            }
            
            return node;
        }

        private void ProcessComponents(AssetsFileInstance fileInst, AssetTypeValueField goBase, SceneNode node)
        {
            var components = goBase["m_Component.Array"];
            foreach (var compData in components)
            {
                var compPtr = compData["component"];
                if (IsPtrNull(compPtr)) continue;
                
                var compExt = _manager.GetExtAsset(fileInst, compPtr);
                var compBase = compExt.baseField;
                var typeId = compExt.info.TypeId;
                
                if (typeId == (int)AssetClassID.MeshFilter)
                {
                    var meshPtr = compBase["m_Mesh"];
                    if (!IsPtrNull(meshPtr)) node.Mesh = LoadMesh(fileInst, meshPtr);
                }
                else if (typeId == (int)AssetClassID.SkinnedMeshRenderer || typeId == (int)AssetClassID.MeshRenderer)
                {
                    if (node.Mesh == null && typeId == (int)AssetClassID.SkinnedMeshRenderer)
                    {
                        var meshPtr = compBase["m_Mesh"];
                        if (!IsPtrNull(meshPtr)) node.Mesh = LoadMesh(fileInst, meshPtr);
                    }

                    // Process Materials
                    var materials = compBase["m_Materials.Array"];
                    if (materials.Children.Count > 0)
                    {
                        var matPtr = materials[0]; // Just grab first material for now
                        if (!IsPtrNull(matPtr))
                        {
                            node.Texture = LoadTextureFromMaterial(fileInst, matPtr);
                        }
                    }
                }
            }
        }

        private MeshObj? LoadMesh(AssetsFileInstance fileInst, AssetTypeValueField meshPtr)
        {
            try
            {
                var meshExt = _manager.GetExtAsset(fileInst, meshPtr);
                return new MeshObj(meshExt.file, meshExt.baseField, new UnityVersion(meshExt.file.file.Metadata.UnityVersion));
            }
            catch { return null; }
        }

        private TextureObj? LoadTextureFromMaterial(AssetsFileInstance fileInst, AssetTypeValueField matPtr)
        {
            try
            {
                var matExt = _manager.GetExtAsset(fileInst, matPtr);
                var matBase = matExt.baseField;
                
                var texEnvs = matBase["m_SavedProperties"]["m_TexEnvs.Array"];
                foreach (var texEnv in texEnvs)
                {
                    if (texEnv["first"].AsString == "_MainTex")
                    {
                        var texPtr = texEnv["second"]["m_Texture"];
                        if (!IsPtrNull(texPtr))
                        {
                            var info = _manager.GetExtAsset(fileInst, texPtr);
                            if (_textureCache.TryGetValue(info.info.PathId, out var existing)) return existing;
                            
                            var texObj = new TextureObj(info.file, info.baseField);
                            _textureCache[info.info.PathId] = texObj;
                            return texObj;
                        }
                    }
                }
                return null;
            }
            catch { return null; }
        }
    }
}
