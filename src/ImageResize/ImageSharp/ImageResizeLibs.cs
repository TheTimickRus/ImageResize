using ImageResize.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace ImageResize.ImageSharp;

public static class ImageResizeLibs
{
    public static bool ExecuteForFile(FileInfo file, int resizePercentValue, int jpegQuality, out string newName)
    {
        try
        {
            var imgOutDir = file.DirectoryName;
            var imgOutName = Path.GetFileNameWithoutExtension(file.Name);
            var imgOutExt = Path.GetExtension(file.Name).ToLower();
            var imgOutputFullName = Path.Combine(imgOutDir ?? "", $"{imgOutName}_Conv{imgOutExt}");
            
            using var img = Resize(file.FullName, resizePercentValue);

            newName = Path.GetFileName(imgOutputFullName);
            
            switch (imgOutExt)
            {
                case ".jpg":
                    img.SaveAsJpeg(imgOutputFullName, new JpegEncoder { Quality = jpegQuality });
                    break; 
                case ".png":
                    img.SaveAsPng(imgOutputFullName);
                    break;
                default:
                    return false;
            }
            
            return true;
        }
        catch
        {
            newName = "";
            return false;
        }
    }

    public static FileStatus ExecuteForFolder(string basePath, FileInfo file, int resizePercentValue, int jpegQuality)
    {
        var imgOutDir = file.DirectoryName?.Replace(basePath, $"{basePath}_Conv") ?? "";
        var imgOutName = Path.GetFileNameWithoutExtension(file.Name);
        var imgOutExt = Path.GetExtension(file.Name).ToLower();
        var imgOutputFullName = Path.Combine(imgOutDir, $"{imgOutName}{imgOutExt}");
        
        if (Directory.Exists(imgOutDir) is false)
            Directory.CreateDirectory(imgOutDir);
        
        try
        {
            using var img = Resize(file.FullName, resizePercentValue);
            
            switch (imgOutExt)
            {
                case ".jpg":
                    img.SaveAsJpeg(imgOutputFullName, new JpegEncoder { Quality = jpegQuality });
                    break; 
                case ".png":
                    img.SaveAsPng(imgOutputFullName);
                    break;
                default:
                    return FileStatus.Skip;
            }
            
            return FileStatus.Processed;
        }
        catch
        {
            file.CopyTo(imgOutputFullName);
            return FileStatus.Copy;
        }
    }
    
    /// <summary>
    /// Уменьшение разрешения на Х процентов
    /// </summary>
    /// <param name="file">Путь до изображения</param>
    /// <param name="resizePercentValue">Процент (X), на который нужно уменьшить разрешение изображения</param>
    /// <returns>Изображение</returns>
    private static Image? Resize(string file, int resizePercentValue)
    {
        try
        {
            var img = Image.Load(file);
            
            var percent = resizePercentValue / 100.0;
            var newWidth = (int)(img.Width * percent);
            var newHeight = (int)(img.Height * percent);
            
            img.Mutate(x => x.Resize(newWidth, newHeight));

            return img;
        }
        catch
        {
            return null;
        } 
    }
}