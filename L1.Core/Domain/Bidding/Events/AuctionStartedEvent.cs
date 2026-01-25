using L1.Core.Base.Event;

namespace L1.Core.Domain.Bidding.Events;

public record AuctionStartedEvent(Guid AuctionId) : DomainEvent {
  public override Guid AggregateId => AuctionId;
}
