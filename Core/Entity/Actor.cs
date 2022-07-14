using System;
using Microsoft.Xna.Framework;

namespace Teuria;

public class Actor : Entity, IPhysicsEntity
{
    public Shape Collider { get => Body.Collider; }

    public PhysicsBody Body;

    public PhysicsComponent PhysicsComponent { get => Body; } 
    public Vector2 Velocity; 

    protected void SetupHitbox(RectangleShape hitbox) 
    {
        Body = new PhysicsBody(hitbox, true);
        AddComponent(Body);
    }
}