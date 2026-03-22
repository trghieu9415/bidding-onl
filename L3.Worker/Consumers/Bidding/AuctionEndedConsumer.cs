using L1.Core.Domain.Bidding.Events;
using L2.Application.Ports.Realtime;
using MassTransit;

namespace L3.Worker.Consumers.Bidding;

public class AuctionEndedConsumer(
  IBiddingNotifier biddingNotifier,
  IUserNotifier userNotifier
) : IConsumer<AuctionEndedEvent> {
  public async Task Consume(ConsumeContext<AuctionEndedEvent> context) {
    var msg = context.Message;

    await biddingNotifier.NotifyAuctionEnded(
      msg.AuctionId,
      msg.WinnerId,
      msg.FinalPrice,
      context.CancellationToken
    );

    await userNotifier.SendToUser(
      msg.OwerId,
      "AuctionFinished",
      new {
        msg.IsSold,
        msg.FinalPrice
      },
      context.CancellationToken
    );
  }
}
