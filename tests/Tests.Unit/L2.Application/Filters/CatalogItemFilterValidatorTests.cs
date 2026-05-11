using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.Filters;
using Xunit;

namespace Tests.Unit.L2.Application.Filters;

public class CatalogItemFilterValidatorTests {
  private readonly CatalogItemFilterValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_MinStartingPrice_Is_Greater_Than_MaxStartingPrice() {
    // Arrange
    var filter = new CatalogItemFilter { MinStartingPrice = 1000, MaxStartingPrice = 500 };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrors()
      .WithErrorMessage("Giá khởi điểm tối thiểu không được lớn hơn giá khởi điểm tối đa.");
  }

  [Fact]
  public void Should_Have_Error_When_Description_Exceeds_MaxLength() {
    // Arrange
    var filter = new CatalogItemFilter { Description = new string('s', 1001) };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Description)
      .WithErrorMessage("Mô tả sản phẩm không được vượt quá 1000 ký tự.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_StartingPrices_Are_Valid() {
    // Arrange
    var filter = new CatalogItemFilter { MinStartingPrice = 0, MaxStartingPrice = 1000000 };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.IsValid.Should().BeTrue();
  }
}
