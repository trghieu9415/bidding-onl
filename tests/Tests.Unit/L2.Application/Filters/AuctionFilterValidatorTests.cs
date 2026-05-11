using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.Filters;
using Xunit;

namespace Tests.Unit.L2.Application.Filters;

public class AuctionFilterValidatorTests {
  private readonly AuctionFilterValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_MinPrice_Is_Greater_Than_MaxPrice() {
    // Arrange
    var filter = new AuctionFilter { MinPrice = 200, MaxPrice = 100 };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrors()
      .WithErrorMessage("Giá tối thiểu không được lớn hơn giá tối đa.");
  }

  [Fact]
  public void Should_Have_Error_When_MinPrice_Is_Negative() {
    // Arrange
    var filter = new AuctionFilter { MinPrice = -1 };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.MinPrice)
      .WithErrorMessage("Giá tối thiểu không được nhỏ hơn 0.");
  }

  [Fact]
  public void Should_Have_Error_When_MinWinningAt_Is_Greater_Than_MaxWinningAt() {
    // Arrange
    var filter = new AuctionFilter {
      MinWinningAt = DateTime.Now.AddDays(1),
      MaxWinningAt = DateTime.Now
    };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrors()
      .WithErrorMessage("Thời gian thắng tối thiểu không được lớn hơn thời gian thắng tối đa.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Data_Is_Valid() {
    // Arrange
    var filter = new AuctionFilter {
      MinPrice = 10,
      MaxPrice = 100,
      MinWinningAt = DateTime.Now,
      MaxWinningAt = DateTime.Now.AddHours(1)
    };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.IsValid.Should().BeTrue();
  }
}
