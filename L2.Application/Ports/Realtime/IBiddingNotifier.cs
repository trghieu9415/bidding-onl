namespace L2.Application.Ports.Realtime;

public interface IBiddingNotifier {
  Task NotifyNewBid(Guid auctionId, Guid bidderId, decimal amount, CancellationToken ct = default);
  Task NotifyAuctionEnded(Guid auctionId, Guid? winnerId, decimal finalPrice, CancellationToken ct = default);
}
