using L1.Core.Base.Event;

namespace L1.Core.Domain.Transaction.Events;

public record OrderCreatedEvent(
  Guid OrderId,
  Guid BidderId,
  Guid AuctionId
) : DomainEvent {
  public override Guid AggregateId => OrderId;
}
