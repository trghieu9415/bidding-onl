using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.UseCases.Auctions.Commands.AddAuction;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Auctions;

public class AddAuctionHandlerTests {
  [Fact]
  public async Task Handle_WhenItemApproved_CreatesAuctionFromItemData() {
    var catalogItemId = Guid.NewGuid();
    var sessionId = Guid.NewGuid();
    var ownerId = Guid.NewGuid();
    var itemRepo = new StubReadRepository<CatalogItem, CatalogItemDto> {
      EntityByIdResult = new CatalogItemDto {
        Id = catalogItemId,
        OwnerId = ownerId,
        Status = ItemStatus.Approval,
        StartingPrice = 150m,
        Name = "Laptop",
        Description = "Gaming laptop"
      }
    };
    var auctionRepo = new StubRepository<Auction> { CreateResult = Guid.NewGuid() };
    var handler = new AddAuctionHandler(auctionRepo, itemRepo);

    var result = await handler.Handle(new AddAuctionCommand(catalogItemId, sessionId, 10m, 200m), TestContext.Current.CancellationToken);

    Assert.Equal(auctionRepo.CreateResult, result);
    var createdAuction = Assert.IsType<Auction>(auctionRepo.CreatedEntity);
    Assert.Equal(catalogItemId, createdAuction.CatalogItemId);
    Assert.Equal(sessionId, createdAuction.SessionId);
    Assert.Equal(150m, createdAuction.CurrentPrice);
    Assert.Equal(10m, createdAuction.Rules.StepPrice);
    Assert.Equal(200m, createdAuction.Rules.ReservePrice);
    Assert.Equal(ownerId, createdAuction.OwnerId);
  }

  [Fact]
  public async Task Handle_WhenItemMissing_ThrowsWorkflowException() {
    var handler = new AddAuctionHandler(
      new StubRepository<Auction>(),
      new StubReadRepository<CatalogItem, CatalogItemDto>()
    );
    var catalogItemId = Guid.NewGuid();

    var exception = await Assert.ThrowsAsync<WorkflowException>(async () =>
      await handler.Handle(new AddAuctionCommand(catalogItemId, Guid.NewGuid(), 10m, 200m), default)
    );

    Assert.Equal(404, exception.StatusCode);
    Assert.Contains(catalogItemId.ToString(), exception.Message);
  }

  [Fact]
  public async Task Handle_WhenItemNotApproved_ThrowsWorkflowException() {
    var itemRepo = new StubReadRepository<CatalogItem, CatalogItemDto> {
      EntityByIdResult = new CatalogItemDto {
        Id = Guid.NewGuid(),
        Status = ItemStatus.Pending,
        StartingPrice = 150m
      }
    };
    var handler = new AddAuctionHandler(new StubRepository<Auction>(), itemRepo);

    var exception = await Assert.ThrowsAsync<WorkflowException>(async () =>
      await handler.Handle(new AddAuctionCommand(Guid.NewGuid(), Guid.NewGuid(), 10m, 200m), default)
    );

    Assert.Equal(400, exception.StatusCode);
    Assert.Contains("chưa được phê duyệt", exception.Message);
  }
}
