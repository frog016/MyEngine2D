namespace MyEngine2D.Core.Input;

public sealed class InputSystem
{
    private readonly Dictionary<Type, InputActionBase> _inputActions = new();
    private readonly Dictionary<InputActionBase, List<Action>> _inputListeners = new();

    public void AddInput<TAction>(TAction action) where TAction : InputActionBase
    {
        AddInput((InputActionBase)action);
    }

    public void AddInput(InputActionBase action)
    {
        var actionType = action.GetType();

        if (_inputActions.TryAdd(actionType, action) == false)
            throw new ArgumentException($"Can't add existing action of type: {actionType}.");
    }

    public void RemoveInput<TAction>() where TAction : InputActionBase
    {
        var actionType = typeof(TAction);
        RemoveInput(actionType);
    }

    public void RemoveInput(Type actionType)
    {
        if (_inputActions.Remove(actionType) == false)
            throw new ArgumentException($"Can't remove non existing action of type: {actionType}.");
    }

    public InputActionBase GetInputAction<TAction>() where TAction : InputActionBase
    {
        var actionType = typeof(TAction);

        if (_inputActions.TryGetValue(actionType, out var inputAction) == false)
            throw new ArgumentException($"Can't get non existing action of type: {actionType}.");

        return inputAction;
    }

    public void SubscribeInputListener<TAction>(Action listener) where TAction : InputActionBase
    {
        var inputAction = GetInputAction<TAction>();

        _inputListeners.TryAdd(inputAction, new List<Action>());
        _inputListeners[inputAction].Add(listener);
    }

    public void UnsubscribeInputListener<TAction>(Action listener) where TAction : InputActionBase
    {
        var inputAction = GetInputAction<TAction>();

        if (_inputListeners.TryGetValue(inputAction, out var listeners) == false)
            throw new ArgumentException($"Can't unsubscribe unsubscribed listener for action of type: {typeof(TAction)}.");

        listeners.Remove(listener);
    }

    internal void UpdateInput()
    {
        foreach (var (type, inputAction) in _inputActions)
        {
            var isPressed = inputAction.IsPressed();
            inputAction.WasPressedThisFrame = isPressed;

            if (isPressed == false || _inputListeners.TryGetValue(inputAction, out var listeners) == false)
            {
                continue;
            }

            foreach (var listener in listeners)
            {
                listener.Invoke();
            }
        }
    }
}