using MyEngine2D.Core;
using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Input;
using MyEngine2D.Core.Level;
using SharpDX.DirectInput;

namespace MyEngine2D
{
    public class Program
    {
        public static void Main()
        {
            var game = CreateTestGame();
            game.Run();
        }

        private static Game CreateTestGame()
        {
            var testLevel = new GameLevel("Test Level");
            
            var game = new GameBuilder()
                .WithCustomLevels(testLevel)
                .WithInputActions(new SpaceKeyboardInput())
                .Build();

            var testGameObject = testLevel.Instantiate("Test Object");
            testGameObject.AddComponent<TestLogComponent>();

            return game;
        }

        public class TestLogComponent : Component
        {
            private readonly InputSystem _inputSystem;

            public TestLogComponent(GameObject gameObject, InputSystem inputSystem) : base(gameObject)
            {
                _inputSystem = inputSystem;
            }

            public override void Start()
            {
                _inputSystem.SubscribeInputListener<SpaceKeyboardInput>(OnSpaceKeyPressedDown);
            }

            public override void OnDestroy()
            {
                _inputSystem.UnsubscribeInputListener<SpaceKeyboardInput>(OnSpaceKeyPressedDown);
            }

            public override void Update(float deltaTime)
            {
                //Console.WriteLine($"Update for {GameObject.Name} - deltaTime: {deltaTime}.");
            }

            public override void FixedUpdate(float fixedDeltaTime)
            {
                //Console.WriteLine($"FixedUpdate for {GameObject.Name} - fixedDeltaTime: {fixedDeltaTime}.");
            }

            private void OnSpaceKeyPressedDown()
            {
                Console.WriteLine($"Space button was pressed.");
            }
        }

        public class SpaceKeyboardInput : KeyboardInputAction
        {
            protected override Key TriggeredKey => Key.Space;
        }
    }
}