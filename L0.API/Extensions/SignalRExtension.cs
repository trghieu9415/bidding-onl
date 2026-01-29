using L0.API.Adapters.Realtime;
using L2.Application.Ports.Realtime;

namespace L0.API.Extensions;

public static class SignalRExtension {
  public static IServiceCollection AddSignalRServices(this IServiceCollection services) {
    services.AddSignalR();
    services.AddScoped<IBiddingNotifier, BiddingNotifier>();
    services.AddScoped<IUserNotifier, UserNotifier>();
    return services;
  }
}
