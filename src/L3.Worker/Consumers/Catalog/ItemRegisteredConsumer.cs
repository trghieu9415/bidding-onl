using L1.Core.Domain.Catalog.Events;
using MassTransit;

namespace L3.Worker.Consumers.Catalog;

public class ItemRegisteredConsumer : IConsumer<ItemRegisteredEvent> {
  public Task Consume(ConsumeContext<ItemRegisteredEvent> context) {
    return Task.CompletedTask;
  }
}
