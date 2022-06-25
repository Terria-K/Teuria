using System;
using System.Collections.Generic;

namespace Teuria;

public class Actor : Entity, IPhysicsEntity
{
    public HashSet<IPhysicsEntity> Collided = new HashSet<IPhysicsEntity>();
    public Collider Collider { get; set; }

    public PhysicsBody Body;

    public AABB BoundingArea => new AABB(Collider.GlobalX, Collider.GlobalY, Collider.Width, Collider.Height);

    protected void SetupHitbox(Hitbox hitbox) 
    {
        Collider = hitbox;
        Body = new PhysicsBody(Collider, true);
        AddComponent(Body);
    }
    
    public void Detect(HashSet<IPhysicsEntity> entity)
    {
        Collided = entity;
        // Collided.Add(entity);
        // Collision?.Invoke(entity);
    }
}