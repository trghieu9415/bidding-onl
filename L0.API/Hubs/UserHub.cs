using Microsoft.AspNetCore.SignalR;

namespace L0.API.Hubs;

public class UserHub : Hub {
  public override async Task OnConnectedAsync() {
    await base.OnConnectedAsync();
  }
}
