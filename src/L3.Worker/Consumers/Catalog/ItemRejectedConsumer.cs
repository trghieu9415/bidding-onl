using L1.Core.Domain.Catalog.Events;
using L2.Application.Ports.Realtime;
using MassTransit;

namespace L3.Worker.Consumers.Catalog;

public class ItemRejectedConsumer(ISellerNotifier sellerNotifier) : IConsumer<ItemRejectedEvent> {
  public async Task Consume(ConsumeContext<ItemRejectedEvent> context) {
    var msg = context.Message;

    await sellerNotifier.SendItemRejectedAlertAsync(
      msg.OwnerId,
      msg.ItemId,
      context.CancellationToken
    );
  }
}
