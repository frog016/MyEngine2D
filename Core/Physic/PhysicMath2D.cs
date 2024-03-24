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
            Math2D.Sqr(Vector2.CrossProduct(second.ContactVector, normal)) * second.InverseInertia; ;

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

        var closestX = Math2D.Clamp(rotatedCircleCenter.X, rectCenter.X, rectCenter.X + rectSize.X);
        var closestY = Math2D.Clamp(rotatedCircleCenter.Y, rectCenter.Y, rectCenter.Y + rectSize.Y);
        var closestPoint = new Vector2(closestX, closestY);

        var closestDistance = Vector2.Distance(rotatedCircleCenter, closestPoint);
        if (closestDistance > circleRadius)
            return null;

        var normalStartPoint = closestDistance == 0 ? rectCenter : closestPoint;
        var normal = (rotatedCircleCenter - normalStartPoint)
            .Normalize()
            .Rotate(rectRotation);

        var depth = circleRadius - closestDistance;

        return new CollisionManifold(normal, depth, closestPoint);
    }

    internal static CollisionManifold? IntersectRectangleWithRectangle(
        Vector2 firstRectCenter, Vector2 firstRectSize, float firstRectRotation,
        Vector2 secondRectCenter, Vector2 secondRectSize, float secondRectRotation)
    {
        var firstRect = new OrientedRectangle(firstRectCenter, firstRectSize, firstRectRotation);
        var secondRect = new OrientedRectangle(secondRectCenter, secondRectSize, secondRectRotation);

        var axes = firstRect.GetEdgeNormals();

        var minOverlap = float.MaxValue;
        var normal = Vector2.Zero;

        foreach (var axis in axes)
        {
            var (firstMin, firstMax) = ProjectVerticesOnAxis(axis, firstRect.GetCornerVertices());
            var (secondMin, secondMax) = ProjectVerticesOnAxis(axis, secondRect.GetCornerVertices());

            if (firstMax < secondMin || secondMax < firstMin)
                return null;

            var overlap = Math2D.Min(firstMax, secondMax) - Math2D.Max(firstMin, secondMin);
            if (overlap >= minOverlap)
                continue;

            minOverlap = overlap;
            normal = axis;
        }

        var contactPoint = GetContactPoint(firstRect, normal, minOverlap);
        return new CollisionManifold(normal, minOverlap, contactPoint);

        static (float min, float max) ProjectVerticesOnAxis(Vector2 axis, Vector2[] vertices)
        {
            var min = 0f;
            var max = 0f;

            foreach (var vertex in vertices)
            {
                var projection = Vector2.DotProduct(axis, vertex);

                min = Math2D.Min(min, projection);
                max = Math2D.Max(max, projection);
            }

            return (min, max);
        }

        static Vector2 GetContactPoint(OrientedRectangle first, Vector2 normal, float depth)
        {
            var projectedHalfSize = Vector2.DotProduct(first.Size / 2f, normal);
            return first.Center + (projectedHalfSize - depth / 2f) * normal;
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