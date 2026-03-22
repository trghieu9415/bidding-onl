using L2.Application.Models;
using L2.Application.Ports.Search;
using MediatR;

namespace L2.Application.UseCases.Catalog.Bidder.SearchItem;

public class SearchItemHandler(ISearchService searchService)
  : IRequestHandler<SearchItemQuery, SearchItemResult> {
  public async Task<SearchItemResult> Handle(SearchItemQuery request, CancellationToken ct) {
    var (total, items) = await searchService.SearchAsync(
      request.Keyword, request.CategoryIds,
      request.MinPrice, request.MaxPrice,
      request.Status,
      request.FromDate, request.ToDate,
      request.Page, request.PageSize,
      ct
    );

    return new SearchItemResult(items, Meta.Create(request.Page, request.PageSize, total));
  }
}
