// ReSharper disable TemplateIsNotCompileTimeConstantProblem

using Serilog;
using Serilog.Core;
using Serilog.Formatting.Json;

namespace ImageResize.Services;

public static class TiLogger
{
    private static bool _isConfiguration;
    private static Logger? _logger;
    
    public static void Info(string str)
    {
        Configuration();
        _logger?.Information(str);
    }
    
    public static void Warn(string str)
    {
        Configuration();
        _logger?.Warning(str);
    }
    
    public static void Error(string str)
    {
        Configuration();
        _logger?.Error(str);
    }
    
    public static void Error(Exception ex)
    {
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
            .WriteTo.File(Constants.LogFileName)
            .CreateLogger();

        _isConfiguration = true;
    }
}