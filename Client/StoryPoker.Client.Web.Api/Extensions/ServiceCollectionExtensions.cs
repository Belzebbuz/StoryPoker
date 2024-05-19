using Microsoft.AspNetCore.Authentication.Cookies;
using StoryPoker.Client.Web.Api.Abstractions;
using StoryPoker.Client.Web.Api.Abstractions.Notifications;
using StoryPoker.Client.Web.Api.Abstractions.Observers;
using StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver;
using StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver.Channels;
using StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver.Observers;
using StoryPoker.Client.Web.Api.Infrastructure.Hubs;
using StoryPoker.Client.Web.Api.Infrastructure.Notifications;
using StoryPoker.Server.Abstractions.Notifications;

namespace StoryPoker.Client.Web.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrainObserving(this IServiceCollection services)
    {
        services.AddScoped<IRoomNotificationObserver, RoomNotificationObserver>();
        services.AddScoped<IRoomNotificationGrainSubscriber, RoomNotificationObserver>();

        services.AddSingleton<IGrainSubscriptionBus, GrainsSubscriptionMessageChannel>();
        services.AddHostedService<GrainObserverService>();
        return services;
    }

    public static IServiceCollection AddClientNotifications(this IServiceCollection services)
    {
        services.AddTransient<NotificationHub>();
        services.AddSignalR();
        services.AddTransient<INotificationService, NotificationService>();
        services.AddSingleton<IConnectionStorage, ConnectionStorage>();
        return services;
    }

    public static IServiceCollection AddDefaultCorsPolicy(this IServiceCollection services, string name)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name, config => config
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });
        return services;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, configure =>
            {
                configure.LoginPath = "/api/account/login";
            });
        return services;
    }
}
