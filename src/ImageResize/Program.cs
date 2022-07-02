using ImageResize;
using ImageResize.Services;
using Spectre.Console;
using Spectre.Console.Cli;

var app = new CommandApp<ImageResizeCommand>();
app.Configure(configurator =>
{
    configurator.PropagateExceptions();
});

try
{
    TiLogger.Info("Программа запущена!");
    return app.Run(args);
}
catch (Exception ex)
{
    TiLogger.Fatal(ex);
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);

    Console.ReadKey();
    return -1;
}
finally
{
    TiLogger.Info("Работа программы завершена!\n");
}
