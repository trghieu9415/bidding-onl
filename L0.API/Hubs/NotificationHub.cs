using L2.Application.Ports.Security;
using Microsoft.AspNetCore.SignalR;

namespace L0.API.Hubs;

public class NotificationHub(ICurrentUser currentUser) : Hub {
  public override async Task OnConnectedAsync() {
    var userId = currentUser.User.Id;
    await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
    await base.OnConnectedAsync();
  }

  public override async Task OnDisconnectedAsync(Exception? exception) {
    var userId = currentUser.User.Id;
    await base.OnDisconnectedAsync(exception);
  }
}
