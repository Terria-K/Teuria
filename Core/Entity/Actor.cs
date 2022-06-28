using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Teuria;

public class Actor : Entity, IPhysicsEntity
{
    public Collider Collider { get => Body.Collider; }

    public PhysicsBody Body;

    public PhysicsComponent PhysicsComponent { get => Body; } 

    protected void SetupHitbox(Hitbox hitbox) 
    {
        Body = new PhysicsBody(hitbox, true);
        AddComponent(Body);
    }
}