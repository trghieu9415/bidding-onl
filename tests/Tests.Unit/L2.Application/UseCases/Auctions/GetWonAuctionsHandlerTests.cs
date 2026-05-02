using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using L2.Application.UseCases.Auctions.Queries.GetWonAuctions;
using Tests.Unit.L2.Application.UseCases.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auctions;

public class GetWonAuctionsHandlerTests {
  [Fact]
  public async Task Handle_AppliesWinningCriteriaAndReturnsMeta() {
    var dto = new AuctionDto { Id = Guid.NewGuid() };
    var readRepo = new StubReadRepository<Auction, AuctionDto> { GetAsyncResult = (1, [dto]) };
    var filter = new AuctionFilter { Page = 1, PerPage = 10 };
    var userId = Guid.NewGuid();
    var handler = new GetWonAuctionsHandler(readRepo);

    var result = await handler.Handle(
      new GetWonAuctionsQuery(userId, filter),
      TestContext.Current.CancellationToken
    );

    Assert.Equal([dto], result.Auctions);
    Assert.Equal(Meta.Create(1, 10, 1), result.Meta);
    Assert.NotNull(readRepo.LastCriteria);

    var criteria = readRepo.LastCriteria!;
    Assert.True(criteria.Compile()(CreateSoldAuctionFor(userId)));
    Assert.False(criteria.Compile()(CreateSoldAuctionFor(Guid.NewGuid())));
  }

  private static Auction CreateSoldAuctionFor(Guid bidderId) {
    var auction = Auction.Create(Guid.NewGuid(), Guid.NewGuid(), 100m, 10m, 200m);
    auction.Start();
    auction.PlaceBid(bidderId, "Bidder", 250m);
    auction.End();
    return auction;
  }
}
