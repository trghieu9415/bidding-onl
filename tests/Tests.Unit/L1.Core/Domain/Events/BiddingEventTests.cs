using L1.Core.Domain.Bidding.Events;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.Events;

public class BiddingEventTests {
  [Fact]
  public void Events_ExposeExpectedAggregateIds() {
    var auctionId = Guid.NewGuid();
    var sessionId = Guid.NewGuid();

    var auctionEndedEvent = new AuctionEndedEvent(auctionId, Guid.NewGuid(), 500m, Guid.NewGuid(), true);
    var auctionStartedEvent = new AuctionStartedEvent(auctionId, Guid.NewGuid(), Guid.NewGuid());
    var bidPlacedEvent = new BidPlacedEvent(auctionId, Guid.NewGuid(), "Bidder", 500m);
    var outbidEvent = new OutbidEvent(auctionId, Guid.NewGuid(), 510m);
    var sessionPublishedEvent =
      new SessionPublishedEvent(sessionId, "Session", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));

    Assert.Equal(auctionId, auctionEndedEvent.AggregateId);
    Assert.Equal(auctionId, auctionStartedEvent.AggregateId);
    Assert.Equal(auctionId, bidPlacedEvent.AggregateId);
    Assert.Equal(auctionId, outbidEvent.AggregateId);
    Assert.Equal(sessionId, sessionPublishedEvent.AggregateId);
  }
}
