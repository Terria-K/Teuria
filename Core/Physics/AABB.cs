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
        get => X;
        set => X = value;
    }

    public float Right 
    {
        get => X + Width;
        set => X = value - Width;
    }

    public float Top 
    {
        get => Y;
        set => Y = value;
    }

    public float Bottom 
    {
        get => Y + Height;
        set => Y = value - Height;
    }

    public Vector2 Position 
    {
        get => new Vector2(X, Y);
        set 
        {
            X = value.X;
            Y = value.Y;
        }
    }

    public Vector2 Size 
    {
        get => new Vector2(Width, Height);
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

    public bool Contains(AABB other) => 
        Left < other.Right &&
        Right > other.Left &&
        Bottom > other.Top &&
        Top < other.Bottom;


    public bool Contains(Vector2 value) => 
        X <= value.X &&
        value.X < X + Width &&
        Y <= value.Y &&
        value.Y < Y + Height;
    
    public bool Contains(float x, float y) => 
        X <= x &&
        x < X + Width &&
        Y <= y &&
        y < Y + Height;

    public bool Contains(ref Vector2 value) => 
        X <= value.X &&
        value.X < X + Width &&
        Y <= value.Y &&
        value.Y < Y + Height;

    public bool Overlaps(AABB other) =>
        !(Width < X || X > other.Width) && 
        !(Height < other.Y || Y > other.Height);

    public bool Equals(AABB one, AABB two) =>
        one.X == two.X && 
        one.Y == two.Y &&
        one.Width == two.Width &&
        one.Height == two.Height;

    public int GetHashCode([DisallowNull] AABB obj)
    {
        throw new System.NotImplementedException();
    }

    public static implicit operator Rectangle(AABB other) 
    {
        return new Rectangle((int)other.X, (int)other.Y, (int)other.Width, (int)other.Height);
    }
}