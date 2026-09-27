using System.Collections.Generic;
using System.Numerics;

namespace UABEANext4.Logic.Mesh;

public class SceneNode
{
    public string Name { get; set; } = "GameObject";
    
    public SceneNode? Parent { get; set; }
    public List<SceneNode> Children { get; } = new();

    public Vector3 LocalPosition { get; set; } = Vector3.Zero;
    public Quaternion LocalRotation { get; set; } = Quaternion.Identity;
    public Vector3 LocalScale { get; set; } = Vector3.One;

    public Matrix4x4 LocalMatrix
    {
        get
        {
            return Matrix4x4.CreateScale(LocalScale) *
                   Matrix4x4.CreateFromQuaternion(LocalRotation) *
                   Matrix4x4.CreateTranslation(LocalPosition);
        }
    }

    public Matrix4x4 WorldMatrix
    {
        get
        {
            if (Parent == null) return LocalMatrix;
            return LocalMatrix * Parent.WorldMatrix;
        }
    }

    // Associated Mesh (if any)
    public MeshObj? Mesh { get; set; }
    
    // Material/Texture info
    public TextureObj? Texture { get; set; }
    
    public void AddChild(SceneNode child)
    {
        child.Parent = this;
        Children.Add(child);
    }
}
