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
    AnsiConsoleLib.ShowFiglet(Constants.Titles.VeryShortTitle, Justify.Center, Constants.Colors.MainColor);
    SerilogLib.Info($"{Constants.Titles.FullTitle} - Программа запущена!");
    return app.Run(args);
}
catch (Exception ex)
{
    AnsiConsoleLib.ShowFiglet(Constants.Titles.VeryShortTitle, Justify.Center, Constants.Colors.ErrorColor);
    AnsiConsole.MarkupLine("\n> [bold red]В работе программы возникла фатальная ошибка![/]\n");
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
    
    SerilogLib.Fatal(ex);

    AnsiConsole.Console.Input.ReadKey(true);
    return -1;
}
finally
{
    SerilogLib.Info($"{Constants.Titles.FullTitle} - Программа завершена!\n");
}
