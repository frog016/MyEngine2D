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

        return new CollisionManifold(normal, depth, /*TODO: найти точки контакта*/ Array.Empty<Vector2>());
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