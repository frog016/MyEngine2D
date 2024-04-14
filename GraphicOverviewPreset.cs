﻿using MyEngine2D.Core;
using MyEngine2D.Core.Graphic;
using MyEngine2D.Core.Level;
using MyEngine2D.Core.Math;
using MyEngine2D.Core.Structure;

namespace MyEngine2D;

public class GraphicOverviewPreset : OverviewPreset
{
    private const float ScreenX = 1920;
    private const float ScreenY = 1080;

    protected override Game CreateOverviewGame()
    {
        var testLevel = new GameLevel("Graphic Level");

        var game = new GameBuilder()
            .WithCustomLevels(testLevel)
            .WithInputActions()
            .Build();

        var position = new Vector2(ScreenX, ScreenY) / 2f;
        CreateObject(testLevel, position - new Vector2(ScreenX / 4f, 0), Math2D.PI /  3);
        CreateObject(testLevel, position + new Vector2(ScreenX / 4f, 0), 2.15f);

        return game;
    }

    private static void CreateObject(GameLevel level, Vector2 position, float rotation)
    {
        var testObject = level.Instantiate("Test Object", position, rotation);
        var spriteRender = testObject.AddComponent<SpriteRenderer>();

        var sprite = new Sprite(@"D:\C#\MyEngine2D\Ball.png");
        spriteRender.Initialize(sprite);
    }
}