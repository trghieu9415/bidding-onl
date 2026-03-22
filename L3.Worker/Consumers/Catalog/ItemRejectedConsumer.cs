using L1.Core.Domain.Catalog.Events;
using L2.Application.Ports.Realtime;
using MassTransit;

namespace L3.Worker.Consumers.Catalog;

public class ItemRejectedConsumer(IUserNotifier userNotifier) : IConsumer<ItemRejectedEvent> {
  public async Task Consume(ConsumeContext<ItemRejectedEvent> context) {
    var msg = context.Message;

    await userNotifier.SendToUser(
      msg.OwnerId,
      "ItemRejected",
      new {
        msg.ItemId,
        msg.Reason
      },
      context.CancellationToken
    );
  }
}
