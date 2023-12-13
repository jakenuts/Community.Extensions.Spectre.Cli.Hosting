using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Spectre.Console.Extensions.Hosting;

internal sealed class CustomTypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _outerProvider;

    private readonly IServiceProvider _serviceProvider;

    private readonly ILogger<CustomTypeResolver> _log;

    public CustomTypeResolver(IServiceProvider internalProvider, IServiceProvider outerProvider)
    {
        _outerProvider = outerProvider;
        _serviceProvider = internalProvider ?? throw new ArgumentNullException(nameof(internalProvider));

        _log = _outerProvider.GetRequiredService<ILogger<CustomTypeResolver>>();
    }

    public void Dispose()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    public object? Resolve(Type? type)
    {
        if (type == null)
        {
            return null;
        }

        var service = _outerProvider.GetService(type);

        if (service == null)
        {
            service = _serviceProvider.GetService(type);

            if (service == null)
            {
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
}