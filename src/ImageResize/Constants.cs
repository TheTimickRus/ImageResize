// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using Spectre.Console;

namespace ImageResize;

public static class Constants
{
    public static class Titles
    {
        /// <summary>
        /// Имя программы
        /// </summary>
        public const string AppName = "ImageResize";
        /// <summary>
        /// *Версия программы* (v.1.0)
        /// </summary>
        public const string Version = "v.2.0";
        /// <summary>
        /// *Версия программы с датой* (v.1.0 (02.09.2022))
        /// </summary>
        public const string VersionWithDate = $"{Version} (19.10.2022)";
        /// <summary>
        /// *Название программы* (*Версия* (*дата*)) by *Разработчик*
        /// </summary>
        public const string FullTitle = $"{AppName} ({VersionWithDate}) by Timick";
        /// <summary>
        /// *Название программы* by *Разработчик*
        /// </summary>
        public const string ShortTitle = $"{AppName} by Timick";
        /// <summary>
        /// *Название программы*
        /// </summary>
        public const string VeryShortTitle = $"{AppName}";
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
        public static readonly Color MainColor = Color.Plum4;
        /// <summary>
        /// Второй цвет
        /// </summary>
        public static readonly Color SecondColor = Color.SlateBlue1;
        /// <summary>
        /// Цвет успеха
        /// </summary>
        public static readonly Color SuccessColor = Color.Lime;
        /// <summary>
        /// Цвет ошибки
        /// </summary>
        public static readonly Color ErrorColor = Color.Red; 
    }
}