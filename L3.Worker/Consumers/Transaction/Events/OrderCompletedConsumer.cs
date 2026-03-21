using L1.Core.Domain.Transaction.Events;
using MassTransit;

namespace L3.Worker.Consumers.Transaction.Events;

public record OrderCompletedConsumer : IConsumer<OrderCompletedEvent> {
  public Task Consume(ConsumeContext<OrderCompletedEvent> context) {
    throw new NotImplementedException();
  }
}
