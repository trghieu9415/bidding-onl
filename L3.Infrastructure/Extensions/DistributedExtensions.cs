using L2.Application.Ports.Cache;
using L2.Application.Ports.Concurrency;
using L3.Infrastructure.Adapters.Cache;
using L3.Infrastructure.Adapters.Concurrency;
using L3.Infrastructure.Options;
using L3.Infrastructure.Services;
using L3.Infrastructure.Services.Abstractions;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace L3.Infrastructure.Extensions;

public static class DistributedExtensions {
  public static IServiceCollection AddDistributedInfrastructure(
    this IServiceCollection services,
    IConfiguration config
  ) {
    var redisSettings = config.GetSection(RedisSettings.SectionName).Get<RedisSettings>()!;
    var baseConfig = ConfigurationOptions.Parse(redisSettings.Configuration);
    baseConfig.AbortOnConnectFail = false;

    var lazyCacheConnection = CreateLazyConnection(redisSettings.InstanceName);
    var lazyLockConnection = CreateLazyConnection(redisSettings.InstanceLockName);

    services.AddStackExchangeRedisCache(cacheOptions => {
      cacheOptions.InstanceName = redisSettings.InstanceName;
      cacheOptions.ConnectionMultiplexerFactory = () => Task.FromResult(lazyCacheConnection.Value);
    });

    services.AddSingleton<IDistributedLockProvider>(_ =>
      new RedisDistributedSynchronizationProvider(lazyLockConnection.Value.GetDatabase())
    );

    services.AddSingleton<ICacheService, RedisCacheService>();
    services.AddSingleton<IDistributedLockService, RedisLockService>();
    services.AddScoped<IBusinessCache, BusinessCache>();

    return services;

    Lazy<IConnectionMultiplexer> CreateLazyConnection(string clientName) {
      return new Lazy<IConnectionMultiplexer>(() => {
        var clonedConfig = baseConfig.Clone();
        clonedConfig.ClientName = clientName;
        return ConnectionMultiplexer.Connect(clonedConfig);
      });
    }
  }
}
