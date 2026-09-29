using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Market.Extensions;

public sealed class AppLogger : ILogger
{
    private readonly Serilog.ILogger _logger;

    public AppLogger(Serilog.ILogger logger)
    {
        _logger = logger;
    }

    public Serilog.ILogger InnerLogger => _logger;
    public void Write(LogEvent logEvent) => _logger.Write(logEvent);
}