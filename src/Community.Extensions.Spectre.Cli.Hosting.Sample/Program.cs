using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using Community.Extensions.Spectre.Cli.Hosting;
using Community.Extensions.Spectre.Cli.Hosting.Sample.Commands;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddSimpleConsole();

// Yes this is duplicated, this is the one we'll use and that
// can receive services from the outer host service provider.
builder.Services.AddCommand<HelloCommand, HelloCommand.Options>();

builder.UseSpectreConsole<HelloCommand>(config =>
{
#if DEBUG
    config.PropagateExceptions();
    config.ValidateExamples();
#endif
    config.SetApplicationName("hello");
    config.SetExceptionHandler(BasicExceptionHandler.WriteException);

    // This configures the command with the internal service provider. 
    // Unfortunately, it comes after the external service provider & host have
    // already been built. In future configuration should be extracted to a builder
    // that can be configured prior to service provider creation allowing the two
    // AddCommand calls to be combined.
    config.AddCommand<HelloCommand>("hello");
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