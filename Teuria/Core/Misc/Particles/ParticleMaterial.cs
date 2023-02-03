using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Teuria;

public class ParticleMaterial 
{
    public enum Fade { None, Linear, Late, InAndOut };
    public SpriteTexture Texture;
    public Picker<SpriteTexture> TexturePicker;
    public Color Color = Color.White;
    public Vector2 Acceleration;
    public float MinSpeed;
    public float MaxSpeed;
    public float RotationSpeed;
    public float Direction;
    public float DirectionRange;
    public float Friction;
    public float LifeTime = 1f;
    public float Scale = 1f;
    public float Spin;
    public Fade FadeMode;

    public Particle Create(ref Particle particle, Vector2 pos) =>
        Create(ref particle, null, pos);

    public Particle Create(ref Particle particle, Entity entity, Vector2 pos) 
    {
        particle.Material = this;
        particle.Emitting = true;
        particle.Position = pos;
        particle.Source = ChooseTexture();

        particle.Scale = Scale;

        particle.Color = Color;

        var moveDirection = Direction - DirectionRange / 2 + MathUtils.Randomizer.NextSingle() * DirectionRange;
        particle.Velocity = MathUtils.DegToVec(moveDirection, MathUtils.Randomizer.Range(MinSpeed, MaxSpeed));

        particle.StartLife = particle.LifeTime = MathUtils.Randomizer.Range(0, LifeTime);

        particle.Rotation = RotationSpeed;
        particle.Spin = Spin;

        return particle;
    }

    private SpriteTexture ChooseTexture() 
    {
        if (TexturePicker != null)
            return TexturePicker.Pick();
        if (Texture != null)
            return Texture;
        Debug.Assert(Texture != null, "Must have a texture!");
        return null;
    }
}