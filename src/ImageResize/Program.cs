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
    AnsiConsole.Clear();
    AnsiConsole.Write(
        new FigletText(Constants.Titles.VeryShortTitle)
        {
            Alignment = Justify.Center, 
            Color = Constants.Colors.MainColor
        });
    AnsiConsole.WriteLine();
    
    TiLogger.Info($"{Constants.Titles.FullTitle} - Программа запущена!");
    
    return app.Run(args);
}
catch (Exception ex)
{
    AnsiConsole.Clear();
    AnsiConsole.Write(new FigletText(Constants.Titles.VeryShortTitle) { Color = Constants.Colors.MainColor });
    AnsiConsole.MarkupLine("\n> [bold red]В работе программы возникла фатальная ошибка![/]\n");
    
    TiLogger.Fatal(ex);
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);

    AnsiConsole.Console.Input.ReadKey(true);
    return -1;
}
finally
{
    TiLogger.Info($"{Constants.Titles.FullTitle} - Программа завершена!\n");
}
