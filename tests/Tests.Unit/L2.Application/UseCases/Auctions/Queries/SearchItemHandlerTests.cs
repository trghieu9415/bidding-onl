using FluentAssertions;
using L2.Application.DTOs;
using L2.Application.Ports.Search;
using L2.Application.UseCases.Auctions.Queries.SearchItem;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auctions.Queries;

public class SearchItemHandlerTests {
  private readonly ISearchService _searchService = Substitute.For<ISearchService>();
  private readonly SearchItemHandler _sut;

  public SearchItemHandlerTests() {
    _sut = new SearchItemHandler(_searchService);
  }

  [Fact]
  public async Task Handle_Should_ReturnItems_And_Meta() {
    var filter = new AuctionSearchRequest(Keyword: "iphone", Page: 2, PerPage: 5);
    var query = new SearchItemQuery(filter);
    var items = new List<AuctionSearchDto> {
      new() {
        AuctionId = Guid.NewGuid(),
        CatalogItemId = Guid.NewGuid(),
        Name = "Iphone 15",
        CurrentPrice = 1500,
        AuctionStatus = "Active",
        SessionId = Guid.NewGuid(),
        StartTime = DateTime.UtcNow,
        EndTime = DateTime.UtcNow.AddHours(1),
        MainImageUrl = "https://example.com/iphone.jpg"
      }
    };

    _searchService.SearchAsync(filter, CancellationToken.None)
      .Returns((7, items));

    var result = await _sut.Handle(query, CancellationToken.None);

    result.Items.Should().BeSameAs(items);
    result.Meta.Page.Should().Be(2);
    result.Meta.PerPage.Should().Be(5);
    result.Meta.Total.Should().Be(7);
    result.Meta.TotalPages.Should().Be(2);
    result.Meta.HasPreviousPage.Should().BeTrue();
    result.Meta.HasNextPage.Should().BeFalse();

    await _searchService.Received(1).SearchAsync(filter, CancellationToken.None);
  }
}
