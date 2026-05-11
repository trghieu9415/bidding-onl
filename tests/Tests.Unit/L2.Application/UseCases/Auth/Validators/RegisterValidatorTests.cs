using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.UseCases.Auth.Commands.Register;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth.Validators;

public class RegisterValidatorTests {
  private readonly RegisterValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_FullName_Exceeds_MaxLength() {
    // Arrange
    var command = new RegisterCommand("test@gmail.com", new string('a', 201), "Pass1234", "0987654321");

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.FullName)
      .WithErrorMessage("Họ và tên không được vượt quá 200 ký tự.");
  }

  [Theory]
  [InlineData("12345678")]
  [InlineData("098765432")]
  [InlineData("not-a-phone")]
  public void Should_Have_Error_When_PhoneNumber_Is_Invalid(string phone) {
    // Arrange
    var command = new RegisterCommand("test@gmail.com", "User", "SecurePass1", phone);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
      .WithErrorMessage("Số điện thoại không hợp lệ.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Register_Data_Is_Valid() {
    // Arrange
    var command = new RegisterCommand("valid@gmail.com", "Trg. Hiếu", "Admin123", "0912345678");

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.IsValid.Should().BeTrue();
  }
}
