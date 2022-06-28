using System;
using System.Diagnostics.CodeAnalysis;

namespace Teuria;

[Obsolete("Everything in the SpatialHash is unfinished")]
public struct CellIndex 
{
    public int X;
    public int Y;

    public CellIndex(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static bool operator ==(CellIndex a, CellIndex b) 
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(CellIndex a, CellIndex b) 
    {
        return a.X != b.X || a.Y != b.Y;
    }

    public override bool Equals([NotNullWhen(true)] object obj)
    {
        if (obj is not CellIndex)
            return false;
        
        return base.Equals((CellIndex)obj);
    }

    public bool Equals(CellIndex other) 
    {
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return GenerateHashCode((int)X, (int)Y);
    }

    private static int GenerateHashCode(int x, int y) 
    {
        return (x << 2)^y;
    }
}