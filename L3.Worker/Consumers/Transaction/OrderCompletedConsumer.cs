using L1.Core.Domain.Transaction.Events;
using L2.Application.Ports.Background;
using L2.Application.Ports.Realtime;
using L3.Infrastructure.Services.Abstractions;
using MassTransit;

namespace L3.Worker.Consumers.Transaction;

public class OrderCompletedConsumer(
  ITaskQueue taskQueue
) : IConsumer<OrderCompletedEvent> {
  public Task Consume(ConsumeContext<OrderCompletedEvent> context) {
    var msg = context.Message;
    throw new NotImplementedException();

  }
}
