using L1.Core.Base.Event;

namespace L1.Core.Domain.Bidding.Events;

public record AuctionEndedEvent(
  Guid AuctionId,
  Guid? WinnerId,
  decimal FinalPrice,
  Guid OwerId,
  bool IsSold
) : DomainEvent {
  public AuctionEndedEvent() : this(Guid.Empty, null, 0, Guid.Empty, false) {}
  public override Guid AggregateId => AuctionId;
}
