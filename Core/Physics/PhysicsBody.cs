using System;
using Microsoft.Xna.Framework;

namespace Teuria;

public class PhysicsBody : PhysicsComponent
{
    private bool isOnFloor;
    public bool IsOnFloor => isOnFloor;

    public PhysicsBody(Collider collider, bool collidable) : base(collider, collidable)
    {
    }

    public bool IsTouchingUp(AABB other) => 
        Collider.GlobalLeft < other.Right &&
        Collider.GlobalRight > other.Left &&
        Collider.GlobalBottom > other.Top &&
        Collider.GlobalTop < other.Top;

    public bool Intersect(AABB other, Vector2 offset) =>
        Collider.GlobalLeft + offset.X < other.Right &&
        Collider.GlobalRight + offset.X > other.Left &&
        Collider.GlobalBottom + offset.Y > other.Top &&
        Collider.GlobalTop + offset.Y < other.Bottom;

    public void CollidingWith(AABB other) 
    {
        isOnFloor = false;
        if (IsTouchingUp(other)) 
        {
            isOnFloor = true;
        }
    }
    
}