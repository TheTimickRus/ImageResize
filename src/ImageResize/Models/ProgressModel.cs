using ByteSizeLib;

namespace ImageResize.Models;

public class ProgressModel
{
    /// <summary>
    /// Общее кол-во файлов
    /// </summary>
    public int AllFilesCount { get; set; }
    /// <summary>
    /// Обработано файлов
    /// </summary>
    public int ProcessedFilesCount { get; set; }
    /// <summary>
    /// Общий размер
    /// </summary>
    public ByteSize AllFilesSize { get; set; }
    /// <summary>
    /// Размер обработанных файлов
    /// </summary>
    public ByteSize ProcessedFilesSize { get; set; }

    public string Percent => $"{(ProcessedFilesCount * 100 / (double)AllFilesCount):F} %";
}