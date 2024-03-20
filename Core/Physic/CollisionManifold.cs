using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Physic;

public readonly struct CollisionManifold
{
    public readonly Vector2 Normal;
    public readonly float Depth;

    public CollisionManifold(Vector2 normal, float depth)
    {
        Normal = normal;
        Depth = depth;
    }
}