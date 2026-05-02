using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.UseCases.Auctions.Commands.SearchItem;
using Tests.Unit.L2.Application.UseCases.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auctions;

public class SearchItemHandlerTests {
  [Fact]
  public async Task Handle_ReturnsItemsAndMeta() {
    var filter = new AuctionSearchRequest("laptop", Page: 2, PerPage: 3);
    var items = new List<AuctionSearchDto> { new() { AuctionId = Guid.NewGuid(), Name = "Laptop" } };
    var searchService = new StubSearchService { SearchResult = (7, items) };
    var handler = new SearchItemHandler(searchService);

    var result = await handler.Handle(
      new SearchItemQuery(filter),
      TestContext.Current.CancellationToken
    );

    Assert.Same(filter, searchService.LastRequest);
    Assert.Equal(items, result.Items);
    Assert.Equal(Meta.Create(2, 3, 7), result.Meta);
  }
}
