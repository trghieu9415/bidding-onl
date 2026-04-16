using System.Security.Claims;
using L0.API.Adapters.Security;
using L0.API.Response;
using L2.Application.Ports.Security;
using L3.Infrastructure.Options;
using L3.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace L0.API.Extensions;

public static class SecurityExtensions {
  public static IServiceCollection AddJwtAuthentication(
    this IServiceCollection services,
    IConfiguration config
  ) {
    services.AddScoped<ICurrentUser, CurrentUser>();

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options => {
        var jwtOptions = config.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
        options.TokenValidationParameters = JwtService.GetValidationParameters(jwtOptions!);
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
          var result = ApiResponse.Fail("Token không hợp lệ hoặc đã bị thu hồi do thay đổi mật khẩu.", 401);
          await context.Response.WriteAsJsonAsync(result.Value);
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
