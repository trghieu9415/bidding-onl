using L1.Core.Domain.Bidding.Events;
using L2.Application.Ports.Realtime;
using MassTransit;

namespace L3.Worker.Consumers.Bidding;

public class BidPlacedConsumer(
  IAuctionNotifier auctionNotifier,
  IBidderNotifier bidderNotifier
) : IConsumer<BidPlacedEvent> {
  public async Task Consume(ConsumeContext<BidPlacedEvent> context) {
    var msg = context.Message;

    await auctionNotifier.BroadcastNewBidAsync(
      msg.AuctionId,
      msg.Amount,
      msg.BidderName,
      context.CancellationToken
    );
  }
}
