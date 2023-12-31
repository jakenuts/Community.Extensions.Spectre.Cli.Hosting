using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Community.Extensions.Spectre.Cli.Hosting.Internal;

internal sealed class CustomTypeResolver : ITypeResolver, IDisposable
{
    private readonly ILogger<CustomTypeResolver> _log;

    /// <summary>
    ///     Allows for resolving types from the host service provider or from the internal
    ///     services registered by Spectre.Console.Cli.
    /// </summary>
    /// <param name="internalProvider"></param>
    /// <param name="hostServiceProvider"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CustomTypeResolver(IServiceProvider internalProvider, IServiceProvider hostServiceProvider)
    {
        _hostServiceRootProvider = hostServiceProvider;
        _hostServiceScope = _hostServiceRootProvider.CreateScope();

        InternalServiceProvider = internalProvider ?? throw new ArgumentNullException(nameof(internalProvider));
        _log = HostServiceProvider.GetRequiredService<ILogger<CustomTypeResolver>>();
    }

    /// <summary>
    ///     Cleans up the internal service provider and host service scope
    /// </summary>
    public void Dispose()
    {
        if (InternalServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        // Dispose of the outer service scope
        _hostServiceScope?.Dispose();
        _hostServiceScope = null;
    }

    /// <summary>
    ///     Called by Spectre.Console.Cli to resolve a type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public object? Resolve(Type? type)
    {
        if (type == null)
        {
            return null;
        }

        // First try to resolve from the standard host service provider
        var service = HostServiceProvider.GetService(type);

        if (service == null)
        {
            // Next try to resolve from the service provider associated with internal service
            service = InternalServiceProvider.GetService(type);

            if (service == null)
            {
                // Last fall back on activator
                service = Activator.CreateInstance(type);

                _log.LogDebug($"✨[Activator-Provider] Returned {type.FullName}");
            }
            else
            {
                _log.LogDebug($"✨[Inner-Provider] Returned {type.FullName}");
            }
        }
        else
        {
            _log.LogDebug($"✨[Outer-Provider] Returned {type.FullName}");
        }

        return service;
    }

    #region Services Registered by the Host

    private readonly IServiceProvider _hostServiceRootProvider;

    private IServiceScope? _hostServiceScope;

    private IServiceProvider HostServiceProvider => _hostServiceScope?.ServiceProvider ?? _hostServiceRootProvider;

    #endregion

    #region Internal Services Registered by the Spectre.Console.Cli

    private IServiceProvider InternalServiceProvider { get; }

    #endregion
}