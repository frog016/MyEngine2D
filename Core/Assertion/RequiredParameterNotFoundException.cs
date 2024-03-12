namespace MyEngine2D.Core.Assertion;

public class RequiredParameterNotFoundException : Exception
{
    private const string MessageFormat = "Required parameter not found. Type: {0}.";

    public RequiredParameterNotFoundException(Type type) : base(string.Format(MessageFormat, type))
    {
    }

    public RequiredParameterNotFoundException(Type type, string message) : base($"{string.Format(MessageFormat, type)} {message}")
    {
    }
}