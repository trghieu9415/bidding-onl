using L1.Core.Domain.Bidding.Events;
using L2.Application.Ports.Realtime;
using L2.Application.Ports.Realtime.Contracts;
using MassTransit;

// Chứa IRealtimeService

namespace L3.Worker.Consumers.Bidding;

public class AuctionEndedConsumer(IRealtimeService realtimeService)
  : IConsumer<AuctionEndedEvent> {
  public async Task Consume(ConsumeContext<AuctionEndedEvent> context) {
    var msg = context.Message;

    await realtimeService.PublishAsync(
      HubKeys.Bidding,
      msg.AuctionId.ToString(),
      "AuctionFinished",
      new {
        msg.WinnerId,
        msg.FinalPrice,
        msg.IsSold
      },
      context.CancellationToken
    );
  }
}
