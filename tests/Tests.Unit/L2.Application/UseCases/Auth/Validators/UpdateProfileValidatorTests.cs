using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.Models;
using L2.Application.UseCases.Auth.Commands.UpdateProfile;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auth.Validators;

public class UpdateProfileValidatorTests {
  private readonly UpdateProfileValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_UserId_Is_Empty() {
    // Arrange
    var request = new UpdateProfileRequest("Trg. Hiếu", "0912345678", "https://github.com/hieu");
    var command = new UpdateProfileCommand(Guid.Empty, UserRole.Bidder, request);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.UserId)
      .WithErrorMessage("Id người dùng không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_Data_Is_Null() {
    // Arrange
    var command = new UpdateProfileCommand(Guid.NewGuid(), UserRole.Bidder, null!);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Data)
      .WithErrorMessage("Thông tin cập nhật không được để trống.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Command_Is_Valid() {
    // Arrange
    var request = new UpdateProfileRequest("Trg. Hiếu", "0912345678", "https://hieu.dev");
    var command = new UpdateProfileCommand(Guid.NewGuid(), UserRole.Admin, request);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.IsValid.Should().BeTrue();
  }

  [Fact]
  public void Should_Not_Have_Error_When_Optional_Fields_Are_Null() {
    // Arrange
    var request = new UpdateProfileRequest("Trg. Hiếu", null, null);
    var command = new UpdateProfileCommand(Guid.NewGuid(), UserRole.Bidder, request);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.IsValid.Should().BeTrue();
  }
}
