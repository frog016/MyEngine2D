namespace MyEngine2D.Core.Assertion;

public sealed class ServiceMultipleRegistrationException : Exception
{
    private const string MessageFormat = "Attempt to register already existing service. Type: {0}. All services must be single.";

    public ServiceMultipleRegistrationException(Type serviceType) : base(string.Format(MessageFormat, serviceType))
    {
    }
}