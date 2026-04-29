using L0.API.Hubs;
using L2.Application.Ports.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace L0.API.Adapters.Realtime;

public class SignalRBidderNotifier(IHubContext<UserHub> hubContext) : IBidderNotifier {
  public Task SendOutbidAlertAsync(
    Guid userId, Guid auctionId, decimal currentHighestPrice,
    CancellationToken ct = default
  ) {
    return SendToUserAsync(userId, "outbid", new {
      AuctionId = auctionId, HighestPrice = currentHighestPrice
    }, ct);
  }

  public Task SendAuctionWonAlertAsync(
    Guid userId, Guid auctionId,
    CancellationToken ct = default
  ) {
    return SendToUserAsync(userId, "auction-won", new { AuctionId = auctionId }, ct);
  }

  public Task SendAuctionLostAlertAsync(
    Guid userId, Guid auctionId,
    CancellationToken ct = default
  ) {
    return SendToUserAsync(userId, "auction-lost", new { AuctionId = auctionId }, ct);
  }

  public Task SendPaymentSuccessAlertAsync(
    Guid userId, Guid orderId,
    CancellationToken ct = default
  ) {
    return SendToUserAsync(userId, "payment-success", new { OrderId = orderId }, ct);
  }

  public Task SendPaymentFailedAlertAsync(
    Guid userId, Guid orderId, string reason,
    CancellationToken ct = default
  ) {
    return SendToUserAsync(userId, "payment-failed", new { OrderId = orderId, Reason = reason }, ct);
  }

  public Task SendAuctionFinishedAlertAsync(
    Guid sellerId, Guid auctionId, decimal finalPrice,
    CancellationToken ct = default
  ) {
    return SendToUserAsync(sellerId, "auction-finished", new {
      AuctionId = auctionId, FinalPrice = finalPrice
    }, ct);
  }

  // NOTE: ========== [Helper Method] ==========
  private Task SendToUserAsync(
    Guid targetId, string method, object payload, CancellationToken ct) {
    return hubContext.Clients.User(targetId.ToString()).SendAsync(method, payload, ct);
  }
}
