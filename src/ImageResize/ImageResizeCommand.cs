﻿// ReSharper disable RedundantNullableFlowAttribute
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InvertIf

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
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
        [Description("Путь к изображению(-ям) или папкам с изображением(-ями)")]
        [CommandArgument(0, "[Paths]")]
        public string[]? Paths { get; init; }

        [Description("На сколько процентов уменьшить разрешение изображения? (от 10 до 100)")]
        [CommandOption("-p|--percent")]
        [DefaultValue(75)]
        public int ResizePercent { get; init; }
        
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

    private readonly ProgressModel _progress = new();
    
    public ImageResizeCommand()
    {
        Console.Title = Constants.AppFullTitle;
        
        // Создаем таблицу:
        _table = new Table
        {
            Title = new TableTitle(Constants.AppShortTitle, new Style(Constants.AppColor)),
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
    
    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
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
                
                TiLogger.Info($"Добавлен файл: {file.Name}");
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
                
                TiLogger.Info($"Добавлена директория: {directory.Name}");
                directoryFiles.ToList().ForEach(fInfo => TiLogger.Info($"\tДобавлен файл: {fInfo.Name}"));
            }

            // Для расчета прогресса
            _progress.AllFilesCount = _files.Count + _directories.Sum(tuple => tuple.Item2.Count);
        }

        AnsiConsole.Live(_table)
            .Start(Progress);

        AnsiConsole.Write(
            new Rule("Работа программы завершена! Нажмите любую кнопку, чтобы выйти...")
            {
                Style = new Style(Constants.AppColor)
            });
        AnsiConsole.WriteLine();
        Console.ReadKey();
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

        _progress.ProcessedFilesCount++;
        
        TiLogger.Info(string.Join(',', data));
        
        _table.AddRow(data);
        _table.Caption(
            new TableTitle(
                $"Выполнено: {_progress.ProcessedFilesCount} из {_progress.AllFilesCount} | {_progress.Percent}"));
        
        ctx.Refresh();
    }
}