using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.UseCases.Auctions.Commands.UpdateAuction;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auctions.Validators;

public class UpdateAuctionValidatorTests {
  private readonly UpdateAuctionValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_ReservePrice_Is_Less_Than_StepPrice() {
    // Arrange
    var request = new UpdateAuctionRequest(500, 200);

    // Act
    var result = _validator.TestValidate(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ReservePrice)
      .WithErrorMessage("Giá khởi điểm phải lớn hơn hoặc bằng giá bước.");
  }

  [Theory]
  [InlineData(-1, 100)]
  [InlineData(100, -5)]
  public void Should_Have_Error_When_Prices_Are_Not_Positive(decimal step, decimal reserve) {
    // Arrange
    var request = new UpdateAuctionRequest(step, reserve);

    // Act
    var result = _validator.TestValidate(request);

    // Assert
    if (step <= 0) {
      result.ShouldHaveValidationErrorFor(x => x.StepPrice);
    }

    if (reserve <= 0) {
      result.ShouldHaveValidationErrorFor(x => x.ReservePrice);
    }
  }

  [Fact]
  public void Should_Not_Have_Error_When_Request_Is_Valid() {
    // Arrange
    var request = new UpdateAuctionRequest(100, 500);

    // Act
    var result = _validator.TestValidate(request);

    // Assert
    result.IsValid.Should().BeTrue();
  }
}
