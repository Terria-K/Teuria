using System;
using Microsoft.Xna.Framework;

namespace Teuria;


public class Transform 
{
    private Transform? parent = null;
    private bool dirty = true;

    private float rotation;
    private float localRotation;
    private Vector2 scale = Vector2.One;
    private Vector2 localScale = Vector2.One;
    private Vector2 position;
    private Vector2 localPosition;
    private Vector2 pivotOffset;

    private Matrix2D worldMatrix = Matrix2D.Identity;
    private Matrix2D localMatrix = Matrix2D.Identity;
    private Matrix2D worldToLocalMatrix = Matrix2D.Identity;

    public Action? OnPropertyChanged;
#if DEBUG
    public Action? OnTreePrinted;
#endif

    public Vector2 Position 
    {
        get 
        {
            if (dirty)
                UpdateTransform();
            
            return position;
        }
        set 
        {
            if (parent == null) 
            {
                LocalPosition = value;
                return;
            }
            LocalPosition = MathUtils.Transform(value, WorldToLocalMatrix);
        }
    }

    public Vector2 LocalPosition 
    {
        get => localPosition;
        set 
        {
            if (localPosition != value) 
            {
                localPosition = value;
                SetDirty();
            }
        }
    }

    public float LocalPosX
    {
        get => localPosition.X;
        set => LocalPosition = new Vector2(value, localPosition.Y);
    }

    public float LocalPosY
    {
        get => localPosition.Y;
        set => LocalPosition = new Vector2(localPosition.X, value);
    }

    public float PosX
    {
        get => Position.X;
        set => Position = new Vector2(value, Position.Y);
    }

    public float PosY 
    {
        get => Position.Y;
        set => Position = new Vector2(Position.X, value);
    }

    public Vector2 Scale 
    {
        get 
        {
            if (dirty)
                UpdateTransform();
            return scale;
        }
        set 
        {
            if (parent == null) 
            {
                localScale = value;
                return;
            }
            if (parent.Scale.X == 0)
                value.X = 0;
            else
                value.X = value.X / parent.Scale.X;

            if (parent.Scale.Y == 0)
                value.Y = 0;
            else
                value.Y = value.Y / parent.Scale.Y;
            
            LocalScale = value;
        }
    }

    public Vector2 LocalScale 
    {
        get => localScale;
        set 
        {
            if (localScale != value) 
            {
                localScale = value;
                SetDirty();
            }
        }
    }

    public float Rotation
    {
        get 
        {
            if (dirty)
                UpdateTransform();
            return rotation;
        }
        set 
        {
            if (parent == null) 
            {
                LocalRotation = value;
                return;
            }
            LocalRotation = value - parent.Rotation;
        }
    }

    public float LocalRotation 
    {
        get => localRotation;
        set 
        {
            if (localRotation != value) 
            {
                localRotation = value;
                SetDirty();
            }
        }
    }

    public Vector2 PivotOffset 
    {
        get => pivotOffset;
        set 
        {
            if (pivotOffset != value) 
            {
                pivotOffset = value;
                SetDirty();
            }
        }
    }

    public float RotationDegrees 
    {
        get 
        {
            if (dirty)
                UpdateTransform();
            return rotation * MathUtils.Degrees;
        } 
        set 
        {
            if (parent == null) 
            {
                LocalRotation = value * MathUtils.Radians;
                return;
            }
            LocalRotation = (value * MathUtils.Radians) - (parent.Rotation * MathUtils.Radians); 
        }
    }

    public Matrix2D LocalMatrix 
    {
        get 
        {
            if (dirty)
                UpdateTransform();
            return localMatrix;
        }
    }

    public Matrix2D WorldMatrix 
    {
        get 
        {
            if (dirty)
                UpdateTransform();
            return worldMatrix;
        }
    }

    public Matrix2D WorldToLocalMatrix 
    {
        get 
        {
            if (dirty)
                UpdateTransform();
            return worldToLocalMatrix;
        }
    }

    public Transform? Parent 
    {
        get => parent;
        set => SetParent(value, false);
    } 

    public void SetParent(Transform? value, bool stay) 
    {
        if (parent == value)
            return;
        if (parent != null) 
        {
            parent.OnPropertyChanged -= SetDirty;
#if DEBUG
            parent.OnTreePrinted -= PrintTree;
#endif
        }

        var position = Position;
        var scale = Scale;
        var rotation = Rotation;


        parent = value;
        dirty = true;

        if (stay) 
        {
            Position = position;
            Scale = scale;
            Rotation = rotation;
        }

        if (parent != null) 
        {
            parent.OnPropertyChanged += SetDirty;
#if DEBUG
            parent.OnTreePrinted += PrintTree;
#endif
        }
        
        OnPropertyChanged?.Invoke();
    }

    public void SetDirty() 
    {
        if (dirty)
            return;
        dirty = true;
        OnPropertyChanged?.Invoke();
    }

    private void UpdateTransform() 
    {
        dirty = false;
        localMatrix = CreateMatrix(ref localPosition, ref pivotOffset, ref localScale, ref localRotation);

        if (parent == null) 
        {
            worldMatrix = localMatrix;
            worldToLocalMatrix = Matrix2D.Identity;
            position = localPosition;
            scale = localScale;
            rotation = localRotation;
            return;
        }

        worldMatrix = localMatrix * parent.WorldMatrix;
        Matrix2D.Invert(ref parent.worldMatrix, out worldToLocalMatrix);
        position = MathUtils.Transform(localPosition, parent.WorldMatrix);
        scale = localScale * parent.Scale;
        rotation = localRotation + parent.Rotation;
    }

    public void PrintTree() 
    {
#if DEBUG
        SkyLog.Print($"Transform \n L \t");
        OnTreePrinted?.Invoke();
#endif
    }

    public static Matrix2D CreateMatrix(ref Vector2 position, ref Vector2 origin, ref Vector2 scale, ref float rotation) 
    {
        Matrix2D result = Matrix2D.Identity;


        if (origin != Vector2.Zero)
            result =  Matrix2D.CreateTranslation(-origin.X, -origin.Y);

        if (scale != Vector2.One)
            result *= Matrix2D.CreateScale(scale.X, scale.Y);
        
        if (rotation != 0)
            result *= Matrix2D.CreateRotation(rotation);

        if (position != Vector2.Zero)
            result *= Matrix2D.CreateTranslation(position.X, position.Y);
        
        return result;
    }
}