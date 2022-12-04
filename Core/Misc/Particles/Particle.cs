using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public struct Particle 
{
    public Entity Follow;
    public SpriteTexture Source;
    public ParticleMaterial Material;

    public Vector2 Position;
    public Vector2 Velocity;
    public Color StartColor;
    public Color Color;
    public float Angle;
    public float AngularVelocity;
    public float Scale;
    public float StartSize;
    public float Rotation;
    public float Spin;
    public float LifeTime;
    public bool Emitting;

    public void Update() 
    {
        var dt = TeuriaEngine.DeltaTime;
        LifeTime -= dt;
        if (LifeTime < 0f) 
        {
            Emitting = false;
        }
        Position += Velocity * dt;
        Velocity += Material.Acceleration * dt;
        Velocity = MathUtils.MoveTowards(Velocity, Vector2.Zero, Material.Friction * dt);
        Rotation += Spin * dt;
        Angle += AngularVelocity;
    }

    public void Draw(SpriteBatch spriteBatch) 
    {
        var pos = Position.ToInt();
        if (Follow != null)
            pos += Follow.Position;
        Rectangle srcRect = new Rectangle(Source.X, Source.Y, Source.Width, Source.Height);
        Vector2 offset = new Vector2(Source.Width / 2f, Source.Height / 2f);

        Source.DrawTexture(spriteBatch, pos, srcRect, Color, Rotation, offset, Scale, SpriteEffects.None, 0);
    }
}