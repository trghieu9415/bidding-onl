using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.UseCases.System.MarkOrderAsPaid;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.System;

public class MarkOrderAsPaidHandlerTests {
  [Fact]
  public async Task Handle_WhenFound_UpdatesOrderStatus() {
    var order = SystemTestData.CreateOrder(Guid.NewGuid());
    var repo = new StubRepository<Order> { EntityByIdResult = order };
    var handler = new MarkOrderAsPaidHandler(repo);

    var result = await handler.Handle(new MarkOrderAsPaidCommand(order.Id), TestContext.Current.CancellationToken);

    Assert.True(result);
    Assert.Same(order, repo.UpdatedEntity);
    Assert.Equal(OrderStatus.Confirmed, order.Status);
  }
}
