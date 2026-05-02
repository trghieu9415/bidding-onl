using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.UseCases.Transactions.Commands.CreatePayment;
using Tests.Unit.L2.Application.UseCases.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Transactions;

public class CreatePaymentHandlerTests {
  [Fact]
  public async Task Handle_WhenPendingPaymentExists_ReturnsExistingUrl() {
    var order = TransactionTestData.CreatePendingOrder(Guid.NewGuid());
    ReflectionTestHelper.SetProperty(order, nameof(Order.Price), 320m);
    var existingPayment = Payment.Create(order.Id, 320m, PaymentMethod.Stripe);
    existingPayment.SetPaymentUrl("https://existing-payment");
    var orderRepo = new StubRepository<Order> { EntityByIdResult = order };
    var paymentRepo = new StubRepository<Payment> { FirstEntityResult = existingPayment };
    var handler = new CreatePaymentHandler(orderRepo, paymentRepo, new StubGatewayFactory());

    var result = await handler.Handle(
      new CreatePaymentCommand(order.Id, order.BidderId, PaymentMethod.Stripe),
      TestContext.Current.CancellationToken
    );

    Assert.Equal("https://existing-payment", result.PaymentUrl);
    Assert.Null(paymentRepo.CreatedEntity);
  }

  [Fact]
  public async Task Handle_WhenNoPendingPayment_CreatesPaymentAndPersists() {
    var order = TransactionTestData.CreatePendingOrder(Guid.NewGuid());
    ReflectionTestHelper.SetProperty(order, nameof(Order.Price), 450m);
    var orderRepo = new StubRepository<Order> { EntityByIdResult = order };
    var paymentRepo = new StubRepository<Payment>();
    var gateway = new StubPaymentGateway { PaymentUrlResult = "https://new-payment" };
    var gatewayFactory = new StubGatewayFactory();
    gatewayFactory.Gateways[PaymentMethod.Paypal] = gateway;
    var handler = new CreatePaymentHandler(orderRepo, paymentRepo, gatewayFactory);

    var result = await handler.Handle(
      new CreatePaymentCommand(order.Id, order.BidderId, PaymentMethod.Paypal),
      TestContext.Current.CancellationToken
    );

    Assert.Equal("https://new-payment", result.PaymentUrl);
    var createdPayment = Assert.IsType<Payment>(paymentRepo.CreatedEntity);
    Assert.Equal(order.Id, createdPayment.OrderId);
    Assert.Equal(450m, createdPayment.Amount);
    Assert.Equal(PaymentMethod.Paypal, createdPayment.Method);
    Assert.Equal("https://new-payment", createdPayment.PaymentUrl);
    Assert.Equal(PaymentMethod.Paypal, gatewayFactory.LastRequestedMethod);
  }
}
