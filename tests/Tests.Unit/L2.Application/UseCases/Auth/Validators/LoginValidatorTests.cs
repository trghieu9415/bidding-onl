using FluentValidation.TestHelper;
using L2.Application.UseCases.Auth.Commands.Login;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth.Validators;

public class LoginValidatorTests {
  private readonly LoginValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_Fields_Are_Empty() {
    // Arrange
    var request = new LoginRequest("", "");

    // Act
    var result = _validator.TestValidate(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Email);
    result.ShouldHaveValidationErrorFor(x => x.Password);
  }

  [Fact]
  public void Should_Have_Error_When_Email_Is_Invalid() {
    // Arrange
    var request = new LoginRequest("wrong-email", "Password123");

    // Act
    var result = _validator.TestValidate(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Email)
      .WithErrorMessage("Email không hợp lệ.");
  }
}
