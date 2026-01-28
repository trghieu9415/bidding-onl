using L1.Core.Base.Event;

namespace L1.Core.Domain.Bidding.Events;

public record AuctionStartedEvent(
  Guid AuctionId,
  Guid OwnerId
) : DomainEvent {
  public AuctionStartedEvent() : this(Guid.Empty, Guid.Empty) {}
  public override Guid AggregateId => AuctionId;
}
