namespace MyEngine2D.Core.Physic;

public sealed class CirclePhysicShape : IPhysicShape
{
    public bool IntersectWith(IPhysicShape otherShape)
    {
        return otherShape.Intersect(this);
    }

    public bool Intersect(RectanglePhysicShape rectangle)
    {
        throw new NotImplementedException();
    }

    public bool Intersect(CirclePhysicShape circle)
    {
        throw new NotImplementedException();
    }
}