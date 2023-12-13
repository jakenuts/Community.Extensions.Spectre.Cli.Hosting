using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Spectre.Console.Extensions.Hosting;

public class SpectreConsoleWorker : BackgroundService
{
    private readonly ICommandApp _commandApp;

    private readonly IHostApplicationLifetime _hostLifetime;

    private readonly ILogger<SpectreConsoleWorker> _logger;

    private int _exitCode;

    public SpectreConsoleWorker(ILogger<SpectreConsoleWorker> logger, ICommandApp commandApp,
                                IHostApplicationLifetime hostLifetime)
    {
        _logger = logger;
        _commandApp = commandApp;
        _hostLifetime = hostLifetime;

        logger.LogDebug("Constructed");
    }

    /// <summary>
    ///     This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" /> starts. The
    ///     implementation should return a task that represents
    ///     the lifetime of the long running operation(s) being performed.
    /// </summary>
    /// <param name="stoppingToken">
    ///     Triggered when
    ///     <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is
    ///     called.
    /// </param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the long running operations.</returns>
    /// <remarks>
    ///     See <see href="https://docs.microsoft.com/dotnet/core/extensions/workers">Worker Services in .NET</see> for
    ///     implementation guidelines.
    /// </remarks>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("🧵 Console Background Worker Started");

        try
        {
            var args = Environment.GetCommandLineArgs().Skip(1).ToArray();

            _logger.LogTrace("_commandApp.RunAsync(args)");

            _exitCode = await _commandApp.RunAsync(args);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred");
            _exitCode = 1;
        }
        finally
        {
            _logger.LogDebug("🛑 Console Background Worker Stopping");
            Environment.ExitCode = _exitCode;
            _hostLifetime.StopApplication();
        }
    }
}