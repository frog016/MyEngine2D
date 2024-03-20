namespace MyEngine2D.Core.Physic;

internal sealed class PhysicWorld
{
    internal void ResolveCollision(Contact contact)
    {
        var first = contact.First;
        var second = contact.Second;

        var elasticity = System.Math.Min(first.Material.Elasticity, second.Material.Elasticity);
        var relativeVelocity = first.LinearVelocity - second.LinearVelocity;

        var contactNormal = contact.Manifold.Normal;

        var impulseScalar = PhysicMath2D.ComputeCollisionImpulse(relativeVelocity, contactNormal, elasticity, first.InverseMass, second.InverseMass);
        var impulse = contactNormal * impulseScalar;

        first.ApplyLinearImpulse(impulse);
        second.ApplyLinearImpulse(-impulse);
    }
}