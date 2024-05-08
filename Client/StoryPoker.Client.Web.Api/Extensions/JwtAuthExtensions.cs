using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Throw;

namespace StoryPoker.Client.Web.Api.Extensions;

public static class JwtAuthExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthorization();
        var secureKey = config["JwtKey"].ThrowIfNull();
        var key = Encoding.ASCII.GetBytes(secureKey);
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(setup =>
            {
                setup.RequireHttpsMetadata = false;
                setup.SaveToken = true;
                setup.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.Zero
                };
                setup.Events = new JwtBearerEvents()
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        if (!context.Response.HasStarted)
                        {
                            throw new UnauthorizedAccessException("Authentication Failed.");
                        }

                        return Task.CompletedTask;
                    },
                    OnForbidden = _ => throw new UnauthorizedAccessException("You are not authorized to access this resource."),
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        if (!string.IsNullOrEmpty(accessToken) &&
                            context.HttpContext.Request.Path.StartsWithSegments("/api/notifications"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        return services;
    }
}
