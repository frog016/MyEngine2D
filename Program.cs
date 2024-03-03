using MyEngine2D.Core;
using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Input;
using MyEngine2D.Core.Level;
using SharpDX.DirectInput;

namespace MyEngine2D
{
    public class Program
    {
        private static Game _game;

        public static void Main()
        {
            _game = CreateTestGame();
            _game.Run();
        }

        private static Game CreateTestGame()
        {
            var testLevel = new GameLevel("Test Level");
            var testGameObject = testLevel.Instantiate("Test Object");
            testGameObject.AddComponent<TestLogComponent>();

            return new GameBuilder()
                .WithCustomLevels(testLevel)
                .WithInputActions(new SpaceKeyboardInput())
                .Build();
        }

        public class TestLogComponent : Component
        {
            public TestLogComponent(GameObject gameObject) : base(gameObject)
            {
            }

            public override void Start()
            {
                _game.InputSystem.SubscribeInputListener<SpaceKeyboardInput>(OnSpaceKeyPressedDown);
            }

            public override void OnDestroy()
            {
                _game.InputSystem.UnsubscribeInputListener<SpaceKeyboardInput>(OnSpaceKeyPressedDown);
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