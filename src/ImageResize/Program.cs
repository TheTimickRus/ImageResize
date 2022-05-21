using ImageResize;
using Spectre.Console;
using Spectre.Console.Cli;

var app = new CommandApp<MainCommand>();
app.Configure(configurator =>
{
    configurator.PropagateExceptions();
});

try
{
    return app.Run(args);
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
    Console.ReadKey();
    return -1;
}
