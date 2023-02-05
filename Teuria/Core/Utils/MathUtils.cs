using System;
using System.Collections.Generic;
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


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MoveTowards(float current, float target, float maxDelta)
    {
        if (Math.Abs(target - current) <= maxDelta)
        {
            return target;
        }
        return current + Math.Sign(target - current) * maxDelta;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(float value, float min, float max) 
    {
        Debug.Assert(min > max, "Minimum value is greater than Maximum");
        return value < min ? min : value > max ? max : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Wrap(float value, float min, float max) 
    {
        if (value < min) { return max; }
        if (value > max) { return min; }
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MoveTowards(int current, int target, int maxDelta)
    {
        if (Math.Abs(target - current) <= maxDelta)
        {
            return target;
        }
        return current + Math.Sign(target - current) * maxDelta;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Wrap(int value, int min, int max) 
    {
        if (value < min) { return max; }
        if (value > max) { return min; }
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(int value, int min, int max) 
    {
        if (value < min) { return min; }
        if (value > max) { return max; }
        return value;
    }

#region Vector2
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Angle(Vector2 from, Vector2 to)
    {
        return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 DegToVec(float radians, float length) 
    {
        return new Vector2((float)Math.Cos(radians) * length, (float)Math.Sin(radians) * length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ToInt(this Vector2 vec) 
    {
        var (x, y) = vec;
        return new Vector2((int)vec.X, (int)vec.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max) 
    {
        return new Vector2(Clamp(value.X, min.X, min.Y), Clamp(value.Y, min.Y, max.Y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Clamp(this Vector2 value, float minX, float maxX, float minY, float maxY) 
    {
        return new Vector2(Clamp(value.X, minX, maxX), Clamp(value.Y, minY, maxY));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Floor(this Vector2 vec) 
    {
        return new Vector2((int)Math.Floor(vec.X), (int)Math.Floor(vec.Y));
    }

// MonoGame already has this functions, FNA doesn't
#if FNA
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ToVector2(this Point point) 
    {
        return new Vector2(point.X, point.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point ToPoint(this Vector2 vector2) 
    {
        return new Point((int)vector2.X, (int)vector2.Y);
    }
#endif

#endregion

#region Randomizer
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Range(this Random random, int min, int max) => min + random.Next(max - min);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Range(this Random random, float min, float max) => min + random.NextSingle() * (max - min);

    private static Stack<Random> random = new();
    public static void StartRandScope(int seed) 
    {
#if DEBUG
        Debug.Assert(random.Count == 0, "StartRandScope cannot be called when an existing scope has not been ended");
#endif
        random.Push(Randomizer);
        Randomizer = new Random(seed);
    }

    public static void EndRandScope() 
    {
        Randomizer = random.Pop();
    }
#endregion
}