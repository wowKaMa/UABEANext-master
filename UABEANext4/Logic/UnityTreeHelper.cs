using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.Collections.Generic;

namespace UABEANext4.Logic;

public static class UnityTreeHelper
{
    public class HierarchyNode : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        public string Name { get; set; } = string.Empty;
        public AssetExternal Asset { get; set; }
        public List<HierarchyNode> Children { get; set; } = new();

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }
    }

    public static AssetTypeValueField GetLastChild(this AssetTypeValueField field)
    {
        return field.Children[field.Children.Count - 1];
    }

    public static HierarchyNode? BuildHierarchyFromGameObject(AssetsManager manager, AssetsFileInstance inst, AssetExternal gameObjectExt)
    {
        var baseField = gameObjectExt.baseField;
        if (baseField == null) return null;

        var node = new HierarchyNode
        {
            Name = baseField["m_Name"].AsString,
            Asset = gameObjectExt
        };

        if (baseField["m_IsActive"].AsBool == false)
        {
            node.Name += " (Inactive)";
        }

        // Get Transform
        var components = baseField["m_Component.Array"];
        if (components.Children.Count > 0)
        {
            var transformRef = components[0].GetLastChild(); // Usually Transform is first
            var transformExt = manager.GetExtAsset(inst, transformRef);
            
            if (transformExt.baseField != null)
            {
                var children = transformExt.baseField["m_Children.Array"];
                foreach (var childRef in children.Children)
                {
                    var childTransformExt = manager.GetExtAsset(inst, childRef);
                    if (childTransformExt.baseField != null)
                    {
                        var childGameObjectRef = childTransformExt.baseField["m_GameObject"];
                        var childGameObjectExt = manager.GetExtAsset(inst, childGameObjectRef);
                        
                        var childNode = BuildHierarchyFromGameObject(manager, inst, childGameObjectExt);
                        if (childNode != null)
                        {
                            node.Children.Add(childNode);
                        }
                    }
                }
            }
        }

        return node;
    }

    public static List<HierarchyNode> BuildHierarchy(AssetsManager manager, AssetsFileInstance inst)
    {
        var roots = new List<HierarchyNode>();
        var table = inst.file.Metadata.TypeTreeEnabled ? inst.file.Metadata.TypeTreeTypes : null;
        
        // Find all GameObjects
        foreach (var info in inst.file.GetAssetsOfType(AssetClassID.GameObject))
        {
            var gameObjectExt = manager.GetExtAsset(inst, 0, info.PathId);
            
            // Check if it's a root (has no parent)
            if (IsRoot(manager, inst, gameObjectExt))
            {
                var node = BuildHierarchyFromGameObject(manager, inst, gameObjectExt);
                if (node != null)
                {
                    roots.Add(node);
                }
            }
        }
        return roots;
    }

    private static bool IsRoot(AssetsManager manager, AssetsFileInstance inst, AssetExternal gameObjectExt)
    {
        var components = gameObjectExt.baseField["m_Component.Array"];
        if (components.Children.Count == 0) return true;

        var transformRef = components[0].GetLastChild();
        var transformExt = manager.GetExtAsset(inst, transformRef);

        if (transformExt.baseField == null) return true;

        var father = transformExt.baseField["m_Father"];
        return father.GetLastChild().AsLong == 0; // PathID 0 means no parent
    }
}
