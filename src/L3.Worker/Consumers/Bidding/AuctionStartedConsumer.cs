using L1.Core.Domain.Bidding.Events;
using L2.Application.Ports.Realtime;
using MassTransit;

namespace L3.Worker.Consumers.Bidding;

public class AuctionStartedConsumer(
  ISellerNotifier sellerNotifier
) : IConsumer<AuctionStartedEvent> {
  public async Task Consume(ConsumeContext<AuctionStartedEvent> context) {
    var msg = context.Message;

    await sellerNotifier.SendAuctionStartedAlertAsync(
      msg.OwnerId,
      msg.ItemId,
      msg.AuctionId,
      context.CancellationToken
    );
  }
}
