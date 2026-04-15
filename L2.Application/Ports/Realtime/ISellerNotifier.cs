namespace L2.Application.Ports.Realtime;

public interface ISellerNotifier {
  Task SendItemReceivedNewBidAlertAsync(
    Guid sellerId, Guid auctionId, decimal newPrice,
    CancellationToken ct = default
  );

  Task SendAuctionStartedAlertAsync(
    Guid sellerId, Guid itemId, Guid auctionId,
    CancellationToken ct = default
  );

  Task SendAuctionFinishedAlertAsync(
    Guid sellerId, Guid auctionId, decimal finalPrice,
    CancellationToken ct = default
  );

  Task SendAuctionFailedNoBidsAlertAsync(
    Guid sellerId, Guid auctionId,
    CancellationToken ct = default
  );

  Task SendItemApprovedAlertAsync(
    Guid sellerId, Guid itemId,
    CancellationToken ct = default
  );

  Task SendItemRejectedAlertAsync(
    Guid sellerId, Guid itemId,
    CancellationToken ct = default
  );
}
