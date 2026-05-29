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
}