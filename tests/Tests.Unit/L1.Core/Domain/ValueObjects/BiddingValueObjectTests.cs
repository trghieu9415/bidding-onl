using FluentAssertions;
using L1.Core.Domain.Bidding.ValueObjects;
using L1.Core.Exceptions;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.ValueObjects;

public class BiddingValueObjectTests {
  [Fact]
  public void AuctionRules_RecordUsesValueEquality() {
    // Arrange
    var left = new AuctionRules(10m, 500m);
    var right = new AuctionRules(10m, 500m);

    // Act
    var isEqual = left.Equals(right);

    // Assert
    left.StepPrice.Should().Be(10m);
    left.ReservePrice.Should().Be(500m);
    isEqual.Should().BeTrue();
    left.Should().Be(right);
  }

  [Fact]
  public void AuctionTimeFrame_RecordStoresStartAndEndTime() {
    // Arrange
    var startTime = DateTime.UtcNow.AddHours(1);
    var endTime = DateTime.UtcNow.AddHours(2);

    // Act
    var timeFrame = new AuctionTimeFrame(startTime, endTime);

    // Assert
    timeFrame.StartTime.Should().Be(startTime);
    timeFrame.EndTime.Should().Be(endTime);
  }

  [Fact]
  public void AuctionTimeFrame_Validate_WithValidRange_DoesNotThrow() {
    // Arrange
    var startTime = DateTime.UtcNow.AddHours(1);
    var endTime = DateTime.UtcNow.AddHours(2);

    // Act
    var act = () => AuctionTimeFrame.Validate(startTime, endTime);

    // Assert
    act.Should().NotThrow();
  }

  [Fact]
  public void AuctionTimeFrame_Validate_WhenStartIsNotBeforeEnd_ThrowsDomainException() {
    // Arrange
    var timePoint = DateTime.UtcNow.AddHours(1);

    // Act
    var act = () => AuctionTimeFrame.Validate(timePoint, timePoint);

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Thời gian bắt đầu phải trước thời gian kết thúc.");
  }

  [Fact]
  public void AuctionTimeFrame_Validate_WhenEndIsInPast_ThrowsDomainException() {
    // Arrange
    var startTime = DateTime.UtcNow.AddHours(-2);
    var endTime = DateTime.UtcNow.AddHours(-1);

    // Act
    var act = () => AuctionTimeFrame.Validate(startTime, endTime);

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Thời gian kết thúc không được ở trong quá khứ.");
  }
}
