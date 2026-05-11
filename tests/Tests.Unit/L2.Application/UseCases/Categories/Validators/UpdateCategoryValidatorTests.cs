using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.UseCases.Categories.Commands.UpdateCategory;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Categories.Validators;

public class UpdateCategoryValidatorTests {
  private readonly UpdateCategoryValidator _validator = new();

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Should_Have_Error_When_Name_Is_Empty(string name) {
    var request = new UpdateCategoryRequest(name, null);

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.Name)
      .WithErrorMessage("Tên danh mục không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_Name_Exceeds_MaxLength() {
    var request = new UpdateCategoryRequest(new string('a', 201), null);

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.Name)
      .WithErrorMessage("Tên danh mục không được vượt quá 200 ký tự.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Request_Is_Valid() {
    var request = new UpdateCategoryRequest("Điện tử", Guid.NewGuid());

    var result = _validator.TestValidate(request);

    result.IsValid.Should().BeTrue();
  }
}
