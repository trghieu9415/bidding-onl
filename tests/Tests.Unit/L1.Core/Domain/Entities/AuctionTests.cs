using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L1.Core.Domain.Bidding.Events;
using L1.Core.Exceptions;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.Entities;

public class AuctionTests {
  private static Auction CreateStandardAuction() {
    return Auction.Create(
      Guid.NewGuid(),
      Guid.NewGuid(),
      100m,
      10m,
      500m
    ).SetOwnerId(Guid.NewGuid());
  }

  [Fact]
  public void Create_ValidParameters_InitializesScheduledAuction() {
    var catalogItemId = Guid.NewGuid();
    var sessionId = Guid.NewGuid();

    var auction = Auction.Create(catalogItemId, sessionId, 100m, 10m, 500m);

    Assert.Equal(catalogItemId, auction.CatalogItemId);
    Assert.Equal(sessionId, auction.SessionId);
    Assert.Equal(100m, auction.CurrentPrice);
    Assert.Equal(10m, auction.Rules.StepPrice);
    Assert.Equal(500m, auction.Rules.ReservePrice);
    Assert.Equal(AuctionStatus.Scheduled, auction.Status);
    Assert.Empty(auction.Bids);
    Assert.Empty(auction.DomainEvents);
  }

  [Fact]
  public void SetOwnerId_AssignsOwnerAndReturnsSameAuction() {
    var auction = Auction.Create(Guid.NewGuid(), Guid.NewGuid(), 100m, 10m, 500m);
    var ownerId = Guid.NewGuid();

    var returnedAuction = auction.SetOwnerId(ownerId);

    Assert.Same(auction, returnedAuction);
    Assert.Equal(ownerId, auction.OwnerId);
  }

  [Fact]
  public void UpdateRules_WhenScheduled_UpdatesRules() {
    var auction = CreateStandardAuction();

    auction.UpdateRules(20m, 600m);

    Assert.Equal(20m, auction.Rules.StepPrice);
    Assert.Equal(600m, auction.Rules.ReservePrice);
  }

  [Fact]
  public void UpdateRules_WhenNotScheduled_ThrowsDomainException() {
    var auction = CreateStandardAuction();
    auction.Start();

    var exception = Assert.Throws<DomainException>(() => auction.UpdateRules(20m, 600m));

    Assert.Equal("Chỉ được sửa quy tắc khi đấu giá chưa bắt đầu.", exception.Message);
  }

  [Fact]
  public void Start_WhenScheduled_ChangesStatusAndRaisesEvent() {
    var auction = CreateStandardAuction();

    auction.Start();

    Assert.Equal(AuctionStatus.Active, auction.Status);
    var startedEvent = Assert.IsType<AuctionStartedEvent>(Assert.Single(auction.DomainEvents));
    Assert.Equal(auction.Id, startedEvent.AggregateId);
    Assert.Equal(auction.CatalogItemId, startedEvent.ItemId);
    Assert.Equal(auction.OwnerId, startedEvent.OwnerId);
  }

  [Fact]
  public void Start_WhenNotScheduled_ThrowsDomainException() {
    var auction = CreateStandardAuction();
    auction.Start();

    var exception = Assert.Throws<DomainException>(() => auction.Start());

    Assert.Equal("Phiên đấu giá không ở trạng thái được lên lịch", exception.Message);
  }

  [Fact]
  public void Cancel_WhenScheduled_ChangesStatusToCanceled() {
    var auction = CreateStandardAuction();

    auction.Cancel();

    Assert.Equal(AuctionStatus.Canceled, auction.Status);
  }

  [Fact]
  public void Cancel_WhenNotScheduled_ThrowsDomainException() {
    var auction = CreateStandardAuction();
    auction.Start();

    var exception = Assert.Throws<DomainException>(() => auction.Cancel());

    Assert.Equal("Không thể hủy phiên đấu giá đang diễn ra", exception.Message);
  }

  [Fact]
  public void PlaceBid_WhenNotActive_ThrowsDomainException() {
    var auction = CreateStandardAuction();

    var exception = Assert.Throws<DomainException>(() => auction.PlaceBid(Guid.NewGuid(), "Bidder", 100m));

    Assert.Equal("Chỉ có thể đặt giá khi đấu giá đang diễn ra.", exception.Message);
  }

  [Fact]
  public void PlaceBid_WhenFirstBidIsBelowCurrentPrice_ThrowsDomainException() {
    var auction = CreateStandardAuction();
    auction.Start();

    var exception = Assert.Throws<DomainException>(() => auction.PlaceBid(Guid.NewGuid(), "Bidder", 99m));

    Assert.Equal("Giá đặt phải tối thiểu là 100", exception.Message);
  }

  [Fact]
  public void PlaceBid_WhenFirstBidIsValid_AddsBidAndRaisesBidPlacedEvent() {
    var auction = CreateStandardAuction();
    auction.Start();
    auction.ClearEvents();
    var bidderId = Guid.NewGuid();

    auction.PlaceBid(bidderId, "Bidder A", 100m);

    var bid = Assert.Single(auction.Bids);
    Assert.Equal(100m, auction.CurrentPrice);
    Assert.Equal(bidderId, bid.BidderId);

    var bidPlacedEvent = Assert.IsType<BidPlacedEvent>(Assert.Single(auction.DomainEvents));
    Assert.Equal(auction.Id, bidPlacedEvent.AggregateId);
    Assert.Equal(bidderId, bidPlacedEvent.BidderId);
    Assert.Equal("Bidder A", bidPlacedEvent.BidderName);
    Assert.Equal(100m, bidPlacedEvent.Amount);
  }

  [Fact]
  public void PlaceBid_WhenAmountDoesNotMeetStepPrice_ThrowsDomainException() {
    var auction = CreateStandardAuction();
    auction.Start();
    auction.PlaceBid(Guid.NewGuid(), "Bidder A", 100m);

    var exception = Assert.Throws<DomainException>(() => auction.PlaceBid(Guid.NewGuid(), "Bidder B", 109m));

    Assert.Equal("Giá đặt phải tối thiểu là 110", exception.Message);
  }

  [Fact]
  public void PlaceBid_WhenDifferentBidderOutbids_RaisesBidPlacedAndOutbidEvents() {
    var auction = CreateStandardAuction();
    auction.Start();
    var firstBidderId = Guid.NewGuid();
    var secondBidderId = Guid.NewGuid();
    auction.PlaceBid(firstBidderId, "Bidder A", 100m);
    auction.ClearEvents();

    auction.PlaceBid(secondBidderId, "Bidder B", 120m);

    Assert.Equal(120m, auction.CurrentPrice);
    Assert.Equal(2, auction.Bids.Count);
    Assert.Equal(2, auction.DomainEvents.Count);

    Assert.Contains(auction.DomainEvents, domainEvent => domainEvent is BidPlacedEvent {
      BidderId: var bidderId,
      Amount: 120m,
      BidderName: "Bidder B"
    } && bidderId == secondBidderId);

    Assert.Contains(auction.DomainEvents, domainEvent => domainEvent is OutbidEvent {
      PreviousBidderId: var previousBidderId,
      NewPrice: 120m
    } && previousBidderId == firstBidderId);
  }

  [Fact]
  public void PlaceBid_WhenSameBidderIncreasesOwnBid_DoesNotRaiseOutbidEvent() {
    var auction = CreateStandardAuction();
    auction.Start();
    var bidderId = Guid.NewGuid();
    auction.PlaceBid(bidderId, "Bidder A", 100m);
    auction.ClearEvents();

    auction.PlaceBid(bidderId, "Bidder A", 120m);

    Assert.Single(auction.DomainEvents);
    Assert.IsType<BidPlacedEvent>(auction.DomainEvents.Single());
  }

  [Fact]
  public void End_WhenNotActive_ThrowsDomainException() {
    var auction = CreateStandardAuction();

    var exception = Assert.Throws<DomainException>(() => auction.End());

    Assert.Equal("Không thể kết thúc phiên đấu giá đang diễn ra.", exception.Message);
  }

  [Fact]
  public void End_WhenNoBids_MarksAuctionAsUnsoldAndRaisesEvent() {
    var auction = CreateStandardAuction();
    auction.Start();
    auction.ClearEvents();

    auction.End();

    Assert.Equal(AuctionStatus.EndedUnsold, auction.Status);
    Assert.Null(auction.WinningBidId);
    Assert.Null(auction.WinningAt);

    var endedEvent = Assert.IsType<AuctionEndedEvent>(Assert.Single(auction.DomainEvents));
    Assert.Equal(auction.Id, endedEvent.AggregateId);
    Assert.False(endedEvent.IsSold);
    Assert.Equal(auction.OwnerId, endedEvent.OwnerId);
    Assert.Null(endedEvent.WinnerId);
    Assert.Equal(100m, endedEvent.FinalPrice);
  }

  [Fact]
  public void End_WhenHighestBidDoesNotMeetReserve_MarksAuctionAsUnsold() {
    var auction = CreateStandardAuction();
    auction.Start();
    auction.PlaceBid(Guid.NewGuid(), "Bidder A", 300m);
    auction.ClearEvents();

    auction.End();

    Assert.Equal(AuctionStatus.EndedUnsold, auction.Status);
    Assert.Null(auction.WinningBidId);
    Assert.Null(auction.WinningAt);
    Assert.False(Assert.IsType<AuctionEndedEvent>(Assert.Single(auction.DomainEvents)).IsSold);
  }

  [Fact]
  public void End_WhenHighestBidMeetsReserve_MarksAuctionAsSoldAndStoresWinner() {
    var auction = CreateStandardAuction();
    auction.Start();
    auction.PlaceBid(Guid.NewGuid(), "Bidder A", 600m);
    auction.ClearEvents();
    var before = DateTime.UtcNow;

    auction.End();

    var after = DateTime.UtcNow;
    Assert.Equal(AuctionStatus.EndedSold, auction.Status);
    Assert.NotNull(auction.WinningBidId);
    Assert.NotNull(auction.WinningAt);
    Assert.InRange(auction.WinningAt!.Value, before, after);

    var topBid = auction.Bids.Single();
    Assert.Equal(topBid.Id, auction.WinningBidId);

    var endedEvent = Assert.IsType<AuctionEndedEvent>(Assert.Single(auction.DomainEvents));
    Assert.True(endedEvent.IsSold);
    Assert.Equal(auction.WinningBidId, endedEvent.WinnerId);
    Assert.Equal(600m, endedEvent.FinalPrice);
  }

  [Fact]
  public void Paid_WhenAuctionIsNotEndedSold_ThrowsDomainException() {
    var auction = CreateStandardAuction();

    var exception = Assert.Throws<DomainException>(() => auction.Paid(true));

    Assert.Equal("Cuộc đấu giá chưa ở trạng thái chờ thanh toán", exception.Message);
  }

  [Fact]
  public void Paid_WhenMarkedPaid_ChangesStatusToCompleted() {
    var auction = CreateStandardAuction();
    auction.Start();
    auction.PlaceBid(Guid.NewGuid(), "Bidder A", 600m);
    auction.End();

    auction.Paid(true);

    Assert.Equal(AuctionStatus.Completed, auction.Status);
  }

  [Fact]
  public void Paid_WhenMarkedUnpaid_ChangesStatusToCanceled() {
    var auction = CreateStandardAuction();
    auction.Start();
    auction.PlaceBid(Guid.NewGuid(), "Bidder A", 600m);
    auction.End();

    auction.Paid(false);

    Assert.Equal(AuctionStatus.Canceled, auction.Status);
  }
}
