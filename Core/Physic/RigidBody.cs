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
    public float Inertia => Shape.Inertia;
    public float InverseInertia => 1 / Shape.Inertia;

    public Vector2 Position { get => GameObject.Transform.Position; set => GameObject.Transform.Position = value; }

    public const float Gravity = 9.80665f;

    private Vector2 _externalForce;
    private float _torque;

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

    public void ApplyImpulse(Vector2 impulse, Vector2 contactVector)
    {
        LinearVelocity += InverseMass * impulse;
        AngularVelocity += InverseInertia * Vector2.CrossProduct(contactVector, impulse);
    }

    public void ApplyForce(Vector2 force, Vector2 point)
    {
        _externalForce += force;
        _torque += Vector2.CrossProduct(point - Position, force);
    }

    internal void UpdateBodyPhysic(float fixedDeltaTime)
    {
        if (IsStatic)
            return;

        UpdateGravity(fixedDeltaTime);
        UpdateForces(fixedDeltaTime);
        UpdateVelocity(fixedDeltaTime);
    }

    internal void ResetForces()
    {
        _externalForce = Vector2.Zero;
        _torque = 0;
    }

    private void UpdateGravity(float deltaTime)
    {
        LinearVelocity += Vector2.Down * ScaledGravity * deltaTime;
    }

    private void UpdateForces(float deltaTime)
    {
        LinearVelocity += _externalForce * InverseMass * deltaTime;
        AngularVelocity += _torque * InverseInertia * deltaTime;
    }

    private void UpdateVelocity(float deltaTime)
    {
        var transform = GameObject.Transform;
        transform.Position += LinearVelocity * deltaTime;
        transform.Rotation += AngularVelocity * deltaTime;
    }

    private static float CalculateBodyMass(IPhysicShape shape, PhysicMaterial material)
    {
        var density = material.Density;
        var volume = shape.Volume;

        return PhysicMath2D.ComputeMass(density, volume);
    }
}