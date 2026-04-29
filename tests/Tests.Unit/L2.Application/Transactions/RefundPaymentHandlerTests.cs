using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.UseCases.Transactions.Commands.RefundPayment;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Transactions;

public class RefundPaymentHandlerTests {
  [Fact]
  public async Task Handle_WhenOrderBelongsToDifferentUser_ThrowsWorkflowException() {
    var payment = Payment.Create(Guid.NewGuid(), 150m, PaymentMethod.Stripe);
    payment.MarkAsCompleted("txn-001");
    var paymentRepo = new StubRepository<Payment> { EntityByIdResult = payment };
    var orderReadRepo = new StubOrderReadRepository {
      FirstEntityResult = new OrderDto { Id = payment.OrderId, BidderId = Guid.NewGuid() }
    };
    var handler = new RefundPaymentHandler(paymentRepo, orderReadRepo);

    var exception = await Assert.ThrowsAsync<WorkflowException>(async () =>
      await handler.Handle(new RefundPaymentCommand(payment.Id, Guid.NewGuid()), TestContext.Current.CancellationToken));

    Assert.Equal(403, exception.StatusCode);
    Assert.Equal("Bạn không có quyền hoàn trả đơn hàng này", exception.Message);
  }
}
