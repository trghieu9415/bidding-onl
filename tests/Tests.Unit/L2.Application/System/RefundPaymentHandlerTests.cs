using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;
using SystemRefundPaymentCommand = L2.Application.UseCases.System.RefundPayment.RefundPaymentCommand;
using SystemRefundPaymentHandler = L2.Application.UseCases.System.RefundPayment.RefundPaymentHandler;

namespace Tests.Unit.L2.Application.System;

public class RefundPaymentHandlerTests {
  [Fact]
  public async Task Handle_WhenPaymentMissing_ReturnsFalse() {
    var handler = new SystemRefundPaymentHandler(new StubRepository<Payment>(), new StubGatewayFactory());

    var result = await handler.Handle(
      new SystemRefundPaymentCommand(Guid.NewGuid()),
      TestContext.Current.CancellationToken
    );

    Assert.False(result);
  }

  [Fact]
  public async Task Handle_WhenPaymentExists_UsesGatewayAndPersistsRefund() {
    var payment = Payment.Create(Guid.NewGuid(), 120m, PaymentMethod.Stripe);
    payment.MarkAsCompleted("txn-001");
    var paymentRepo = new StubRepository<Payment> { EntityByIdResult = payment };
    var gateway = new StubPaymentGateway { RefundPaymentResult = true };
    var gatewayFactory = new StubGatewayFactory();
    gatewayFactory.Gateways[PaymentMethod.Stripe] = gateway;
    var handler = new SystemRefundPaymentHandler(paymentRepo, gatewayFactory);

    var result = await handler.Handle(
      new SystemRefundPaymentCommand(payment.Id),
      TestContext.Current.CancellationToken
    );

    Assert.True(result);
    Assert.Same(payment, paymentRepo.UpdatedEntity);
    Assert.Equal(PaymentStatus.Refunded, payment.Status);
    Assert.Same(payment, gateway.LastRefundPaymentInput);
  }
}
