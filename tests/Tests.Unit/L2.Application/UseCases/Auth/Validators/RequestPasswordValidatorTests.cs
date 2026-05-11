using FluentValidation.TestHelper;
using L2.Application.UseCases.Auth.Commands.RequestPassword;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth.Validators;

public class RequestPasswordValidatorTests {
  private readonly RequestPasswordValidator _validator = new();

  [Theory]
  [InlineData("")]
  public void Should_Have_Error_When_Email_Is_Empty(string email) {
    // Arrange
    var command = new RequestPasswordCommand(email);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Email)
      .WithErrorMessage("Email không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_Email_Is_Invalid() {
    // Arrange
    var command = new RequestPasswordCommand("invalid-email-format");

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Email)
      .WithErrorMessage("Email không hợp lệ.");
  }
}
