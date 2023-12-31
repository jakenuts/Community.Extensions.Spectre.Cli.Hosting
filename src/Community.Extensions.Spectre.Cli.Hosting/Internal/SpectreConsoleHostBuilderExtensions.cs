using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Community.Extensions.Spectre.Cli.Hosting.Internal;

/// <summary>
///     Extends <see cref="IHostBuilder" /> with SpectreConsole commands.
/// </summary>
public static class CommandRegistrationExtensions
{
   
    /// <summary>
    ///     Returns registered commands
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static IEnumerable<CommandRegistration> GetRegisteredCommands(this IServiceProvider serviceProvider) =>
        serviceProvider.GetServices<CommandRegistration>();

    /// <summary>
    ///     Registers a command with it's primary type, name and optional configuration action
    /// </summary>
    /// <param name="services"></param>
    /// <param name="name"></param>
    /// <param name="commandConfigurator"></param>
    /// <typeparam name="TCommand"></typeparam>
    /// <returns></returns>
    public static IServiceCollection RegisterCommand<TCommand>(this IServiceCollection services, string name,
                                                               Action<ICommandConfigurator>? commandConfigurator = null)
        where TCommand : class, ICommand
    {
        return services.AddTransient<CommandRegistration, TypedCommandRegistration<TCommand>>(c =>
            new TypedCommandRegistration<TCommand>(name, commandConfigurator));
    }

    /// <summary>
    /// Adds registered commands to the provided app and allows further customization of the app and commands
    /// </summary>
    /// <param name="app"></param>
    /// <param name="provider"></param>
    /// <param name="configureCommandApp"></param>
    /// <returns></returns>
    internal static ICommandApp ConfigureAppAndRegisteredCommands(this ICommandApp app, IServiceProvider provider, Action<IConfigurator>? configureCommandApp = null)
    {
        app.Configure(config =>
        {
            // Add/Configure registered commands
            foreach (var cmd in provider.GetRegisteredCommands())
            {
                cmd.Configure(config);
            }

            // Optionally allow caller to configure the command app
            configureCommandApp?.Invoke(config);
        });

        return app;
    }
}