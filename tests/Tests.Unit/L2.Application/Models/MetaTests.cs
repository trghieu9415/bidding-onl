using FluentAssertions;
using L2.Application.Models;
using Xunit;

namespace Tests.Unit.L2.Application.Models;

public class MetaTests {
  [Theory]
  [InlineData(1, 10, 25, 3, true, false, true)]
  [InlineData(2, 10, 25, 3, true, true, true)]
  [InlineData(3, 10, 25, 3, true, true, false)]
  [InlineData(1, 10, 5, 1, false, false, false)]
  public void Create_ShouldCalculateCorrectPagination(
    int page, int perPage, int total,
    int expectedTotalPages, bool expectedHasPagination,
    bool expectedHasPrev, bool expectedHasNext) {
    // Act
    var result = Meta.Create(page, perPage, total);

    // Assert
    result.TotalPages.Should().Be(expectedTotalPages);
    result.HasPagination.Should().Be(expectedHasPagination);
    result.HasPreviousPage.Should().Be(expectedHasPrev);
    result.HasNextPage.Should().Be(expectedHasNext);
  }

  [Fact]
  public void Create_ShouldHandleZeroTotal_Correctly() {
    // Act
    var result = Meta.Create(1, 10, 0);

    // Assert
    result.TotalPages.Should().Be(0);
    result.Page.Should().Be(0);
    result.HasPagination.Should().BeFalse();
  }

  [Theory]
  [InlineData(0, 1)]
  [InlineData(-5, 1)]
  public void Create_ShouldNormalizeInvalidPage_ToMinimum(int inputPage, int expectedPage) {
    // Act
    var result = Meta.Create(inputPage, 10, 50);

    // Assert
    result.Page.Should().Be(expectedPage);
  }

  [Fact]
  public void Create_ShouldClampPage_ToTotalPages() {
    var result = Meta.Create(10, 10, 50);
    result.Page.Should().Be(5);
  }

  [Theory]
  [InlineData(1, -5, 10, 1, 10)]
  [InlineData(1, 10, -10, 10, 0)]
  [InlineData(1, -1, -1, 1, 0)]
  public void Create_ShouldNormalizeNegativeValues_Correctly(
    int page,
    int inputPerPage,
    int inputTotal,
    int expectedPerPage,
    int expectedTotal
  ) {
    // Act
    var result = Meta.Create(page, inputPerPage, inputTotal);

    // Assert
    result.PerPage.Should().Be(expectedPerPage);
    result.Total.Should().Be(expectedTotal);
  }
}
