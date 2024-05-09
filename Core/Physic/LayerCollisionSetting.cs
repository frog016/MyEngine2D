namespace MyEngine2D.Core.Physic;

public struct LayerCollisionSetting
{
    public readonly ConcreteLayer FirstLayer;
    public readonly ConcreteLayer SecondLayer;

    public LayerCollisionSetting(ConcreteLayer firstLayer, ConcreteLayer secondLayer)
    {
        FirstLayer = firstLayer;
        SecondLayer = secondLayer;
    }
}