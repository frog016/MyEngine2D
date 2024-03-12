namespace MyEngine2D.Core.Assertion;

public sealed class ServiceNotFoundException : Exception
{
    private const string MessageFormat = "Service not found. Type: {0}. Register it before using.";

    public ServiceNotFoundException(Type serviceType) : base(string.Format(MessageFormat, serviceType))
    {
    }
}