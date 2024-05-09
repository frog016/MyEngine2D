using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Physic;

public sealed class PhysicLayerSystem
{
    private readonly Dictionary<Layer, HashSet<Layer>> _layerCollisionTable;
    private readonly Dictionary<string, Layer> _layerNames;

    public PhysicLayerSystem(LayerCollisionSetting[] layerSettings)
    {
        _layerCollisionTable = CreateLayerCollisionTable(layerSettings);
        _layerNames = InitializeLayerNames(layerSettings);
    }

    public bool LayersColliding(string firstLayerName, string secondLayerName)
    {
        return _layerNames.TryGetValue(firstLayerName, out var firstLayer) &&
               _layerNames.TryGetValue(secondLayerName, out var secondLayer) &&
               LayersColliding(firstLayer, secondLayer);
    }

    public bool LayersColliding(ConcreteLayer firstLayer, ConcreteLayer secondLayer)
    {
        return _layerNames.ContainsKey(firstLayer.Name) &&
               _layerNames.ContainsKey(secondLayer.Name) &&
               LayersColliding(firstLayer.Layer, secondLayer.Layer);
    }

    public bool LayersColliding(Layer firstLayer, Layer secondLayer)
    {
        return _layerCollisionTable.TryGetValue(firstLayer, out var collidingLayers) &&
               collidingLayers.Contains(secondLayer);
    }

    private static Dictionary<Layer, HashSet<Layer>> CreateLayerCollisionTable(IEnumerable<LayerCollisionSetting> layerSettings)
    {
        var layerCollisionTable = new Dictionary<Layer, HashSet<Layer>>();
        foreach (var layer in layerSettings)
        {
            layerCollisionTable.TryAdd(layer.FirstLayer.Layer, new HashSet<Layer>());
            layerCollisionTable[layer.FirstLayer.Layer].Add(layer.SecondLayer.Layer);
        }

        return layerCollisionTable;
    }

    private static Dictionary<string, Layer> InitializeLayerNames(IEnumerable<LayerCollisionSetting> layerSettings)
    {
        var layerNames = new Dictionary<string, Layer>();
        foreach (var layerSetting in layerSettings)
        {
            layerNames.TryAdd(layerSetting.FirstLayer.Name, layerSetting.FirstLayer.Layer);
            layerNames.TryAdd(layerSetting.SecondLayer.Name, layerSetting.SecondLayer.Layer);
        }

        return layerNames;
    }
}