using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Physic;

public sealed class RigidBody : Component
{
    public IPhysicShape Shape { get; private set; }
    public PhysicMaterial Material { get; private set; }

    public bool IsStatic { get; set; }
    public float GravityScale { get; set; }
    public float ScaledGravity => GravityScale * Gravity;

    public Vector2 LinearVelocity { get; private set; }
    public float AngularVelocity { get; private set; }
    public float Mass { get; private set; }
    public float InverseMass { get; private set; }

    public const float Gravity = 9.80665f;

    private float _force;
    private float _acceleration;
    private float _toruqe;

    public RigidBody(GameObject gameObject) : base(gameObject)
    {
    }

    public void Initialize(IPhysicShape shape, PhysicMaterial material, 
        float gravityScale = 1f, bool isStatic = false)
    {
        Shape = shape;
        Material = material;
        IsStatic = isStatic;
        GravityScale = gravityScale;

        Mass = CalculateBodyMass(shape, material);
        InverseMass = 1 / Mass;
    }

    public void ApplyLinearImpulse(Vector2 impulse)
    {
        LinearVelocity += InverseMass * impulse;
    }

    private static float CalculateBodyMass(IPhysicShape shape, PhysicMaterial material)
    {
        var density = material.Density;
        var volume = shape.Volume;

        return PhysicMath2D.ComputeMass(density, volume);
    }
}