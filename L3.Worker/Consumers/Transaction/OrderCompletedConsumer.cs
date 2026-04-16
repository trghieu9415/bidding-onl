using L1.Core.Domain.Transaction.Events;
using MassTransit;

namespace L3.Worker.Consumers.Transaction;

public class OrderCompletedConsumer : IConsumer<OrderCompletedEvent> {
  public Task Consume(ConsumeContext<OrderCompletedEvent> context) {
    var msg = context.Message;
    throw new NotImplementedException();
  }
}
