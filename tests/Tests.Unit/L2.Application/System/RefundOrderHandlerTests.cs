using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.UseCases.System.RefundOrder;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.System;

public class RefundOrderHandlerTests {
  [Fact]
  public async Task Handle_WhenFound_RefundsAndPersists() {
    var order = SystemTestData.CreateOrder(Guid.NewGuid());
    order.MarkAsPaid(order.BidderEmail);
    var repo = new StubRepository<Order> { EntityByIdResult = order };
    var handler = new RefundOrderHandler(repo);

    var result = await handler.Handle(new RefundOrderCommand(order.Id), TestContext.Current.CancellationToken);

    Assert.True(result);
    Assert.Same(order, repo.UpdatedEntity);
    Assert.Equal(OrderStatus.Refunded, order.Status);
  }
}
