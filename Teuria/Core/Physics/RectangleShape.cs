using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class RectangleShape : Shape 
{
    public AABB BoundingArea => new AABB(GlobalX, GlobalY, Width, Height);

    public RectangleShape(float width, float height, Vector2 pos) 
    {
        Width = width;
        Height = height;
        Position = pos;        
    }
    
    public override bool Collide(RectangleShape other, Vector2 offset = default) 
    {
        return GlobalLeft + offset.X < other.GlobalRight && 
            GlobalRight + offset.X > other.GlobalLeft 
            && GlobalBottom + offset.Y > other.GlobalTop 
            && GlobalTop + offset.Y < other.GlobalBottom;
    }

    public override bool Collide(float x, float y, float width, float height, Vector2 offset = default) => 
        GlobalLeft + offset.X < x + width && 
        GlobalRight + offset.X > x && 
        GlobalBottom + offset.Y > y && 
        GlobalTop + offset.Y < y + height;

    public override bool Collide(Rectangle rect, Vector2 offset = default) =>
        GlobalLeft + offset.X < rect.Right &&
        GlobalRight + offset.X > rect.Left &&
        GlobalTop + offset.Y < rect.Bottom &&
        GlobalBottom + offset.Y > rect.Top;

    public override bool Collide(AABB aabb, Vector2 offset = default) =>
        GlobalLeft + offset.X < aabb.Right &&
        GlobalRight + offset.X > aabb.Left &&
        GlobalTop + offset.Y < aabb.Bottom &&
        GlobalBottom + offset.Y > aabb.Top;

    public override bool Collide(Point value)
    {
        if (GlobalX <= value.X && value.X < GlobalX + Width && GlobalY <= value.Y)
        {
            return value.Y < GlobalY + Height;
        }
        return false;
    }

    public override bool Collide(Vector2 value)
    {
        if (GlobalX <= value.X && value.X < GlobalX + Width && GlobalY <= value.Y)
        {
            return value.Y < GlobalY + Height;
        }
        return false;
    }

    public override bool Collide(TileGrid grid, Vector2 offset = default)
    {
        return grid.Collide(this, offset);
    }

    public static AABB Intersect(RectangleShape a, RectangleShape b) 
    {
        var left = MathHelper.Max(a.GlobalX, b.GlobalX);
        var top = MathHelper.Max(a.GlobalY, b.GlobalY);
        var right = MathHelper.Min(a.GlobalRight, b.GlobalRight);
        var bottom = MathHelper.Min(a.GlobalBottom, b.GlobalBottom);

        if (right < left || bottom < top) 
        {
            return default;
        }
        return new AABB(left, top, right - left, bottom - top);
    }

    public override bool Collide(CircleShape other, Vector2 offset = default)
    {
        return other.Collide(this);
    }

    public override void DebugDraw(SpriteBatch spriteBatch)
    {
        Canvas.DrawRect(spriteBatch, (int)GlobalX, (int)GlobalY, (int)Width, (int)Height, 1, Color.Red);
    }

    public override bool Collide(Colliders other, Vector2 offset = default)
    {
        return other.Collide(this);
    }

    public override Shape Clone()
    {
        return new RectangleShape(Width, Height, Position);
    }
}