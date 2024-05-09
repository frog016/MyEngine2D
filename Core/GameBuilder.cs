using MyEngine2D.Core.Graphic;
using MyEngine2D.Core.Input;
using MyEngine2D.Core.Level;
using MyEngine2D.Core.Physic;
using MyEngine2D.Core.Resource;
using MyEngine2D.Core.Utility;

namespace MyEngine2D.Core;

public sealed class GameBuilder
{
    private readonly List<Func<GameLevel>> _levels = new();
    private readonly List<InputActionBase> _inputActions = new();
    private readonly List<ResourceImporter> _resourceImporters = new();
    private readonly List<LayerCollisionSetting> _physicCollisionLayers = new();

    private static readonly Type[] LevelConfiguratorTypes;

    static GameBuilder()
    {
        var type = typeof(GameLevelConfigurator);
        LevelConfiguratorTypes = type
            .GetAllChild()
            .ToArray();
    }

    public GameBuilder WithConfiguredLevels(params GameLevelConfigurator[] gameLevelConfigurators)
    {
        var getLevelFunctions = gameLevelConfigurators
            .Select(configurator => configurator.CreateLevel());

        _levels.AddRange(getLevelFunctions);
        return this;
    }

    public GameBuilder WithCustomLevels(params GameLevel[] levels)
    {
        foreach (var level in levels)
        {
            var levelLink = level;
            _levels.Add(() => levelLink);
        }

        return this;
    }

    public GameBuilder WithInputActions(params InputActionBase[] inputActions)
    {
        _inputActions.AddRange(inputActions);
        return this;
    }

    public GameBuilder WithResourceImporters(params ResourceImporter[] resourceImporters)
    {
        _resourceImporters.AddRange(resourceImporters);
        return this;
    }

    public GameBuilder WithPhysicLayers(params LayerCollisionSetting[] layerCollisionSettings)
    {
        _physicCollisionLayers.AddRange(layerCollisionSettings);
        return this;
    }

    public Game Build()
    {
        var resourceManager = CreateResourceManager();
        ServiceLocator.Instance.RegisterInstance(resourceManager);

        var time = new Time();
        ServiceLocator.Instance.RegisterInstance(time);

        var inputSystem = CreateInputSystem();
        ServiceLocator.Instance.RegisterInstance(inputSystem);

        var lazyLevelManager = new Lazy<GameLevelManager>(() =>
            ServiceLocator.Instance.Get<GameLevelManager>());

        var physicWorld = CreatePhysicWorld(lazyLevelManager);
        ServiceLocator.Instance.RegisterInstance(physicWorld);

        var graphicRender = CreateGraphicRender(lazyLevelManager);
        ServiceLocator.Instance.RegisterInstance(graphicRender);

        var levelManager = CreateLevelManager();
        ServiceLocator.Instance.RegisterInstance(levelManager);

        var game = new Game(time, levelManager, inputSystem, physicWorld, graphicRender);
        ServiceLocator.Instance.RegisterInstance(game);

        Clear();
        return game;
    }

    private void Clear()
    {
        _levels.Clear();
        _inputActions.Clear();
        _resourceImporters.Clear();
        _physicCollisionLayers.Clear();
    }

    private ResourceManager CreateResourceManager()
    {
        var resourceManager = new ResourceManager();
        var importers = GetDefaultRecourseImporters()
            .Concat(_resourceImporters);

        foreach (var recourseImporter in importers)
        {
            resourceManager.BindImporter(recourseImporter);
        }

        return resourceManager;
    }

    private GameLevelManager CreateLevelManager()
    {
        var levels = _levels
            .Select(getLevel => getLevel())
            .ToArray();

        return new GameLevelManager(levels);
    }

    private InputSystem CreateInputSystem()
    {
        var inputSystem = new InputSystem();
        foreach (var inputAction in _inputActions)
            inputSystem.AddInput(inputAction);

        return inputSystem;
    }

    private PhysicWorld CreatePhysicWorld(Lazy<GameLevelManager> lazyLevelManager)
    {
        if (_physicCollisionLayers.Count == 0)
        {
            var defaultLayerCollisionSetting = new LayerCollisionSetting(ConcreteLayer.Default, ConcreteLayer.Default);
            _physicCollisionLayers.Add(defaultLayerCollisionSetting);
        }

        var physicLayerSystem = new PhysicLayerSystem(_physicCollisionLayers.ToArray());
        return new PhysicWorld(physicLayerSystem, lazyLevelManager);
    }

    private static IEnumerable<ResourceImporter> GetDefaultRecourseImporters()
    {
        yield return new SpriteResourceImporter();
    }

    private static GraphicRender CreateGraphicRender(Lazy<GameLevelManager> lazyLevelManager)
    {
        var description = new GraphicWindowDescription(1920, 1080);
        return new GraphicRender(lazyLevelManager, description);
    }
}