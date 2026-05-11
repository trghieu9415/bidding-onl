using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.UseCases.System.AssignWinner;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.System.Validators;

public class AssignWinnerValidatorTests {
  private readonly AssignWinnerValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_CatalogItemId_Is_Empty() {
    var command = new AssignWinnerCommand(Guid.Empty, true);

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.CatalogItemId)
      .WithErrorMessage("Id sản phẩm không được để trống");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Command_Is_Valid() {
    var command = new AssignWinnerCommand(Guid.NewGuid(), true);

    var result = _validator.TestValidate(command);

    result.IsValid.Should().BeTrue();
  }
}
