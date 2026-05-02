using L1.Core.Domain.Transaction.Enums;
using L1.Core.Domain.Transaction.Events;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.Events;

public class TransactionEventTests {
  [Fact]
  public void Events_ExposeExpectedAggregateIds() {
    var orderId = Guid.NewGuid();
    var paymentId = Guid.NewGuid();

    var canceledEvent = new OrderCanceledEvent(orderId, Guid.NewGuid(), Guid.NewGuid());
    var completedEvent =
      new OrderCompletedEvent(orderId, Guid.NewGuid(), Guid.NewGuid(), "Bidder", "bidder@example.com");
    var createdEvent = new OrderCreatedEvent(orderId, Guid.NewGuid(), Guid.NewGuid());
    var paymentCompletedEvent =
      new PaymentCompletedEvent(paymentId, Guid.NewGuid(), 150m, PaymentMethod.Paypal, "txn-001");
    var paymentRefundedEvent =
      new PaymentRefundedEvent(paymentId, Guid.NewGuid(), 150m, PaymentMethod.Paypal, "txn-001");

    Assert.Equal(orderId, canceledEvent.AggregateId);
    Assert.Equal(orderId, completedEvent.AggregateId);
    Assert.Equal(orderId, createdEvent.AggregateId);
    Assert.Equal(paymentId, paymentCompletedEvent.AggregateId);
    Assert.Equal(paymentId, paymentRefundedEvent.AggregateId);
  }
}
