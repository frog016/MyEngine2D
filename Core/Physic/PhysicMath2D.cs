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

    public static float ComputeCollisionImpulse(Vector2 relativeVelocity, Vector2 normal,
        float elasticity, float firstInverseMass, float secondInverseMass)
    {
        return -(1 + elasticity) * Vector2.DotProduct(relativeVelocity, normal) /
               (firstInverseMass + secondInverseMass);
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

        return new CollisionManifold(normal, depth);
    }

    #endregion
}