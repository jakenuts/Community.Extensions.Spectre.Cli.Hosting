using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using Community.Extensions.Spectre.Cli.Hosting;
using Community.Extensions.Spectre.Cli.Hosting.Sample.Commands;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddSimpleConsole();

// Adds the commands to the outer IServiceCollection and registers them
// to be added when Spectre.Console.Cli is configured below.

builder.Services.AddCommand<HelloCommand, HelloCommand.Options>("hello", config =>
{
    config
        .WithDescription("A command that says hello")
        .WithExample("An example of other stuff");

    //.WithAlias("yo");
});

builder.Services.AddCommand<OtherCommand, OtherCommand.Options>("other");


builder.UseSpectreConsole<HelloCommand>(config =>
{
    // All commands above are passed to config.AddCommand() by this point
#if DEBUG
    config.PropagateExceptions();
    config.ValidateExamples();
#endif
    config.SetApplicationName("hello");
    config.SetExceptionHandler(BasicExceptionHandler.WriteException);
});

var app = builder.Build();
await app.RunAsync();

#if DEBUG
if (Debugger.IsAttached)
{
    AnsiConsole.WriteLine();
    AnsiConsole.Markup("Hit <Enter> to exit");
    Console.ReadLine();
}
#endif