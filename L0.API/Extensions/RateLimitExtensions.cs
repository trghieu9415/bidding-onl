using System.Security.Claims;
using L3.Infrastructure.Options;
using RedisRateLimiting;
using StackExchange.Redis;

namespace L0.API.Extensions;

public static class RateLimitExtensions {
  public static IServiceCollection AddAppRateLimiter(
    this IServiceCollection services,
    IConfiguration configuration
  ) {
    var rateLimitConfig = configuration.GetSection(RateLimitSettings.SectionName).Get<RateLimitSettings>();

    if (rateLimitConfig is not { IsEnabled: true }) {
      return services;
    }

    services.AddRateLimiter(options => {
      options.RejectionStatusCode = 429;

      foreach (var (policyName, config) in rateLimitConfig.Policies) {
        options.AddPolicy(policyName, context => {
          var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
          var clientIp = context.Connection.RemoteIpAddress?.ToString();

          var identifier = userId ?? clientIp ?? "anonymous_user";
          var partitionKey = $"ratelimit:{policyName}:{identifier}";

          // NOTE: ========== [REDIS_RATE_LIMIT] ==========
          var redisConnection = context.RequestServices
            .GetRequiredKeyedService<IConnectionMultiplexer>(RedisSettings.MutexKeys.Critical);

          return RedisRateLimitPartition.GetTokenBucketRateLimiter(partitionKey, _ =>
            new RedisTokenBucketRateLimiterOptions {
              ConnectionMultiplexerFactory = () => redisConnection,
              TokenLimit = config.TokenLimit,
              TokensPerPeriod = config.TokensPerPeriod,
              ReplenishmentPeriod = TimeSpan.FromSeconds(config.ReplenishmentPeriodInSeconds)
            });
        });
      }
    });

    return services;
  }
}
