using NLog;

namespace ImageResize.Services;

public static class TiLogger
{
    private static bool _isConfiguration = false;
    private static Logger? _logger;
    
    public static void Info(string str)
    {
        Configuration();
        _logger?.Info(str);
    }
    
    public static void Warn(string str)
    {
        Configuration();
        _logger?.Warn(str);
    }
    
    public static void Error(string str)
    {
        Configuration();
        _logger?.Error(str);
    }
    
    public static void Error(Exception ex)
    {
        Configuration();
        _logger?.Error(ex);
    }
    
    public static void Fatal(Exception ex)
    {
        Configuration();
        _logger?.Fatal(ex);
    }
    
    private static void Configuration()
    {
        if (!_isConfiguration)
            return;
        
        var config = new NLog.Config.LoggingConfiguration();
        
        var logfile = new NLog.Targets.FileTarget("logfile")
        {
            Name = Constants.AppFullTitle,
            FileName = $"Log ({DateTime.Now.ToString("s").Replace(":", "-")}).txt",
        };
        
        config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
        
        LogManager.Configuration = config;

        _logger = LogManager.GetCurrentClassLogger();
        _isConfiguration = true;
    }
}