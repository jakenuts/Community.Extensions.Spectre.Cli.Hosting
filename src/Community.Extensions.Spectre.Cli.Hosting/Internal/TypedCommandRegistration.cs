using Spectre.Console.Cli;

namespace Community.Extensions.Spectre.Cli.Hosting.Internal;

/// <summary>
///     A typed registration class for commands with their types and name
/// </summary>
/// <param name="Name"></param>
/// <typeparam name="TCommand"></typeparam>
public record TypedCommandRegistration<TCommand>(string Name, Action<ICommandConfigurator>? CommandConfigurator = null)
    : CommandRegistration(typeof(TCommand), Name) where TCommand : class, ICommand
{
    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    public override void Configure(IConfigurator configuration)
    {
        // Add the command to Spectre's configuration
        var cmdConfig = configuration.AddCommand<TCommand>(Name);

        // Optionally configure the command
        CommandConfigurator?.Invoke(cmdConfig);
    }
}