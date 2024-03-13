using MyEngine2D.Core.Assertion;
using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core;

public sealed class ServiceLocator : Singleton<ServiceLocator>, IServiceLocator, IDisposable
{
    private readonly Dictionary<Type, object> _services = new();

    public TService Get<TService>()
    {
        var serviceType = typeof(TService);
        return (TService)Get(serviceType);
    }

    public object Get(Type serviceType)
    {
        if (_services.TryGetValue(serviceType, out var service) == false)
            throw new ServiceNotFoundException(serviceType);

        return service;
    }

    public void Register<TService>()
    {
        var serviceType = typeof(TService);
        Register(serviceType);
    }

    public void Register(Type serviceType)
    {
        try
        {
            AssertIfServiceExists(serviceType);

            var service = Activator.CreateInstance(serviceType);
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _services.Add(serviceType, service);
        }
        catch (Exception exception)
        {
            throw new ServiceCreationException(serviceType, exception);
        }
    }

    public void RegisterInstance<TService>(TService service)
    {
        var serviceType = typeof(TService);
        RegisterInstance(serviceType, service);
    }

    public void RegisterInstance(Type serviceType, object service)
    {
        try
        {
            AssertIfServiceExists(serviceType);

            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _services.Add(serviceType, service);
        }
        catch (Exception exception)
        {
            throw new ServiceCreationException(serviceType, exception);
        }
    }

    public void Dispose()
    {
        foreach (var service in _services.Values)
        {
            if (service is IDisposable disposableService)
            {
                disposableService.Dispose();
            }
        }
    }

    private void AssertIfServiceExists(Type serviceType)
    {
        if (_services.ContainsKey(serviceType))
            throw new ServiceMultipleRegistrationException(serviceType);
    }
}