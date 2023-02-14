using Microsoft.Xna.Framework;

namespace Teuria;

public struct Circle 
{
    public Vector2 Position;
    public float Radius;

    public Circle(float x, float y, float radius) 
    {
        Position = new Vector2(x, y);
        Radius = radius;
    }

    public bool Contains(float x, float y) 
    {
        return Vector2.DistanceSquared(Position, new Vector2(x, y)) <= Radius * Radius;
    }

    public bool Contains(Vector2 from, Vector2 to) 
    {
        return Vector2.DistanceSquared(Position, MathUtils.ClosestPointOnLine(from, to, Position)) < Radius * Radius;
    }

    public bool Contains(float rX, float rY, float rW, float rH) 
    {
        if (Position.X >= rX && Position.Y >= rY && Position.X < rX + rW && Position.Y < rY + rH)
            return true;
        
        var from = Vector2.Zero;
        var to = Vector2.Zero;
        var sector = MathUtils.GetSector(rX, rY, rW, rH, Position);

        if ((sector & MathUtils.SectorTop) != 0)
        {
            from = new Vector2(rX, rY);
            to = new Vector2(rX + rW, rY);
            if (Contains(from, to))
                return true;
        }

        if ((sector & MathUtils.SectorBottom) != 0)
        {
            from = new Vector2(rX, rY + rH);
            to = new Vector2(rX + rW, rY + rH);
            if (Contains(from, to))
                return true;
        }

        if ((sector & MathUtils.SectorLeft) != 0)
        {
            from = new Vector2(rX, rY);
            to = new Vector2(rX, rY + rH);
            if (Contains(from, to))
                return true;
        }

        if ((sector & MathUtils.SectorRight) != 0)
        {
            from = new Vector2(rX + rW, rY);
            to = new Vector2(rX + rW, rY + rH);
            if (Contains(from, to))
                return true;
        }

        return false;
    }

    public bool Contains(AABB aabb) 
    {
        return Contains(aabb.X, aabb.Y, aabb.Width, aabb.Height);
    }

    public bool Contains(Rectangle aabb) 
    {
        return Contains(aabb.X, aabb.Y, aabb.Width, aabb.Height);
    }

    public static implicit operator Rectangle(Circle circ) => 
        new Rectangle((int)(circ.Position.X - circ.Radius), (int)(circ.Position.Y - circ.Radius),
                    (int)(circ.Radius * 2), (int)(circ.Radius * 2));
}