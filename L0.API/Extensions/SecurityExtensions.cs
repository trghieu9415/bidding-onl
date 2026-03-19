using System.Security.Claims;
using L0.API.Adapters.Security;
using L2.Application.Ports.Security;
using L3.Infrastructure.Configs.Options;
using L3.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace L0.API.Extensions;

public static class SecurityExtensions {
  public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
    IConfiguration configuration) {
    services.AddScoped<ICurrentUser, CurrentUser>();

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options => {
        var jwtOptions = services.BuildServiceProvider().GetRequiredService<JwtOptions>();
        options.TokenValidationParameters = JwtService.GetValidationParameters(jwtOptions);
        options.Events = GetEvents();
      });

    return services;
  }

  private static JwtBearerEvents GetEvents() {
    return new JwtBearerEvents {
      OnTokenValidated = async context => {
        var auth = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
        var claims = context.Principal;

        var userId = claims?.FindFirstValue(ClaimTypes.NameIdentifier);
        var tokenStamp = claims?.FindFirstValue("security_stamp");

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tokenStamp) ||
            !Guid.TryParse(userId, out var userGuid) ||
            !await auth.ValidateSecurityStampAsync(userGuid, tokenStamp, context.HttpContext.RequestAborted)) {
          context.Fail("Unauthorized");
        }
      },
      OnMessageReceived = context => {
        var accessToken = context.Request.Query["access_token"];
        if (
          !string.IsNullOrEmpty(accessToken) &&
          context.HttpContext.Request.Path.StartsWithSegments("/hubs")
        ) {
          context.Token = accessToken;
        }

        return Task.CompletedTask;
      }
    };
  }
}
