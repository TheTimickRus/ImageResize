using ByteSizeLib;

namespace ImageResize.Models;

public enum FileStatus
{
    Processed,
    Copy,
    Skip
}

public class FileModel
{
    public FileStatus Status { get; set; }
    public string? Name { get; set; }
    public string? OriginalResolution { get; set; }
    public string? Resolution { get; set; }
    public ByteSize OriginalSize { get; set; }
    public ByteSize Size { get; set; }
}