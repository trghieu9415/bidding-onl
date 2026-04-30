using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.UseCases.Bids.Commands.PlaceBid;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Bids;

public class PlaceBidHandlerTests {
  [Fact]
  public async Task Handle_WhenAuctionMissing_ThrowsWorkflowException() {
    var handler = new PlaceBidHandler(new StubRepository<Auction>());

    var exception = await Assert.ThrowsAsync<WorkflowException>(async () =>
      await handler.Handle(
        new PlaceBidCommand(Guid.NewGuid(), Guid.NewGuid(), "Bidder", new PlaceBidRequest(100m)),
        TestContext.Current.CancellationToken
      ));

    Assert.Equal(404, exception.StatusCode);
    Assert.Equal("Cuộc đấu giá không tồn tại", exception.Message);
  }

  [Fact]
  public async Task Handle_WhenValid_AddsBidAndReturnsBidId() {
    var auction = Auction.Create(Guid.NewGuid(), Guid.NewGuid(), 100m, 10m, 200m);
    auction.Start();
    var repo = new StubRepository<Auction> { EntityByIdResult = auction };
    var bidderId = Guid.NewGuid();
    var handler = new PlaceBidHandler(repo);

    var result = await handler.Handle(new PlaceBidCommand(auction.Id, bidderId, "Bidder", new PlaceBidRequest(100m)),
      default);

    Assert.Same(auction, repo.UpdatedEntity);
    Assert.Equal(auction.Bids.Last().Id, result);
    Assert.Equal(bidderId, auction.Bids.Last().BidderId);
  }
}
