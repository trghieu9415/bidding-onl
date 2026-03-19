using L0.API.Adapters.Realtime;
using L2.Application.Ports.Realtime;

namespace L0.API.Extensions;

public static class RealtimeExtensions {
  public static IServiceCollection AddSignalRAdapters(this IServiceCollection services) {
    services.AddSignalR();
    services.AddScoped<IBiddingNotifier, BiddingNotifier>();
    services.AddScoped<IUserNotifier, UserNotifier>();

    return services;
  }
}
