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
    public float StartLife;
    public float LifeTime;
    public bool Emitting;

    public void Update() 
    {
        var dt = Time.Delta;
        var ease = LifeTime / StartLife;
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

        // float alpha = Material.FadeMode switch {
        //     ParticleMaterial.Fade.Linear => ease,
        //     ParticleMaterial.Fade.Late => Math.Min(1f, ease / .25f),
        //     ParticleMaterial.Fade.InAndOut => ease switch {
        //         > .75f => 1 - ((ease - .75f) / .25f),
        //         < .25f => ease / .25f,
        //         _ => 1f
        //     },
        //     _ => 1f
        // };

        float alpha;
        if (Material.FadeMode == ParticleMaterial.Fade.Linear)
            alpha = ease;
        else if (Material.FadeMode == ParticleMaterial.Fade.Late)
            alpha = Math.Min(1f, ease / .25f);
        else if (Material.FadeMode == ParticleMaterial.Fade.InAndOut)
        {
            if (ease > .75f)
                alpha = 1 - ((ease - .75f) / .25f);
            else if (ease < .25f)
                alpha = ease / .25f;
            else
                alpha = 1f;
        }
        else
            alpha = 1f;

        if (alpha == 0)
            Color = Color.Transparent;
        else if (alpha < 1f)
            Color *= alpha;
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