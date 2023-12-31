namespace Community.Extensions.Spectre.Cli.Hosting.Sample.Commands;

/// <summary>
///     A SampleService to show commands can have dependencies injected
/// </summary>
public class SampleService(string message)
{
    /// <summary>
    ///     A message
    /// </summary>
    public string Message { get; set; } = message;
}