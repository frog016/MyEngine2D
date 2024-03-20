namespace MyEngine2D.Core.Physic;

public readonly struct PhysicMaterial
{
    public readonly float Density;
    public readonly float StaticFriction;
    public readonly float DynamicFriction;
    public readonly float Elasticity;

    public PhysicMaterial(float density, float staticFriction, float dynamicFriction, float elasticity)
    {
        Density = density;
        StaticFriction = staticFriction;
        DynamicFriction = dynamicFriction;
        Elasticity = elasticity;
    }
}