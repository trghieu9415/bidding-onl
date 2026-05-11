using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.UseCases.Items.Commands.ApproveItem;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Items.Validators;

public class ApproveItemValidatorTests {
  private readonly ApproveItemValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_Id_Is_Empty() {
    var command = new ApproveItemCommand(Guid.Empty);

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Id)
      .WithErrorMessage("Id sản phẩm là bắt buộc");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Command_Is_Valid() {
    var command = new ApproveItemCommand(Guid.NewGuid());

    var result = _validator.TestValidate(command);

    result.IsValid.Should().BeTrue();
  }
}
