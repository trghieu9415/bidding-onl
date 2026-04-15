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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace L3.Infrastructure.Extensions;

public static class DistributedExtensions {
  public static IServiceCollection
    AddDistributedInfrastructure(this IServiceCollection services) {
    // Redis Cache
    services.AddOptions<RedisCacheOptions>()
      .Configure<IOptions<RedisOptions>>((cacheOptions, redisOptionsRef) => {
        var redisOptions = redisOptionsRef.Value;
        cacheOptions.Configuration = redisOptions.Configuration;
        cacheOptions.InstanceName = redisOptions.InstanceName;
      });
    services.AddStackExchangeRedisCache(_ => {});
    services.AddScoped<ICacheService, RedisCacheService>();
    services.AddScoped<IBusinessCache, BusinessCache>();

    // Redis Connection (Multiplexer for Locking)
    services.AddSingleton<IConnectionMultiplexer>(serviceProvider => {
      var redisOptions = serviceProvider.GetRequiredService<IOptions<RedisOptions>>().Value;
      return ConnectionMultiplexer.Connect(redisOptions.Configuration);
    });

    // Distributed Lock (Medallion)
    services.AddSingleton<IDistributedLockProvider>(serviceProvider => {
      var connection = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
      return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
    });
    services.AddScoped<IDistributedLockService, RedisLockService>();

    return services;
  }
}
