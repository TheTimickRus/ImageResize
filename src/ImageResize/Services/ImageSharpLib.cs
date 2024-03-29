﻿using ByteSizeLib;
using ImageResize.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace ImageResize.Services;

public static class ImageSharpLib
{
    /// <summary>
    /// Обработка для файлов
    /// </summary>
    /// <param name="file"></param>
    /// <param name="resizePercent"></param>
    /// <param name="jpegQuality"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static FileModel ExecuteForFile(FileInfo file, int resizePercent, int jpegQuality, int threshold)
    {
        try
        {
            var imgOutDir = file.DirectoryName;
            var imgOutName = Path.GetFileNameWithoutExtension(file.Name);
            var imgOutExt = Path.GetExtension(file.Name).ToLower();
            var imgOutputFullName = Path.Combine(imgOutDir ?? "", $"{imgOutName}_Conv{imgOutExt}");
            
            if (file.Length < threshold * 1024)
            {
                return new FileModel(FileStatus.Threshold, file.Name, ByteSize.FromBytes(file.Length));
            }
            
            if (imgOutExt is not (".jpg" or ".jpeg" or ".png"))
            {
                return new FileModel(FileStatus.Skip, file.Name, ByteSize.FromBytes(file.Length));
            }
            
            using var img = Resize(file.FullName, resizePercent, out var origResolution, out var newResolution);
            
            switch (imgOutExt)
            {
                case ".jpg":
                case ".jpeg":
                    img.SaveAsJpeg(imgOutputFullName, new JpegEncoder { Quality = jpegQuality });
                    break; 
                case ".png":
                    img.SaveAsPng(imgOutputFullName);
                    break;
                default:
                    throw new Exception();
            }
            
            return new FileModel
            {
                Status = FileStatus.Processed,
                Name  = imgOutputFullName,
                OriginalResolution = origResolution,
                Resolution = newResolution,
                OriginalSize = ByteSize.FromBytes(file.Length),
                Size = ByteSize.FromBytes(new FileInfo(imgOutputFullName).Length)
            };
        }
        catch
        {
            return new FileModel(FileStatus.Skip, file.Name, ByteSize.FromBytes(file.Length));
        }
    }

    /// <summary>
    /// Обработка для файлов из директории
    /// </summary>
    /// <param name="basePath"></param>
    /// <param name="file"></param>
    /// <param name="resizePercent"></param>
    /// <param name="jpegQuality"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static FileModel ExecuteForFolder(
        string basePath, 
        FileInfo file, 
        int resizePercent, 
        int jpegQuality, 
        int threshold)
    {
        var imgOutDir = file.DirectoryName?.Replace(basePath, $"{basePath}_Conv") ?? "";
        var imgOutName = Path.GetFileNameWithoutExtension(file.Name);
        var imgOutExt = Path.GetExtension(file.Name).ToLower();
        var imgOutputFullName = Path.Combine(imgOutDir, $"{imgOutName}{imgOutExt}");

        if (!Directory.Exists(imgOutDir))
            Directory.CreateDirectory(imgOutDir);

        if (file.Length < threshold * 1024)
        {
            file.CopyTo(imgOutputFullName, true);
            return new FileModel(FileStatus.Threshold, file.Name, ByteSize.FromBytes(file.Length));
        }

        if (imgOutExt is not (".jpg" or ".jpeg" or ".png"))
        {
            file.CopyTo(imgOutputFullName, true);
            return new FileModel(FileStatus.Copy, file.Name, ByteSize.FromBytes(file.Length));
        }

        try
        {
            using var img = Resize(file.FullName, resizePercent, out var origResolution, out var newResolution);
            
            switch (imgOutExt)
            {
                case ".jpg":
                    img.SaveAsJpeg(imgOutputFullName, new JpegEncoder { Quality = jpegQuality });
                    break; 
                case ".png":
                    img.SaveAsPng(imgOutputFullName);
                    break;
                default:
                    throw new Exception();
            }

            return new FileModel
            {
                Status = FileStatus.Processed,
                Name  = imgOutputFullName,
                OriginalResolution = origResolution,
                Resolution = newResolution,
                OriginalSize = ByteSize.FromBytes(file.Length),
                Size = ByteSize.FromBytes(new FileInfo(imgOutputFullName).Length)
            };
        }
        catch
        {
            file.CopyTo(imgOutputFullName, true);
            return new FileModel(FileStatus.Copy, file.Name, ByteSize.FromBytes(file.Length));
        }
    }

    /// <summary>
    /// Уменьшение разрешения на Х процентов
    /// </summary>
    /// <param name="file">Путь до изображения</param>
    /// <param name="resizePercentValue">Процент (X), на который нужно уменьшить разрешение изображения</param>
    /// <param name="origResolution">Оригинальное разрешение</param>
    /// <param name="newResolution">Новое разрешение</param>
    /// <returns>Изображение</returns>
    private static Image? Resize(
        string file, 
        int resizePercentValue, 
        out ResolutionModel? origResolution, 
        out ResolutionModel? newResolution)
    {
        try
        {
            var img = Image.Load(file);
            
            var percent = resizePercentValue / 100.0;
            var newWidth = (int)(img.Width * percent);
            var newHeight = (int)(img.Height * percent);
            
            origResolution = new ResolutionModel(img.Width, img.Height);
            newResolution = new ResolutionModel(newWidth, newHeight);
            
            img.Mutate(x => x.Resize(newWidth, newHeight));
            
            return img;
        }
        catch
        {
            origResolution = null;
            newResolution = null;
            return null;
        } 
    }
}