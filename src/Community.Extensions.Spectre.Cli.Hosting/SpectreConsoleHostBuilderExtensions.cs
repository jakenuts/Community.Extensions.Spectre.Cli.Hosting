using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Community.Extensions.Spectre.Cli.Hosting;

/// <summary>
///     Extends <see cref="IHostBuilder" /> with SpectreConsole commands.
/// </summary>
public static class SpectreConsoleHostBuilderExtensions
{
    /// <summary>
    /// Adds a command and it's options to the service collection
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TOptions"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCommand<TCommand, TOptions>(this IServiceCollection services)
        where TCommand : class, ICommand<TOptions>
        where TOptions : CommandSettings

    {
        // Could use ConfigurationHelper.GetSettingsType(typeof(TCommand)) but I want options flexible
        services.AddSingleton<TCommand>();
        services.AddTransient<TOptions>();
        return services;
    }

    /// <summary>
    ///    Adds the internal services to the host builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static HostApplicationBuilder AddInternalServices(HostApplicationBuilder builder)
    {
        System.Console.OutputEncoding = Encoding.Default;

        builder.Services.AddHostedService<SpectreConsoleWorker>();
        builder.Services.AddSingleton(x => AnsiConsole.Console);
        builder.Services.AddSingleton<IHostLifetime, ConsoleLifetime>();

        return builder;
    }

    /// <summary>
    ///     Adds a entry point for a command line application with multi commands.
    /// </summary>
    /// <param name="builder">The host builder to configure.</param>
    /// <param name="configureCommandApp">Configures the command line application commands.</param>
    /// <returns>The host builder</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static HostApplicationBuilder UseSpectreConsole(this HostApplicationBuilder builder,
                                                           Action<IConfigurator>? configureCommandApp = null)
    {
        builder = builder ?? throw new ArgumentNullException(nameof(builder));

        builder.Services.AddSingleton<ICommandApp>(x =>
        {
            var command = new CommandApp(new CustomTypeRegistrar(builder.Services, x));

            if (configureCommandApp != null)
            {
                command.Configure(configureCommandApp);
            }

            return command;
        });

        return AddInternalServices(builder);
    }

    /// <summary>
    ///     Adds a entry point for a command line application with a default command.
    /// </summary>
    /// <param name="builder">The host builder to configure.</param>
    /// <param name="configureCommandApp">Configures the command line application.</param>
    /// <typeparam name="TDefaultCommand">The default command.</typeparam>
    /// <returns>The host builder.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static HostApplicationBuilder UseSpectreConsole<TDefaultCommand>(this HostApplicationBuilder builder,
                                                                            Action<IConfigurator>? configureCommandApp = null)
        where TDefaultCommand : class, ICommand
    {
        builder = builder ?? throw new ArgumentNullException(nameof(builder));

        builder.Services.AddSingleton<ICommandApp>(x =>
        {
            var command = new CommandApp<TDefaultCommand>(new CustomTypeRegistrar(builder.Services, x));

            if (configureCommandApp != null)
            {
                command.Configure(configureCommandApp);
            }

            return command;
        });

        return AddInternalServices(builder);
    }
}