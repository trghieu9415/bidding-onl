using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.ValueObjects;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.UseCases.Transactions.Commands.CreateOrder;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Transactions;

public class CreateOrderHandlerTests {
  [Fact]
  public async Task Handle_WhenAuctionNotEndedSold_ThrowsWorkflowException() {
    var auction = Auction.Create(Guid.NewGuid(), Guid.NewGuid(), 100m, 10m, 200m);
    var auctionRepo = new StubRepository<Auction> { EntityByIdResult = auction };
    var handler = new CreateOrderHandler(
      auctionRepo,
      new StubOrderReadRepository(),
      new StubRepository<Order>(),
      new StubReadRepository<CatalogItem, CatalogItemDto>()
    );
    var request = new CreateOrderCommand(
      Guid.NewGuid(),
      "Bidder",
      "bidder@example.com",
      new CreateOrderRequest(Guid.NewGuid(), new Address("John Doe", "0123456789", "123 Street"))
    );

    var exception = await Assert.ThrowsAsync<WorkflowException>(async () => await handler.Handle(request,
      TestContext.Current.CancellationToken
    ));

    Assert.Equal("Đấu giá chưa kết thúc hoặc không có người chiến thắng", exception.Message);
  }
}
