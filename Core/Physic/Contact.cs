namespace MyEngine2D.Core.Physic;

public readonly struct Contact
{
    public readonly RigidBody First;
    public readonly RigidBody Second;
    public readonly CollisionManifold Manifold;

    public Contact(RigidBody first, RigidBody second, CollisionManifold manifold)
    {
        First = first;
        Second = second;
        Manifold = manifold;
    }
}