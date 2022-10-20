// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace ImageResize.Models;

public class ResolutionModel
{
    /// <summary>
    /// Ширина изображения
    /// </summary>
    public int Width { get; set; }
    
    /// <summary>
    /// Высота изображения
    /// </summary>
    public int Height { get; set; }
    
    /// <summary>
    /// Конструктор с параметрами
    /// </summary>
    /// <param name="width">Ширина изображения</param>
    /// <param name="height">Высота изображения</param>
    public ResolutionModel(int width, int height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Вывод параметров изображения на кран
    /// </summary>
    /// <returns>{Width}x{Height}</returns>
    public override string ToString()
    {
        return $"{Width}x{Height}";
    }
}