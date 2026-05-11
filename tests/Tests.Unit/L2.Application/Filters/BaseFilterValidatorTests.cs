using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.Filters;
using Xunit;

namespace Tests.Unit.L2.Application.Filters;

public class TestFilter : BaseFilter {}

public class TestFilterValidator : BaseFilterValidator<TestFilter> {}

public class BaseFilterValidatorTests {
  private readonly TestFilterValidator _validator = new();

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void Should_Have_Error_When_Page_Is_Invalid(int page) {
    // Arrange
    var filter = new TestFilter { Page = page };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Page)
      .WithErrorMessage("Số trang phải lớn hơn 0.");
  }

  [Theory]
  [InlineData(0)]
  [InlineData(101)]
  public void Should_Have_Error_When_PerPage_Is_Out_Of_Range(int perPage) {
    // Arrange
    var filter = new TestFilter { PerPage = perPage };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.PerPage)
      .WithErrorMessage("Số lượng bản ghi mỗi trang phải từ 1 đến 100.");
  }

  [Fact]
  public void Should_Have_Error_When_MinCreatedAt_Is_Greater_Than_MaxCreatedAt() {
    // Arrange
    var filter = new TestFilter {
      MinCreatedAt = DateTime.Now.AddDays(1),
      MaxCreatedAt = DateTime.Now
    };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrors()
      .WithErrorMessage("Ngày tạo bắt đầu không được lớn hơn ngày tạo kết thúc.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Base_Filter_Is_Valid() {
    // Arrange
    var filter = new TestFilter {
      Page = 1,
      PerPage = 50,
      MinCreatedAt = DateTime.Now.AddDays(-1),
      MaxCreatedAt = DateTime.Now
    };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.IsValid.Should().BeTrue();
  }
}
