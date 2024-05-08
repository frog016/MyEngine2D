namespace MyEngine2D.Core.Input;

public abstract class InputActionBase
{
    public bool WasPressedThisFrame { get; internal set; }

    public abstract bool IsPressed();
}