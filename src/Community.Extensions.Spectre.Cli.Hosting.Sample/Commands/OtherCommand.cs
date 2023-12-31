using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Community.Extensions.Spectre.Cli.Hosting.Sample.Commands;

/// <summary>
/// Another command, just to show that multiple commands can be added
/// </summary>
public class OtherCommand : AsyncCommand<OtherCommand.Options>
{
    private readonly SampleService _service;

    private readonly IAnsiConsole _console;

    /// <summary>
    /// Creates a OtherCommand with access to the console and logging
    /// </summary>
    /// <param name="service">A sample dependency</param>
    /// <param name="console"></param>
    /// <param name="log"></param>
    public OtherCommand(SampleService service, IAnsiConsole console, ILogger<HelloCommand> log)
    {
        _service = service;
        _console = console;
    }

    /// <summary>Executes the command.</summary>
    /// <param name="context">The command context.</param>
    /// <param name="options">The command options.</param>
    /// <returns>An integer indicating whether or not the command executed successfully.</returns>
    public override Task<int> ExecuteAsync(CommandContext context, Options options)
    {
        _console.MarkupLineInterpolated($"[springgreen2_1] Other {options.Stuff} - {_service.Message}![/]");

        return Task.FromResult(0);
    }

    /// <summary>
    /// 
    /// </summary>
    [Description("OtherOptions")]
    public class Options : CommandSettings
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("Other Stuff")]
        [CommandArgument(0, "<stuff>")]
        public string? Stuff { get; set; }
    }
}