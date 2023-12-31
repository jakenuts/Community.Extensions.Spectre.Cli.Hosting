# Community.Extensions.Spectre.Cli.Hosting

Extension methods and a bit of fancy footwork to host Spectre.Console.Cli in a HostApplicationBuilder

## Usage

```
    dotnet add package Community.Extensions.Spectre.Cli.Hosting
```

## Sample Project

```

var builder = Host.CreateApplicationBuilder(args);

// Add a command and optionally configure it.
builder.Services.AddCommand<HelloCommand>("hello", cmd =>
{
    cmd.WithDescription("A command that says hello");
});


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
```
