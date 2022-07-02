// ReSharper disable RedundantNullableFlowAttribute
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InvertIf

using System.ComponentModel;
using Ardalis.GuardClauses;
using ImageResize.ImageSharp;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;

namespace ImageResize;

internal class ImageResizeCommand : Command<ImageResizeCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("Путь к изображению(-ям) или папкам с изображением(-ями)")]
        [CommandArgument(0, "[Paths]")]
        public string[]? Paths { get; init; }

        [Description("На сколько процентов уменьшить разрешение изображения? (от 10 до 100)")]
        [CommandOption("-p|--percent")]
        [DefaultValue(75)]
        public int ResizePercentValue { get; init; }
        
        [Description("В каком качестве сохранить изображения? (от 10 до 100)")]
        [CommandOption("-q|--jpegQuality")]
        [DefaultValue(50)]
        public int JpegQuality { get; init; } 
        
        [Description("Сколько потоков использовать при работе? (int)")]
        [CommandOption("-t|--threads")]
        public int? ThreadsCount { get; init; }
    }

    private Settings? _settings;
    
    private readonly Table _table;
    
    private readonly List<FileInfo> _files = new();
    private readonly List<(DirectoryInfo, List<FileInfo>)> _directories = new();
    
    private int _allFilesCount;
    private int _successFilesCount;
    
    public ImageResizeCommand()
    {
        // Создаем таблицу:
        _table = new Table
        {
            Title = new TableTitle(Constants.AppFullTitle),
            Border = new MarkdownTableBorder(),
            BorderStyle = new Style(foreground: Color.CornflowerBlue),
            ShowFooters = true
        };
        _table.Centered();
        _table.AddColumns(
            new TableColumn("Статус") { Alignment = Justify.Center },
            new TableColumn("Имя файла") { Alignment = Justify.Left },
            new TableColumn("Разрешение") { Alignment = Justify.Right }, 
            new TableColumn("Размер") { Alignment = Justify.Right });
        // Создаем таблицу.
    }
    
    public override int Execute(CommandContext context, Settings settings)
    {
        Guard.Against.Null(settings.Paths);
        Guard.Against.Zero(settings.Paths.Length);
        
        _settings = settings;
        
        foreach (var path in settings.Paths)
        {
            // Для файлов
            var file = new FileInfo(path);
            if (file.Exists)
            {
                _files.Add(file);
                _allFilesCount++;

                continue;
            }

            // Для папок
            var directory = new DirectoryInfo(path);
            if (directory.Exists)
            {
                var directoryFiles = directory.GetFiles("*.*", SearchOption.AllDirectories);
                if (directoryFiles.Length == 0)
                {
                    continue;
                }

                _directories.Add((directory, directoryFiles.ToList()));
                _allFilesCount += directoryFiles.Length; 
            }
        }

        AnsiConsole.Live(_table)
            .Start(Progress);
        
        return 0;
    }

    private void Progress(LiveDisplayContext obj)
    {
        Guard.Against.Null(_settings);
        
        var threadsCount = _settings.ThreadsCount ?? Environment.ProcessorCount;

        if (_files.Count > 0)
        {
            Parallel.ForEach(_files, new ParallelOptions { MaxDegreeOfParallelism = threadsCount }, fInfo =>
            {
                if (ImageResizeLibs.ExecuteForFile(fInfo, _settings.ResizePercentValue, _settings.JpegQuality, out var newName))
                {
                    _successFilesCount++;
                }
                else
                {
                    
                }
            }); 
        }
    }
}