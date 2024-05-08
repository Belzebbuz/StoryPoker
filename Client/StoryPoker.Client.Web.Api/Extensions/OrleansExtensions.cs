using Orleans.Configuration;
using StoryPoker.Client.Web.Api.Configurations;
using Throw;

namespace StoryPoker.Client.Web.Api.Extensions;

public static class OrleansExtensions
{
    public static IHostBuilder AddOrleansClient(this IHostBuilder builder, IConfiguration config)
    {
        builder.UseOrleansClient(client =>
        {
            var orleansSettings = config.GetSection(nameof(ClusterConfig)).Get<ClusterConfig>();
            orleansSettings.ThrowIfNull("Отсутствуют настройки кластера");
            client.UseZooKeeperClustering(options =>
                {
                    options.ConnectionString = orleansSettings.ConnectionString;
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = orleansSettings.ClusterId;
                    options.ServiceId = orleansSettings.ServiceId;
                });
        });
        return builder;
    }
}
