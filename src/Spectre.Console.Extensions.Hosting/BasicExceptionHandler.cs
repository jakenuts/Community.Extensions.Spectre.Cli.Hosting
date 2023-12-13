using Spectre.Console.Cli;

namespace Spectre.Console.Extensions.Hosting;

public static class BasicExceptionHandler
{
    public static void WriteException(Exception e)
    {
        if (e is CommandAppException cae)
        {
            if (cae.Pretty is { } pretty)
            {
                AnsiConsole.Write(pretty);
            }
            else
            {
                AnsiConsole.MarkupInterpolated($"[red]Error:[/] {e.Message}");
            }
        }
        else
        {
            AnsiConsole.WriteException(e, new ExceptionSettings
            {
                Format = ExceptionFormats.ShortenEverything,
                Style = new()
                {
                    ParameterName = Color.Grey,
                    ParameterType = Color.Grey78,
                    LineNumber = Color.Grey78
                }
            });
        }
    }
}