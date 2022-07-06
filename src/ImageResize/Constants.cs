using Spectre.Console;

namespace ImageResize;

public static class Constants
{
    public static class Titles
    {
        /// <summary>
        /// *Название программы* (*Версия* (*дата*)) by *Разработчик*
        /// </summary>
        public const string FullTitle = "ImageResize (v.1.4 (05.07.2022)) by TheTimickRus";
        /// <summary>
        /// *Название программы* by *Разработчик*
        /// </summary>
        public const string ShortTitle = "ImageResize by TheTimickRus";
        /// <summary>
        /// *Название программы*
        /// </summary>
        public const string VeryShortTitle = "ImageResize";
        /// <summary>
        /// Имя лог-файла
        /// </summary>
        public const string LogFileName = $"{VeryShortTitle}.log";
    }

    public static class Colors
    {
        /// <summary>
        /// Основной цвет
        /// </summary>
        public static Color MainColor = Color.SteelBlue;
        /// <summary>
        /// Цвет успеха
        /// </summary>
        public static Color SuccessColor = Color.SeaGreen1;
        /// <summary>
        /// Цвет ошибки
        /// </summary>
        public static Color ErrorColor = Color.Red; 
    }
}