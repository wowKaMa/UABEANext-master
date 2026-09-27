#nullable enable
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UABEANext4.AssetWorkspace;

namespace UABEANext4.Logic.Hierarchy;
// 定义命名空间 UABEANext4.Logic.Hierarchy，用于组织层级（Hierarchy）相关逻辑类型（保留英文原名：UABEANext4.Logic.Hierarchy）

public partial class HierarchyItem : ObservableObject
{
    public AssetInst? Asset { get; set; }

    public ObservableCollection<HierarchyItem> Children { get; } = new();

    public string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public bool _expanded = false;

    // --- New properties for drag-and-drop reparenting ---
    public HierarchyItem? Parent { get; set; }
    public long TransformPathId { get; set; }
    public AssetsFileInstance? FileInstance { get; set; }

    // Properties moved to logic assembly

    internal class GameObjectInfo
    {
        public AssetInst? Asset { get; set; } = null;
        public string Name { get; set; } = string.Empty;
    }

    internal class TransformInfo
    {
        public bool IsRoot { get; set; } = false;
        public AssetInst? GameObjectInst { get; set; } = null;
        public List<TransformInfo> Children { get; } = new();
        public string Name { get; set; } = string.Empty;
        public long TransformPathId { get; set; }
    }

    public static List<HierarchyItem> CreateRootItems(Workspace workspace, AssetsFileInstance fileInst, bool alphabetical)
    {
        var mapping = CreateTransformMapping(workspace, fileInst);
        var rootSeItems = new List<HierarchyItem>();
        var rootTfmItems = mapping.Where(t => t.IsRoot);

        foreach (var rootItem in rootTfmItems)
        {
            rootSeItems.Add(CreateItemRecursive(rootItem, null, fileInst));
        }

        if (alphabetical)
        {
            rootSeItems.Sort((x, y) => x.Name.CompareTo(y.Name));
        }

        return rootSeItems;
    }

    private static HierarchyItem CreateItemRecursive(TransformInfo inf, HierarchyItem? parent, AssetsFileInstance fileInst)
    {
        var item = new HierarchyItem()
        {
            Asset = inf.GameObjectInst,
            Name = inf.Name,
            Parent = parent,
            TransformPathId = inf.TransformPathId,
            FileInstance = fileInst,
        };

        foreach (var childInf in inf.Children)
        {
            item.Children.Add(CreateItemRecursive(childInf, item, fileInst));
        }

        return item;
    }

    private static List<TransformInfo> CreateTransformMapping(Workspace workspace, AssetsFileInstance fileInst)
    {
        var goInfs = new Dictionary<AssetPPtr, GameObjectInfo>();
        var ptrToTfmInf = new Dictionary<AssetPPtr, TransformInfo>();
        var tfmInfToChildren = new Dictionary<TransformInfo, IEnumerable<AssetPPtr>>();

        var transformInfs = fileInst.file.AssetInfos.Where(a =>
            a.TypeId == (uint)AssetClassID.Transform || a.TypeId == (uint)AssetClassID.RectTransform
        );

        foreach (var goInf in fileInst.file.GetAssetsOfType(AssetClassID.GameObject))
        {
            var gameObjectAsset = workspace.GetAssetInst(fileInst, 0, goInf.PathId);
            if (gameObjectAsset == null)
                continue;

            var gameObjectBf = workspace.GetBaseField(gameObjectAsset);
            if (gameObjectBf == null)
                continue;

            var componentPair = gameObjectBf["m_Component.Array"][0];
            var tfmPtr = AssetPPtr.FromField(componentPair[componentPair.Children.Count - 1]);

            var goInfObj = new GameObjectInfo();
            goInfObj.Asset = gameObjectAsset;
            goInfObj.Name = gameObjectBf["m_Name"].AsString;
            goInfs[tfmPtr] = goInfObj;
        }

        foreach (var tfmInf in transformInfs)
        {
            var transformBf = workspace.GetBaseField(fileInst, tfmInf.PathId);
            if (transformBf == null)
                continue;

            var tfmPtr = new AssetPPtr(0, tfmInf.PathId);

            var tfmInfObj = new TransformInfo();
            tfmInfObj.IsRoot = transformBf["m_Father"]["m_PathID"].AsLong == 0;
            tfmInfObj.TransformPathId = tfmInf.PathId;

            if (goInfs.TryGetValue(tfmPtr, out GameObjectInfo? goInf))
            {
                tfmInfObj.GameObjectInst = goInf.Asset;
                tfmInfObj.Name = goInf.Name;
            }
            else
            {
                tfmInfObj.GameObjectInst = null;
                tfmInfObj.Name = "[missing gameobject]";
            }

            ptrToTfmInf[new AssetPPtr(0, tfmInf.PathId)] = tfmInfObj;
            tfmInfToChildren[tfmInfObj] = transformBf["m_Children.Array"].Select(f => AssetPPtr.FromField(f));
        }

        foreach (var tfmInfObj in ptrToTfmInf.Values)
        {
            if (tfmInfToChildren.TryGetValue(tfmInfObj, out IEnumerable<AssetPPtr>? childrenPptrs))
            {
                foreach (var childPptr in childrenPptrs)
                {
                    if (!childPptr.IsNull())
                    {
                        tfmInfObj.Children.Add(ptrToTfmInf[childPptr]);
                    }
                }
            }
        }

        return ptrToTfmInf.Values.ToList();
    }

}
// 类体结束（HierarchyItem）
