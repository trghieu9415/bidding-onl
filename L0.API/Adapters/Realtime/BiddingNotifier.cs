using L0.API.Hubs;
using L2.Application.Ports.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace L0.API.Adapters.Realtime;

public class BiddingNotifier(IHubContext<BiddingHub> hubContext) : IBiddingNotifier {
  public async Task NotifyNewBid(
    Guid auctionId,
    Guid bidderId,
    string bidderName,
    decimal amount,
    CancellationToken ct = default
  ) {
    await hubContext.Clients.Group(auctionId.ToString())
      .SendAsync("NewBidReceived", new { bidderId, bidderName, amount }, ct);
  }

  public async Task NotifyAuctionEnded(Guid auctionId, Guid? winnerId, decimal finalPrice,
    CancellationToken ct = default) {
    await hubContext.Clients.Group(auctionId.ToString())
      .SendAsync("AuctionEnded", new { winnerId, finalPrice }, ct);
  }
}
