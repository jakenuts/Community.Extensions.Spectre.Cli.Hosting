using System.Text;
using Community.Extensions.Spectre.Cli.Hosting.Internal;
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
    ///     Adds a command and it's options to the service collection. Also registers the command
    ///     to be added & configured during the UseSpectreConsole call.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TOptions"></typeparam>
    /// <param name="services"></param>
    /// <param name="name"></param>
    /// <param name="commandConfigurator">The configuration action applied to the command</param>
    /// <returns></returns>
    public static IServiceCollection AddCommand<TCommand, TOptions>(this IServiceCollection services, string name,
                                                                    Action<ICommandConfigurator>? commandConfigurator = null)
        where TCommand : class, ICommand<TOptions>
        where TOptions : CommandSettings

    {
        // Could use ConfigurationHelper.GetSettingsType(typeof(TCommand)) but I want options flexible
        services.AddSingleton<TCommand>();
        services.AddTransient<TOptions>(); // Not actually using currently?
        services.RegisterCommand<TCommand>(name, commandConfigurator);
        return services;
    }

    /// <summary>
    ///     Adds the internal services to the host builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static HostApplicationBuilder AddInternalServices(HostApplicationBuilder builder)
    {
        Console.OutputEncoding = Encoding.Default;

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
            var app = new CommandApp(new CustomTypeRegistrar(builder.Services, x));
            return app.ConfigureAppAndRegisteredCommands(x, configureCommandApp);
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
            // Create the command app
            var app = new CommandApp<TDefaultCommand>(new CustomTypeRegistrar(builder.Services, x));
            return app.ConfigureAppAndRegisteredCommands(x, configureCommandApp);
        });

        return AddInternalServices(builder);
    }
}