namespace MyEngine2D.Core.Assertion;

public sealed class PositionalRequiredParameterNotFoundException : RequiredParameterNotFoundException
{
    private const string MessageFormat = "{0} parameter must at position: {1}.";


    public PositionalRequiredParameterNotFoundException(Type type) : base(type)
    {
    }

    public PositionalRequiredParameterNotFoundException(Type type, int position) : base(type, string.Format(MessageFormat, type.Name, position))
    {
    }
}