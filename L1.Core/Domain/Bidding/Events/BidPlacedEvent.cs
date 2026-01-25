using L1.Core.Base.Event;

namespace L1.Core.Domain.Bidding.Events;

public record BidPlacedEvent(Guid AuctionId, Guid BidderId, decimal Amount) : DomainEvent {
  public override Guid AggregateId => AuctionId;
}
