using L1.Core.Base.Event;
using L1.Core.Domain.Transaction.Enums;

namespace L1.Core.Domain.Transaction.Events;

public record PaymentRefundedEvent(
  Guid PaymentId,
  Guid OrderId,
  decimal Amount,
  PaymentMethod Method,
  string? TransactionId
) : DomainEvent {
  public override Guid AggregateId => PaymentId;
}
