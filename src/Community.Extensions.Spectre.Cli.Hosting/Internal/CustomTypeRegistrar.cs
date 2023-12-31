using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Community.Extensions.Spectre.Cli.Hosting.Internal;

internal sealed class CustomTypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _internalBuilder;

    private readonly ILogger _log;

    private readonly IServiceCollection _outerCollection;

    private readonly IServiceProvider _serviceProvider;

    public CustomTypeRegistrar(IServiceCollection outerCollection, IServiceProvider serviceProvider)
    {
        _log = serviceProvider.GetRequiredService<ILogger<CustomTypeRegistrar>>();

        _log.LogTrace("Constructed");

        _outerCollection = outerCollection;
        _serviceProvider = serviceProvider;
        _internalBuilder = new ServiceCollection();
    }

    public ITypeResolver Build()
    {
        _log.LogTrace("✨[Inner-Provider] ====== Building Inner Provider ====");

        return new CustomTypeResolver(_internalBuilder.BuildServiceProvider(), _serviceProvider);
    }

    public void Register(Type service, Type implementation)
    {
        if (HostServicesAlreadyContainServiceType(service))
        {
            _log.LogTrace($"✔️[Inner-Provider] Skipping duplicate registration of {implementation}");
            return;
        }

        _log.LogTrace($"✨[Inner-Provider] Register {implementation}");

        var descriptor = ServiceDescriptor.Singleton(service, implementation);
        _internalBuilder.Add(descriptor);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        if (HostServicesAlreadyContainServiceType(service))
        {
            _log.LogTrace($"✔️[Inner-Provider] Skipping duplicate registration of {implementation}");
            return;
        }

        _log.LogTrace($"✨[Inner-Provider] RegisterInstance {implementation}");

        var descriptor = ServiceDescriptor.Singleton(service, implementation);

        _internalBuilder.Add(descriptor);
    }

    public void RegisterLazy(Type service, Func<object> func)
    {
        _log.LogTrace($"✨[Inner-Provider] RegisterLazy {service}");

        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        _internalBuilder.AddSingleton(service, _ => func());
    }

    /// <summary>
    ///     Checks the service collection for any registered services matching the type
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    private bool HostServicesAlreadyContainServiceType(Type serviceType)
    {
        var count = _outerCollection.Count;

        for (var i = 0; i < count; i++)
        {
            if (_outerCollection[i].ServiceType == serviceType)
            {
                // Already added
                return true;
            }
        }

        return false;
    }
}