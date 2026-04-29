using L0.API.Hubs;
using L2.Application.Ports.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace L0.API.Adapters.Realtime;

public class SignalRSellerNotifier(IHubContext<AuctionHub> hubContext) : ISellerNotifier {
  public Task SendItemReceivedNewBidAlertAsync(
    Guid sellerId, Guid auctionId, decimal newPrice,
    CancellationToken ct = default
  ) {
    return SendToSellerAsync(sellerId, "item-received-new-bid", new { AuctionId = auctionId, NewPrice = newPrice }, ct);
  }

  public Task SendAuctionStartedAlertAsync(
    Guid sellerId, Guid itemId, Guid auctionId,
    CancellationToken ct = default
  ) {
    return SendToSellerAsync(sellerId, "auction-started", new { ItemId = itemId, AuctionId = auctionId }, ct);
  }

  public Task SendAuctionFinishedAlertAsync(
    Guid sellerId, Guid auctionId, decimal finalPrice,
    CancellationToken ct = default
  ) {
    return SendToSellerAsync(sellerId, "auction-finished", new { AuctionId = auctionId, FinalPrice = finalPrice }, ct);
  }

  public Task SendAuctionFailedNoBidsAlertAsync(
    Guid sellerId, Guid auctionId,
    CancellationToken ct = default
  ) {
    return SendToSellerAsync(sellerId, "auction-failed-no-bids", new { AuctionId = auctionId }, ct);
  }

  public Task SendItemApprovedAlertAsync(
    Guid sellerId, Guid itemId,
    CancellationToken ct = default
  ) {
    return SendToSellerAsync(sellerId, "item-approved", new { ItemId = itemId }, ct);
  }

  public Task SendItemRejectedAlertAsync(
    Guid sellerId, Guid itemId,
    CancellationToken ct = default
  ) {
    return SendToSellerAsync(sellerId, "item-rejected", new { ItemId = itemId }, ct);
  }

  // NOTE: ========== [Helper Method] ==========
  private Task SendToSellerAsync(Guid targetId, string method, object payload, CancellationToken ct) {
    return hubContext.Clients.User(targetId.ToString()).SendAsync(method, payload, ct);
  }
}
