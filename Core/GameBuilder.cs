﻿using MyEngine2D.Core.Input;
using MyEngine2D.Core.Level;
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
        var time = new Time();
        var levelManager = CreateLevelManager();
        var inputSystem = CreateInputSystem();

        Clear();
        return new Game(time, levelManager, inputSystem);
    }

    private void Clear()
    {
        _levels.Clear();
        _inputActions.Clear();
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
}