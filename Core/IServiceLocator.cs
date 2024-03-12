namespace MyEngine2D.Core;

public interface IServiceLocator
{
    TService Get<TService>();
    object Get(Type serviceType);
    void Register<TService>();
    void Register(Type serviceType);
    void RegisterInstance<TService>(TService service);
    void RegisterInstance(Type serviceType, object service);
}