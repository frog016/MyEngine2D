namespace MyEngine2D.Core.Structure;

public struct Layer : IEquatable<Layer>, IComparable<Layer>
{
    public int Index { get; set; }

    public Layer(int index)
    {
        Index = index;
    }

    public bool Equals(Layer other)
    {
        return Index == other.Index;
    }

    public int CompareTo(Layer other)
    {
        return Index.CompareTo(other.Index);
    }

    public override bool Equals(object? obj)
    {
        return obj is Layer other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Index.GetHashCode();
    }

    public override string ToString()
    {
        return $"Layer: {Index}";
    }

    public static implicit operator int(Layer layer)
    {
        return layer.Index;
    }

    public static implicit operator Layer(int index)
    {
        return new Layer(index);
    }

    public static bool operator ==(Layer left, Layer right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Layer left, Layer right)
    {
        return !(left == right);
    }

    public static bool operator <(Layer left, Layer right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Layer left, Layer right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Layer left, Layer right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Layer left, Layer right)
    {
        return left.CompareTo(right) >= 0;
    }
}