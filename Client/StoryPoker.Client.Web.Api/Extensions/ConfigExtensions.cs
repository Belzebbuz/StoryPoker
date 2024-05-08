using System.Globalization;
using Serilog;

namespace StoryPoker.Client.Web.Api.Extensions;

public static class ConfigExtensions
{
    internal static IHostBuilder AddConfigurations(this IHostBuilder host)
    {
        host.ConfigureAppConfiguration((context, config) =>
        {
            const string configurationsDirectory = "ConfigurationFiles";
            var env = context.HostingEnvironment;
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/serilog.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/serilog.{env.EnvironmentName}.json", optional: true,
                    reloadOnChange: true);
        });

        return host;
    }
}
public static class StaticLogger
{
    public static void EnsureInitialized()
    {
        if (Log.Logger is not Serilog.Core.Logger)
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
                .CreateLogger();
    }
}
