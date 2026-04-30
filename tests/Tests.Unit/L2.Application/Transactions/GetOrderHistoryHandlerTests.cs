using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using L2.Application.UseCases.Transactions.Queries.GetOrderHistory;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Transactions;

public class GetOrderHistoryHandlerTests {
  [Fact]
  public async Task Handle_AppliesBidderCriteriaAndReturnsMeta() {
    var userId = Guid.NewGuid();
    var orders = new List<OrderDto> {
      new() {
        Id = Guid.NewGuid(),
        BidderId = userId,
        Address = TransactionTestData.Address,
        CatalogName = "Laptop",
        CatalogImage = "img.png",
        BidderName = "John Doe"
      }
    };
    var filter = new OrderFilter { Page = 1, PerPage = 10 };
    var readRepo = new StubReadRepository<Order, OrderDto> { GetAsyncResult = (2, orders) };
    var handler = new GetOrderHistoryHandler(readRepo);

    var result = await handler.Handle(
      new GetOrderHistoryQuery(userId, filter),
      TestContext.Current.CancellationToken
    );

    Assert.Equal(orders, result.Orders);
    Assert.Equal(Meta.Create(1, 10, 2), result.Meta);
    Assert.NotNull(readRepo.LastCriteria);

    var criteria = readRepo.LastCriteria!;
    Assert.True(criteria.Compile()(TransactionTestData.CreatePendingOrder(userId)));
    Assert.False(criteria.Compile()(TransactionTestData.CreatePendingOrder(Guid.NewGuid())));
  }
}
