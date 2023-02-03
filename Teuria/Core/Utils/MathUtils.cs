using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Teuria;

public static class MathUtils 
{
    public const float Radians = MathHelper.Pi / 180f;
    public const float Degrees = 180f / MathHelper.Pi;
    public static Random Randomizer = new Random();
    public const float Epsilon = 0.00000001f;



    public static float MoveTowards(float current, float target, float maxDelta)
    {
        if (MathF.Abs(target - current) <= maxDelta)
        {
            return target;
        }
        return current + MathF.Sign(target - current) * maxDelta;
    }

    public static float Wrapf(float value, float min, float max) 
    {
        if (value < min) { return max; }
        if (value > max) { return min; }
        return value;
    }

    public static int Wrap(int value, int min, int max) 
    {
        if (value < min) { return max; }
        if (value > max) { return min; }
        return value;
    }

    public static int Clamp(int value, int min, int max) 
    {
        if (value < min) { return min; }
        if (value > max) { return max; }
        return value;
    }

    public static float Clamp(float value, float min, float max) 
    {
        Debug.Assert(min > max, "Minimum value is greater than Maximum");
        return value < min ? min : value > max ? max : value;
    }

#region Vector2
    public static float Angle(Vector2 from, Vector2 to)
    {
        return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
    }

    public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDelta)
    {
        var vecX = target.X - current.X;
        var vecY = target.Y - current.Y;

        var dist = vecX * vecX * vecY * vecY;

        if (dist != 0 && (maxDelta < 0 || dist > maxDelta * maxDelta))
        {
            float squaredDist = (float)Math.Sqrt(dist);

            return new Vector2(current.X + vecY / dist * maxDelta, current.Y + vecY / dist * maxDelta);
        }

        return target;
    }
    public static Vector2 DegToVec(float radians, float length) 
    {
        return new Vector2((float)Math.Cos(radians) * length, (float)Math.Sin(radians) * length);
    }

    public static Vector2 Approach(Vector2 val, Vector2 target, float maxMove)
    {
        if (maxMove == 0 || val == target)
            return val;

        var diff = target - val;
        var length = diff.Length();

        if (length < maxMove)
            return target;

        diff.Normalize();
        return val + diff * maxMove;
    }

    public static Vector2 ToInt(this Vector2 vec) 
    {
        return new Vector2((int)vec.X, (int)vec.Y);
    }

    public static (float, float) Destruct(this Vector2 vec) 
    {
        return (vec.X, vec.Y);
    }


    public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max) 
    {
        return new Vector2(Clamp(value.X, min.X, min.Y), Clamp(value.Y, min.Y, max.Y));
    }

    public static Vector2 Clamp(this Vector2 value, float minX, float maxX, float minY, float maxY) 
    {
        return new Vector2(Clamp(value.X, minX, maxX), Clamp(value.Y, minY, maxY));
    }
    public static Vector2 Floor(this Vector2 vec) 
    {
        return new Vector2((int)Math.Floor(vec.X), (int)Math.Floor(vec.Y));
    }

#if FNA
    public static Vector2 ToVector2(this Point point) 
    {
        return new Vector2(point.X, point.Y);
    }
    public static Point ToPoint(this Vector2 vector2) 
    {
        return new Point((int)vector2.X, (int)vector2.Y);
    }
#endif

#endregion

#region Randomizer
    public static int Range(this Random random, int min, int max) => min + random.Next(max - min);
    public static float Range(this Random random, float min, float max) => min + random.NextSingle() * (max - min);
#endregion
}