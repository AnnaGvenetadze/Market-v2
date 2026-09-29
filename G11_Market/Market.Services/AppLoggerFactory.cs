using Market.Extensions;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Market.Services;

public static class AppLoggerFactory
{
    public static AppLogger Create(IConfiguration configuration)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        return new AppLogger(logger);
    }
}