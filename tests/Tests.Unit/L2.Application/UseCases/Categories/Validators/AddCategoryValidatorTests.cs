using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.UseCases.Categories.Commands.AddCategory;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Categories.Validators;

public class AddCategoryValidatorTests {
  private readonly AddCategoryValidator _validator = new();

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Should_Have_Error_When_Name_Is_Empty(string name) {
    var command = new AddCategoryCommand(name, null);

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Name)
      .WithErrorMessage("Tên danh mục không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_Name_Exceeds_MaxLength() {
    var command = new AddCategoryCommand(new string('a', 201), null);

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Name)
      .WithErrorMessage("Tên danh mục không được vượt quá 200 ký tự.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Command_Is_Valid() {
    var command = new AddCategoryCommand("Điện tử", Guid.NewGuid());

    var result = _validator.TestValidate(command);

    result.IsValid.Should().BeTrue();
  }
}
