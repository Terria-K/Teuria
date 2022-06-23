using Microsoft.Xna.Framework;

namespace Teuria;

public class Actor : Entity, IPhysicsEntity
{
    public Hitbox Hitbox { get; private set; }

    public AABB BoundingArea => new AABB(Hitbox.GlobalX, Hitbox.GlobalY, Hitbox.Width, Hitbox.Height);

    protected bool highlight;


    public bool Highlight 
    {
        get => highlight;
        set => highlight = value;
    }

    protected void SetupHitbox(Hitbox hitbox) 
    {
        Hitbox = hitbox;
        AddComponent(this.Hitbox);
        // BoundingArea = new AABB(hitbox.X, hitbox.Y, hitbox.Width, hitbox.Height);
    }

    
    public bool Intersects(AABB hitbox) 
    {
        return BoundingArea.Contains(hitbox);
    }

    public bool Collide(Hitbox hitbox)
    {
        throw new System.NotImplementedException();
    }

    public bool Collide(Rectangle boundingBox)
    {
        throw new System.NotImplementedException();
    }

    public int Width 
    {
        get => (int)Hitbox.Width;
    }

    public int Height 
    {
        get => (int)Hitbox.Height;
    }
}