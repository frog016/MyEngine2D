using SharpDX.DirectInput;

namespace MyEngine2D.Core.Input;

public abstract class KeyboardInputAction : InputActionBase
{
    protected abstract Key TriggeredKey { get; }

    protected static readonly Keyboard Keyboard;

    static KeyboardInputAction()
    {
        Keyboard = new Keyboard(new DirectInput());

        Keyboard.Properties.BufferSize = 128;
        Keyboard.Acquire();
    }

    public override bool IsPressed()
    {
        return Keyboard.GetCurrentState().IsPressed(TriggeredKey);
    }
}