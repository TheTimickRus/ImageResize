using ByteSizeLib;

namespace ImageResize.Models;

public class FileModel
{
    public FileStatus Status { get; init; }
    public string? Name { get; init; }
    public ResolutionModel? OriginalResolution { get; init; }
    public ResolutionModel? Resolution { get; init; }
    public ByteSize OriginalSize { get; init; }
    public ByteSize Size { get; init; }

    public FileModel()
    { }

    public FileModel(FileStatus status, string? name, ByteSize originalSize)
    {
        Status = status;
        Name = name;
        OriginalSize = originalSize;
        Size = originalSize;
    }
}