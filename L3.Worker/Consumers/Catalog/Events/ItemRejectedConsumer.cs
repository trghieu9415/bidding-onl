using L1.Core.Domain.Catalog.Events;
using L2.Application.Ports.Realtime;
using L2.Application.Ports.Realtime.Contracts;
using MassTransit;

namespace L3.Worker.Consumers.Catalog.Events;

public class ItemRejectedConsumer(IRealtimeService realtimeService) : IConsumer<ItemRejectedEvent> {
  public async Task Consume(ConsumeContext<ItemRejectedEvent> context) {
    var msg = context.Message;

    await realtimeService.PublishAsync(
      HubKeys.Notification,
      msg.OwnerId.ToString(),
      "ItemRejected",
      new {
        msg.ItemId,
        msg.Reason
      },
      context.CancellationToken
    );
  }
}
