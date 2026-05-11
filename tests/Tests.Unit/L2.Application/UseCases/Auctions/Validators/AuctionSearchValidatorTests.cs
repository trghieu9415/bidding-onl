using FluentValidation.TestHelper;
using L2.Application.UseCases.Auctions.Queries.SearchItem;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auctions.Validators;

public class AuctionSearchValidatorTests {
  private readonly AuctionSearchValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_Keyword_Exceeds_MaxLength() {
    // Arrange
    var request = new AuctionSearchRequest { Keyword = new string('k', 201) };

    // Act
    var result = _validator.TestValidate(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Keyword)
      .WithErrorMessage("Từ khóa tìm kiếm không được vượt quá 200 ký tự.");
  }

  [Fact]
  public void Should_Have_Error_When_MinPrice_Is_Greater_Than_MaxPrice() {
    // Arrange
    var request = new AuctionSearchRequest { MinPrice = 1000, MaxPrice = 500 };

    // Act
    var result = _validator.TestValidate(request);

    // Assert
    result.ShouldHaveValidationErrors()
      .WithErrorMessage("Giá tối thiểu không được lớn hơn giá tối đa.");
  }

  [Fact]
  public void Should_Have_Error_When_FromDate_Is_Greater_Than_ToDate() {
    // Arrange
    var request = new AuctionSearchRequest {
      FromDate = DateTime.Now.AddDays(1),
      ToDate = DateTime.Now
    };

    // Act
    var result = _validator.TestValidate(request);

    // Assert
    result.ShouldHaveValidationErrors()
      .WithErrorMessage("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
  }

  [Theory]
  [InlineData(0, 50)]
  [InlineData(1, 101)]
  public void Should_Have_Error_When_Pagination_Is_Invalid(int page, int perPage) {
    // Arrange
    var request = new AuctionSearchRequest { Page = page, PerPage = perPage };

    // Act
    var result = _validator.TestValidate(request);

    // Assert
    if (page <= 0) {
      result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    if (perPage < 1 || perPage > 100) {
      result.ShouldHaveValidationErrorFor(x => x.PerPage);
    }
  }
}
