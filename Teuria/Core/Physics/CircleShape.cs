using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class CircleShape : Shape
{
    public Circle BoundingArea => new Circle(Entity.Position.X + Position.X, Entity.Position.Y + Position.Y, circleShape.Radius);
    private Circle circleShape;
    public new Vector2 Position;
    public float Radius => BoundingArea.Radius;

    public CircleShape(float radius, Vector2 offset) 
    {
        Position = offset;
        circleShape = new Circle(offset.X, offset.Y, radius);
    }

    public override bool Collide(float x, float y, float width, float height, Vector2 offset = default)
    {
        return BoundingArea.Contains(x + offset.X, y + offset.Y, width, height);
    }

    public override bool Collide(RectangleShape other, Vector2 offset = default)
    {
        return BoundingArea.Contains(other.GlobalX + offset.X, other.GlobalY + offset.Y, other.Width, other.Height);
    }

    public override bool Collide(TileGrid grid, Vector2 offset = default)
    {
        return false;
        // throw new System.NotImplementedException();
    }

    public override bool Collide(Rectangle rect, Vector2 offset = default)
    {
        var otherRect = new Rectangle(rect.X + (int)offset.X, rect.Y + (int)offset.Y, rect.Width, rect.Height);
        return BoundingArea.Contains(otherRect);
    }

    public override bool Collide(AABB aabb, Vector2 offset = default)
    {
        var otherAABB = new AABB(aabb.X + offset.X, aabb.Y + offset.Y, aabb.Width, aabb.Height);
        return BoundingArea.Contains(otherAABB);
    }

    public override bool Collide(Point value)
    {
        return BoundingArea.Contains(value.X, value.Y);
    }

    public override bool Collide(Vector2 value)
    {
        return BoundingArea.Contains(value.X, value.Y);
    }

    public override bool Collide(CircleShape other, Vector2 offset = default)
    {
        return Vector2.DistanceSquared(GlobalPosition, other.GlobalPosition) < (Radius + other.Radius) * (Radius + other.Radius);
    }

    public override void DebugDraw(SpriteBatch spriteBatch)
    {
        Canvas.DrawCircle(spriteBatch, Entity.Position + Position, Radius, Color.Red, 4);
    }
}