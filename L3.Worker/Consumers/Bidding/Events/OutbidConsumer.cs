using L1.Core.Domain.Bidding.Events;
using L2.Application.Ports.Realtime;
using L2.Application.Ports.Realtime.Contracts;
using MassTransit;

namespace L3.Worker.Consumers.Bidding.Events;

public class OutbidConsumer(IRealtimeService realtimeService) : IConsumer<OutbidEvent> {
  public async Task Consume(ConsumeContext<OutbidEvent> context) {
    var msg = context.Message;
    await realtimeService.PublishAsync(
      HubKeys.Notification,
      msg.PreviousBidderId.ToString(),
      "ReceiveNotification",
      new {
        Message = "Bạn đã bị đặt giá cao hơn!",
        msg.NewPrice,
        Timestamp = msg.OccurredOn
      }
    );
  }
}
