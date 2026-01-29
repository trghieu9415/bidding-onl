using L0.API.Hubs;
using L2.Application.Ports.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace L0.API.Adapters.Realtime;

public class UserNotifier(IHubContext<NotificationHub> hubContext) : IUserNotifier {
  public async Task SendToUser(Guid userId, string method, object data, CancellationToken ct = default) {
    await hubContext.Clients.Group(userId.ToString()).SendAsync(method, data, ct);
  }
}
