using System;

namespace Teuria;

public static class MathUtils 
{
    public static float MoveTowards(float current, float target, float delta) 
    {
        return Math.Abs(target - current) <= delta ? target : current + Math.Sign(target - current) * delta;
    }
}