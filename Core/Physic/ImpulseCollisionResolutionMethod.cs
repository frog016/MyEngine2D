using MyEngine2D.Core.Math;
using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Physic;

public sealed class ImpulseCollisionResolutionMethod : ICollisionResolutionMethod
{
    public void ResolveCollision(Contact contact)
    {
        var first = contact.First;
        var second = contact.Second;

        var contactNormal = contact.Manifold.Normal;
        var contactPoints = contact.Manifold.ContactPoints;
        var contactCount = contactPoints.Length;

        foreach (var contactPoint in contactPoints)
        {
            ApplyImpulseInPoint(first, second, contactNormal, contactPoint, contactCount);
        }
    }

    private static void ApplyImpulseInPoint(RigidBody first, RigidBody second, Vector2 contactNormal, Vector2 contactPoint, int contactCount)
    {
        var firstContactVector = contactPoint - first.Position;
        var secondContactVector = contactPoint - second.Position;

        var relativeVelocity = ComputeRelativeVelocity(first, firstContactVector, second, secondContactVector);

        var firstImpulseArgs = new PhysicMath2D.ImpulseArgs(firstContactVector, first.InverseMass, first.InverseInertia);
        var secondImpulseArgs = new PhysicMath2D.ImpulseArgs(secondContactVector, second.InverseMass, second.InverseInertia);

        var impulseScalar = ApplyLinearImpulse(first, second, relativeVelocity, contactNormal, firstImpulseArgs, secondImpulseArgs, contactCount);
        ApplyFrictionImpulse(first, second, relativeVelocity, contactNormal, firstImpulseArgs, secondImpulseArgs, impulseScalar, contactCount);
    }

    private static float ApplyLinearImpulse(
        RigidBody first, RigidBody second, Vector2 relativeVelocity, Vector2 contactNormal,
        PhysicMath2D.ImpulseArgs firstImpulseArgs, PhysicMath2D.ImpulseArgs secondImpulseArgs,
        int contactCount)
    {
        var elasticity = Math2D.Min(first.Material.Elasticity, second.Material.Elasticity);

        var impulseScalar = PhysicMath2D.ComputeCollisionImpulse(
            relativeVelocity, contactNormal, elasticity,
            firstImpulseArgs, secondImpulseArgs);

        var impulse = contactNormal * impulseScalar / contactCount;

        first.ApplyImpulse(-impulse, firstImpulseArgs.ContactVector);
        second.ApplyImpulse(impulse, secondImpulseArgs.ContactVector);

        return impulseScalar;
    }

    private static void ApplyFrictionImpulse(
        RigidBody first, RigidBody second, Vector2 relativeVelocity, Vector2 contactNormal,
        PhysicMath2D.ImpulseArgs firstImpulseArgs, PhysicMath2D.ImpulseArgs secondImpulseArgs,
        float impulseScalar, int contactCount)
    {
        var tangentNormal = (relativeVelocity - contactNormal * Vector2.DotProduct(relativeVelocity, contactNormal))
            .Normalize();

        var tangentImpulseScalar =
            PhysicMath2D.ComputeCollisionImpulse(relativeVelocity, tangentNormal, 0, firstImpulseArgs, secondImpulseArgs);

        var staticFriction =
            PhysicMath2D.ComputeAverageFriction(first.Material.StaticFriction, second.Material.StaticFriction);

        var dynamicFriction =
            PhysicMath2D.ComputeAverageFriction(first.Material.DynamicFriction, second.Material.DynamicFriction);

        var tangentImpulse = Math2D.Abs(tangentImpulseScalar) < impulseScalar * staticFriction
            ? tangentImpulseScalar * tangentNormal
            : -impulseScalar * tangentNormal * dynamicFriction;

        tangentImpulse /= contactCount;

        first.ApplyImpulse(-tangentImpulse, firstImpulseArgs.ContactVector);
        second.ApplyImpulse(tangentImpulse, secondImpulseArgs.ContactVector);
    }

    private static Vector2 ComputeRelativeVelocity(
        RigidBody first, Vector2 firstContactVector,
        RigidBody second, Vector2 secondContactVector)
    {
        return second.LinearVelocity + Vector2.CrossProduct(second.AngularVelocity, secondContactVector) -
            first.LinearVelocity - Vector2.CrossProduct(first.AngularVelocity, firstContactVector);
    }
}