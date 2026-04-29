namespace L2.Application.Ports.Realtime;

public interface IBidderNotifier {
  Task SendOutbidAlertAsync(
    Guid userId, Guid auctionId, decimal currentHighestPrice,
    CancellationToken ct = default
  );

  Task SendAuctionWonAlertAsync(
    Guid userId, Guid auctionId,
    CancellationToken ct = default
  );

  Task SendAuctionLostAlertAsync(
    Guid userId, Guid auctionId,
    CancellationToken ct = default
  );

  Task SendPaymentSuccessAlertAsync(
    Guid userId, Guid orderId,
    CancellationToken ct = default
  );

  Task SendPaymentFailedAlertAsync(
    Guid userId, Guid orderId, string reason,
    CancellationToken ct = default
  );

  Task SendAuctionFinishedAlertAsync(
    Guid sellerId, Guid auctionId, decimal finalPrice,
    CancellationToken ct = default
  );
}
