namespace MyEngine2D.Core.Physic;

public interface ICollisionResolutionMethod
{
    void ResolveCollision(Contact contact);
}