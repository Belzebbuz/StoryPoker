using System.Text.Json;
using Orleans.Configuration;
using OrleansDashboard;
using StackExchange.Redis;
using StoryPoker.Server.Grains.Constants;
using StoryPoker.Server.Silo.Configurations;
using Throw;

namespace StoryPoker.Server.Silo.Extensions;

internal static class OrleansExtensions
{
    internal static IHostBuilder AddOrleans(this IHostBuilder builder)
    {
        builder.UseOrleans((hostBuilder, silo) =>
        {
            var siloSettings = hostBuilder.Configuration.GetSection(nameof(SiloConfig)).Get<SiloConfig>();
            Console.WriteLine(JsonSerializer.Serialize(siloSettings));
            siloSettings.ThrowIfNull("Не установлены настройки Silo");
            var dashboardOptions = hostBuilder.Configuration.GetSection(nameof(DashboardOptions)).Get<DashboardOptions>();
            dashboardOptions.ThrowIfNull("Не установлены настройки DashboardOptions");
            silo
                .UseRedisClustering(options => options.ConfigurationOptions = new()
                {
                    EndPoints = new EndPointCollection()
					{
						new(siloSettings.ClusterConfig.ConnectionString)
					}
                })
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = siloSettings.ClusterConfig.ClusterId;
                options.ServiceId = siloSettings.ClusterConfig.ServiceId;
            })
            .AddRedisGrainStorage(StorageConstants.PersistenceStorage, options =>
            {
                options.ConfigurationOptions = new()
                {
                  EndPoints  = new()
                  {
                      new(siloSettings.RedisPersistenceConfig.ConnectionString)
                  }
                };
            })
            .ConfigureLogging(logging => logging.AddConsole())
            .UseDashboard(options =>
            {
                options.Username = dashboardOptions.Username;
                options.Password = dashboardOptions.Password;
                options.Host = dashboardOptions.Host;
                options.Port = dashboardOptions.Port;
                options.HostSelf = dashboardOptions.HostSelf;
                options.CounterUpdateIntervalMs = dashboardOptions.CounterUpdateIntervalMs;
            });
        });
        return builder;
    }
}
