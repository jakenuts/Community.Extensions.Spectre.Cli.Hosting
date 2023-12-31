using Spectre.Console;
using Spectre.Console.Cli;

namespace Community.Extensions.Spectre.Cli.Hosting;

/// <summary>
/// A basic handler that writes the exception to the AnsiConsole.
/// </summary>
public static class BasicExceptionHandler
{
    /// <summary>
    /// Writes the exception to the AnsiConsole.
    /// </summary>
    /// <param name="e"></param>
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