using FluentAssertions;
using FluentValidation.TestHelper;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.UseCases.Items.Commands.UpdateRegisteredItem;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Items.Validators;

public class UpdateRegisteredItemValidatorTests {
  private readonly UpdateRegisteredItemValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_Id_Is_Empty() {
    var command = CreateValidCommand() with { Id = Guid.Empty };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Id)
      .WithErrorMessage("Id không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_UserId_Is_Empty() {
    var command = CreateValidCommand() with { UserId = Guid.Empty };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.UserId)
      .WithErrorMessage("UserId không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_Data_Is_Null() {
    var command = CreateValidCommand() with { Data = null! };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Data);
  }

  [Fact]
  public void Should_Have_Error_When_Name_Exceeds_MaxLength() {
    var command = CreateValidCommand() with { Data = CreateValidRequest() with { Name = new string('a', 201) } };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor("Data.Name")
      .WithErrorMessage("Tên không quá 200 ký tự.");
  }

  [Fact]
  public void Should_Have_Error_When_Description_Exceeds_MaxLength() {
    var command = CreateValidCommand() with { Data = CreateValidRequest() with { Description = new string('a', 2001) } };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor("Data.Description")
      .WithErrorMessage("Mô tả không quá 2000 ký tự.");
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void Should_Have_Error_When_StartingPrice_Is_Not_Positive(decimal startingPrice) {
    var command = CreateValidCommand() with { Data = CreateValidRequest() with { StartingPrice = startingPrice } };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor("Data.StartingPrice")
      .WithErrorMessage("Giá khởi điểm phải lớn hơn 0.");
  }

  [Fact]
  public void Should_Have_Error_When_Condition_Is_Invalid() {
    var command = CreateValidCommand() with { Data = CreateValidRequest() with { Condition = (ItemCondition)999 } };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor("Data.Condition")
      .WithErrorMessage("Tình trạng không hợp lệ.");
  }

  [Fact]
  public void Should_Have_Error_When_CategoryIds_Is_Empty() {
    var command = CreateValidCommand() with { Data = CreateValidRequest() with { CategoryIds = [] } };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor("Data.CategoryIds")
      .WithErrorMessage("Danh mục không được rỗng.");
  }

  [Fact]
  public void Should_Have_Error_When_CategoryIds_Contains_Empty_Id() {
    var command = CreateValidCommand() with { Data = CreateValidRequest() with { CategoryIds = [Guid.Empty] } };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor("Data.CategoryIds[0]")
      .WithErrorMessage("CategoryId không hợp lệ.");
  }

  [Fact]
  public void Should_Have_Error_When_MainImageUrl_Is_Invalid() {
    var command = CreateValidCommand() with { Data = CreateValidRequest() with { MainImageUrl = "invalid-url" } };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor("Data.MainImageUrl")
      .WithErrorMessage("MainImageUrl không hợp lệ.");
  }

  [Fact]
  public void Should_Have_Error_When_SubImageUrl_Is_Invalid() {
    var command = CreateValidCommand() with { Data = CreateValidRequest() with { SubImageUrls = ["invalid-url"] } };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor("Data.SubImageUrls[0]")
      .WithErrorMessage("SubImageUrl không hợp lệ.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Command_Is_Valid() {
    var command = CreateValidCommand();

    var result = _validator.TestValidate(command);

    result.IsValid.Should().BeTrue();
  }

  private static UpdateRegisteredItemCommand CreateValidCommand() {
    return new UpdateRegisteredItemCommand(Guid.NewGuid(), Guid.NewGuid(), CreateValidRequest());
  }

  private static UpdateRegisteredItemRequest CreateValidRequest() {
    return new UpdateRegisteredItemRequest(
      "Iphone 15",
      "Mo ta san pham hop le",
      1000,
      ItemCondition.NewSealed,
      [Guid.NewGuid()],
      "https://example.com/main.jpg",
      ["https://example.com/sub.jpg"]
    );
  }
}
