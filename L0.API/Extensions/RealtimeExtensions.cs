using L0.API.Adapters.Realtime;
using L2.Application.Ports.Realtime;
using L3.Infrastructure.Configs.Options;
using StackExchange.Redis;

namespace L0.API.Extensions;

public static class RealtimeExtensions {
  public static IServiceCollection AddSignalRAdapters(this IServiceCollection services, IConfiguration config) {
    var redisOptions = config.GetSection(RedisOptions.SectionName).Get<RedisOptions>();

    services.AddSignalR()
      .AddStackExchangeRedis(redisOptions!.Configuration, options => {
        options.Configuration.ChannelPrefix = RedisChannel.Literal("Realtime_");
      });
    services.AddTransient<IBiddingNotifier, BiddingNotifier>();
    services.AddTransient<IUserNotifier, UserNotifier>();

    return services;
  }
}
