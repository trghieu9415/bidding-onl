using L0.API.Adapters.Realtime;
using L2.Application.Ports.Realtime;
using L3.Infrastructure.Options;
using Microsoft.AspNetCore.SignalR.StackExchangeRedis;
using StackExchange.Redis;

namespace L0.API.Extensions;

public static class RealtimeExtensions {
  public static IServiceCollection AddSignalRAdapters(
    this IServiceCollection services,
    IConfiguration config
  ) {
    var redisSettings = config.GetSection(RedisSettings.SectionName).Get<RedisSettings>()!;

    // NOTE: ========== [REDIS_BACKPLANE] ==========
    services
      .AddSignalR()
      .AddStackExchangeRedis();

    services
      .AddOptions<RedisOptions>()
      .Configure<IServiceProvider>((opt, sp) => {
        opt.Configuration.ChannelPrefix = RedisChannel.Literal(redisSettings.Keys.Backplane);
        opt.ConnectionFactory = _ =>
          Task.FromResult(
            sp.GetRequiredKeyedService<IConnectionMultiplexer>(RedisSettings.MutexKeys.Backplane)
          );
      });


    services.AddSingleton<IAuctionNotifier, SignalRAuctionNotifier>();
    services.AddSingleton<IBidderNotifier, SignalRBidderNotifier>();
    services.AddSingleton<ISellerNotifier, SignalRSellerNotifier>();

    return services;
  }
}
