namespace MyEngine2D.Core.Structure;

public readonly struct AxisAlignedBoundingBox
{
    public readonly Vector2 Min;
    public readonly Vector2 Max;

    public AxisAlignedBoundingBox(Vector2 min, Vector2 max)
    {
        Min = min;
        Max = max;
    }

    public bool Intersect(AxisAlignedBoundingBox other)
    {
        return Max.X > other.Min.X && Min.X < other.Max.X &&
               Max.Y > other.Min.Y && Min.Y < other.Max.Y;
    }
}