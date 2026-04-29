using L1.Core.Domain.Bidding.Events;
using L2.Application.Ports.Realtime;
using MassTransit;

namespace L3.Worker.Consumers.Bidding;

public class AuctionEndedConsumer(
  IAuctionNotifier auctionNotifier,
  IBidderNotifier bidderNotifier,
  ISellerNotifier sellerNotifier
) : IConsumer<AuctionEndedEvent> {
  public async Task Consume(ConsumeContext<AuctionEndedEvent> context) {
    var msg = context.Message;

    await auctionNotifier.BroadcastAuctionEndedAsync(
      msg.AuctionId, context.CancellationToken
    );


    if (msg.IsSold) {
      await bidderNotifier.SendAuctionWonAlertAsync(
        msg.WinnerId!.Value, msg.AuctionId,
        context.CancellationToken
      );
      await sellerNotifier.SendAuctionFinishedAlertAsync(
        msg.OwnerId, msg.AuctionId, msg.FinalPrice,
        context.CancellationToken
      );
    } else {
      await sellerNotifier.SendAuctionFailedNoBidsAlertAsync(
        msg.AuctionId, msg.AuctionId, context.CancellationToken
      );
    }
  }
}
