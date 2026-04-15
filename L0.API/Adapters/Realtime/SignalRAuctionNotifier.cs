using L0.API.Hubs;
using L2.Application.Ports.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace L0.API.Adapters.Realtime;

public class SignalRAuctionNotifier(IHubContext<AuctionHub> hubContext) : IAuctionNotifier {
  public Task BroadcastNewBidAsync(
    Guid auctionId, decimal newPrice, string highestBidderName,
    CancellationToken ct = default
  ) {
    return BroadcastToGroupAsync(auctionId, "new-bid",
      new { AuctionId = auctionId, CurrentPrice = newPrice, BidderName = highestBidderName }, ct);
  }

  public Task BroadcastAuctionEndedAsync(
    Guid auctionId,
    CancellationToken ct = default
  ) {
    return BroadcastToGroupAsync(auctionId, "auction-ended", new { AuctionId = auctionId }, ct);
  }

  // NOTE: ========== [Helper Method] ==========
  private Task BroadcastToGroupAsync(Guid targetGroup, string method, object payload, CancellationToken ct) {
    return hubContext.Clients.Group(targetGroup.ToString()).SendAsync(method, payload, ct);
  }
}
