// ReSharper disable RedundantNullableFlowAttribute
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ImageResize.ImageSharp;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ImageResize;

internal class MainCommand : Command<MainCommand.Settings>
{
    private readonly List<FileInfo> _files = new();
    private readonly List<(DirectoryInfo, List<FileInfo>)> _directories = new();
    
    private int _allFilesCount;
    private int _successFilesCount;

    private int _progressPrevValue;
    private readonly CancellationTokenSource _progressCancellationTokenSource = new();

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

    public override ValidationResult Validate([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        if (settings.Paths is null || settings.Paths.Length == 0)
        {
            return ValidationResult.Error("Не найдено файлов для обработки!");
        }
        
        return ValidationResult.Success();
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        /* Шапка */
        Console.Title = "ImageResize by TheTimickRus";

        AnsiConsole.Write(new FigletText("ImageResize").Color(Color.Yellow));
        var titleRule = new Rule("ImageResize (v.1.3 (21.05.2022)) by TheTimickRus")
        {
            Alignment = Justify.Right,
            Style = new Style(Color.Blue)
        };
        AnsiConsole.Write(titleRule);
        AnsiConsole.WriteLine();
        /* Шапка */
        
        /* Обработка */
        if (settings.Paths is null || settings.Paths.Length == 0)
        {
            throw new Exception("Не найдено файлов для обработки!");
        }

        var threadsCount = settings.ThreadsCount ?? Environment.ProcessorCount;
        var startTime = DateTime.Now;

        Task.Factory.StartNew(() =>
        {
            while (true)
            {
                if (_successFilesCount != _progressPrevValue)
                {
                    Console.Title = $"ImageResize by TheTimickRus | Выполнено: {(_successFilesCount * 100 / (double)_allFilesCount):F}% ({_successFilesCount} из {_allFilesCount})";
                    _progressPrevValue = _successFilesCount;
                }

                if (_progressCancellationTokenSource.IsCancellationRequested)
                {
                    Console.Title = "ImageResize by TheTimickRus";
                    break;
                }
                
                Thread.Sleep(2000);
            }
        }, _progressCancellationTokenSource.Token);

        AnsiConsole.MarkupLine(" [bold orange1]S:[/] Программа запущена!\n");
        AnsiConsole.MarkupLine(" [bold orange1]L:[/] Анализ данных... Пожалуйста, подождите...");
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

        if (_files.Count > 0)
        {
            AnsiConsole.MarkupLine(" [bold orange1]L:[/] Обработка файлов... Пожалуйста, подождите...");
            Parallel.ForEach(_files, new ParallelOptions { MaxDegreeOfParallelism = threadsCount }, fInfo =>
            {
                AnsiConsole.MarkupLine($" [bold orange1]L:[/] Чтение: {fInfo.Name}");
                if (ImageResizeLibs.ExecuteForFile(fInfo, settings.ResizePercentValue, settings.JpegQuality, out var newName))
                {
                    AnsiConsole.MarkupLine($" [bold orange1]L:[/] [green]Сохранено:[/] {newName}");
                    _successFilesCount++;
                }
                else
                {
                    AnsiConsole.MarkupLine($" [bold red]E:[/] [red]Ошибка при обработке файла:[/] {fInfo.Name}");
                }
            }); 
        }

        if (_directories.Count > 0)
        {
            AnsiConsole.MarkupLine(" [bold orange1]L:[/] Обработка директорий... Пожалуйста, подождите...");
            foreach (var (baseDirInfo, baseDirFiles) in _directories)
            {
                Parallel.ForEach(baseDirFiles, new ParallelOptions { MaxDegreeOfParallelism = threadsCount }, fInfo =>
                {
                    AnsiConsole.MarkupLine($" [bold orange1]L:[/] Чтение: {fInfo.Name}");
                    if (ImageResizeLibs.ExecuteForFolder(baseDirInfo.FullName, fInfo, settings.ResizePercentValue, settings.JpegQuality))
                    {
                        AnsiConsole.MarkupLine($" [bold orange1]L:[/] [green]Сохранено:[/] {fInfo.Name}");
                        _successFilesCount++;
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($" [bold red]E:[/] [red]Ошибка при обработке файла:[/] {fInfo.Name}");
                    }
                });
            }   
        }

        _progressCancellationTokenSource.Cancel();

        AnsiConsole.MarkupLine($"\n [bold orange1]F:[/] Обработка завершена! Прошло: [bold green3]{(DateTime.Now - startTime):g}[/]\n");
        /* Обработка */

        /* Верификация */
        AnsiConsole.MarkupLine(_successFilesCount == _allFilesCount
            ? $"\n\n [bold orange1]R:[/] [green]Обработка успешно завершена! Обработано файлов: {_successFilesCount} из {_allFilesCount}[/]\n"
            : $"\n\n [bold orange1]R:[/] [red]Возникли ошибки при обработке файлов! Обработано: {_successFilesCount} из {_allFilesCount}[/]\n");
        /* Верификация */

        /* Футер */
        AnsiConsole.MarkupLine("\t[blue]Нажмите любую кнопку, чтобы выйти...[/]");
        /* Футер */

        Console.ReadKey();
        return 0;
    }
}