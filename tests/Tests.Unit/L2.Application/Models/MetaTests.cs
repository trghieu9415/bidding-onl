using FluentAssertions;
using L2.Application.Models;
using Xunit;

namespace Tests.Unit.L2.Application.Models;

public class MetaTests {
  [Fact]
  public void Create_Should_CreateMetaWithoutPagination_WhenSinglePage() {
    // Act
    var result = Meta.Create(
      1,
      10,
      5
    );

    // Assert
    result.Page.Should().Be(1);
    result.PerPage.Should().Be(10);
    result.Total.Should().Be(5);
    result.TotalPages.Should().Be(1);

    result.HasPagination.Should().BeFalse();
    result.HasPreviousPage.Should().BeFalse();
    result.HasNextPage.Should().BeFalse();
  }

  [Fact]
  public void Create_Should_CreateMetaWithPagination_WhenMultiplePages() {
    // Act
    var result = Meta.Create(
      2,
      10,
      25
    );

    // Assert
    result.Page.Should().Be(2);
    result.PerPage.Should().Be(10);
    result.Total.Should().Be(25);
    result.TotalPages.Should().Be(3);

    result.HasPagination.Should().BeTrue();
    result.HasPreviousPage.Should().BeTrue();
    result.HasNextPage.Should().BeTrue();
  }

  [Fact]
  public void Create_Should_SetHasNextPageFalse_WhenLastPage() {
    // Act
    var result = Meta.Create(
      3,
      10,
      25
    );

    // Assert
    result.Page.Should().Be(3);
    result.TotalPages.Should().Be(3);

    result.HasPagination.Should().BeTrue();
    result.HasPreviousPage.Should().BeTrue();
    result.HasNextPage.Should().BeFalse();
  }

  [Fact]
  public void Create_Should_NormalizeInvalidValues() {
    // Act
    var result = Meta.Create(
      -10,
      0,
      -5
    );

    // Assert
    result.Page.Should().Be(0);
    result.PerPage.Should().Be(1);
    result.Total.Should().Be(0);
    result.TotalPages.Should().Be(0);

    result.HasPagination.Should().BeFalse();
    result.HasPreviousPage.Should().BeFalse();
    result.HasNextPage.Should().BeFalse();
  }

  [Fact]
  public void Create_Should_ClampPageToTotalPages_WhenPageExceedsTotalPages() {
    // Act
    var result = Meta.Create(
      999,
      10,
      25
    );

    // Assert
    result.Page.Should().Be(3);
    result.PerPage.Should().Be(10);
    result.Total.Should().Be(25);
    result.TotalPages.Should().Be(3);

    result.HasPagination.Should().BeTrue();
    result.HasPreviousPage.Should().BeTrue();
    result.HasNextPage.Should().BeFalse();
  }
}
