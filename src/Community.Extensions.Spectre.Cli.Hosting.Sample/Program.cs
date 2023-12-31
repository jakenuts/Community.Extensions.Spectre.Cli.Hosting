using System.Diagnostics;
using Community.Extensions.Spectre.Cli.Hosting;
using Community.Extensions.Spectre.Cli.Hosting.Sample.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Spectre.Console.Cli;

var builder = Host.CreateApplicationBuilder(args);

// Add a command and optionally configure it.
builder.Services.AddCommand<HelloCommand>("hello", cmd => { cmd.WithDescription("A command that says hello"); });

// Add another command and its dependent service

builder.Services.AddCommand<OtherCommand>("other");
builder.Services.AddScoped(s => new SampleService("Other Service"));

//
// The standard call save for the commands will be pre-added & configured
//
builder.UseSpectreConsole<HelloCommand>(config =>
{
    // All commands above are passed to config.AddCommand() by this point

#if DEBUG
    config.PropagateExceptions();
    config.ValidateExamples();
#endif
    config.SetApplicationName("hello");
    config.UseBasicExceptionHandler();
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