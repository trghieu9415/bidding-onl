using L2.Application.Abstractions;

namespace L2.Application.UseCases.Bidding.Bidder.PlaceBid;

public record PlaceBidCommand(Guid AuctionId, decimal Amount) : ICommand<Guid>, ILockable {
  public string LockKey => $"locks:auction:{AuctionId}";
  public TimeSpan Expiration => TimeSpan.FromSeconds(10);
  public TimeSpan WaitTime => TimeSpan.FromSeconds(5);
}
