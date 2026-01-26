// FILE: L0.API/Adapters/Notification/SignalRRealtimeService.cs

using System.Collections.Concurrent;
using L2.Application.Ports.Realtime;
using L2.Application.Ports.Realtime.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace L0.API.Adapters.Realtime;

public class SignalRRealtimeService(
  IServiceProvider serviceProvider,
  HubRegistry registry
) : IRealtimeService {
  private static readonly ConcurrentDictionary<Type, object> HubContextCache = new();

  public async Task PublishAsync(string hubKey, string group, string method, object data,
    CancellationToken ct = default) {
    if (!registry.TryGetHubType(hubKey, out var hubType) || hubType == null) {
      throw new Exception($"Hub với key '{hubKey}' chưa được đăng ký trong HubRegistry.");
    }

    var hubContext = HubContextCache.GetOrAdd(hubType, type => {
      var hubContextType = typeof(IHubContext<>).MakeGenericType(type);
      return serviceProvider.GetRequiredService(hubContextType);
    });

    dynamic dynamicContext = hubContext;
    await dynamicContext.Clients.Group(group).SendAsync(method, data, ct);
  }
}
