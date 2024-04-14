using MyEngine2D.Core;
using MyEngine2D.Core.Level;
using MyEngine2D.Core.Math;
using MyEngine2D.Core.Physic;
using MyEngine2D.Core.Structure;

namespace MyEngine2D;

public class PhysicOverviewPreset : OverviewPreset
{
    private static readonly PhysicMaterial IronMaterial = new(7874f, 0.15f, 0.1f, 0.6f);

    private const float MinSize = 40f;
    private const float MaxSize = 65;

    protected override Game CreateOverviewGame()
    {
        const float sizeX = 1920f;
        const float sizeY = 1080f;

        var testLevel = new GameLevel("Test Level");

        var game = new GameBuilder()
            .WithCustomLevels(testLevel)
            .WithInputActions()
            .Build();

        var yGround = -370f;

        var groundPosition = new Vector2(sizeX / 6, yGround);
        var rotation = Math2D.ToRadians(-10);
        CreateGroundRectangle(groundPosition, rotation, 0, testLevel);

        groundPosition = new Vector2(5 * sizeX / 6, yGround);
        rotation = Math2D.ToRadians(10);
        CreateGroundRectangle(groundPosition, rotation, 1, testLevel);

        var startPosition = new Vector2(100, sizeY / 3f);
        var endPosition = new Vector2(sizeX - 100, sizeY / 3f);
        for (var index = 0; index < 20;)
        {
            var currentPosition = ToRight(index);
            //var currentPosition = ToLeft(index);
            //var currentPosition = new Vector2(sizeX / 2 - MaxSize / 2f * 1.9f, sizeY);
            CreateTestCircle(testLevel, currentPosition, index);
            //CreateTestRectangle(testLevel, currentPosition, index);
            index++;

            currentPosition = ToRight(index);
            //currentPosition = ToLeft(index);
            //currentPosition = new Vector2(sizeX / 2 + MaxSize / 2f * 1.9f, sizeY);
            //CreateTestCircle(testLevel, currentPosition, index);
            CreateTestRectangle(testLevel, currentPosition, index);
            index++;
        }

        return game;

        Vector2 ToRight(int index)
        {
            return startPosition + Vector2.Right * index * MaxSize;
        }

        Vector2 ToLeft(int index)
        {
            return endPosition + Vector2.Left * index * MaxSize;
        }
    }

    private static void CreateTestCircle(GameLevel testLevel, Vector2 position, int index)
    {
        var gameObject = testLevel.Instantiate($"Circle Object: {index}.", position);
        var rigidBody = gameObject.AddComponent<RigidBody>();

        var randomRadius = GetRandomSize() / 2f;
        var shape = new CirclePhysicShape(rigidBody, randomRadius);

        rigidBody.Initialize(shape, IronMaterial);
    }

    private static void CreateTestRectangle(GameLevel testLevel, Vector2 position, int index)
    {
        var gameObject = testLevel.Instantiate($"Rect Object: {index}.", position);
        var rigidBody = gameObject.AddComponent<RigidBody>();

        var randomSize = new Vector2(GetRandomSize(), GetRandomSize());
        var randomRotation = GetRandomSize(0, 2 * Math2D.PI);

        rigidBody.Rotation = randomRotation;
        var shape = new RectanglePhysicShape(rigidBody, randomSize);

        rigidBody.Initialize(shape, IronMaterial);
    }

    private static void CreateGroundRectangle(Vector2 position, float rotation, int index, GameLevel testLevel)
    {
        var testGameObject = testLevel.Instantiate($"Test Ground {index}.", position, rotation);
        var body = testGameObject.AddComponent<RigidBody>();

        var size = new Vector2(1920, 1000);
        var shape = new RectanglePhysicShape(body, size);

        body.Initialize(shape, IronMaterial, gravityScale: 0, isStatic: true);
    }

    private static float GetRandomSize(float min = MinSize, float max = MaxSize)
    {
        var unClampedSize = Random.Shared.NextSingle() * max;
        return Math2D.Clamp(unClampedSize, min, max);
    }
}