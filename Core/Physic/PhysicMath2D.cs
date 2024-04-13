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
    }

    private static (float? minOverlap, Vector2 normal) ProjectSecondRectOnFirstRectAxes(Vector2[] firstRectVertices, Vector2[] secondRectVertices, Vector2[] firstRectAxes)
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

    private static (float min, float max) ProjectVerticesOnAxis(Vector2 axis, Vector2[] vertices)
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