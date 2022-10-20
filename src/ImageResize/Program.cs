using System.Text;
using ImageResize;
using ImageResize.Commands;
using ImageResize.Services;
using Spectre.Console;
using Spectre.Console.Cli;

Console.OutputEncoding = Encoding.UTF8;
Console.Title = Constants.Titles.ShortTitle;
SerilogLib.FileName = Constants.Titles.LogFileName;

var app = new CommandApp<ImageResizeCommand>();
app.Configure(conf =>
{
    conf.AddExample(new[] { @"C:\Users\Timick\Desktop\Images1" });
    conf.AddExample(new[] { @"C:\Users\Timick\Desktop\Images1", @"C:\Users\Timick\Desktop\Images2" });
    conf.AddExample(new[] { @"C:\Users\Timick\Desktop\Images1", @"C:\Users\Timick\Desktop\Images2", "-t 500", "--threads 64", "--logging" });
    conf.AddExample(new[] { @"C:\Users\Timick\Desktop\Images1", @"C:\Users\Timick\Desktop\Images2", "-p 60", "-q 60" });
    
    conf.Settings.ApplicationName = $"{Constants.Titles.VeryShortTitle}.exe";
    conf.Settings.ApplicationVersion = Constants.Titles.VersionWithDate;
    conf.Settings.ExceptionHandler += ex => 
    {
        AnsiConsoleLib.ShowFiglet(Constants.Titles.VeryShortTitle, Justify.Center, Constants.Colors.ErrorColor);
        AnsiConsoleLib.ShowRule(Constants.Titles.FullTitle, Justify.Right, Constants.Colors.ErrorColor);
        
        AnsiConsole.MarkupLine("\n> [bold red]A fatal error has occurred in the operation of the program![/]\n");
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
        
        SerilogLib.Fatal(ex);
        
        AnsiConsole.Console.Input.ReadKey(true);
        return -1;
    };
});

return app.Run(args);