using L1.Core.Domain.Catalog.Events;
using L2.Application.Ports.Realtime;
using MassTransit;

namespace L3.Worker.Consumers.Catalog;

public class ItemRegisteredConsumer(
  ISellerNotifier sellerNotifier
) : IConsumer<ItemRegisteredEvent> {
  public Task Consume(ConsumeContext<ItemRegisteredEvent> context) {
    var msg = context.Message;
    return Task.CompletedTask;
  }
}
