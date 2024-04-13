namespace MyEngine2D.Core.Structure;

public readonly struct Edge
{
    public readonly Vector2 Start;
    public readonly Vector2 End;
    public readonly Vector2 Normal;

    public Edge(Vector2 start, Vector2 end)
    {
        Start = start;
        End = end;
        Normal = Vector2.CrossProduct(end - start, -1f).Normalize();
    }

    public Edge Reverse()
    {
        return new Edge(End, Start);
    }

    public static float DotProduct(Edge edge, Vector2 vector)
    {
        var directionVector = (edge.End - edge.Start).Normalize();
        return Vector2.DotProduct(directionVector, vector);
    }
}