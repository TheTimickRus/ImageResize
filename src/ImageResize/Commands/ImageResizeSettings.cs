// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel;
using Spectre.Console.Cli;

namespace ImageResize.Commands;

/// <summary>
/// Настройки
/// </summary>
public sealed class ImageResizeSettings : CommandSettings
{
    [Description("Path to images or folders")]
    [CommandArgument(0, "[Paths]")]
    public string[]? Paths { get; init; }
        
    [Description("Files smaller than threshold will be skipped/copied without processing (int) (Default = 350 KB)")]
    [CommandOption("-t|--threshold")]
    [DefaultValue(350)]
    public int Threshold { get; init; }

    [Description("In what resolution to save files? (from 10 to 100) (Default = 75 %)")]
    [CommandOption("-p|--percent")]
    [DefaultValue(75)]
    public int ResizePercent { get; init; }
        
    [Description("In what quality to save files? (from 10 to 100) (Default = 50 %)")]
    [CommandOption("-q|--quality")]
    [DefaultValue(50)]
    public int JpegQuality { get; init; } 
        
    [Description("How many threads should I use when working? (int) (Default = Number of cores)")]
    [CommandOption("--threads")]
    public int? ThreadsCount { get; init; }
        
    [Description("Logging to a file (Default = false)")]
    [CommandOption("--logging")]
    [DefaultValue(false)]
    public bool IsLogging { get; init; }
}