using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Community.Extensions.Spectre.Cli.Hosting;

/// <summary>
///     A background service that runs the Spectre Console App
/// </summary>
public class SpectreConsoleWorker(
    ILogger<SpectreConsoleWorker> logger,
    ICommandApp commandApp,
    IHostApplicationLifetime hostLifetime) : BackgroundService
{
    /// <summary>
    ///     The exit code returned by the Spectre Console App
    /// </summary>
    private int _exitCode;

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
        logger.LogDebug("🧵 Console Background Worker Started");

        try
        {
            var args = Environment.GetCommandLineArgs().Skip(1).ToArray();

            logger.LogTrace("_commandApp.RunAsync(args)");

            _exitCode = await commandApp.RunAsync(args);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred");
            _exitCode = 1;
        }
        finally
        {
            logger.LogDebug("🛑 Console Background Worker Stopping");
            Environment.ExitCode = _exitCode;
            hostLifetime.StopApplication();
        }
    }
}