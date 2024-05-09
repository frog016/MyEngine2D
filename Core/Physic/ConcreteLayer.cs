using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Physic;

public readonly struct ConcreteLayer
{
    public readonly string Name;
    public readonly Layer Layer;

    public static readonly ConcreteLayer Default = new("Default", 0);

    public ConcreteLayer(string name, Layer layer)
    {
        Name = name;
        Layer = layer;
    }
}