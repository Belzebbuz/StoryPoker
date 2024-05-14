using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Threading.Channels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using StoryPoker.Client.Web.Api.Abstractions;
using StoryPoker.Client.Web.Api.Abstractions.Notifications;
using StoryPoker.Client.Web.Api.Configurations;
using StoryPoker.Client.Web.Api.Domain.Room.Features.Get;
using StoryPoker.Client.Web.Api.Extensions;
using StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver;
using StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver.Channels;
using StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver.Observers;
using StoryPoker.Client.Web.Api.Infrastructure.Hubs;
using StoryPoker.Client.Web.Api.Infrastructure.Notifications;
using StoryPoker.Server.Abstractions.Notifications;
using StoryPoker.Server.Abstractions.Room;
using StoryPoker.Server.Abstractions.Room.Models;

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddConfigurations(builder.Environment.EnvironmentName);
    builder.Host.UseSerilog((_, config) =>
    {
        config.WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .ReadFrom.Configuration(builder.Configuration);
    });
    builder.Host.AddOrleansClient(builder.Configuration);
    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, configure =>
        {
            configure.LoginPath = "/api/account/login";
        });
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(config =>
        {
            config.InvalidModelStateResponseFactory = context =>
            {
                return new BadRequestObjectResult(
                    string.Join(Environment.NewLine,
                        context.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
                );
            };
        });
    builder.Services.Configure<WebHookConfig>(builder.Configuration.GetSection(nameof(WebHookConfig)));
    builder.Services.AddCurrentUser();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
    });
    builder.Services.AddTransient<NotificationHub>();
    builder.Services.AddSignalR();
    builder.Services.AddTransient<INotificationService, NotificationService>();
    builder.Services.AddTransient<IRoomNotificationObserver, RoomNotificationObserver>();
    builder.Services.AddSingleton<IGrainSubscriptionBus, GrainsMessageChannel>();
    builder.Services.AddSingleton<IConnectionStorage, ConnectionStorage>();
    builder.Services.AddHostedService<GrainObserverService>();
    var app = builder.Build();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCurrentUser();
    app.UseStaticFiles();
    app.MapFallbackToFile("index.html");
    app.MapHub<NotificationHub>("/api/notifications");
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}
