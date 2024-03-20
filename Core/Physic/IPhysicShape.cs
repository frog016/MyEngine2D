namespace MyEngine2D.Core.Physic;

//  TODO: Подумать над даннымы, которые возвращаются при успешном пересечении
public interface IPhysicShape : ICollisionVisitor
{
    float Volume { get; }
    float Inertia { get; }
    bool IntersectWith(IPhysicShape otherShape);
}