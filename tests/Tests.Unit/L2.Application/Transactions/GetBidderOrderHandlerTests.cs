using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.UseCases.Transactions.Queries.GetBidderOrder;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Transactions;

public class GetBidderOrderHandlerTests {
  [Fact]
  public async Task Handle_WhenOrderBelongsToDifferentBidder_ThrowsWorkflowException() {
    var order = new OrderDto { Id = Guid.NewGuid(), BidderId = Guid.NewGuid() };
    var orderReadRepo = new StubOrderReadRepository { OrderPaymentsResult = (order, []) };
    var handler = new GetBidderOrderHandler(orderReadRepo);

    var exception = await Assert.ThrowsAsync<WorkflowException>(async () =>
      await handler.Handle(new GetBidderOrderQuery(order.Id, Guid.NewGuid()), TestContext.Current.CancellationToken));

    Assert.Equal(404, exception.StatusCode);
    Assert.Equal("Không tìm thấy đơn hàng", exception.Message);
  }
}
