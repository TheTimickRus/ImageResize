// ReSharper disable RedundantNullableFlowAttribute
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InvertIf

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Ardalis.GuardClauses;
using ImageResize.Models;
using ImageResize.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;

namespace ImageResize;

internal class ImageResizeCommand : Command<ImageResizeCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("Path to images or folders")]
        [CommandArgument(0, "[Paths]")]
        public string[]? Paths { get; init; }

        [Description("How many percent to reduce the image resolution? (from 10 to 100)")]
        [CommandOption("-p|--percent")]
        [DefaultValue(75)]
        public int ResizePercent { get; init; }
        
        [Description("In what quality should I save the images? (from 10 to 100)")]
        [CommandOption("-q|--jpegQuality")]
        [DefaultValue(50)]
        public int JpegQuality { get; init; } 
        
        [Description("How many threads should I use when working? (int)")]
        [CommandOption("-t|--threads")]
        public int? ThreadsCount { get; init; }
        
        [Description("Logging to a file")]
        [CommandOption("-l|--logging")]
        [DefaultValue(false)]
        public bool IsLogging { get; init; }
    }

    private Settings? _settings;
    
    private readonly Table _table;
    
    private readonly List<FileInfo> _files = new();
    private readonly List<(DirectoryInfo, List<FileInfo>)> _directories = new();

    private readonly ProgressModel _progress = new();
    
    public ImageResizeCommand()
    {
        Console.Title = Constants.Titles.FullTitle;
        
        _table = new Table
        {
            Title = new TableTitle(Constants.Titles.ShortTitle, new Style(Constants.Colors.MainColor)),
            Border = new MarkdownTableBorder(),
            BorderStyle = new Style(foreground: Constants.Colors.MainColor),
            ShowFooters = true
        };
        _table.Centered();
        _table.AddColumns(
            new TableColumn("Status") { Alignment = Justify.Center },
            new TableColumn("Filename") { Alignment = Justify.Left },
            new TableColumn("Resolution") { Alignment = Justify.Right }, 
            new TableColumn("Size") { Alignment = Justify.Right });
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        _settings = settings;

        try
        {
            Guard.Against.Null(settings.Paths);
        }
        catch
        {
            RestartWithHelp();
            return -1;
        }
        
        AnsiConsoleLib.ShowFiglet(Constants.Titles.VeryShortTitle, Justify.Center, Constants.Colors.MainColor);
        
        SerilogLib.IsLogging = _settings.IsLogging;
        SerilogLib.Info($"{Constants.Titles.FullTitle} - The program is running!");
        SerilogLib.Info("\tAdding files...");

        foreach (var path in settings.Paths)
        {
            // Для файлов
            var file = new FileInfo(path);
            if (file.Exists)
            {
                _files.Add(file);

                SerilogLib.Info($"\t\tFile added: {file.Name}");
                continue;
            }

            // Для папок
            var directory = new DirectoryInfo(path);
            if (directory.Exists)
            {
                var directoryFiles = directory.GetFiles("*.*", SearchOption.AllDirectories);
                if (directoryFiles.Length == 0)
                    continue;

                _directories.Add((directory, directoryFiles.ToList()));

                SerilogLib.Info($"\tAdded directory: {directory.Name}");
                directoryFiles.ToList().ForEach(fInfo => SerilogLib.Info($"\t\tFile added: {fInfo.Name}"));
            }
        }

        _progress.AllFilesCount = _files.Count + _directories.Sum(tuple => tuple.Item2.Count);
        _progress.ProcessedFilesCount = 0;
        
        Guard.Against.Zero(_progress.AllFilesCount);
        
        SerilogLib.Info($"\tAdding files is complete! Number of files: {_progress.AllFilesCount}\nFile processing...");
        AnsiConsole.Live(_table)
            .Start(Progress);
        SerilogLib.Info("\tFile processing is complete!");

        if (_progress.AllFilesCount == _progress.ProcessedFilesCount)
        {
            SerilogLib.Info("The work of the program is completed!");
            AnsiConsoleLib.ShowRule(
                "The work of the program is completed! Press any key to exit...",
                Justify.Center,
                Constants.Colors.SuccessColor);
        }
        else
        {
            SerilogLib.Info("Not all files have been processed!");
            AnsiConsoleLib.ShowRule(
                "Not all files have been processed! Press any key to exit...",
                Justify.Center,
                Constants.Colors.ErrorColor);
        }
        
        SerilogLib.Info($"{Constants.Titles.FullTitle} - The program is completed!\n");
        AnsiConsole.Console.Input.ReadKey(true);
        return 0;
    }

    private void Progress(LiveDisplayContext ctx)
    {
        Guard.Against.Null(_settings);

        var threadsCount = _settings.ThreadsCount ?? Environment.ProcessorCount;

        if (_files.Count > 0)
        {
            Parallel.ForEach(_files, new ParallelOptions { MaxDegreeOfParallelism = threadsCount }, 
                fInfo =>
                {
                    var newFile = ImageSharpLib.ExecuteForFile(fInfo, _settings.ResizePercent, _settings.JpegQuality);
                    AddRowToTable(ctx, newFile);
                });
        }
        
        if (_directories.Count > 0)
        {
            foreach (var (baseDirInfo, baseDirFiles) in _directories)
            {
                Parallel.ForEach(baseDirFiles, new ParallelOptions { MaxDegreeOfParallelism = threadsCount }, 
                    fInfo =>
                    {
                        var newFile = ImageSharpLib.ExecuteForFolder(
                            baseDirInfo.FullName, 
                            fInfo, 
                            _settings.ResizePercent, 
                            _settings.JpegQuality);

                        AddRowToTable(ctx, newFile);
                    });
            }   
        }
    }

    private void AddRowToTable(LiveDisplayContext ctx, FileModel? fileModel)
    {
        var data = new[]
        {
            fileModel?.Status.ToString() ?? FileStatus.Skip.ToString(), 
            Path.GetFileName(fileModel?.Name) ?? "n/n", 
            $"{fileModel?.OriginalResolution?.ToString() ?? "n/n"} -> {fileModel?.Resolution?.ToString() ?? "n/n"}", 
            $"{fileModel?.OriginalSize.ToString("0.00")} -> {fileModel?.Size.ToString("0.00")}" 
        };

        lock (_progress)
        {
            _progress.ProcessedFilesCount++;
        }
        
        SerilogLib.Info($"\t\t{string.Join(", ", data)}");
        
        _table.AddRow(data);
        _table.Caption(
            new TableTitle(
                $"Completed: {_progress.ProcessedFilesCount} of {_progress.AllFilesCount} ({_progress.Percent})"));
        
        ctx.Refresh();
    }

    /// <summary>
    /// При попытке запустить программу без аргументов показываем Help
    /// </summary>
    private static void RestartWithHelp()
    {
        var exeName = AppDomain.CurrentDomain.FriendlyName;
        var exeNameFullPath = Assembly.GetEntryAssembly()?.Location.Replace(".dll", ".exe");
        
        var cmdArgs = $"cls && \"{exeNameFullPath}\" -h && echo: &&  pause";
        AnsiConsole.WriteLine(cmdArgs);

        Process.Start(
            new ProcessStartInfo
            {
                FileName = "cmd", 
                Arguments = $"/c {cmdArgs}", 
                WindowStyle = ProcessWindowStyle.Hidden
            });
    }
}