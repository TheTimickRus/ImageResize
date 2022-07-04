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
        new FigletText(Constants.AppVeryShortTitle)
        {
            Alignment = Justify.Center, 
            Color = Constants.AppColor
        });
    AnsiConsole.WriteLine();
    
    TiLogger.Info($"{Constants.AppFullTitle} - Программа запущена!");
    return app.Run(args);
}
catch (Exception ex)
{
    AnsiConsole.Clear();
    AnsiConsole.Write(new FigletText(Constants.AppVeryShortTitle) { Color = Constants.AppColor });
    AnsiConsole.MarkupLine("\n> [bold red]В работе программы возникла фатальная ошибка![/]\n");
    
    TiLogger.Fatal(ex);
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);

    Console.ReadKey();
    return -1;
}
finally
{
    TiLogger.Info($"{Constants.AppFullTitle} - Программа завершена!\n");
}
