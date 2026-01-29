using L1.Core.Domain.Bidding.Events;
using L2.Application.Ports.Realtime;
using MassTransit;

namespace L3.Worker.Consumers.Bidding.Events;

public class BidPlacedConsumer(
  IBiddingNotifier biddingNotifier
) : IConsumer<BidPlacedEvent> {
  public async Task Consume(ConsumeContext<BidPlacedEvent> context) {
    var msg = context.Message;

    await biddingNotifier.NotifyNewBid(
      msg.AuctionId,
      msg.BidderId,
      msg.Amount,
      context.CancellationToken
    );
  }
}
