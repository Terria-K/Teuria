using System;
using Microsoft.Xna.Framework;

namespace Teuria;

public class PhysicsBody : PhysicsComponent
{
    private Vector2 velocity;
    private Vector2 remainder;

    public PhysicsBody(Collider collider, bool collidable) : base(collider, collidable) {}

    public Vector2 MoveAndSlide(Vector2 motion, Vector2 direction) 
    {
        velocity = motion;
        velocity.X += direction.X;
        velocity.Y -= direction.Y;
        MoveX(velocity.X * TeuriaEngine.DeltaTime, OnCollisionX);
        MoveY(velocity.Y * TeuriaEngine.DeltaTime, OnCollisionY);
        return velocity;
    }

    private void OnCollisionX() 
    {
        velocity.X = 0;
        remainder.X = 0;
    }

    private void OnCollisionY() 
    {
        velocity.Y = 0;
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

    private void MoveXExact(float move, Action onCollide) 
    {
        int sign = Math.Sign(move);

        while (move != 0)
        {
            if (Check(PhysicsEntity, "Walls", new Vector2(sign, 0)))
            {
                onCollide?.Invoke();
                break;
            }
            Entity.Position.X += sign;
            move -= sign;
        }
    }

    private void MoveYExact(float move, Action onCollide) 
    {
        int sign = Math.Sign(move);

        while (move != 0)
        {
            if (Check(PhysicsEntity, "Walls", new Vector2(0, sign)))
            {
                onCollide?.Invoke();
                break;
            }
            Entity.Position.Y += sign;
            move -= sign;
        }
    }
}