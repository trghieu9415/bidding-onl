using L1.Core.Domain.Transaction.Events;
using MassTransit;

namespace L3.Worker.Consumers.Transaction.Events;

public record OrderCanceledConsumer : IConsumer<OrderCanceledEvent> {
  public Task Consume(ConsumeContext<OrderCanceledEvent> context) {
    throw new NotImplementedException();
  }
}
