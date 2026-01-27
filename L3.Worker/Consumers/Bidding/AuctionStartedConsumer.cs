using L1.Core.Domain.Bidding.Events;
using L2.Application.Ports.Realtime;
using L2.Application.Ports.Realtime.Contracts;
using MassTransit;

namespace L3.Worker.Consumers.Bidding;

public class AuctionStartedConsumer(IRealtimeService realtimeService) : IConsumer<AuctionStartedEvent> {
  public async Task Consume(ConsumeContext<AuctionStartedEvent> context) {
    var msg = context.Message;

    await realtimeService.PublishAsync(
      HubKeys.Notification,
      msg.OwnerId.ToString(),
      "AuctionStarted",
      new {
        Message = "Sản phẩm đã được mở đấu giá!",
        msg.AuctionId
      },
      context.CancellationToken
    );
  }
}
