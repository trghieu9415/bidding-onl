namespace L2.Application.Ports.Realtime;

public interface IAuctionNotifier {
  Task BroadcastNewBidAsync(
    Guid auctionId, decimal newPrice,
    string highestBidderName,
    CancellationToken ct = default
  );

  Task BroadcastAuctionEndedAsync(
    Guid auctionId,
    CancellationToken ct = default
  );
}
