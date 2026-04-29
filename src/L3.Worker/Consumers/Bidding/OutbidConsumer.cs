using L1.Core.Domain.Bidding.Events;
using L2.Application.Ports.Realtime;
using MassTransit;

namespace L3.Worker.Consumers.Bidding;

public class OutbidConsumer(IBidderNotifier bidderNotifier) : IConsumer<OutbidEvent> {
  public async Task Consume(ConsumeContext<OutbidEvent> context) {
    var msg = context.Message;
    await bidderNotifier.SendOutbidAlertAsync(
      msg.PreviousBidderId,
      msg.AuctionId,
      msg.NewPrice,
      context.CancellationToken
    );
  }
}
