using L1.Core.Base.Event;

namespace L1.Core.Domain.Bidding.Events;

public record OutbidEvent(
  Guid AuctionId,
  Guid PreviousBidderId,
  decimal NewPrice
) : DomainEvent {
  public OutbidEvent() : this(Guid.Empty, Guid.Empty, 0) {}
  public override Guid AggregateId => AuctionId;
}
