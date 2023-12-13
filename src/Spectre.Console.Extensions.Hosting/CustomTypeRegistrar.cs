using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;


namespace Spectre.Console.Extensions.Hosting;

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
        var descriptor = ServiceDescriptor.Singleton(service, implementation);

        if (ContainsMatchingServiceDescriptor(_outerCollection, descriptor))
        {
            _log.LogTrace($"✔️[Inner-Provider] Skipping duplicate registration of {implementation}");
            return;
        }

        _log.LogTrace($"✨[Inner-Provider] Register {implementation}");

        _internalBuilder.Add(descriptor);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        var descriptor = ServiceDescriptor.Singleton(service, implementation);

        if (ContainsMatchingServiceDescriptor(_outerCollection, descriptor))
        {
            _log.LogTrace($"✔️[Inner-Provider] Skipping duplicate registration of {implementation}");
            return;
        }

        _log.LogTrace($"✨[Inner-Provider] RegisterInstance {implementation}");

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
    ///     Easier to implement with linq but this is the frameworks internal implementation
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="descriptor"></param>
    /// <returns></returns>
    private static bool ContainsMatchingServiceDescriptor(IServiceCollection collection, ServiceDescriptor descriptor)
    {
        var count = collection.Count;

        for (var i = 0; i < count; i++)
        {
            if (collection[i].ServiceType == descriptor.ServiceType
                && collection[i].ServiceKey == descriptor.ServiceKey)
            {
                // Already added
                return true;
            }
        }

        return false;
    }
}