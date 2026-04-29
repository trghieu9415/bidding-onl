using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using L2.Application.UseCases.Bids.Queries.GetBiddingActivity;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Bids;

public class GetBiddingActivityHandlerTests {
  [Fact]
  public async Task Handle_AppliesBidderCriteriaAndReturnsMeta() {
    var userId = Guid.NewGuid();
    var auctions = new List<AuctionDto> { new() { Id = Guid.NewGuid() } };
    var filter = new AuctionFilter { Page = 1, PerPage = 10 };
    var readRepo = new StubReadRepository<Auction, AuctionDto> { GetAsyncResult = (2, auctions) };
    var handler = new GetBiddingActivityHandler(readRepo);

    var result = await handler.Handle(new GetBiddingActivityQuery(userId, filter), TestContext.Current.CancellationToken);

    Assert.Equal(auctions, result.Auctions);
    Assert.Equal(Meta.Create(1, 10, 2), result.Meta);
    Assert.NotNull(readRepo.LastCriteria);

    var criteria = readRepo.LastCriteria!;
    var matchingAuction = Auction.Create(Guid.NewGuid(), Guid.NewGuid(), 100m, 10m, 200m);
    matchingAuction.Start();
    matchingAuction.PlaceBid(userId, "Bidder", 120m);
    var otherAuction = Auction.Create(Guid.NewGuid(), Guid.NewGuid(), 100m, 10m, 200m);
    otherAuction.Start();
    otherAuction.PlaceBid(Guid.NewGuid(), "Other", 120m);

    Assert.True(criteria.Compile()(matchingAuction));
    Assert.False(criteria.Compile()(otherAuction));
  }
}
