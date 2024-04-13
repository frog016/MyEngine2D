using MyEngine2D.Core.Math;
using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Physic;

internal static class RectangleCollisionHelper
{
    internal static Vector2[] GetContactPoints(OrientedRectangle first, OrientedRectangle second, Vector2 normal)
    {
        var firstFarthestVertex = GetFarthestVertexAlongNormal(first.GetCornerVertices(), normal);
        var firstEdge = GetClosestEdgeToNormal(first, firstFarthestVertex, normal);

        var secondFarthestVertex = GetFarthestVertexAlongNormal(second.GetCornerVertices(), -normal);
        var secondEdge = GetClosestEdgeToNormal(second, secondFarthestVertex, -normal);

        var (reference, incident, flip) = GetReferenceAndIncidentEdges(firstEdge, secondEdge, normal);

        var offset = Edge.DotProduct(reference, reference.Start);
        var clippedPoints = ClipPointsByEdge(incident.Start, incident.End, reference, offset);
        if (clippedPoints.Count < 2)
        {
            return Array.Empty<Vector2>();
        }

        offset = Edge.DotProduct(reference, reference.End);
        clippedPoints = ClipPointsByEdge(clippedPoints[0], clippedPoints[1], reference.Reverse(), -offset);
        if (clippedPoints.Count < 2)
        {
            return Array.Empty<Vector2>();
        }

        var referenceNormal = flip ? -reference.Normal : reference.Normal;
        var max = Vector2.DotProduct(referenceNormal, reference.Start);

        var resultPoints = new List<Vector2>(clippedPoints.Count);
        if (Vector2.DotProduct(referenceNormal, clippedPoints[0]) >= max)
        {
            resultPoints.Add(clippedPoints[0]);
        }

        if (Vector2.DotProduct(referenceNormal, clippedPoints[1]) >= max)
        {
            resultPoints.Add(clippedPoints[1]);
        }

        return resultPoints.ToArray();
    }

    private static Vector2 GetFarthestVertexAlongNormal(Vector2[] vertices, Vector2 normal)
    {
        var maxDistance = float.MinValue;
        var farthestVertex = Vector2.Zero;

        foreach (var vertex in vertices)
        {
            var distance = Vector2.DotProduct(normal, vertex);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestVertex = vertex;
            }
        }

        return farthestVertex;
    }

    private static Edge GetClosestEdgeToNormal(OrientedRectangle rectangle, Vector2 farthestVertex, Vector2 normal)
    {
        var (leftVertex, rightVertex) = rectangle.GetCornerVertexNeighbors(farthestVertex);

        var leftEdge = farthestVertex - leftVertex;
        var rightEdge = farthestVertex - rightVertex;

        return Vector2.DotProduct(leftEdge, normal) <= Vector2.DotProduct(rightEdge, normal)
            ? new Edge(leftVertex, farthestVertex)
            : new Edge(farthestVertex, rightVertex);
    }

    private static (Edge reference, Edge incident, bool flip) GetReferenceAndIncidentEdges(Edge firstEdge, Edge secondEdge, Vector2 normal)
    {
        var firstProjection = Edge.DotProduct(firstEdge, normal);
        var secondProjection = Edge.DotProduct(secondEdge, normal);

        return Math2D.Abs(firstProjection) <= Math2D.Abs(secondProjection)
            ? (firstEdge, secondEdge, false)
            : (secondEdge, firstEdge, true);
    }

    private static List<Vector2> ClipPointsByEdge(Vector2 start, Vector2 end, Edge edge, float offset)
    {
        var startDistance = Edge.DotProduct(edge, start) - offset;
        var endDistance = Edge.DotProduct(edge, end) - offset;

        var clippedPoints = new List<Vector2>();
        if (startDistance >= 0)
        {
            clippedPoints.Add(start);
        }

        if (endDistance >= 0)
        {
            clippedPoints.Add(end);
        }

        if (startDistance * endDistance < 0)
        {
            var distance = startDistance / (startDistance - endDistance);
            var edgeVector = end - start;

            clippedPoints.Add(distance * edgeVector + start);
        }

        return clippedPoints;
    }
}