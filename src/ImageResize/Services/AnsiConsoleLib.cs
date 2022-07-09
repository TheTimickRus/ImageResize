using Spectre.Console;

namespace ImageResize.Services;

public static class AnsiConsoleLib
{
    public static void ShowFiglet(string text, Justify? alignment, Color? color)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText(text) { Alignment = alignment, Color = color });
        AnsiConsole.WriteLine();
    }

    public static void ShowRule(string text, Justify? alignment, Color? color)
    {
        AnsiConsole.Write(
            new Rule(text)
            {
                Alignment = alignment,
                Style = new Style(color)
            });
        AnsiConsole.WriteLine();
    }
}