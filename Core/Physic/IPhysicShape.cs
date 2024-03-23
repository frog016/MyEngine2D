namespace MyEngine2D.Core.Physic;

public interface IPhysicShape : ICollisionVisitor
{
    float Volume { get; }
    float Inertia { get; }
    bool IntersectWith(IPhysicShape otherShape);
}