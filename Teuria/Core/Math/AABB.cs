using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace Teuria;

public struct AABB : IEqualityComparer<AABB>
{
    public float X;
    public float Y;
    public float Width;
    public float Height;

    public float Left 
    {
        readonly get => X;
        set => X = value;
    }

    public float Right 
    {
        readonly get => X + Width;
        set => X = value - Width;
    }

    public float Top 
    {
        readonly get => Y;
        set => Y = value;
    }

    public float Bottom 
    {
        readonly get => Y + Height;
        set => Y = value - Height;
    }

    public Vector2 Position 
    {
        readonly get => new(X, Y);
        set 
        {
            X = value.X;
            Y = value.Y;
        }
    }

    public Vector2 Size 
    {
        readonly get => new(Width, Height);
        set 
        {
            Width = value.X;
            Height = value.Y;
        }
    }

    public AABB(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public AABB(Vector2 pos, Vector2 size) 
    {
        X = pos.X;
        Y = pos.Y;
        Width = size.X;
        Height = size.Y;
    }

    public readonly bool Contains(AABB other, Vector2 offset = default) => 
        Left + offset.X < other.Right &&
        Right > other.Left &&
        Bottom > other.Top &&
        Top + offset.Y < other.Bottom;


    public readonly bool Contains(Vector2 value) => 
        X <= value.X &&
        value.X < X + Width &&
        Y <= value.Y &&
        value.Y < Y + Height;
    
    public readonly bool Contains(float x, float y) => 
        X <= x &&
        x < X + Width &&
        Y <= y &&
        y < Y + Height;

    public readonly bool Contains(ref Vector2 value) => 
        X <= value.X &&
        value.X < X + Width &&
        Y <= value.Y &&
        value.Y < Y + Height;

    public readonly bool Overlaps(AABB other) =>
        !(Width < X || X > other.Width) && 
        !(Height < other.Y || Y > other.Height);

    public readonly bool Equals(AABB one, AABB two) =>
        one.X == two.X && 
        one.Y == two.Y &&
        one.Width == two.Width &&
        one.Height == two.Height;

    public static AABB Intersect(AABB a, AABB b) 
    {
        var left = MathHelper.Max(a.X, b.X);
        var top = MathHelper.Max(a.Y, b.Y);
        var right = MathHelper.Min(a.Right, b.Right);
        var bottom = MathHelper.Min(a.Bottom, b.Bottom);

        if (right < left || bottom < top) 
        {
            return default;
        }
        return new AABB(left, top, right - left, bottom - top);
    }

    public int GetHashCode([DisallowNull] AABB obj)
    {
        return obj.GetHashCode();
    }

    public static implicit operator Rectangle(AABB other) 
    {
        return new Rectangle((int)other.X, (int)other.Y, (int)other.Width, (int)other.Height);
    }
}