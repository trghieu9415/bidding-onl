using L1.Core.Base.Event;

namespace L1.Core.Domain.Bidding.Events;

public record AuctionEndedEvent(Guid AuctionId, Guid? WinnerId, decimal FinalPrice, bool IsSold) : DomainEvent {
  public override Guid AggregateId => AuctionId;
}
