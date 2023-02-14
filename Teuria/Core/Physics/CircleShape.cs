using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class CircleShape : Shape
{
    public Circle BoundingShape;
    public new Vector2 Position;
    public float Radius => BoundingShape.Radius;
    public override AABB BoundingArea { get; }

    public CircleShape(float radius, Vector2 offset) 
    {
        Position = offset;
        BoundingShape = new Circle(offset.X, offset.Y, radius);
    }

    // TODO: Do not update the position per colliding, but keeping the collision correct
    public override bool Collide(float x, float y, float width, float height, Vector2 offset = default)
    {
        BoundingShape.Position = Entity.Position + Position;
        return BoundingShape.Contains(x + offset.X, y + offset.Y, width, height);
    }

    public override bool Collide(RectangleShape other, Vector2 offset = default)
    {
        BoundingShape.Position = Entity.Position + Position;
        return BoundingShape.Contains(other.GlobalX + offset.X, other.GlobalY + offset.Y, other.Width, other.Height);
    }

    public override bool Collide(TileGrid grid, Vector2 offset = default)
    {
        return false;
        // throw new System.NotImplementedException();
    }

    public override bool Collide(Rectangle rect, Vector2 offset = default)
    {
        BoundingShape.Position = Entity.Position + Position;
        var otherRect = new Rectangle(rect.X + (int)offset.X, rect.Y + (int)offset.Y, rect.Width, rect.Height);
        return BoundingShape.Contains(otherRect);
    }

    public override bool Collide(AABB aabb, Vector2 offset = default)
    {
        BoundingShape.Position = Entity.Position + Position;
        var otherAABB = new AABB(aabb.X + offset.X, aabb.Y + offset.Y, aabb.Width, aabb.Height);
        return BoundingShape.Contains(otherAABB);
    }

    public override bool Collide(Point value)
    {
        BoundingShape.Position = Entity.Position + Position;
        return BoundingShape.Contains(value.X, value.Y);
    }

    public override bool Collide(Vector2 value)
    {
        BoundingShape.Position = Entity.Position + Position;
        return BoundingShape.Contains(value.X, value.Y);
    }

    public override bool Collide(CircleShape other, Vector2 offset = default)
    {
        BoundingShape.Position = Entity.Position + Position;
        return Vector2.DistanceSquared(GlobalPosition, other.GlobalPosition) < (Radius + other.Radius) * (Radius + other.Radius);
    }

    public override void DebugDraw(SpriteBatch spriteBatch)
    {
        Canvas.DrawCircle(spriteBatch, Entity.Position + Position, Radius, Color.Red, 4);
    }
}