using FluentAssertions;
using L1.Core.Domain.Bidding.Enums;
using L1.Core.Domain.Bidding.Events;
using L1.Core.Exceptions;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.Entities;

public class AuctionTests {
  [Fact]
  public void Create_ValidParameters_InitializesScheduledAuction() {
    // Arrange
    var catalogItemId = Guid.NewGuid();
    var sessionId = Guid.NewGuid();
    var builder = new AuctionBuilder()
      .WithCatalogItemId(catalogItemId)
      .WithSessionId(sessionId)
      .WithPrices(100m, 10m, 500m)
      .WithOwnerId(null);

    // Act
    var auction = builder.Build();

    // Assert
    auction.CatalogItemId.Should().Be(catalogItemId);
    auction.SessionId.Should().Be(sessionId);
    auction.CurrentPrice.Should().Be(100m);
    auction.Rules.StepPrice.Should().Be(10m);
    auction.Rules.ReservePrice.Should().Be(500m);
    auction.Status.Should().Be(AuctionStatus.Scheduled);
    auction.Bids.Should().BeEmpty();
    auction.DomainEvents.Should().BeEmpty();
  }

  [Fact]
  public void SetOwnerId_AssignsOwnerAndReturnsSameAuction() {
    // Arrange
    var auction = new AuctionBuilder().WithOwnerId(null).Build();
    var ownerId = Guid.NewGuid();

    // Act
    var returnedAuction = auction.SetOwnerId(ownerId);

    // Assert
    returnedAuction.Should().BeSameAs(auction);
    auction.OwnerId.Should().Be(ownerId);
  }

  [Fact]
  public void UpdateRules_WhenScheduled_UpdatesRules() {
    // Arrange
    var auction = new AuctionBuilder().Build();

    // Act
    auction.UpdateRules(20m, 600m);

    // Assert
    auction.Rules.StepPrice.Should().Be(20m);
    auction.Rules.ReservePrice.Should().Be(600m);
  }

  [Fact]
  public void UpdateRules_WhenNotScheduled_ThrowsDomainException() {
    // Arrange
    var auction = new AuctionBuilder().Build();
    auction.Start();

    // Act
    var act = () => auction.UpdateRules(20m, 600m);

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Chỉ được sửa quy tắc khi đấu giá chưa bắt đầu.");
  }

  [Fact]
  public void Start_WhenScheduled_ChangesStatusAndRaisesEvent() {
    // Arrange
    var auction = new AuctionBuilder().Build();

    // Act
    auction.Start();

    // Assert
    auction.Status.Should().Be(AuctionStatus.Active);
    var startedEvent = auction.DomainEvents.Should().ContainSingle().Subject.As<AuctionStartedEvent>();
    startedEvent.AggregateId.Should().Be(auction.Id);
    startedEvent.ItemId.Should().Be(auction.CatalogItemId);
    startedEvent.OwnerId.Should().Be(auction.OwnerId);
  }

  [Fact]
  public void Start_WhenNotScheduled_ThrowsDomainException() {
    // Arrange
    var auction = new AuctionBuilder().Build();
    auction.Start();

    // Act
    var act = () => auction.Start();

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Phiên đấu giá không ở trạng thái được lên lịch");
  }

  [Fact]
  public void Cancel_WhenScheduled_ChangesStatusToCanceled() {
    // Arrange
    var auction = new AuctionBuilder().Build();

    // Act
    auction.Cancel();

    // Assert
    auction.Status.Should().Be(AuctionStatus.Canceled);
  }

  [Fact]
  public void Cancel_WhenNotScheduled_ThrowsDomainException() {
    // Arrange
    var auction = new AuctionBuilder().Build();
    auction.Start();

    // Act
    var act = () => auction.Cancel();

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Không thể hủy phiên đấu giá đang diễn ra");
  }

  [Fact]
  public void PlaceBid_WhenNotActive_ThrowsDomainException() {
    // Arrange
    var auction = new AuctionBuilder().Build();

    // Act
    Action act = () => auction.PlaceBid(Guid.NewGuid(), "Bidder", 100m);

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Chỉ có thể đặt giá khi đấu giá đang diễn ra.");
  }

  [Fact]
  public void PlaceBid_WhenFirstBidIsBelowCurrentPrice_ThrowsDomainException() {
    // Arrange
    var auction = new AuctionBuilder().WithPrices(100m, 10m, 500m).Build();
    auction.Start();

    // Act
    Action act = () => auction.PlaceBid(Guid.NewGuid(), "Bidder", 99m);

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Giá đặt phải tối thiểu là 100");
  }

  [Fact]
  public void PlaceBid_WhenFirstBidIsValid_AddsBidAndRaisesBidPlacedEvent() {
    // Arrange
    var auction = new AuctionBuilder().WithPrices(100m, 10m, 500m).Build();
    auction.Start();
    auction.ClearEvents();
    var bidderId = Guid.NewGuid();

    // Act
    auction.PlaceBid(bidderId, "Bidder A", 100m);

    // Assert
    var bid = auction.Bids.Should().ContainSingle().Subject;
    auction.CurrentPrice.Should().Be(100m);
    bid.BidderId.Should().Be(bidderId);

    var bidPlacedEvent = auction.DomainEvents.Should().ContainSingle().Subject.As<BidPlacedEvent>();
    bidPlacedEvent.AggregateId.Should().Be(auction.Id);
    bidPlacedEvent.BidderId.Should().Be(bidderId);
    bidPlacedEvent.BidderName.Should().Be("Bidder A");
    bidPlacedEvent.Amount.Should().Be(100m);
  }

  [Fact]
  public void PlaceBid_WhenAmountDoesNotMeetStepPrice_ThrowsDomainException() {
    // Arrange
    var auction = new AuctionBuilder().WithPrices(100m, 10m, 500m).Build();
    auction.Start();
    auction.PlaceBid(Guid.NewGuid(), "Bidder A", 100m);

    // Act
    Action act = () => auction.PlaceBid(Guid.NewGuid(), "Bidder B", 109m);

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Giá đặt phải tối thiểu là 110");
  }

  [Fact]
  public void PlaceBid_WhenDifferentBidderOutbids_RaisesBidPlacedAndOutbidEvents() {
    // Arrange
    var auction = new AuctionBuilder().WithPrices(100m, 10m, 500m).Build();
    auction.Start();
    var firstBidderId = Guid.NewGuid();
    var secondBidderId = Guid.NewGuid();
    auction.PlaceBid(firstBidderId, "Bidder A", 100m);
    auction.ClearEvents();

    // Act
    auction.PlaceBid(secondBidderId, "Bidder B", 120m);

    // Assert
    auction.CurrentPrice.Should().Be(120m);
    auction.Bids.Should().HaveCount(2);
    auction.DomainEvents.Should().HaveCount(2);

    auction.DomainEvents.OfType<BidPlacedEvent>().Should().ContainSingle(e =>
      e.BidderId == secondBidderId &&
      e.Amount == 120m &&
      e.BidderName == "Bidder B"
    );

    auction.DomainEvents.OfType<OutbidEvent>().Should().ContainSingle(e =>
      e.PreviousBidderId == firstBidderId &&
      e.NewPrice == 120m
    );
  }

  [Fact]
  public void PlaceBid_WhenSameBidderIncreasesOwnBid_DoesNotRaiseOutbidEvent() {
    // Arrange
    var auction = new AuctionBuilder().WithPrices(100m, 10m, 500m).Build();
    auction.Start();
    var bidderId = Guid.NewGuid();
    auction.PlaceBid(bidderId, "Bidder A", 100m);
    auction.ClearEvents();

    // Act
    auction.PlaceBid(bidderId, "Bidder A", 120m);

    // Assert
    auction.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<BidPlacedEvent>();
    auction.DomainEvents.OfType<OutbidEvent>().Should().BeEmpty();
  }

  [Fact]
  public void End_WhenNotActive_ThrowsDomainException() {
    // Arrange
    var auction = new AuctionBuilder().Build();

    // Act
    var act = () => auction.End();

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Không thể kết thúc phiên đấu giá đang diễn ra.");
  }

  [Fact]
  public void End_WhenNoBids_MarksAuctionAsUnsoldAndRaisesEvent() {
    // Arrange
    var auction = new AuctionBuilder().WithPrices(100m, 10m, 500m).Build();
    auction.Start();
    auction.ClearEvents();

    // Act
    auction.End();

    // Assert
    auction.Status.Should().Be(AuctionStatus.EndedUnsold);
    auction.WinningBidId.Should().BeNull();
    auction.WinningAt.Should().BeNull();

    var endedEvent = auction.DomainEvents.Should().ContainSingle().Subject.As<AuctionEndedEvent>();
    endedEvent.AggregateId.Should().Be(auction.Id);
    endedEvent.IsSold.Should().BeFalse();
    endedEvent.OwnerId.Should().Be(auction.OwnerId);
    endedEvent.WinnerId.Should().BeNull();
    endedEvent.FinalPrice.Should().Be(100m);
  }

  [Fact]
  public void End_WhenHighestBidDoesNotMeetReserve_MarksAuctionAsUnsold() {
    // Arrange
    var auction = new AuctionBuilder().WithPrices(100m, 10m, 500m).Build();
    auction.Start();
    auction.PlaceBid(Guid.NewGuid(), "Bidder A", 300m);
    auction.ClearEvents();

    // Act
    auction.End();

    // Assert
    auction.Status.Should().Be(AuctionStatus.EndedUnsold);
    auction.WinningBidId.Should().BeNull();
    auction.WinningAt.Should().BeNull();
    auction.DomainEvents.Should().ContainSingle().Subject.As<AuctionEndedEvent>().IsSold.Should().BeFalse();
  }

  [Fact]
  public void End_WhenHighestBidMeetsReserve_MarksAuctionAsSoldAndStoresWinner() {
    // Arrange
    var auction = new AuctionBuilder().WithPrices(100m, 10m, 500m).Build();
    auction.Start();
    auction.PlaceBid(Guid.NewGuid(), "Bidder A", 600m);
    auction.ClearEvents();
    var before = DateTime.UtcNow;

    // Act
    auction.End();

    // Assert
    var after = DateTime.UtcNow;
    auction.Status.Should().Be(AuctionStatus.EndedSold);
    auction.WinningBidId.Should().NotBeNull();
    auction.WinningAt.Should().NotBeNull();
    auction.WinningAt!.Value.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);

    var topBid = auction.Bids.Should().ContainSingle().Subject;
    topBid.Id.Should().Be(auction.WinningBidId.Value);

    var endedEvent = auction.DomainEvents.Should().ContainSingle().Subject.As<AuctionEndedEvent>();
    endedEvent.IsSold.Should().BeTrue();
    endedEvent.WinnerId.Should().Be(auction.WinningBidId);
    endedEvent.FinalPrice.Should().Be(600m);
  }

  [Fact]
  public void Paid_WhenAuctionIsNotEndedSold_ThrowsDomainException() {
    // Arrange
    var auction = new AuctionBuilder().Build();

    // Act
    var act = () => auction.Paid(true);

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Cuộc đấu giá chưa ở trạng thái chờ thanh toán");
  }

  [Fact]
  public void Paid_WhenMarkedPaid_ChangesStatusToCompleted() {
    // Arrange
    var auction = new AuctionBuilder().WithPrices(100m, 10m, 500m).Build();
    auction.Start();
    auction.PlaceBid(Guid.NewGuid(), "Bidder A", 600m);
    auction.End();

    // Act
    auction.Paid(true);

    // Assert
    auction.Status.Should().Be(AuctionStatus.Completed);
  }

  [Fact]
  public void Paid_WhenMarkedUnpaid_ChangesStatusToCanceled() {
    // Arrange
    var auction = new AuctionBuilder().WithPrices(100m, 10m, 500m).Build();
    auction.Start();
    auction.PlaceBid(Guid.NewGuid(), "Bidder A", 600m);
    auction.End();

    // Act
    auction.Paid(false);

    // Assert
    auction.Status.Should().Be(AuctionStatus.Canceled);
  }
}
