using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Community.Extensions.Spectre.Cli.Hosting.Sample.Commands;

/// <summary>
/// </summary>
public class HelloCommand : AsyncCommand<HelloCommand.Options>
{
    private readonly IAnsiConsole _console;

    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Creates a HelloCommand with access to services, the console and logging
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="console"></param>
    /// <param name="log"></param>
    public HelloCommand(IServiceProvider serviceProvider, IAnsiConsole console, ILogger<HelloCommand> log)
    {
        _serviceProvider = serviceProvider;
        _console = console;
    }

    /// <summary>Executes the command.</summary>
    /// <param name="context">The command context.</param>
    /// <param name="options">The command options.</param>
    /// <returns>An integer indicating whether or not the command executed successfully.</returns>
    public override async Task<int> ExecuteAsync(CommandContext context, Options options)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        // var coolService = scope.ServiceProvider.GetRequiredService<ITheCoolestService>();

        _console.MarkupLineInterpolated($"[darkseagreen2_1] Hello {options.Name}![/]");

        if (!string.IsNullOrEmpty(options.DogsName))
        {
            _console.MarkupLineInterpolated($"[darkseagreen2_1] Ooooo who's a good pup? {options.DogsName} thats who! 🐶[/]");
        }

        return 0;
    }

    /// <summary>
    /// </summary>
    [Description("Says 'Hello' to you and optionally your dog.")]
    public class Options : CommandSettings
    {
        /// <summary>
        /// </summary>
        [Description("Your Name")]
        [CommandArgument(0, "<name>")]
        [Required]
        public string? Name { get; set; }

        /// <summary>
        /// </summary>
        [Description("Your Dogs Name (Optional)")]
        [CommandArgument(1, "[name]")]
        [CommandOption("-p|--pup")]
        public string? DogsName { get; set; }
    }
}