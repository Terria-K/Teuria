using Microsoft.Xna.Framework;

namespace Teuria;

public struct Circle 
{
    public Vector2 Position;
    public float Radius;
    public float MinRadius;

    public Circle(float x, float y, float radius, float minRadius) 
    {
        Position = new Vector2(x, y);
        Radius = radius;
        MinRadius = minRadius;
    }

    public Circle(float x, float y, float radius) 
    {
        Position = new Vector2(x, y);
        Radius = radius;
        MinRadius = 0f;
    }

    public bool Contains(float x, float y) 
    {
        if (Radius <= 0f)
            return false;
        
        var r = Radius * Radius;
        var nx = Position.X - x;
        var ny = Position.Y - y;
        nx *= nx;
        ny *= ny;
        return (nx + ny <= r);
    }

    public static implicit operator Rectangle(Circle circ) => 
        new Rectangle((int)(circ.Position.X - circ.Radius), (int)(circ.Position.Y - circ.Radius),
                    (int)(circ.Radius * 2), (int)(circ.Radius * 2));
}