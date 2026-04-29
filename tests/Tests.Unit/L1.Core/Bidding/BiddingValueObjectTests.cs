using L1.Core.Domain.Bidding.ValueObjects;
using L1.Core.Exceptions;
using Xunit;

namespace Tests.Unit.L1.Core.Bidding;

public class BiddingValueObjectTests {
  [Fact]
  public void AuctionRules_RecordUsesValueEquality() {
    var left = new AuctionRules(10m, 500m);
    var right = new AuctionRules(10m, 500m);

    Assert.Equal(10m, left.StepPrice);
    Assert.Equal(500m, left.ReservePrice);
    Assert.Equal(left, right);
  }

  [Fact]
  public void AuctionTimeFrame_RecordStoresStartAndEndTime() {
    var startTime = DateTime.UtcNow.AddHours(1);
    var endTime = DateTime.UtcNow.AddHours(2);

    var timeFrame = new AuctionTimeFrame(startTime, endTime);

    Assert.Equal(startTime, timeFrame.StartTime);
    Assert.Equal(endTime, timeFrame.EndTime);
  }

  [Fact]
  public void AuctionTimeFrame_Validate_WithValidRange_DoesNotThrow() {
    var exception = Record.Exception(() => AuctionTimeFrame.Validate(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2)));

    Assert.Null(exception);
  }

  [Fact]
  public void AuctionTimeFrame_Validate_WhenStartIsNotBeforeEnd_ThrowsDomainException() {
    var timePoint = DateTime.UtcNow.AddHours(1);

    var exception = Assert.Throws<DomainException>(() => AuctionTimeFrame.Validate(timePoint, timePoint));

    Assert.Equal("Thời gian bắt đầu phải trước thời gian kết thúc.", exception.Message);
  }

  [Fact]
  public void AuctionTimeFrame_Validate_WhenEndIsInPast_ThrowsDomainException() {
    var exception = Assert.Throws<DomainException>(() => AuctionTimeFrame.Validate(DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1)));

    Assert.Equal("Thời gian kết thúc không được ở trong quá khứ.", exception.Message);
  }
}
