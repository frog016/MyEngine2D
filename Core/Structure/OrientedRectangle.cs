namespace MyEngine2D.Core.Structure;

public readonly struct OrientedRectangle
{
    public readonly Vector2 Center;
    public readonly Vector2 Size;
    public readonly float Rotation;

    public OrientedRectangle(Vector2 center, Vector2 size, float rotation)
    {
        Center = center;
        Size = size;
        Rotation = rotation;
    }

    public Vector2[] GetEdgeNormals()
    {
        return new Vector2[]
        {
            Vector2.Left.Rotate(Rotation),
            Vector2.Up.Rotate(Rotation),
            Vector2.Right.Rotate(Rotation),
            Vector2.Down.Rotate(Rotation),
        };
    }

    public Vector2[] GetCornerVertices()
    {
        var center = Center;
        var halfSize = Size / 2f;
        var rotation = Rotation;

        var cornerDirections = new Vector2[]
        {
            new Vector2(-1, -1),
            new Vector2(-1, 1),
            new Vector2(1, -1),
            new Vector2(1, 1),
        };

        return cornerDirections
            .Select(direction => Vector2.MultiplyComponentwise(direction, halfSize))
            .Select(scaledDirection => scaledDirection.Rotate(rotation))
            .Select(localVertex => center + localVertex)
            .ToArray();
    }
}