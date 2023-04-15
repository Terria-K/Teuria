using System;
using Microsoft.Xna.Framework;

namespace Teuria;

public sealed class Shaker : Component 
{
    public float Timer { get; private set; }
    public Vector2 Value { get; private set; }
    public float Intensity { get; set; }

    private static Random random = new Random();
    private static int[] offsets = new int[5] { -1, -1, 0, 1, 1 };

    public Shaker() 
    {
        Active = false;
    }


    public override void Update()
    {
        if (!Active && Timer <= 0) 
        {
            return;
        }
        Timer -= Time.Delta;
        if (Timer <= 0) 
        {
            Active = false;
            Value = Vector2.Zero;
            return;
        }
        Value = new Vector2(offsets[random.Next(offsets.Length)], offsets[random.Next(offsets.Length)]) * Intensity;

        base.Update();
    }

    public void ShakeFor(float timer) 
    {
        Timer = timer;
        Active = true;
    }
}