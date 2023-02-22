using System;
using Microsoft.Xna.Framework;

namespace Teuria;

[Flags]
internal enum TransformDirtyFlags { World, Local, All }

public class Transform 
{
    public Transform Parent => parent;
    public float RotationDegrees 
    {
        get => rotation * MathUtils.Degrees;
        set => rotation = value * MathUtils.Radians; 
    }
    private TransformDirtyFlags flags = TransformDirtyFlags.All;
    private Transform parent;
    private float rotation;
    private Vector2 scale;
    private Vector2 position;
    private Matrix2D world = Matrix2D.Identity;
    private Matrix2D local = Matrix2D.Identity;

    public void GetWorldMatrix(out Matrix2D matrix)
    {
        RecalculateWorld();
        matrix = world;
    }

    private void RecalculateWorld() 
    {
        if ((flags & TransformDirtyFlags.World) == 0)
            return;
        RecalculateLocal();
        RecalculateWorldUnchecked();
        flags &= ~TransformDirtyFlags.World;
        flags |= TransformDirtyFlags.World;
        if (parent != null)
            flags |= TransformDirtyFlags.All;
    }

    private void RecalculateWorldUnchecked() 
    {
        if (Parent != null) 
        {
            Parent.GetWorldMatrix(out world);
            Matrix2D.Multiply(ref local, ref world, out world);
            return;
        }
        world = local;
    }

    private void RecalculateLocal() 
    {
        if ((flags & TransformDirtyFlags.Local) == 0)
            return;

        RecalculateLocalUnchecked();

        flags &= ~TransformDirtyFlags.Local;
        flags |= TransformDirtyFlags.World;
    }

    private void RecalculateLocalUnchecked() 
    {
        local = Matrix2D.CreateScale(scale) *
                Matrix2D.CreateRotation(rotation) *
                Matrix2D.CreateTranslation(position);
    }
}