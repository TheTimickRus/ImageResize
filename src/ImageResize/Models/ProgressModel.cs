namespace ImageResize.Models;

public class ProgressModel
{
    public int AllFilesCount { get; set; }
    public int ProcessedFilesCount { get; set; }

    public string Percent => $"{(ProcessedFilesCount * 100 / (double)AllFilesCount):F} %";
}