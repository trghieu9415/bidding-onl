using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.UseCases.Auctions.Commands.AddAuction;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auctions.Validators;

public class AddAuctionValidatorTests {
  private readonly AddAuctionValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_Ids_Are_Empty() {
    // Arrange
    var command = new AddAuctionCommand(Guid.Empty, Guid.Empty, 100, 1000);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CatalogItemId)
      .WithErrorMessage("Id sản phẩm không được để trống.");
    result.ShouldHaveValidationErrorFor(x => x.SessionId)
      .WithErrorMessage("Id phiên đấu giá không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_ReservePrice_Is_Less_Than_StepPrice() {
    // Arrange
    var command = new AddAuctionCommand(Guid.NewGuid(), Guid.NewGuid(), 500, 200);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ReservePrice)
      .WithErrorMessage("Giá khởi điểm phải lớn hơn hoặc bằng giá bước.");
  }

  [Theory]
  [InlineData(0, 100)]
  [InlineData(100, 0)]
  public void Should_Have_Error_When_Prices_Are_Not_Positive(decimal step, decimal reserve) {
    // Arrange
    var command = new AddAuctionCommand(Guid.NewGuid(), Guid.NewGuid(), step, reserve);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    if (step <= 0) {
      result.ShouldHaveValidationErrorFor(x => x.StepPrice);
    }

    if (reserve <= 0) {
      result.ShouldHaveValidationErrorFor(x => x.ReservePrice);
    }
  }

  [Fact]
  public void Should_Not_Have_Error_When_Command_Is_Valid() {
    // Arrange
    var command = new AddAuctionCommand(Guid.NewGuid(), Guid.NewGuid(), 100, 1000);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.IsValid.Should().BeTrue();
  }
}
