using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.Filters;
using Xunit;

namespace Tests.Unit.L2.Application.Filters;

public class CategoryFilterValidatorTests {
  private readonly CategoryFilterValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_Name_Exceeds_MaxLength() {
    // Arrange
    var filter = new CategoryFilter { Name = new string('x', 201) };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Name)
      .WithErrorMessage("Tên danh mục không được vượt quá 200 ký tự.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Name_Is_Valid() {
    // Arrange
    var filter = new CategoryFilter { Name = "Điện thoại" };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.IsValid.Should().BeTrue();
  }
}
