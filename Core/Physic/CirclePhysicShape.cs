using MyEngine2D.Core.Math;
using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Physic;

public sealed class CirclePhysicShape : IPhysicShape
{
    public float Volume => Math2D.PI * Radius * Radius; //  PI * R^2
    public float Inertia => _parentBody.Mass * Radius * Radius / 2;  //  M * R^2 / 2

    public Vector2 Center => _parentBody.Position;
    public float Radius { get; set; }

    private readonly RigidBody _parentBody;

    public CirclePhysicShape(RigidBody parentBody, float radius)
    {
        _parentBody = parentBody;
        Radius = radius;
    }

    public CollisionManifold? IntersectWith(IPhysicShape otherShape)
    {
        return otherShape.Intersect(this);
    }

    public CollisionManifold? Intersect(RectanglePhysicShape rectangle)
    {
        var manifold = PhysicMath2D.IntersectCircleWithRectangle(
            rectangle.Center, rectangle.Size, rectangle.Rotation,
            Center, Radius);

        return manifold.HasValue 
            ? new CollisionManifold(-manifold.Value.Normal, manifold.Value.Depth, manifold.Value.ContactPoints)
            : manifold;
    }

    public CollisionManifold? Intersect(CirclePhysicShape circle)
    {
        return PhysicMath2D.IntersectCircleWithCircle(
            circle.Center, circle.Radius,
            Center, Radius);
    }

    public AxisAlignedBoundingBox GetBoundingBox()
    {
        var halfSize = Vector2.One * Radius;
        return new AxisAlignedBoundingBox(Center - halfSize, Center + halfSize);
    }

    public override string ToString()
    {
        return $"[Circle] Radius: {Radius}";
    }
}