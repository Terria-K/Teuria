using System;
using Microsoft.Xna.Framework;

namespace Teuria;

public class Actor : Entity, IPhysicsEntity
{
    public Collider Collider { get => Body.Collider; }

    public PhysicsBody Body;

    public PhysicsComponent PhysicsComponent { get => Body; } 
    private Vector2 remainder;
    public Vector2 Velocity; 

    protected void SetupHitbox(Hitbox hitbox) 
    {
        Body = new PhysicsBody(hitbox, true);
        AddComponent(Body);
    }

    public void OnCollisionX() 
    {
        Velocity.X = 0;
        remainder.X = 0;
    }

    public void OnCollisionY() 
    {
        Velocity.Y = 0;
        remainder.Y = 0;
    }

    public void MoveX(float amount, Action onCollide)
    {
        remainder.X += amount;
        int move = (int)Math.Round(remainder.X);
        if (move == 0)
        {
            return;
        }
        remainder.X -= move;
        MoveXExact(move, onCollide);
    }

    public void MoveY(float amount, Action onCollide)
    {
        remainder.Y += amount;
        int move = (int)Math.Round(remainder.Y);
        if (move == 0)
        {
            return;
        }
        remainder.Y -= move;
        MoveYExact(move, onCollide);
    }

    public void MoveXExact(float move, Action onCollide) 
    {
        int sign = Math.Sign(move);

        while (move != 0)
        {
            if (Body.CheckWallCollision(this, "Walls", new Vector2(sign, 0)))
            {
                onCollide?.Invoke();
                break;
            }
            Position.X += sign;
            move -= sign;
        }
    }

    public void MoveYExact(float move, Action onCollide) 
    {
        int sign = Math.Sign(move);

        while (move != 0)
        {
            if (Body.CheckWallCollision(this, "Walls", new Vector2(0, sign)))
            {
                onCollide?.Invoke();
                break;
            }
            Position.Y += sign;
            move -= sign;
        }
    }
}