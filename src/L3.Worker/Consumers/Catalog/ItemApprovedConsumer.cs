using L1.Core.Domain.Catalog.Events;
using L2.Application.Ports.Realtime;
using MassTransit;

namespace L3.Worker.Consumers.Catalog;

public class ItemApprovedConsumer(ISellerNotifier sellerNotifier) : IConsumer<ItemApprovedEvent> {
  public async Task Consume(ConsumeContext<ItemApprovedEvent> context) {
    var msg = context.Message;
    Console.WriteLine($"Item {msg.ItemId} approved!");

    await sellerNotifier.SendItemApprovedAlertAsync(
      msg.OwnerId,
      msg.ItemId,
      context.CancellationToken
    );
  }
}
