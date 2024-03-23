namespace MyEngine2D.Core.Physic;

public sealed class PhysicWorld
{
    private ICollisionResolutionMethod _collisionResolution = new ImpulseCollisionResolutionMethod();

    public void SetCollisionMethod(ICollisionResolutionMethod collisionResolution)
    {
        _collisionResolution = collisionResolution;
    }

    internal void UpdatePhysic()
    {
        //  find collisions
        //  solve collisions
        //  update rigid body states
        //  correct positions of collided bodies
        //  reset forces
    }

    private void ResolveCollision(Contact contact)
    {
        _collisionResolution.ResolveCollision(contact);
    }

    private static void CorrectCollisionContactPositions(Contact contact)
    {
        const float depthCorrectPercent = 0.25f;

        var first = contact.First;
        var second = contact.Second;
        var manifold = contact.Manifold;

        var correction = manifold.Depth / (first.InverseMass + second.InverseMass) * manifold.Normal * depthCorrectPercent;

        first.Position -= correction * first.InverseMass;
        second.Position += correction * second.InverseMass;
    }
}