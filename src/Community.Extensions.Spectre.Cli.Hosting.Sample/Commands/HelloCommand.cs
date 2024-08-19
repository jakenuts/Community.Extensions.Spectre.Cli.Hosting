using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Community.Extensions.Spectre.Cli.Hosting.Sample.Commands;

/// <summary>
///  Creates a HelloCommand with access to services, the console and logging
/// </summary>
/// <param name="serviceProvider"></param>
/// <param name="console"></param>
public class HelloCommand(IServiceProvider serviceProvider, IAnsiConsole console) : AsyncCommand<HelloCommand.Options>
{
    /// <summary>Executes the command.</summary>
    /// <param name="context">The command context.</param>
    /// <param name="options">The command options.</param>
    /// <returns>An integer indicating whether or not the command executed successfully.</returns>
    public override async Task<int> ExecuteAsync(CommandContext context, Options options)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        // var coolService = scope.ServiceProvider.GetRequiredService<ITheCoolestService>();

        console.MarkupLineInterpolated($"[darkseagreen2_1] Hello {options.Name}![/]");

        if (!string.IsNullOrEmpty(options.DogsName))
        {
            console.MarkupLineInterpolated($"[darkseagreen2_1] Ooooo who's a good pup? {options.DogsName} thats who! 🐶[/]");
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