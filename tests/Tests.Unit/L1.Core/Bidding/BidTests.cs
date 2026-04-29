using L1.Core.Domain.Bidding.Entities;
using Xunit;

namespace Tests.Unit.L1.Core.Bidding;

public class BidTests {
  [Fact]
  public void Create_ValidParameters_InitializesBid() {
    var auction = Auction.Create(Guid.NewGuid(), Guid.NewGuid(), 100m, 10m, 500m);
    var bidderId = Guid.NewGuid();
    var before = DateTime.UtcNow;

    var bid = Bid.Create(auction, bidderId, 120m);

    var after = DateTime.UtcNow;
    Assert.Equal(auction.Id, bid.AuctionId);
    Assert.Same(auction, bid.Auction);
    Assert.Equal(bidderId, bid.BidderId);
    Assert.Equal(120m, bid.Amount);
    Assert.InRange(bid.TimePoint, before, after);
  }
}
