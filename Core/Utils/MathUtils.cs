using System;

namespace Teuria;

public static class MathUtils 
{

    public const float Epsilon = 0.00000001f;
    public static float MoveTowards(float current, float target, float maxDelta)
    {
        if (MathF.Abs(target - current) <= maxDelta)
        {
            return target;
        }
        return current + MathF.Sign(target - current) * maxDelta;
    }
}