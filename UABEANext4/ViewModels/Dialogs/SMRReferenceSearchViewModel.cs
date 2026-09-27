using AssetsTools.NET;
using AssetsTools.NET.Extra;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UABEANext4.AssetWorkspace;
using UABEANext4.Interfaces;

namespace UABEANext4.ViewModels.Dialogs
{
    public partial class SMRReferenceSearchViewModel : ObservableObject, IDialogAware
    {
        public string Title => "SMR 引用查询 (SMR Reference Search)";
        public int Width => 600;
        public int Height => 700;

        private readonly Workspace _workspace;
        private readonly AssetInst _smrAsset;

        [ObservableProperty] private string _gameObjectName = "";
        [ObservableProperty] private string _gameObjectPathId = "";
        [ObservableProperty] private string _hierarchyPath = "";
        [ObservableProperty] private string _meshName = "";
        [ObservableProperty] private string _meshPathId = "";
        [ObservableProperty] private string _rootBoneName = "";
        [ObservableProperty] private string _rootBonePathId = "";

        [ObservableProperty] private string _probeAnchorName = "";
        [ObservableProperty] private string _probeAnchorPathId = "";

        [ObservableProperty] private string _staticBatchRootName = "";
        [ObservableProperty] private string _staticBatchRootPathId = "";

        [ObservableProperty] private string _lightProbeVolumeOverrideName = "";
        [ObservableProperty] private string _lightProbeVolumeOverridePathId = "";

        [ObservableProperty] private int _boneWarningCount = 0;

        public ObservableCollection<ReferenceItem> Bones { get; } = new();
        public ObservableCollection<ReferenceItem> Materials { get; } = new();

        public SMRReferenceSearchViewModel(Workspace workspace, AssetInst smrAsset)
        {
            _workspace = workspace;
            _smrAsset = smrAsset;
            LoadInfo();
        }

        private void LoadInfo()
        {
            var smrBf = _workspace.GetBaseField(_smrAsset);
            if (smrBf == null) return;

            // 1. GameObject Info
            var goPtr = smrBf["m_GameObject"];
            if (goPtr != null && !goPtr.IsDummy)
            {
                long goPathId = goPtr["m_PathID"].AsLong;
                GameObjectPathId = goPathId.ToString();
                var goAsset = _workspace.GetAssetInst(_smrAsset.FileInstance, 0, goPathId);
                if (goAsset != null)
                {
                    var goBf = _workspace.GetBaseField(goAsset);
                    if (goBf != null)
                    {
                        GameObjectName = goBf["m_Name"].AsString;
                        var path = new List<string> { GameObjectName };
                        
                        // Get Transform component for hierarchy walking
                        var components = goBf["m_Component"]["Array"];
                        if (components != null && !components.IsDummy)
                        {
                            var transformPtr = components.Children.FirstOrDefault(c => 
                            {
                                var compAsset = _workspace.GetAssetInst(_smrAsset.FileInstance, 0, c[c.Children.Count - 1]["m_PathID"].AsLong);
                                return compAsset?.Type == AssetClassID.Transform || compAsset?.Type == AssetClassID.RectTransform;
                            });
                            
                            if (transformPtr != null)
                            {
                                long currentTransformPathId = transformPtr[transformPtr.Children.Count - 1]["m_PathID"].AsLong;
                                while (currentTransformPathId != 0)
                                {
                                    var tfmInst = _workspace.GetAssetInst(_smrAsset.FileInstance, 0, currentTransformPathId);
                                    if (tfmInst == null) break;
                                    var tfmBf = _workspace.GetBaseField(tfmInst);
                                    if (tfmBf == null) break;
                                    long parentPathId = tfmBf["m_Father"]["m_PathID"].AsLong;
                                    if (parentPathId == 0) break;
                                    var parentTfmInst = _workspace.GetAssetInst(_smrAsset.FileInstance, 0, parentPathId);
                                    if (parentTfmInst == null) break;
                                    var parentTfmBf = _workspace.GetBaseField(parentTfmInst);
                                    if (parentTfmBf == null) break;
                                    var parentGoPtr = parentTfmBf["m_GameObject"];
                                    var parentGoInst = _workspace.GetAssetInst(_smrAsset.FileInstance, 0, parentGoPtr["m_PathID"].AsLong);
                                    if (parentGoInst != null)
                                    {
                                        var parentGoBf = _workspace.GetBaseField(parentGoInst);
                                        if (parentGoBf != null) path.Insert(0, parentGoBf["m_Name"].AsString);
                                    }
                                    currentTransformPathId = parentPathId;
                                }
                            }
                        }
                        HierarchyPath = string.Join("/", path);
                    }
                }
            }

            // 2. Mesh Info
            LoadPtrInfo(smrBf["m_Mesh"], n => MeshName = n, p => MeshPathId = p);

            // 3. Root Bone Info
            LoadPtrInfo(smrBf["m_RootBone"], n => RootBoneName = n, p => RootBonePathId = p);

            // 4. Probe Anchor
            LoadPtrInfo(smrBf["m_ProbeAnchor"], n => ProbeAnchorName = n, p => ProbeAnchorPathId = p);

            // 5. Static Batch Root
            LoadPtrInfo(smrBf["m_StaticBatchRoot"], n => StaticBatchRootName = n, p => StaticBatchRootPathId = p);

            // 6. Light Probe Volume Override
            LoadPtrInfo(smrBf["m_LightProbeVolumeOverride"], n => LightProbeVolumeOverrideName = n, p => LightProbeVolumeOverridePathId = p);

            // 7. Bones
            BoneWarningCount = 0;
            LoadArrayInfo(smrBf["m_Bones"]["Array"], Bones, true);

            // 8. Materials
            LoadArrayInfo(smrBf["m_Materials"]["Array"], Materials, false);
        }

        private void LoadPtrInfo(AssetTypeValueField ptrField, Action<string> setName, Action<string> setPathId)
        {
            if (ptrField == null || ptrField.IsDummy) 
            {
                setName("None");
                setPathId("0");
                return;
            }
            long pathId = ptrField["m_PathID"].AsLong;
            int fileId = ptrField["m_FileID"].AsInt;
            setPathId(pathId.ToString());
            if (pathId == 0) 
            {
                setName("None");
                return;
            }

            setName(ResolveName(fileId, pathId));
        }

        private void LoadArrayInfo(AssetTypeValueField arrayField, ObservableCollection<ReferenceItem> collection, bool isBones)
        {
            if (arrayField == null || arrayField.IsDummy) return;
            foreach (var ptr in arrayField.Children)
            {
                long pathId = ptr["m_PathID"].AsLong;
                int fileId = ptr["m_FileID"].AsInt;
                
                if (pathId == 0)
                {
                    collection.Add(new ReferenceItem("[Missing Bone]", "0") { IsWarning = true });
                    if (isBones) BoneWarningCount++;
                    continue;
                }

                collection.Add(new ReferenceItem(ResolveName(fileId, pathId), pathId.ToString()));
            }
        }

        private string ResolveName(int fileId, long pathId)
        {
            var asset = _workspace.GetAssetInst(_smrAsset.FileInstance, fileId, pathId);
            if (asset == null) return "Unresolved";

            var bf = _workspace.GetBaseField(asset);
            if (bf == null) return asset.ToString();

            // Case 1: GameObject
            if (bf.TemplateField.Name == "GameObject")
                return bf["m_Name"].AsString;

            // Case 2: Component (Transform, etc.) - get name from its GameObject
            if (bf.TemplateField.Children.Any(c => c.Name == "m_GameObject"))
            {
                var goPtr = bf["m_GameObject"];
                if (goPtr != null && !goPtr.IsDummy)
                {
                    var goInst = _workspace.GetAssetInst(_smrAsset.FileInstance, 0, goPtr["m_PathID"].AsLong);
                    if (goInst != null)
                    {
                        var goBf = _workspace.GetBaseField(goInst);
                        if (goBf != null) return goBf["m_Name"].AsString;
                    }
                }
            }

            // Case 3: Asset with m_Name
            if (bf.TemplateField.Children.Any(c => c.Name == "m_Name"))
                return bf["m_Name"].AsString;

            return asset.ToString();
        }
    }

    public class ReferenceItem
    {
        public string Name { get; }
        public string PathId { get; }
        public bool IsWarning { get; set; }
        public ReferenceItem(string name, string pathId)
        {
            Name = name;
            PathId = pathId;
        }
    }
}
