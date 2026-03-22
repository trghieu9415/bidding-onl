using L1.Core.Domain.Bidding.Events;
using L2.Application.Ports.Realtime;
using MassTransit;

namespace L3.Worker.Consumers.Bidding;

public class OutbidConsumer(IUserNotifier userNotifier) : IConsumer<OutbidEvent> {
  public async Task Consume(ConsumeContext<OutbidEvent> context) {
    var msg = context.Message;
    await userNotifier.SendToUser(
      msg.PreviousBidderId,
      "Outbid",
      new {
        Message = "Bạn đã bị đặt giá cao hơn!",
        msg.NewPrice,
        Timestamp = msg.OccurredOn
      }
    );
  }
}
