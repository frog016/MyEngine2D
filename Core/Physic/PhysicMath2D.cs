using MyEngine2D.Core.Graphic;
using MyEngine2D.Core.Math;
using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Physic;

public static class PhysicMath2D
{
    #region Quantities

    public static float ComputeVolume(float mass, float density)
    {
        return mass / density;
    }

    public static float ComputeMass(float density, float volume)
    {
        return density * volume;
    }

    public static float ComputeDensity(float mass, float volume)
    {
        return mass / volume;
    }

    public static float ComputeCollisionImpulse(
        Vector2 relativeVelocity, Vector2 normal, float elasticity,
        ImpulseArgs first, ImpulseArgs second)
    {
        var upperEquationPart = -(1 + elasticity) * Vector2.DotProduct(relativeVelocity, normal);

        var lowerEquationPart =
            first.InverseMass + second.InverseMass +
            Math2D.Sqr(Vector2.CrossProduct(first.ContactVector, normal)) * first.InverseInertia +
            Math2D.Sqr(Vector2.CrossProduct(second.ContactVector, normal)) * second.InverseInertia;

        return upperEquationPart / lowerEquationPart;
    }

    #endregion

    #region Collision

    internal static CollisionManifold? IntersectCircleWithCircle(
        Vector2 firstCenter, float firstRadius,
        Vector2 secondCenter, float secondRadius)
    {
        var distance = Vector2.Distance(firstCenter, secondCenter);
        var sumRadius = firstRadius + secondRadius;

        if (distance > sumRadius)
            return null;

        var normal = (secondCenter - firstCenter).Normalize();
        var depth = sumRadius - distance;
        var contactPoint = ComputeContactPoint();

        return new CollisionManifold(normal, depth, contactPoint);

        Vector2 ComputeContactPoint()
        {
            return firstCenter + (firstRadius - depth / 2) * normal;
        }
    }

    internal static CollisionManifold? IntersectCircleWithRectangle(
        Vector2 rectCenter, Vector2 rectSize, float rectRotation,
        Vector2 circleCenter, float circleRadius)
    {
        var rotatedCircleCenter = circleCenter.RotateAround(rectCenter, -rectRotation);
        var rectHalfSize = rectSize / 2;

        var closestX = Math2D.Clamp(rotatedCircleCenter.X, rectCenter.X - rectHalfSize.X, rectCenter.X + rectHalfSize.X);
        var closestY = Math2D.Clamp(rotatedCircleCenter.Y, rectCenter.Y - rectHalfSize.Y, rectCenter.Y + rectHalfSize.Y);
        var closestPoint = new Vector2(closestX, closestY);

        var closestDistance = Vector2.Distance(rotatedCircleCenter, closestPoint);
        if (closestDistance > circleRadius)
            return null;

        var normalPoint = closestDistance == 0 ? rectCenter : closestPoint;
        var normal = (normalPoint - rotatedCircleCenter)
            .Normalize()
            .Rotate(rectRotation);

        var depth = circleRadius - closestDistance;
        var contactPoint = closestPoint.RotateAround(rectCenter, rectRotation);

        return new CollisionManifold(normal, depth, contactPoint);
    }

    internal static CollisionManifold? IntersectRectangleWithRectangle(
        Vector2 firstRectCenter, Vector2 firstRectSize, float firstRectRotation,
        Vector2 secondRectCenter, Vector2 secondRectSize, float secondRectRotation)
    {
        var firstRect = new OrientedRectangle(firstRectCenter, firstRectSize, firstRectRotation);
        var secondRect = new OrientedRectangle(secondRectCenter, secondRectSize, secondRectRotation);

        var firstVertices = firstRect.GetCornerVertices();
        var secondVertices = secondRect.GetCornerVertices();

        var direction = (secondRectCenter - firstRectCenter).Normalize();

        var (firstOverlap, firstNormal) = ProjectSecondRectOnFirstRectAxes(firstVertices, secondVertices, firstRect.GetEdgeNormals());
        if (firstOverlap.HasValue == false)
        {
            return null;
        }

        var (secondOverlap, secondNormal) = ProjectSecondRectOnFirstRectAxes(secondVertices, firstVertices, secondRect.GetEdgeNormals());
        if (secondOverlap.HasValue == false)
        {
            return null;
        }

        firstNormal = Vector2.DotProduct(direction, firstNormal) > 0 ? firstNormal : -firstNormal;
        secondNormal = Vector2.DotProduct(-direction, secondNormal) > 0 ? secondNormal : -secondNormal;

        var (minOverlap, normal) = firstOverlap < secondOverlap
            ? (firstOverlap.Value, firstNormal)
            : (secondOverlap.Value, -secondNormal);

        var contactPoints = RectangleCollisionHelper.GetContactPoints(firstRect, secondRect, normal);
        return new CollisionManifold(normal, minOverlap, contactPoints);

        static (float? minOverlap, Vector2 normal) ProjectSecondRectOnFirstRectAxes(Vector2[] firstRectVertices, Vector2[] secondRectVertices, Vector2[] firstRectAxes)
        {
            var minOverlap = float.MaxValue;
            var normal = Vector2.Zero;

            foreach (var axis in firstRectAxes)
            {
                var (firstMin, firstMax) = ProjectVerticesOnAxis(axis, firstRectVertices);
                var (secondMin, secondMax) = ProjectVerticesOnAxis(axis, secondRectVertices);

                if (firstMax < secondMin || secondMax < firstMin)
                    return (null, Vector2.Zero);

                var overlap = Math2D.Min(firstMax, secondMax) - Math2D.Max(firstMin, secondMin);
                if (overlap >= minOverlap)
                    continue;

                minOverlap = overlap;
                normal = axis;
            }

            return (minOverlap, normal);
        }

        static (float min, float max) ProjectVerticesOnAxis(Vector2 axis, Vector2[] vertices)
        {
            var min = float.MaxValue;
            var max = float.MinValue;

            foreach (var vertex in vertices)
            {
                var projection = Vector2.DotProduct(axis, vertex);

                min = Math2D.Min(min, projection);
                max = Math2D.Max(max, projection);
            }

            return (min, max);
        }
    }

    #endregion

    #region Helper Methods

    public static float ComputeAverageFriction(float firstFriction, float secondFriction)
    {
        return Math2D.Sqrt(firstFriction * firstFriction + secondFriction * secondFriction);
    }

    #endregion

    #region Helper Data Structures

    public readonly struct ImpulseArgs
    {
        public readonly Vector2 ContactVector;
        public readonly float InverseMass;
        public readonly float InverseInertia;

        public ImpulseArgs(Vector2 contactVector, float inverseMass, float inverseInertia)
        {
            ContactVector = contactVector;
            InverseMass = inverseMass;
            InverseInertia = inverseInertia;
        }
    }

    #endregion
}

internal static class RectangleCollisionHelper
{
    //  источник - https://www.codezealot.org/archives/394/#cpg-clip
    //  вспомогательный ресурс - https://research.ncl.ac.uk/game/mastersdegree/gametechnologies/previousinformation/physics5collisionmanifolds/2017%20Tutorial%205%20-%20Collision%20Manifolds.pdf
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

    //  Step 1
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

    //  Step 2
    private static Edge GetClosestEdgeToNormal(OrientedRectangle rectangle, Vector2 farthestVertex, Vector2 normal)
    {
        var (leftVertex, rightVertex) = rectangle.GetCornerVertexNeighbors(farthestVertex);

        var leftEdge = farthestVertex - leftVertex;
        var rightEdge = farthestVertex - rightVertex;

        return Vector2.DotProduct(leftEdge, normal) <= Vector2.DotProduct(rightEdge, normal)
            ? new Edge(leftVertex, farthestVertex)
            : new Edge(farthestVertex, rightVertex);
    }

    //  Step 3
    private static (Edge reference, Edge incident, bool flip) GetReferenceAndIncidentEdges(Edge firstEdge, Edge secondEdge, Vector2 normal)
    {
        var firstProjection = Edge.DotProduct(firstEdge, normal);
        var secondProjection = Edge.DotProduct(secondEdge, normal);

        return Math2D.Abs(firstProjection) <= Math2D.Abs(secondProjection)
            ? (firstEdge, secondEdge, false)
            : (secondEdge, firstEdge, true);
    }

    //  Step 4
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