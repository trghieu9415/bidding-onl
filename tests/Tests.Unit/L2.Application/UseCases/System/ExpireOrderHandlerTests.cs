using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.Exceptions;
using L2.Application.UseCases.System.ExpireOrder;
using Tests.Unit.L2.Application.UseCases.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.System;

public class ExpireOrderHandlerTests {
  [Fact]
  public async Task Handle_OrderNotFound_ThrowsWorkflowException() {
    var repo = new StubRepository<Order> { EntityByIdResult = null };
    var handler = new ExpireOrderHandler(repo);
    var command = new ExpireOrderCommand(Guid.NewGuid());

    var exception = await Assert.ThrowsAsync<WorkflowException>(() =>
      handler.Handle(command, CancellationToken.None));

    Assert.Equal(404, exception.StatusCode);
    Assert.Equal("Đơn hàng không tồn tại", exception.Message);
  }

  [Fact]
  public async Task Handle_OrderAlreadyConfirmed_ReturnsFalse_And_DoesNotUpdate() {
    var orderId = Guid.NewGuid();
    var order = SystemTestData.CreateOrder(orderId);

    order.MarkAsPaid(order.BidderEmail);

    var repo = new StubRepository<Order> { EntityByIdResult = order };
    var handler = new ExpireOrderHandler(repo);

    var result = await handler.Handle(
      new ExpireOrderCommand(orderId),
      CancellationToken.None
    );

    Assert.False(result);
    Assert.Null(repo.UpdatedEntity);
  }

  [Fact]
  public async Task Handle_OrderIsPending_CancelsOrder_ReturnsTrue_And_UpdatesRepository() {
    var bidderId = Guid.NewGuid();
    var order = SystemTestData.CreateOrder(bidderId);
    var orderId = order.Id;

    Assert.NotEqual(OrderStatus.Confirmed, order.Status);

    var repo = new StubRepository<Order> { EntityByIdResult = order };
    var handler = new ExpireOrderHandler(repo);

    var result = await handler.Handle(
      new ExpireOrderCommand(orderId),
      CancellationToken.None
    );

    Assert.True(result);
    Assert.NotNull(repo.UpdatedEntity);
    Assert.Equal(orderId, repo.UpdatedEntity.Id);
    Assert.Equal(OrderStatus.Canceled, order.Status);
  }
}
