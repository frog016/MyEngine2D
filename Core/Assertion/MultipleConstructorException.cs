namespace MyEngine2D.Core.Assertion;

public sealed class MultipleConstructorException : Exception
{
    private const string MessageFormat = "Сlass must use only one constructor. Type: {0}.";

    public MultipleConstructorException(Type type) : base(string.Format(MessageFormat, type))
    {
    }
}