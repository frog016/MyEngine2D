using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Level;
using MyEngine2D.Core.Utility;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Windows;

using DX2D1 = SharpDX.Direct2D1;

namespace MyEngine2D.Core.Graphic;

public sealed class GraphicRender : IDisposable
{
    internal readonly DX2D1.RenderTarget RenderTarget;

    private readonly GameLevelManager _levelManager;
    private readonly RenderForm _window;
    private readonly RenderLoop _renderLoop;
    private readonly DX2D1.Factory _factory;
    private readonly DX2D1.Brush _brush;
    private readonly DX2D1.Brush _debugBrush;

    private RenderLayer _renderLayer;

    private static readonly SharpDX.Color DefaultBackgroundColor = new(32, 103, 176);
    private static readonly SharpDX.Color DefaultShapeColor = new(30, 30, 30);
    private static readonly SharpDX.Color DefaultDebugColor = new(255, 0, 0);

    private const float DebugPointSize = 4f;

    internal GraphicRender(GameLevelManager levelManager, GraphicWindowDescription description)
    {
        _levelManager = levelManager;
        _window = new RenderForm(description.Header)
        {
            ClientSize = new Size(description.Width, description.Height),
            AllowUserResizing = false
        };

        _renderLoop = new RenderLoop(_window);
        _factory = new DX2D1.Factory();

        RenderTarget = InitializeRenderingDevice();
        _brush = new DX2D1.SolidColorBrush(RenderTarget, DefaultShapeColor);
        _debugBrush = new DX2D1.SolidColorBrush(RenderTarget, DefaultDebugColor);
    }

    internal void Run()
    {
        _renderLayer = new RenderLayer(_levelManager.CurrentLevel.GameObjects);

        _levelManager.CurrentLevel.GameObjects.Added += OnObjectAdded;
        _levelManager.CurrentLevel.GameObjects.Removed += OnObjectRemoved;

        _window.Show();
    }

    internal void Render()  //  TODO: Добавить буффер для отрисовки кадров (SwapChain). Понять, как он работает для 2д.
    {
        if (_renderLoop.NextFrame())
        {
            RenderTarget.BeginDraw();
            RenderTarget.Clear(DefaultBackgroundColor);

            RenderGameObjects();

            RenderTarget.EndDraw();
        }
    }

    public void DrawDebugPoint(Structure.Vector2 point, float radius = DebugPointSize)    //  TODO: Улучшить сисетму Debug отрисовки.
    {
        RenderTarget.BeginDraw();
        RenderTarget.DrawEllipse(new DX2D1.Ellipse(point.ToRawVector2(), radius, radius), _debugBrush);
        RenderTarget.EndDraw();
    }

    public void DrawDebugLine(Structure.Vector2 start, Structure.Vector2 end, float width = DebugPointSize)
    {
        RenderTarget.BeginDraw();
        RenderTarget.DrawLine(start.ToRawVector2(), end.ToRawVector2(), _debugBrush, width);
        RenderTarget.EndDraw();
    }

    public void Dispose()
    {
        _levelManager.CurrentLevel.GameObjects.Added -= OnObjectAdded;
        _levelManager.CurrentLevel.GameObjects.Removed -= OnObjectRemoved;

        RenderTarget.Dispose();
        _renderLayer.Dispose();
        _window.Dispose();
        _renderLoop.Dispose();
        _factory.Dispose();
        _brush.Dispose();
        _debugBrush.Dispose();
    }

    private DX2D1.RenderTarget InitializeRenderingDevice()
    {
        var renderTargetProperties = new DX2D1.RenderTargetProperties(
            new DX2D1.PixelFormat(Format.R8G8B8A8_UNorm, DX2D1.AlphaMode.Premultiplied));

        var hwndRenderTargetProperties = new DX2D1.HwndRenderTargetProperties()
        {
            Hwnd = _window.Handle,
            PixelSize = new Size2(_window.Width, _window.Height),
        };

        var renderTarget = new DX2D1.WindowRenderTarget(_factory, renderTargetProperties, hwndRenderTargetProperties);
        renderTarget.Transform =
            Matrix3x2.Scaling(1, -1) *
            Matrix3x2.Translation(0, _window.Height);

        return renderTarget;
    }

    private void RenderGameObjects()
    {
        foreach (var spriteRenderer in _renderLayer)
        {
            spriteRenderer.Render(RenderTarget);
        }
    }

    private void OnObjectAdded(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<SpriteRenderer>(out var renderer))
        {
            _renderLayer.Add(renderer.Layer, renderer);
        }
    }

    private void OnObjectRemoved(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<SpriteRenderer>(out var renderer))
        {
            _renderLayer.Remove(renderer.Layer, renderer);
        }
    }
}