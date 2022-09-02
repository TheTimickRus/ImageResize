using System.Diagnostics.CodeAnalysis;
using Spectre.Console;

namespace ImageResize;

public static class Constants
{
    public static class Titles
    {
        /// <summary>
        /// *Название программы* (*Версия* (*дата*)) by *Разработчик*
        /// </summary>
        public const string FullTitle = "ImageResize (v.1.6.0 (02.09.2022)) by TheTimickRus";
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

    [SuppressMessage("Usage", "CA2211:Поля, не являющиеся константами, не должны быть видимыми")]
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