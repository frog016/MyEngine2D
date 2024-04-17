using System.Collections;
using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Graphic;

internal sealed class RenderLayer : IEnumerable<SpriteRenderer>, IDisposable
{
    private readonly SortedList<Layer, HashSet<SpriteRenderer>> _renderLayers = new();

    public RenderLayer(IEnumerable<GameObject> gameObjects)
    {
        foreach (var gameObject in gameObjects)
        {
            if (gameObject.TryGetComponent<SpriteRenderer>(out var renderer))
            {
                Add(renderer.Layer, renderer);
            }
        }
    }

    public void Add(Layer layer, SpriteRenderer renderer)
    {
        _renderLayers.TryAdd(layer, new HashSet<SpriteRenderer>());
        if (_renderLayers[layer].Add(renderer))
        {
            renderer.LayerChanged += OnLayerChanged;
        }
    }

    public void Remove(Layer layer, SpriteRenderer renderer)
    {
        if (_renderLayers[layer].Remove(renderer) == false)
        {
            return;
        }

        if (_renderLayers[layer].Count == 0)
        {
            _renderLayers.Remove(layer);
        }

        renderer.LayerChanged -= OnLayerChanged;
    }

    public IEnumerator<SpriteRenderer> GetEnumerator()
    {
        return _renderLayers
            .SelectMany(pair => pair.Value)
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Dispose()
    {
        _renderLayers.Clear();
    }

    private void OnLayerChanged(SpriteRenderer renderer, Layer oldLayer, Layer newLayer)
    {
        Remove(oldLayer, renderer);
        Add(newLayer, renderer);
    }
}