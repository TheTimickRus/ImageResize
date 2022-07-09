using ImageResize;
using ImageResize.Services;
using Spectre.Console;
using Spectre.Console.Cli;

var app = new CommandApp<ImageResizeCommand>();
app.Configure(configurator =>
{
    configurator.Settings.ApplicationName = "ImageResize.exe";
    configurator.Settings.ApplicationVersion = "v.1.5.2 (09.07.2022)";
    configurator.Settings.ExceptionHandler += ex => 
    {
        AnsiConsoleLib.ShowFiglet(Constants.Titles.VeryShortTitle, Justify.Center, Constants.Colors.ErrorColor);
        AnsiConsole.MarkupLine("\n> [bold red]A fatal error has occurred in the operation of the program![/]\n");
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
    
        SerilogLib.Fatal(ex);

        AnsiConsole.Console.Input.ReadKey(true);
        return -1;
    };
});

return app.Run(args);