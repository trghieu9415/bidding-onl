using L1.Core.Domain.Bidding.Events;
using L2.Application.Ports.Realtime;
using L2.Application.Ports.Realtime.Contracts;
using MassTransit;

namespace L3.Worker.Consumers.Bidding;

public class BidPlacedConsumer(IRealtimeService realtimeService)
  : IConsumer<BidPlacedEvent> {
  public async Task Consume(ConsumeContext<BidPlacedEvent> context) {
    var msg = context.Message;

    await realtimeService.PublishAsync(
      HubKeys.Bidding,
      msg.AuctionId.ToString(),
      "NewBidReceived",
      new {
        msg.BidderId,
        msg.Amount
      },
      context.CancellationToken
    );
  }
}
