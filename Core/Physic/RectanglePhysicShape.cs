using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Physic;

public sealed class RectanglePhysicShape : IPhysicShape
{
    public float Volume => Size.X * Size.Y; //  Width * Height
    public float Inertia => _parentBody.Mass * (Size.X * Size.X + Size.Y * Size.Y) / 12;    //  1/12 * M * (Width^2 + Height^2)
    
    public Vector2 Center => _parentBody.Position;
    public Vector2 Size { get; set; }
    public float Rotation => _parentBody.Rotation;

    private readonly RigidBody _parentBody;

    public RectanglePhysicShape(RigidBody parentBody, Vector2 size)
    {
        _parentBody = parentBody;
        Size = size;
    }

    public CollisionManifold? IntersectWith(IPhysicShape otherShape)
    {
        return otherShape.Intersect(this);
    }

    public CollisionManifold? Intersect(RectanglePhysicShape rectangle)
    {
        return PhysicMath2D.IntersectRectangleWithRectangle(
            rectangle.Center, rectangle.Size, rectangle.Rotation,
            Center, Size, Rotation);
    }

    public CollisionManifold? Intersect(CirclePhysicShape circle)
    {
        return PhysicMath2D.IntersectCircleWithRectangle(
            Center, Size, Rotation, 
            circle.Center, circle.Radius);
    }

    public AxisAlignedBoundingBox GetBoundingBox()
    {
        var vertices = new OrientedRectangle(Center, Size, Rotation)
            .GetCornerVertices();

        var xCoordinates = vertices
            .Select(vertex => vertex.X)
            .ToArray();

        var yCoordinates = vertices
            .Select(vertex => vertex.Y)
            .ToArray();

        var min = new Vector2(xCoordinates.Min(), yCoordinates.Min());
        var max = new Vector2(xCoordinates.Max(), yCoordinates.Max());
        return new AxisAlignedBoundingBox(min, max);
    }

    public override string ToString()
    {
        return $"[Rectangle] Size: {Size}, Rotation: {Rotation}";
    }
}