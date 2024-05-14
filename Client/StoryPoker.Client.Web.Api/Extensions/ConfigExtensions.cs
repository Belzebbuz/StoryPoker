using System.Globalization;
using Serilog;

namespace StoryPoker.Client.Web.Api.Extensions;

public static class ConfigExtensions
{
    internal static void AddConfigurations(this ConfigurationManager configuration, string env)
    {
        const string configurationsDirectory = "ConfigurationFiles";
        configuration.AddJsonFile($"{configurationsDirectory}/serilog.json", optional: false, reloadOnChange: true);
        configuration.AddJsonFile($"{configurationsDirectory}/serilog.{env}.json", optional: true,
            reloadOnChange: true);
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
