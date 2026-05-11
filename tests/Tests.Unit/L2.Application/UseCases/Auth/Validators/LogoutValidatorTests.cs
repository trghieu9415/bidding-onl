using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.UseCases.Auth.Commands.Logout;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth.Validators;

public class LogoutValidatorTests {
  private readonly LogoutValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_RefreshToken_Is_Empty() {
    // Arrange
    var command = new LogoutCommand(string.Empty, true);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
      .WithErrorMessage("Refresh token không được để trống.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_RefreshToken_Is_Provided() {
    // Arrange
    var command = new LogoutCommand("valid-refresh-token", true);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.IsValid.Should().BeTrue();
  }
}
