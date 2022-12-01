using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class RectangleShape : Shape 
{
#if DEBUG
    public static bool DebugRender = false;

    public override AABB BoundingArea => new AABB(GlobalX, GlobalY, Width, Height);
#endif

    public RectangleShape(float width, float height, Vector2 pos) 
    {
        Width = width;
        Height = height;
        Position = pos;        
    }
    

#if DEBUG
    public void Draw(SpriteBatch spriteBatch, Color color)
    {
        if (!DebugRender || !IsInTheWorld) return;
        Canvas.DrawRect(spriteBatch, (int)GlobalX, (int)GlobalY, (int)Width, (int)Height, 1, color);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Draw(spriteBatch, Color.Red);
    }
#endif

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
}