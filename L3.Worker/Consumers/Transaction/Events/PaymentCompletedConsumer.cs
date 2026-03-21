using L1.Core.Domain.Transaction.Events;
using MassTransit;

namespace L3.Worker.Consumers.Transaction.Events;

public record PaymentCompletedConsumer : IConsumer<PaymentCompletedEvent> {
  public Task Consume(ConsumeContext<PaymentCompletedEvent> context) {
    throw new NotImplementedException();
  }
}
