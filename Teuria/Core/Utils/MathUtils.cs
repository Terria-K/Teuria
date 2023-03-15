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
    public const float Epsilon = 1e-5f;
    public static Random Randomizer = new Random();



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
        SkyLog.Assert(min > max, "Minimum value is greater than Maximum");
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
    public static float Snapped(float px, float step) 
    {
        if (step != 0)
            return (float)Math.Floor((px/ step) + 0.5f) * step;
        return px;
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
        Vector2 velocityDistance = target - current;
        float len = velocityDistance.Length();
        if (len <= maxDelta || len < Epsilon)
            return target;
        return current + (velocityDistance / len * maxDelta);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Slerp(Vector2 current, Vector2 target, float maxDelta) 
    {
        var start = current.LengthSquared();
        var end = target.LengthSquared();

        if (start == 0.0f || end == 0.0f)
            return Vector2.Lerp(current, target, maxDelta);
        
        var startLength = (float)Math.Sqrt(start);
        var endLength = (float)Math.Sqrt(end);
        var result = MathHelper.LerpPrecise(startLength, endLength, maxDelta);
        var angle = Angle(current, target);
        return Rotated(current, angle * maxDelta * (result / startLength));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Rotated(this Vector2 value, float angle) 
    {
        (double sin, double cos) = Math.SinCos(angle);
        return new Vector2(value.X * (float)cos - value.Y * (float)sin , value.X * (float)sin + value.Y * (float)cos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 DegToVec(float radians, float length) 
    {
        return new Vector2((float)Math.Cos(radians) * length, (float)Math.Sin(radians) * length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Transform(Vector2 position, Matrix2D matrix)
    {
        return new Vector2((position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M31,
            (position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M32);
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
    public static Vector2 ClosestPointOnLine(Vector2 lineA, Vector2 lineB, Vector2 closestTo)
    {
        var v = lineB - lineA;
        var w = closestTo - lineA;
        var t = Vector2.Dot(w, v) / Vector2.Dot(v, v);
        t = MathHelper.Clamp(t, 0, 1);

        return lineA + v * t;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ToInt(this Vector2 vec) 
    {
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Snapped(this Vector2 vec, Vector2 step) 
    {
        return new Vector2(Snapped(vec.X, step.X), Snapped(vec.Y, step.Y));
    }

#region Sector
    public const int SectorCenter = 0;
    public const int SectorLeft = 1;
    public const int SectorRight = 2;
    public const int SectorBottom = SectorLeft | SectorRight;
    public const int SectorTop = 4;
    public static int GetSector(float rX, float rY, float rW, float rH, Vector2 line) 
    {
        var sector = SectorCenter;
        if (line.X < rX)
            sector |= SectorLeft;
        else if (line.X >= rX + rW)
            sector |= SectorRight;
        if (line.Y < rY)
            sector |= SectorBottom;
        else if (line.Y >= rY + rH)
            sector |= SectorTop;
        return sector;
    }
#endregion

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
        SkyLog.Assert(random.Count == 0, "StartRandScope cannot be called when an existing scope has not been ended");
        random.Push(Randomizer);
        Randomizer = new Random(seed);
    }

    public static void EndRandScope() 
    {
        Randomizer = random.Pop();
    }
#endregion
}