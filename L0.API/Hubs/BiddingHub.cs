using Microsoft.AspNetCore.SignalR;

namespace L0.API.Hubs;

public class BiddingHub : Hub {
  public async Task JoinAuction(Guid auctionId) {
    await Groups.AddToGroupAsync(Context.ConnectionId, auctionId.ToString());
  }

  public async Task LeaveAuction(Guid auctionId) {
    await Groups.RemoveFromGroupAsync(Context.ConnectionId, auctionId.ToString());
  }
}
