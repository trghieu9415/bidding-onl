using L1.Core.Domain.Bidding.Events;
using L2.Application.Ports.Realtime;
using MassTransit;

namespace L3.Worker.Consumers.Bidding;

public class AuctionStartedConsumer(
  IUserNotifier userNotifier
) : IConsumer<AuctionStartedEvent> {
  public async Task Consume(ConsumeContext<AuctionStartedEvent> context) {
    var msg = context.Message;

    await userNotifier.SendToUser(
      msg.OwnerId,
      "AuctionStarted",
      new {
        Message = "Sản phẩm đã được mở đấu giá!",
        msg.AuctionId
      },
      context.CancellationToken
    );
  }
}
