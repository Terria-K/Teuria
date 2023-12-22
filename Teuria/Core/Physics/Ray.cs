using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class RayShape : Shape
{
    public override Shape Clone()
    {
        throw new System.NotImplementedException();
    }

    public override bool Collide(float x, float y, float width, float height, Vector2 offset = default)
    {
        throw new System.NotImplementedException();
    }

    public override bool Collide(RectangleShape other, Vector2 offset = default)
    {
        throw new System.NotImplementedException();
    }

    public override bool Collide(TileGrid grid, Vector2 offset = default)
    {
        throw new System.NotImplementedException();
    }

    public override bool Collide(CircleShape other, Vector2 offset = default)
    {
        throw new System.NotImplementedException();
    }

    public override bool Collide(Colliders other, Vector2 offset = default)
    {
        throw new System.NotImplementedException();
    }

    public override bool Collide(Rectangle rect, Vector2 offset = default)
    {
        throw new System.NotImplementedException();
    }

    public override bool Collide(AABB aabb, Vector2 offset = default)
    {
        throw new System.NotImplementedException();
    }

    public override bool Collide(Point value)
    {
        throw new System.NotImplementedException();
    }

    public override bool Collide(Vector2 value)
    {
        throw new System.NotImplementedException();
    }

    public override void DebugDraw(SpriteBatch spriteBatch)
    {
        throw new System.NotImplementedException();
    }
}