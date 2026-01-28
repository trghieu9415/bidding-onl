using L1.Core.Base.Event;

namespace L1.Core.Domain.Bidding.Events;

public record BidPlacedEvent(
  Guid AuctionId,
  Guid BidderId,
  decimal Amount = 0
) : DomainEvent {
  public BidPlacedEvent() : this(Guid.Empty, Guid.Empty) {}
  public override Guid AggregateId => AuctionId;
}
