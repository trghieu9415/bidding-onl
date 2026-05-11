using FluentAssertions;
using FluentValidation.TestHelper;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.UseCases.Items.Commands.RegisterItem;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Items.Validators;

public class RegisterItemValidatorTests {
  private readonly RegisterItemValidator _validator = new();

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Should_Have_Error_When_Name_Is_Empty(string name) {
    var request = CreateValidRequest() with { Name = name };

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.Name)
      .WithErrorMessage("Tên sản phẩm không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_Name_Exceeds_MaxLength() {
    var request = CreateValidRequest() with { Name = new string('a', 201) };

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.Name)
      .WithErrorMessage("Tên sản phẩm không được vượt quá 200 ký tự.");
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Should_Have_Error_When_Description_Is_Empty(string description) {
    var request = CreateValidRequest() with { Description = description };

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.Description)
      .WithErrorMessage("Mô tả sản phẩm không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_Description_Exceeds_MaxLength() {
    var request = CreateValidRequest() with { Description = new string('a', 2001) };

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.Description)
      .WithErrorMessage("Mô tả sản phẩm không được vượt quá 2000 ký tự.");
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void Should_Have_Error_When_StartingPrice_Is_Not_Positive(decimal startingPrice) {
    var request = CreateValidRequest() with { StartingPrice = startingPrice };

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.StartingPrice)
      .WithErrorMessage("Giá khởi điểm phải lớn hơn 0.");
  }

  [Fact]
  public void Should_Have_Error_When_CategoryIds_Is_Empty() {
    var request = CreateValidRequest() with { CategoryIds = [] };

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.CategoryIds)
      .WithErrorMessage("Phải chọn ít nhất 1 danh mục.");
  }

  [Fact]
  public void Should_Have_Error_When_CategoryIds_Contains_Empty_Id() {
    var request = CreateValidRequest() with { CategoryIds = [Guid.Empty] };

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor("CategoryIds[0]")
      .WithErrorMessage("Id danh mục không hợp lệ.");
  }

  [Fact]
  public void Should_Have_Error_When_MainImageUrl_Exceeds_MaxLength() {
    var request = CreateValidRequest() with { MainImageUrl = new string('a', 1001) };

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.MainImageUrl)
      .WithErrorMessage("Đường dẫn ảnh chính không được vượt quá 1000 ký tự.");
  }

  [Fact]
  public void Should_Have_Error_When_SubImageUrls_Is_Null() {
    var request = CreateValidRequest() with { SubImageUrls = null! };

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.SubImageUrls)
      .WithErrorMessage("Danh sách ảnh phụ không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_SubImageUrl_Is_Empty() {
    var request = CreateValidRequest() with { SubImageUrls = [""] };

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor("SubImageUrls[0]")
      .WithErrorMessage("Đường dẫn ảnh phụ không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_SubImageUrl_Exceeds_MaxLength() {
    var request = CreateValidRequest() with { SubImageUrls = [new string('a', 1001)] };

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor("SubImageUrls[0]")
      .WithErrorMessage("Đường dẫn ảnh phụ không được vượt quá 1000 ký tự.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Request_Is_Valid() {
    var request = CreateValidRequest();

    var result = _validator.TestValidate(request);

    result.IsValid.Should().BeTrue();
  }

  private static RegisterItemRequest CreateValidRequest() {
    return new RegisterItemRequest(
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
