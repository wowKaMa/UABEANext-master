using System;
using System.Collections.Generic;
using System.Numerics;
using AssetsTools.NET;
using UABEANext4.AssetWorkspace;
using UABEANext4.Logic.Hierarchy;

namespace UABEANext4.Logic.Hierarchy;

public static class HierarchyMath
{
    public static Matrix4x4 GetLocalMatrix(Workspace workspace, HierarchyItem item)
    {
        if (item.FileInstance == null || item.TransformPathId == 0)
            return Matrix4x4.Identity;

        var tfmBf = workspace.GetBaseField(item.FileInstance, item.TransformPathId);
        if (tfmBf == null) return Matrix4x4.Identity;

        var pos = tfmBf["m_LocalPosition"];
        var rot = tfmBf["m_LocalRotation"];
        var scale = tfmBf["m_LocalScale"];

        var translation = new Vector3(pos["x"].AsFloat, pos["y"].AsFloat, pos["z"].AsFloat);
        var rotation = new Quaternion(rot["x"].AsFloat, rot["y"].AsFloat, rot["z"].AsFloat, rot["w"].AsFloat);
        var scaling = new Vector3(scale["x"].AsFloat, scale["y"].AsFloat, scale["z"].AsFloat);

        return Matrix4x4.CreateScale(scaling) * 
               Matrix4x4.CreateFromQuaternion(rotation) * 
               Matrix4x4.CreateTranslation(translation);
    }

    public static Matrix4x4 GetWorldMatrix(Workspace workspace, HierarchyItem item)
    {
        var local = GetLocalMatrix(workspace, item);
        if (item.Parent != null)
        {
            return local * GetWorldMatrix(workspace, item.Parent);
        }
        return local;
    }

    public static Vector3 GetWorldPosition(Workspace workspace, HierarchyItem item)
    {
        var worldMatrix = GetWorldMatrix(workspace, item);
        return worldMatrix.Translation;
    }

    public static Vector3 GetLossyScale(Workspace workspace, HierarchyItem item)
    {
        var tfmBf = workspace.GetBaseField(item.FileInstance!, item.TransformPathId);
        if (tfmBf == null) return Vector3.One;
        var scale = tfmBf["m_LocalScale"];
        var localScale = new Vector3(scale["x"].AsFloat, scale["y"].AsFloat, scale["z"].AsFloat);

        if (item.Parent != null)
        {
            return localScale * GetLossyScale(workspace, item.Parent);
        }
        return localScale;
    }

    public static Quaternion GetWorldRotation(Workspace workspace, HierarchyItem item)
    {
        if (item.Parent == null)
        {
            var tfmBf = workspace.GetBaseField(item.FileInstance!, item.TransformPathId);
            if (tfmBf == null) return Quaternion.Identity;
            var rot = tfmBf["m_LocalRotation"];
            return new Quaternion(rot["x"].AsFloat, rot["y"].AsFloat, rot["z"].AsFloat, rot["w"].AsFloat);
        }

        var parentWorldRot = GetWorldRotation(workspace, item.Parent);
        var tfmBf2 = workspace.GetBaseField(item.FileInstance!, item.TransformPathId);
        if (tfmBf2 == null) return parentWorldRot;
        var rot2 = tfmBf2["m_LocalRotation"];
        var localRot = new Quaternion(rot2["x"].AsFloat, rot2["y"].AsFloat, rot2["z"].AsFloat, rot2["w"].AsFloat);
        
        return parentWorldRot * localRot; // Correct Unity order: World = ParentWorld * Local
    }

    public static Quaternion WorldToLocalRotation(Workspace workspace, HierarchyItem item, Quaternion worldRot)
    {
        if (item.Parent == null) return worldRot;
        var parentWorldRot = GetWorldRotation(workspace, item.Parent);
        var invParentWorldRot = Quaternion.Inverse(parentWorldRot);
        return invParentWorldRot * worldRot; // Local = (ParentWorld^-1) * World
    }

    public static Quaternion FromToRotation(Vector3 from, Vector3 to)
    {
        from = Vector3.Normalize(from);
        to = Vector3.Normalize(to);

        float dot = Vector3.Dot(from, to);
        if (dot > 0.999999f) return Quaternion.Identity;
        if (dot < -0.999999f)
        {
            Vector3 ortho = Vector3.Cross(from, Vector3.UnitX);
            if (ortho.LengthSquared() < 0.0001f)
                ortho = Vector3.Cross(from, Vector3.UnitY);
            return Quaternion.CreateFromAxisAngle(Vector3.Normalize(ortho), (float)Math.PI);
        }

        Vector3 axis = Vector3.Cross(from, to);
        float s = (float)Math.Sqrt((1 + dot) * 2);
        float invS = 1 / s;

        return new Quaternion(axis.X * invS, axis.Y * invS, axis.Z * invS, s * 0.5f);
    }
}
