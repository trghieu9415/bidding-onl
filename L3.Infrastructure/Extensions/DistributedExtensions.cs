using L2.Application.Ports.Cache;
using L2.Application.Ports.Concurrency;
using L3.Infrastructure.Adapters.Cache;
using L3.Infrastructure.Adapters.Concurrency;
using L3.Infrastructure.Options;
using L3.Infrastructure.Services;
using L3.Infrastructure.Services.Abstractions;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace L3.Infrastructure.Extensions;

public static class DistributedExtensions {
  public static IServiceCollection AddDistributedInfrastructure(
    this IServiceCollection services,
    IConfiguration config
  ) {
    var settings = config.GetSection(RedisSettings.SectionName).Get<RedisSettings>()!;

    services
      .AddConnectionMultiplexer(RedisSettings.MutexKeys.Cache, settings.CacheConnection)
      .AddConnectionMultiplexer(RedisSettings.MutexKeys.Critical, settings.CriticalConnection)
      .AddConnectionMultiplexer(RedisSettings.MutexKeys.Backplane, settings.BackplaneConnection);

    // NOTE: ========== [REDIS_CACHE] ==========
    services.AddOptions<RedisCacheOptions>()
      .Configure<IServiceProvider>((opt, sp) => {
        opt.InstanceName = settings.Keys.Cache;
        opt.ConnectionMultiplexerFactory = () => {
          var mux = sp.GetRequiredKeyedService<IConnectionMultiplexer>(RedisSettings.MutexKeys.Cache);
          return Task.FromResult(mux);
        };
      });
    services.AddStackExchangeRedisCache(_ => {});
    services.AddSingleton<ICacheService, RedisCacheService>();
    services.AddScoped<IBusinessCache, BusinessCache>();

    // NOTE: ========== [REDIS_LOCK] ==========
    services.AddSingleton<IDistributedLockProvider>(sp => {
      var mux = sp.GetRequiredKeyedService<IConnectionMultiplexer>(RedisSettings.MutexKeys.Critical);
      return new RedisDistributedSynchronizationProvider(mux.GetDatabase());
    });
    services.AddSingleton<IDistributedLockService, RedisLockService>();

    // NOTE: ========== [REDIS_SECURITY] ==========
    services.AddSingleton<ISecurityService>(sp => {
      var mux = sp.GetRequiredKeyedService<IConnectionMultiplexer>(RedisSettings.MutexKeys.Critical);
      return new SecurityService(mux);
    });

    return services;
  }

  private static IServiceCollection AddConnectionMultiplexer(
    this IServiceCollection services,
    string mutexKey,
    string connection
  ) {
    var opt = ConfigurationOptions.Parse(connection);
    opt.AbortOnConnectFail = false;
    opt.ClientName = $"Bidding:{mutexKey}";
    var mux = ConnectionMultiplexer.Connect(opt);
    services.AddKeyedSingleton<IConnectionMultiplexer>(mutexKey, (_, _) => mux);
    return services;
  }
}
