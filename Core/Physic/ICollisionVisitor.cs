namespace MyEngine2D.Core.Physic;

public interface ICollisionVisitor
{
    CollisionManifold? Intersect(RectanglePhysicShape rectangle);
    CollisionManifold? Intersect(CirclePhysicShape circle);
}