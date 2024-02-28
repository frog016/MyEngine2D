using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Entity;

public class TransformComponent : Component
{
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; } = Vector2.One;

    public TransformComponent(GameObject gameObject) : base(gameObject)
    {
    }
}