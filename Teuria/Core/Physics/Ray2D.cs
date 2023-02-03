using System;
using Microsoft.Xna.Framework;

namespace Teuria;

[Flags]
public enum PointSectors { Inside, Left, Right, Bottom, Top }

public struct Ray2D 
{
    public Vector2 Direction;
    public Vector2 Position;

    public Ray2D(Vector2 position, Vector2 direction) 
    {
        Position = position;
        Direction = direction;
    }

    public Vector2 Cast(Vector2 from, Vector2 to) 
    {
        var x = Position.X + Direction.X;
        var y = Position.Y + Direction.Y;

        var den = (from.X - to.X) * (Position.Y - y) - (from.Y - to.Y) * (Position.X - x);
        if (den == 0)
            return Vector2.Zero;

        var t = ((from.X - Position.X) * (Position.Y - y) - (from.Y - Position.Y) * (Position.X - x)) / den;
        var u = -((from.X - to.X) * (from.Y - Position.Y) - (from.Y - to.Y) * (from.X - Position.X)) / den;
        if (t > 0 && t < 1 && u > 0)
            return new Vector2(from.X + t * (to.X - from.X), from.Y + t * (to.Y - from.Y));

        return Vector2.Zero;
    }

    // public static bool LineCheck(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    // {
    //     Vector2 b = a2 - a1;
    //     Vector2 d = b2 - b1;
    //     float bDotDPerp = b.X * d.Y - b.Y * d.X;

    //     // if b dot d == 0, it means the lines are parallel so have infinite intersection points
    //     if (bDotDPerp == 0)
    //         return false;

    //     Vector2 c = b1 - a1;
    //     float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
    //     if (t < 0 || t > 1)
    //         return false;

    //     float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
    //     if (u < 0 || u > 1)
    //         return false;

    //     return true;
    // } 

    public Vector2 Cast(RectangleShape rectShape, Vector2 from, Vector2 to) 
    {
        var aabb = new AABB(rectShape.GlobalX, rectShape.GlobalY, rectShape.Width, rectShape.Height);
        return Cast(aabb, from, to);
    }

    // public bool Cast(RectangleShape rectShape, Vector2 from, Vector2 to) 
    // {
    //     var aabb = new AABB(rectShape.GlobalX, rectShape.GlobalY, rectShape.Width, rectShape.Height);
    //     return Cast(aabb, from, to);
    // }

    // public bool Cast(AABB rect, Vector2 from, Vector2 to) 
    // {
    //     var sector0 = ComputeSector(rect, from);
    //     var sector1 = ComputeSector(rect, to);

    //     if ((sector0 & sector1) != 0)
    //         return false;
    //     else if (sector0 == PointSectors.Inside || sector1 == PointSectors.Inside)
    //         return true;
    //     else 
    //     {
    //         var both = sector0 | sector1;

    //         Vector2 edgeFrom, edgeTo;

    //         if ((both & PointSectors.Left) != 0) 
    //         {
    //             edgeFrom = new Vector2(rect.X, rect.Y);
    //             edgeTo = new Vector2(rect.X, rect.Y + rect.Height);
    //             return LineCheck(edgeFrom, edgeTo, from, to);
    //         }

    //         if ((both & PointSectors.Right) != 0) 
    //         {
    //             edgeFrom = new Vector2(rect.X + rect.Width, rect.Y);
    //             edgeTo = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);
    //             return LineCheck(edgeFrom, edgeTo, from, to);
    //         }

    //         if ((both & PointSectors.Top) != 0) 
    //         {
    //             edgeFrom = new Vector2(rect.X, rect.Y);
    //             edgeTo = new Vector2(rect.X + rect.Width, rect.Y);
    //             return LineCheck(edgeFrom, edgeTo, from, to);
    //         }

    //         if ((both & PointSectors.Bottom) != 0) 
    //         {
    //             edgeFrom = new Vector2(rect.X, rect.Y + rect.Height);
    //             edgeTo = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);
    //             return LineCheck(edgeFrom, edgeTo, from, to);
    //         }
    //     }
    //     return false;
    // }

    public Vector2 Cast(AABB rect, Vector2 from, Vector2 to) 
    {
        var sector0 = ComputeSector(rect, from);
        var sector1 = ComputeSector(rect, to);

        if ((sector0 & sector1) != 0)
            return Vector2.Zero;
        else if (sector0 == PointSectors.Inside || sector1 == PointSectors.Inside)
            return Vector2.Zero;
        else 
        {
            var both = sector0 | sector1;

            Vector2 edgeFrom, edgeTo;

            if ((both & PointSectors.Left) != 0) 
            {
                edgeFrom = new Vector2(rect.X, rect.Y);
                edgeTo = new Vector2(rect.X, rect.Y + rect.Height);
                return Cast(edgeFrom, edgeTo);
            }

            if ((both & PointSectors.Right) != 0) 
            {
                edgeFrom = new Vector2(rect.X + rect.Width, rect.Y);
                edgeTo = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);
                return Cast(edgeFrom, edgeTo);
            }

            if ((both & PointSectors.Top) != 0) 
            {
                edgeFrom = new Vector2(rect.X, rect.Y);
                edgeTo = new Vector2(rect.X + rect.Width, rect.Y);
                return Cast(edgeFrom, edgeTo);
            }

            if ((both & PointSectors.Bottom) != 0) 
            {
                edgeFrom = new Vector2(rect.X, rect.Y + rect.Height);
                edgeTo = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);
                return Cast(edgeFrom, edgeTo);
            }
        }
        return Vector2.Zero;
    }

    private static PointSectors ComputeSector(AABB rect, Vector2 line) 
    {
        var sector = PointSectors.Inside;
        if (line.X < rect.X)
            sector |= PointSectors.Left;
        else if (line.X >= rect.X + rect.Width)
            sector |= PointSectors.Right;
        if (line.Y < rect.Y)
            sector |= PointSectors.Bottom;
        else if (line.Y >= rect.Y + rect.Height)
            sector |= PointSectors.Top;
        return sector;
    }
}