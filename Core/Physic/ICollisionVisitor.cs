namespace MyEngine2D.Core.Physic;

public interface ICollisionVisitor
{
    bool Intersect(RectanglePhysicShape rectangle);
    bool Intersect(CirclePhysicShape circle);
}