using System.Text;
using L0.API.Adapters.Security;
using L2.Application.Ports.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace L0.API.Extensions;

public static class IdentityExtension {
  public static IServiceCollection AddApiSecurity(this IServiceCollection services, IConfiguration config) {
    services.AddScoped<ICurrentUser, CurrentUser>();
    services.AddScoped<IJwtService, JwtService>();

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = config["Jwt:Issuer"],
          ValidAudience = config["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Secret"]!))
        };

        options.Events = new JwtBearerEvents {
          OnMessageReceived = context => {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs")) {
              context.Token = accessToken;
            }

            return Task.CompletedTask;
          }
        };
      });
    return services;
  }
}
