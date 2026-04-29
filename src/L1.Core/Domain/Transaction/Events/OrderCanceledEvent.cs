using L1.Core.Base.Event;

namespace L1.Core.Domain.Transaction.Events;

public record OrderCanceledEvent(
  Guid OrderId,
  Guid CustomerId,
  Guid AuctionId
) : DomainEvent {
  public override Guid AggregateId => OrderId;
}
