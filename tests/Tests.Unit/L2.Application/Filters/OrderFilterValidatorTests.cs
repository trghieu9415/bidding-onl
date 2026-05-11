using FluentValidation.TestHelper;
using L2.Application.Filters;
using Xunit;

namespace Tests.Unit.L2.Application.Filters;

public class OrderFilterValidatorTests {
  private readonly OrderFilterValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_MinPrice_Is_Greater_Than_MaxPrice() {
    // Arrange
    var filter = new OrderFilter { MinPrice = 500, MaxPrice = 100 };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrors()
      .WithErrorMessage("Giá tối thiểu không được lớn hơn giá tối đa.");
  }

  [Fact]
  public void Should_Have_Error_When_Prices_Are_Negative() {
    // Arrange
    var filter = new OrderFilter { MinPrice = -10, MaxPrice = -5 };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.MinPrice);
    result.ShouldHaveValidationErrorFor(x => x.MaxPrice);
  }

  [Fact]
  public void Should_Have_Error_When_BidderEmail_Is_Invalid() {
    // Arrange
    var filter = new OrderFilter { BidderEmail = "wrong_email" };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.BidderEmail);
  }
}
