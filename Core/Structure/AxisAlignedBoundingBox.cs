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

    public AxisAlignedBoundingBox Expand(float factor)
    {
        var expandSize = Vector2.One * factor;
        return new AxisAlignedBoundingBox(Min - expandSize, Max + expandSize);
    }

    public AxisAlignedBoundingBox ExpandBorder(Vector2 direction)
    {
        var (leftBorder, rightBorder) = ExpandBorder(direction.X, Min.X, Max.X);
        var (bottomBorder, topBorder) = ExpandBorder(direction.Y, Min.Y, Max.Y);

        return new AxisAlignedBoundingBox(
            new Vector2(leftBorder, bottomBorder),
            new Vector2(rightBorder, topBorder));
    }

    private static (float min, float max) ExpandBorder(float delta, float min, float max)
    {
        switch (delta)
        {
            case > 0:
                min += delta;
                break;
            case < 0:
                max += delta;
                break;
        }

        return (min, max);
    }
}