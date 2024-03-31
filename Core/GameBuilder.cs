﻿using MyEngine2D.Core.Graphic;
using MyEngine2D.Core.Input;
using MyEngine2D.Core.Level;
using MyEngine2D.Core.Physic;
using MyEngine2D.Core.Resource;
using MyEngine2D.Core.Structure;
using MyEngine2D.Core.Utility;

namespace MyEngine2D.Core;

public sealed class GameBuilder
{
    private readonly List<GameLevel> _levels = new();
    private readonly List<InputActionBase> _inputActions = new();

    private static readonly Type[] LevelConfiguratorTypes;

    static GameBuilder()
    {
        var type = typeof(GameLevelConfigurator);
        LevelConfiguratorTypes = type
            .GetAllChild()
            .ToArray();
    }

    public GameBuilder WithConfiguredLevels()
    {
        var levelConfigurators = LevelConfiguratorTypes
            .Select(Activator.CreateInstance)
            .Cast<GameLevelConfigurator>();

        var levels = levelConfigurators
            .Select(configurator => configurator.CreateLevel());

        _levels.AddRange(levels);
        return this;
    }

    public GameBuilder WithCustomLevels(params GameLevel[] levels)
    {
        _levels.AddRange(levels);
        return this;
    }

    public GameBuilder WithInputActions(params InputActionBase[] inputActions)
    {
        _inputActions.AddRange(inputActions);
        return this;
    }

    public Game Build()
    {
        var resourceManager = CreateResourceManager();

        var time = new Time();
        var levelManager = CreateLevelManager();
        var inputSystem = CreateInputSystem();
        var physicWorld = new PhysicWorld(levelManager);
        var graphicRender = CreateGraphicRender(levelManager);

        var game = new Game(time, levelManager, inputSystem, physicWorld, graphicRender);

        RegisterGameServices(time, levelManager, inputSystem, game, resourceManager);
        Clear();

        return game;
    }

    private static GraphicRender CreateGraphicRender(GameLevelManager levelManager)
    {
        var description = new GraphicWindowDescription(1920, 1080);
        return new GraphicRender(levelManager, description);
    }

    private static ResourceManager CreateResourceManager()
    {
        var resourceManager = new ResourceManager();
        return resourceManager;
    }

    private GameLevelManager CreateLevelManager()
    {
        var levels = _levels.ToArray();
        return new GameLevelManager(levels);
    }

    private InputSystem CreateInputSystem()
    {
        var inputSystem = new InputSystem();
        foreach (var inputAction in _inputActions)
            inputSystem.AddInput(inputAction);

        return inputSystem;
    }

    private static void RegisterGameServices(Time time, GameLevelManager levelManager, InputSystem inputSystem,
        Game game, ResourceManager resourceManager)
    {
        ServiceLocator.Instance.RegisterInstance(time);
        ServiceLocator.Instance.RegisterInstance(levelManager);
        ServiceLocator.Instance.RegisterInstance(inputSystem);
        ServiceLocator.Instance.RegisterInstance(game);
        ServiceLocator.Instance.RegisterInstance(resourceManager);
    }

    private void Clear()
    {
        _levels.Clear();
        _inputActions.Clear();
    }
}