namespace MyEngine2D.Core.Assertion;

public sealed class ServiceCreationException : Exception
{
    private const string MessageFormat = "Can not create service. Type: {0}, Reason: {1}.";

    public ServiceCreationException(Type serviceType, Exception reason) : base(string.Format(MessageFormat, serviceType, reason.Message))
    {
    }
}