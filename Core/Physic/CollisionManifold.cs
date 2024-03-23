using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Physic;

public readonly struct CollisionManifold
{
    public readonly Vector2 Normal;
    public readonly float Depth;
    public readonly Vector2[] ContactPoints;

    public CollisionManifold(Vector2 normal, float depth, Vector2[] contactPoints)
    {
        Normal = normal;
        Depth = depth;
        ContactPoints = contactPoints;
    }
}