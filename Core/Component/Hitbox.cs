using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Hitbox : Component 
{
    private float width;
    private float height;
    private Vector2 position;
    private AABB boundingBox;
#if DEBUG
    public static bool DebugRender = false;
#endif

    public Hitbox(float width, float height, Vector2 pos) 
    {
        this.width = width;
        this.height = height;
        position = pos;        
        boundingBox = new AABB(pos.X, pos.Y, width, height);
    }

    public AABB BoundingArea 
    {
        get => boundingBox;
        set => boundingBox = value;
    }

    
    public float Width 
    {
        get => width;
        set => width = value;
    }

    public float Height 
    {
        get => height;
        set => height = value;
    }
    
    public Vector2 Position 
    {
        get => position;
        set => position = value;
    }

    public float X 
    {
        get => Entity.Position.X + position.X;        
        set => position.Y = value;
    }

    public float Y 
    {
        get => Entity.Position.Y + position.Y;
        set => position.Y = value;
    }

    public float Left 
    {
        get => position.X;
        set => position.X = value;
    }

    public float Right 
    {
        get => position.X + Width;
        set => position.X = value - Width;
    }

    public float Bottom 
    {
        get => position.Y + Height;
        set => position.Y = value - Height;
    }

    public float Top 
    {
        get => position.Y;
        set => position.Y = value;
    }

    public float GlobalX 
    {
        get => Entity.Position.X + Position.X;
    }

    public float GlobalY
    {
        get => Entity.Position.Y + Position.Y;
    }

    public Vector2 GlobalPosition 
    {
        get => new Vector2(GlobalX, GlobalY);
    }

    public float GlobalLeft 
    {
        get => Left + Entity.Position.X;
    }

    public float GlobalRight 
    {
        get => Right + Entity.Position.X;
    }

    public float GlobalBottom 
    {
        get => Bottom + Entity.Position.Y;
    }

    public float GlobalTop 
    {
        get => Top + Entity.Position.Y;
    }

#if DEBUG
    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!DebugRender) return;
        Canvas.DrawRectangle(spriteBatch, GlobalX, GlobalY, width, height, Color.Red);
        base.Draw(spriteBatch);
    }
#endif

    public bool Collide(Hitbox other) 
    {
        return GlobalLeft < other.GlobalRight && 
            GlobalRight > other.GlobalLeft 
            && GlobalBottom > other.GlobalTop 
            && GlobalTop < other.GlobalBottom;
    }

    
    public bool Collide(float x, float y, float width, float height) 
    {
        return Left < x + width && Right > x && Bottom > y && Top < y + height;
    }

    public bool Collide(Point value)
    {
        if (X <= value.X && value.X < X + Width && Y <= value.Y)
        {
            return value.Y < Y + Height;
        }
        return false;
    }

    public bool Collide(EntityPoint entityPoint) 
    {
        if (X <= entityPoint.X && entityPoint.X < X + Width && Y <= entityPoint.Y) 
        {
            return entityPoint.Y < Y + height;
        }
        return false;
    }

    public bool Collide(Vector2 value)
    {
        if (X <= value.X && value.X < X + Width && Y <= value.Y)
        {
            return value.Y < Y + Height;
        }
        return false;
    }
}