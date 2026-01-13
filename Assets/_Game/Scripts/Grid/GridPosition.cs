using System;

public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int z;

    public GridPosition(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    // --- NEW: MATH OPERATORS (Fixes Error CS0019) ---
    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x + b.x, a.z + b.z);
    }

    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x - b.x, a.z - b.z);
    }
    // ------------------------------------------------

    public override string ToString()
    {
        return $"x: {x}; z: {z}";
    }

    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.x == b.x && a.z == b.z;
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition position && this == position;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }

    public int Distance(GridPosition other)
    {
        return Math.Abs(x - other.x) + Math.Abs(z - other.z);
    }
}