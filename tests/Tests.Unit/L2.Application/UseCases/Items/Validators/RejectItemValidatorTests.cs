using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.UseCases.Items.Commands.RejectItem;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Items.Validators;

public class RejectItemValidatorTests {
  private readonly RejectItemValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_Id_Is_Empty() {
    var command = new RejectItemCommand(Guid.Empty, new RejectItemRequest("Không hợp lệ"));

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Id)
      .WithErrorMessage("Id sản phẩm là bắt buộc");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Command_Is_Valid() {
    var command = new RejectItemCommand(Guid.NewGuid(), new RejectItemRequest("Không hợp lệ"));

    var result = _validator.TestValidate(command);

    result.IsValid.Should().BeTrue();
  }
}
