using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.Filters;
using Xunit;

namespace Tests.Unit.L2.Application.Filters;

public class UserFilterValidatorTests {
  private readonly UserFilterValidator _validator = new();

  [Theory]
  [InlineData(0)]
  [InlineData(101)]
  public void Should_Have_Error_When_PerPage_Is_Invalid(int perPage) {
    // Arrange
    var filter = new UserFilter { PerPage = perPage };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.PerPage);
  }

  [Fact]
  public void Should_Have_Error_When_Email_Is_Invalid() {
    // Arrange
    var filter = new UserFilter { Email = "invalid-email" };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Email)
      .WithErrorMessage("Email không hợp lệ.");
  }

  [Theory]
  [InlineData("123456")]
  [InlineData("01234567")]
  [InlineData("abcdefghijk")]
  public void Should_Have_Error_When_PhoneNumber_Is_Invalid(string phone) {
    // Arrange
    var filter = new UserFilter { PhoneNumber = phone };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
      .WithErrorMessage("Số điện thoại không hợp lệ.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Data_Is_Valid() {
    // Arrange
    var filter = new UserFilter {
      Page = 1,
      PerPage = 10,
      Email = "test@gmail.com",
      PhoneNumber = "0987654321"
    };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.IsValid.Should().BeTrue();
  }
}
