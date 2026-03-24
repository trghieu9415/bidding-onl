using L1.Core.Domain.Transaction.Events;
using L2.Application.Constants;
using L2.Application.Ports.Background;
using L2.Application.Ports.Realtime;
using L3.Infrastructure.Services.Abstractions;
using MassTransit;

namespace L3.Worker.Consumers.Transaction;

public class OrderCompletedConsumer(
  ITaskQueue taskQueue
) : IConsumer<OrderCompletedEvent> {
  public async Task Consume(ConsumeContext<OrderCompletedEvent> context) {
    var msg = context.Message;

    taskQueue.Queue<IEmailService>(e => e.SendOrderConfirmationEmailAsync(
      msg.BidderEmail,
      msg.OrderId.ToString(),
      CancellationToken.None
    ));

    taskQueue.Queue<IUserNotifier>(n => n.SendToUser(
      msg.BidderId,
      ClientMethods.OrderCompleted,
      new { msg.OrderId, msg.AuctionId },
      CancellationToken.None
    ));
    await Task.CompletedTask;
  }
}
