using L0.API.Adapters.Realtime;
using L2.Application.Ports.Realtime;
using L3.Infrastructure.Options;
using StackExchange.Redis;

namespace L0.API.Extensions;

public static class RealtimeExtensions {
  public static IServiceCollection AddSignalRAdapters(this IServiceCollection services, IConfiguration config) {
    var redisSettings = config.GetSection(RedisSettings.SectionName).Get<RedisSettings>()!;

    services
      .AddSignalR()
      .AddStackExchangeRedis(options => {
        var signalRConfig = ConfigurationOptions.Parse(redisSettings.Configuration);
        signalRConfig.AbortOnConnectFail = false;
        signalRConfig.ClientName = redisSettings.InstanceRealtimeName;
        options.Configuration = signalRConfig;
      });

    services.AddSingleton<IAuctionNotifier, SignalRAuctionNotifier>();
    services.AddSingleton<IBidderNotifier, SignalRBidderNotifier>();
    services.AddSingleton<ISellerNotifier, SignalRSellerNotifier>();

    return services;
  }
}
