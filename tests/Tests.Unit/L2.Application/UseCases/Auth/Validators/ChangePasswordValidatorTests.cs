using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.UseCases.Auth.Commands.ChangePassword;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth.Validators;

public class ChangePasswordValidatorTests {
  private readonly ChangePasswordValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_NewPassword_Is_Same_As_OldPassword() {
    // Arrange
    var request = new ChangePasswordRequest("OldPass123", "OldPass123");

    // Act
    var result = _validator.TestValidate(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.NewPassword)
      .WithErrorMessage("Mật khẩu mới không được trùng với mật khẩu cũ.");
  }

  [Theory]
  [InlineData("short", "Mật khẩu mới phải có ít nhất 8 ký tự.")]
  [InlineData("noupper123", "Mật khẩu mới phải chứa ít nhất 1 ký tự in hoa.")]
  [InlineData("NOLOWER123", "Mật khẩu mới phải chứa ít nhất 1 ký tự thường.")]
  [InlineData("NoNumber", "Mật khẩu mới phải chứa ít nhất 1 chữ số.")]
  public void Should_Have_Error_When_NewPassword_Is_Weak(string weakPass, string expectedMessage) {
    // Arrange
    var request = new ChangePasswordRequest("OldPass123", weakPass);

    // Act
    var result = _validator.TestValidate(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.NewPassword)
      .WithErrorMessage(expectedMessage);
  }

  [Fact]
  public void Should_Not_Have_Error_When_Request_Is_Valid() {
    // Arrange
    var request = new ChangePasswordRequest("OldPass123", "NewPass123");

    // Act
    var result = _validator.TestValidate(request);

    // Assert
    result.IsValid.Should().BeTrue();
  }
}
