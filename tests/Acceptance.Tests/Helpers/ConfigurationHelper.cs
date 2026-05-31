using Microsoft.Extensions.Configuration;

namespace Acceptance.Tests.Helpers;

public static class ConfigurationHelper
{
    private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
        .Build();

    public static string GetBaseUrl()
    {
        return Configuration["ApiSettings:BaseUrl"]
            ?? throw new InvalidOperationException("ApiSettings:BaseUrl is not configured.");
    }

    public static int GetTimeoutSeconds()
    {
        var timeoutValue = Configuration["ApiSettings:TimeoutSeconds"];

        if (string.IsNullOrWhiteSpace(timeoutValue))
        {
            return 30;
        }

        if (!int.TryParse(timeoutValue, out var timeoutSeconds))
        {
            throw new InvalidOperationException("ApiSettings:TimeoutSeconds must be a valid number.");
        }

        return timeoutSeconds;
    }

    public static bool IsApiLoggingEnabled()
    {
        var environmentValue = Environment.GetEnvironmentVariable("API_LOGGING");

        if (bool.TryParse(environmentValue, out var environmentLoggingEnabled))
        {
            return environmentLoggingEnabled;
        }

        var configValue = Configuration["ApiSettings:EnableApiLogging"];

        if (bool.TryParse(configValue, out var configLoggingEnabled))
        {
            return configLoggingEnabled;
        }

        return false;
    }
}