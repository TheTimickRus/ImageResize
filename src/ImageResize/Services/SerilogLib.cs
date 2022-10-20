// ReSharper disable TemplateIsNotCompileTimeConstantProblem
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using Serilog;
using Serilog.Core;

namespace ImageResize.Services;

/// <summary>
/// Обертка для логгера Serilog
/// </summary>
public static class SerilogLib
{
    public static bool IsLogging { get; set; }
    public static string FileName { get; set; } = Constants.Titles.LogFileName;
    
    private static bool _isConfiguration;
    private static Logger? _logger;

    public static void Info(string str)
    {
        if (!IsLogging)
            return;

        Configuration();
        _logger?.Information(str);
    }
    
    public static void Warn(string str)
    {
        if (!IsLogging)
            return;
        
        Configuration();
        _logger?.Warning(str);
    }
    
    public static void Error(string str)
    {
        if (!IsLogging)
            return;
        
        Configuration();
        _logger?.Error(str);
    }
    
    public static void Error(Exception ex)
    {
        if (!IsLogging)
            return;
        
        Configuration();
        _logger?.Error(ex, ex.Message);
    }
    
    public static void Fatal(Exception ex)
    {
        Configuration();
        _logger?.Fatal(ex, ex.Message);
    }
    
    private static void Configuration()
    {
        if (_isConfiguration)
            return;

        _logger = new LoggerConfiguration()
            .WriteTo.File(FileName)
            .CreateLogger();

        _isConfiguration = true;
    }
}