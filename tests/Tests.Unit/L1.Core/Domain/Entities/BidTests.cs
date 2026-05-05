using FluentAssertions;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.Entities;

public class BidTests {
  [Fact]
  public void Create_ValidParameters_InitializesBid() {
    // Arrange
    var auction = new AuctionBuilder().Build();
    var bidderId = Guid.NewGuid();
    var builder = new BidBuilder()
      .WithAuction(auction)
      .WithBidderId(bidderId)
      .WithAmount(120m);
    var before = DateTime.UtcNow;

    // Act
    var bid = builder.Build();

    // Assert
    var after = DateTime.UtcNow;
    bid.AuctionId.Should().Be(auction.Id);
    bid.Auction.Should().BeSameAs(auction);
    bid.BidderId.Should().Be(bidderId);
    bid.Amount.Should().Be(120m);
    bid.TimePoint.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
  }
}
