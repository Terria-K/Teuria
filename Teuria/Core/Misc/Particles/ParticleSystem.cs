using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class ParticleSystem : Entity
{
    private Particle[] particles;
    private int slot;

    public int Slot 
    {
        get => slot;
        private set => slot = (slot = value) % particles.Length;
    }

    public ParticleSystem(int maxParticles) 
    {
        particles = new Particle[maxParticles];
    }

    public void Emit(ParticleMaterial material, Vector2 position) 
    {
        material.Create(ref particles[slot], position);
        Slot += 1;
    }

    public void Emit(ParticleMaterial material, Entity entity) 
    {
        material.Create(ref particles[slot], entity, Vector2.Zero);
        Slot += 1;
    }

    public void Emit(ParticleMaterial material, Entity entity, Vector2 position) 
    {
        material.Create(ref particles[slot], entity, position);
        Slot += 1;
    }

    public void EmitMany(ParticleMaterial material, Entity entity, int amount) 
    {
        for (int i = 0; i < amount; i++) 
        {
            material.Create(ref particles[slot], entity, Vector2.Zero);
            Slot += 1;
        }
    }

    public void EmitMany(ParticleMaterial material, Vector2 position, int amount) 
    {
        for (int i = 0; i < amount; i++) 
        {
            material.Create(ref particles[slot], position);
            Slot += 1;
        }
    }

    public void EmitMany(ParticleMaterial material, Entity entity, Vector2 position, int amount) 
    {
        for (int i = 0; i < amount; i++) 
        {
            material.Create(ref particles[slot], entity, position);
            Slot += 1;
        }
    }

    public void Clear() 
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Emitting = false;
        }
    }

    public override void Update()
    {
        for (int i = 0; i < particles.Length; i++) 
        {
            if (particles[i].Emitting)
                particles[i].Update();
        }
        base.Update();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        foreach (var particle in particles) 
        {
            if (particle.Emitting) 
            {
                particle.Draw(spriteBatch);
            }
        }
        base.Draw(spriteBatch);
    }

    public override void ExitScene()
    {
        Clear();
        base.ExitScene();
    }
}