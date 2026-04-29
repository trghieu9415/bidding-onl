using L1.Core.Domain.Transaction.Entities;
using L2.Application.UseCases.System.ExpireOrder;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.System;

public class ExpireOrderHandlerTests {
  [Fact]
  public async Task Handle_WhenOrderAlreadyConfirmed_ReturnsFalse() {
    var order = SystemTestData.CreateOrder(Guid.NewGuid());
    order.MarkAsPaid(order.BidderEmail);
    var repo = new StubRepository<Order> { EntityByIdResult = order };
    var handler = new ExpireOrderHandler(repo);

    var result = await handler.Handle(new ExpireOrderCommand(order.Id), TestContext.Current.CancellationToken);

    Assert.False(result);
    Assert.Null(repo.UpdatedEntity);
  }
}
