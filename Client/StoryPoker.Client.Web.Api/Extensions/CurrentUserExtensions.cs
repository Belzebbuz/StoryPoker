using StoryPoker.Client.Web.Api.Middlewares.CurrentUser;

namespace StoryPoker.Client.Web.Api.Extensions;

public static class CurrentUserExtensions
{
    public static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app) =>
        app.UseMiddleware<CurrentUserMiddleware>();
    public static IServiceCollection AddCurrentUser(this IServiceCollection services) =>
        services
            .AddScoped<CurrentUserMiddleware>()
            .AddScoped<ICurrentUser, CurrentUser>()
            .AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());
}
