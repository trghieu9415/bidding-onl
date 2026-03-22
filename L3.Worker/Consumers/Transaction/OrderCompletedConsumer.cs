using L1.Core.Domain.Transaction.Events;
using L3.Infrastructure.Services.Abstractions;
using MassTransit;

namespace L3.Worker.Consumers.Transaction;

public class OrderCompletedConsumer(
  IEmailService emailService
) : IConsumer<OrderCompletedEvent> {
  public async Task Consume(ConsumeContext<OrderCompletedEvent> context) {
    var msg = context.Message;

    await emailService.SendOrderConfirmationEmailAsync(
      msg.BidderEmail,
      msg.OrderId.ToString(),
      context.CancellationToken
    );
  }
}
