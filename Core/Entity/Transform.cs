using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Entity;

public class Transform : Component
{
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; } = Vector2.One;

    public Transform(GameObject gameObject) : base(gameObject)
    {
    }

    public Vector2 GetForwardVector()
    {
        return Vector2.Right.Rotate(Rotation);
    }
}