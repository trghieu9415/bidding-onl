using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.UseCases.Transactions.Commands.VerifyPayment;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Transactions;

public class VerifyClientPaymentHandlerTests {
  [Fact]
  public async Task Handle_WhenGatewaySucceeds_MarksPaymentCompleted() {
    var payment = Payment.Create(Guid.NewGuid(), 150m, PaymentMethod.Stripe);
    var paymentRepo = new StubRepository<Payment> { EntityByIdResult = payment };
    var gateway = new StubPaymentGateway { ClientVerificationResult = (true, "txn-001") };
    var gatewayFactory = new StubGatewayFactory();
    gatewayFactory.Gateways[PaymentMethod.Stripe] = gateway;
    var handler = new VerifyClientPaymentHandler(paymentRepo, gatewayFactory);

    var result = await handler.Handle(new VerifyClientPaymentCommand(Guid.NewGuid(), new VerifyClientPaymentRequest(payment.Id, TestJson.CreateObject())), TestContext.Current.CancellationToken);

    Assert.True(result);
    Assert.Equal(PaymentStatus.Succeeded, payment.Status);
    Assert.Equal("txn-001", payment.TransactionId);
  }

  [Fact]
  public async Task Handle_WhenGatewayFails_MarksPaymentFailed() {
    var payment = Payment.Create(Guid.NewGuid(), 150m, PaymentMethod.Stripe);
    var paymentRepo = new StubRepository<Payment> { EntityByIdResult = payment };
    var gateway = new StubPaymentGateway { ClientVerificationResult = (false, string.Empty) };
    var gatewayFactory = new StubGatewayFactory();
    gatewayFactory.Gateways[PaymentMethod.Stripe] = gateway;
    var handler = new VerifyClientPaymentHandler(paymentRepo, gatewayFactory);

    var result = await handler.Handle(new VerifyClientPaymentCommand(Guid.NewGuid(), new VerifyClientPaymentRequest(payment.Id, TestJson.CreateObject())), TestContext.Current.CancellationToken);

    Assert.True(result);
    Assert.Equal(PaymentStatus.Failed, payment.Status);
  }
}
