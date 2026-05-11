using FluentValidation.TestHelper;
using L2.Application.UseCases.Auth.Commands.ResetPassword;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth.Validators;

public class ResetPasswordValidatorTests {
  private readonly ResetPasswordValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_Token_Is_Empty() {
    // Arrange
    var command = new ResetPasswordCommand("test@gmail.com", "", "NewPass123");

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Token)
      .WithErrorMessage("Token đặt lại mật khẩu không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_NewPassword_Is_Weak() {
    // Arrange
    var command = new ResetPasswordCommand("test@gmail.com", "valid-token", "123");

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.NewPassword);
  }
}
