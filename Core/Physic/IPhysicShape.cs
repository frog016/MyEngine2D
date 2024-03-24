using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Physic;

public interface IPhysicShape : ICollisionVisitor
{
    float Volume { get; }
    float Inertia { get; }
    CollisionManifold? IntersectWith(IPhysicShape otherShape);
    AxisAlignedBoundingBox GetBoundingBox();
}