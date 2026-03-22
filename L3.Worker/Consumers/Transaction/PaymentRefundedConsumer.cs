using MassTransit;

namespace L3.Worker.Consumers.Transaction;

public class PaymentRefundedConsumer : IConsumer<PaymentRefundedConsumer> {
  public Task Consume(ConsumeContext<PaymentRefundedConsumer> context) {
    throw new NotImplementedException();
  }
}
