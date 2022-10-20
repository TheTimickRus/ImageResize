namespace ImageResize.Models;

public enum FileStatus
{
    /// <summary>
    /// Файл обработан
    /// </summary>
    Processed,
    /// <summary>
    /// Файл скопирован
    /// </summary>
    Copy,
    /// <summary>
    /// Файл пропущен
    /// </summary>
    Skip,
    /// <summary>
    /// Файл пропущен по Threshold
    /// </summary>
    Threshold
}