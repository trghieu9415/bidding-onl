using L1.Core.Base.Event;

namespace L1.Core.Domain.Bidding.Events;

public record OutbidEvent(Guid AuctionId, Guid PreviousBidderId, decimal NewPrice) : DomainEvent {
  public override Guid AggregateId => AuctionId;
}
