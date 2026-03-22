using L1.Core.Domain.Transaction.Events;
using MassTransit;

namespace L3.Worker.Consumers.Transaction;

public record OrderCreatedConsumer : IConsumer<OrderCreatedEvent> {
  public Task Consume(ConsumeContext<OrderCreatedEvent> context) {
    throw new NotImplementedException();
  }
}
