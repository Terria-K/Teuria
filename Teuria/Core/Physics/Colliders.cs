using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Colliders : Shape
{
    public WeakList<Shape> Shapes { get; private set; }

    public Colliders() 
    {
        Shapes = new WeakList<Shape>();
    }
    
    public Colliders(params Shape[] shapes) 
    {
        Shapes = new WeakList<Shape>();
        foreach (var shape in shapes) 
        {
            if (shape != null)
                Shapes.Add(shape);
        }
    }

    public override float Width 
    { 
        get => Right - Left; 
    }

    public override float Height 
    { 
        get => Bottom - Top; 
    }

    public override float Left
    {
        get 
        {
            float left = Shapes[0].Left;
            for (int i = 0; i < Shapes.Count; i++) 
            {
                if (Shapes[i].Left > left)
                    left = Shapes[i].Left;
            }
            return left;
        }
        set 
        {
            float changeX = value - Left;
            for (int i = 0; i < Shapes.Count; i++) 
            {
                Position = new Vector2(Position.X + changeX, Position.Y);
            }
        }
    }

    public override float Right
    {
        get 
        {
            float right = Shapes[0].Right;
            for (int i = 0; i < Shapes.Count; i++) 
            {
                if (Shapes[i].Right > right)
                    right = Shapes[i].Right;
            }
            return right;
        }
        set 
        {
            float changeX = value - Right;
            for (int i = 0; i < Shapes.Count; i++) 
            {
                Position = new Vector2(Position.X + changeX, Position.Y);
            }
        }
    }

    public override float Top 
    {
        get 
        {
            float top = Shapes[0].Top;
            for (int i = 0; i < Shapes.Count; i++) 
            {
                if (Shapes[i].Top > top)
                    top = Shapes[i].Top;
            }
            return top;
        }
        set 
        {
            float changeY = value - Top;
            for (int i = 0; i < Shapes.Count; i++) 
            {
                Position = new Vector2(Position.X, Position.Y + changeY);
            }
        }
    }

    public override float Bottom 
    {
        get 
        {
            float bottom = Shapes[0].Bottom;
            for (int i = 0; i < Shapes.Count; i++) 
            {
                if (Shapes[i].Bottom > bottom)
                    bottom = Shapes[i].Bottom;
            }
            return bottom;
        }
        set 
        {
            float changeY = value - Bottom;
            for (int i = 0; i < Shapes.Count; i++) 
            {
                Position = new Vector2(Position.X, Position.Y + changeY);
            }
        }
    }

    public void AddShape(Shape shape) 
    {
        if (Entity != null)
            shape.Added(Entity);
        Shapes.Add(shape);
    }

    public void RemoveShape(Shape shape) 
    {
        shape.Removed();
        Shapes.Remove(shape);
    }

    public void RemoveShape(int i) 
    {
        Shapes[i].Removed();
        Shapes.RemoveAt(i);
    }

    public bool ContainShape(Shape shape) 
    {
        return Shapes.Contains(shape);
    }

    internal override void Added(Entity entity) 
    {
        base.Added(entity);
        for (int i = 0; i < Shapes.Count; i++) 
        {
            if (Entity != null)
                Shapes[i].Added(Entity);
        }
    }

    internal override void Removed() 
    {
        base.Removed();
        for (int i = 0; i < Shapes.Count; i++) 
        {
            Shapes[i].Removed();
        }
    }

    public override bool Collide(float x, float y, float width, float height, Vector2 offset = default)
    {
        for (int i = 0; i < Shapes.Count; i++)
        {
            if (Shapes[i].Collide(x, y, width, height, offset))
                return true;
        }
        return false;
    }

    public override bool Collide(RectangleShape other, Vector2 offset = default)
    {
        for (int i = 0; i < Shapes.Count; i++)
        {
            if (Shapes[i].Collide(other, offset))
                return true;
        }
        return false;
    }

    public override bool Collide(TileGrid grid, Vector2 offset = default)
    {
        for (int i = 0; i < Shapes.Count; i++)
        {
            if (Shapes[i].Collide(grid, offset))
                return true;
        }
        return false;
    }

    public override bool Collide(CircleShape other, Vector2 offset = default)
    {
        for (int i = 0; i < Shapes.Count; i++)
        {
            if (Shapes[i].Collide(other, offset))
                return true;
        }
        return false;
    }

    public override bool Collide(Rectangle rect, Vector2 offset = default)
    {
        for (int i = 0; i < Shapes.Count; i++)
        {
            if (Shapes[i].Collide(rect, offset))
                return true;
        }
        return false;
    }

    public override bool Collide(AABB aabb, Vector2 offset = default)
    {
        for (int i = 0; i < Shapes.Count; i++)
        {
            if (Shapes[i].Collide(aabb, offset))
                return true;
        }
        return false;
    }

    public override bool Collide(Point value)
    {
        for (int i = 0; i < Shapes.Count; i++)
        {
            if (Shapes[i].Collide(value))
                return true;
        }
        return false;
    }

    public override bool Collide(Vector2 value)
    {
        for (int i = 0; i < Shapes.Count; i++)
        {
            if (Shapes[i].Collide(value))
                return true;
        }
        return false;
    }

    public override bool Collide(Colliders other, Vector2 offset = default)
    {
        for (int i = 0; i < Shapes.Count; i++) 
        {
            if (Shapes[i].Collide(other))
                return true;
        }
        return false;
    }

    public override void DebugDraw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < Shapes.Count; i++) 
        {
            Shapes[i].DebugDraw(spriteBatch);
        }
    }

    public override Shape Clone()
    {
        var clones = new Shape[Shapes.Count];
        for (int i = 0; i < Shapes.Count; i++)
            clones[i] = Shapes[i].Clone();

        return new Colliders(clones);
    }
}
