using Microsoft.Xna.Framework;

namespace Teuria;

public abstract class PhysicsComponent : Component
{
    public bool Collideable;
    
    private readonly Collider collider;

    public Collider Collider => collider;

    public PhysicsComponent(Collider collider, bool collidable = true) 
    {
        Collideable = collidable;
        this.collider = collider;
    }

    public override void Added(Entity entity) 
    {
        base.Added(entity);
        collider.Entity = entity;
    }

    public override void Removed() 
    {
        if (collider != null)
            collider.Entity = null;
        base.Removed();
    }
    
}