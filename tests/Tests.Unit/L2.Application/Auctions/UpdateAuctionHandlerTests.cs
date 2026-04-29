using L1.Core.Domain.Bidding.Entities;
using L2.Application.UseCases.Auctions.Commands.UpdateAuction;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Auctions;

public class UpdateAuctionHandlerTests {
  [Fact]
  public async Task Handle_WhenAuctionExists_UpdatesRulesAndPersists() {
    var auction = Auction.Create(Guid.NewGuid(), Guid.NewGuid(), 100m, 10m, 200m);
    var repo = new StubRepository<Auction> { EntityByIdResult = auction };
    var handler = new UpdateAuctionHandler(repo);

    var result = await handler.Handle(new UpdateAuctionCommand(auction.Id, new UpdateAuctionRequest(25m, 300m)),
      TestContext.Current.CancellationToken);

    Assert.True(result);
    Assert.Same(auction, repo.UpdatedEntity);
    Assert.Equal(25m, auction.Rules.StepPrice);
    Assert.Equal(300m, auction.Rules.ReservePrice);
  }
}
