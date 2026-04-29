using L2.Application.Models;
using L2.Application.Ports.Search;
using MediatR;

namespace L2.Application.UseCases.Auctions.Commands.SearchItem;

public class SearchItemHandler(ISearchService searchService)
  : IRequestHandler<SearchItemQuery, SearchItemResult> {
  public async Task<SearchItemResult> Handle(SearchItemQuery request, CancellationToken ct) {
    var searchFilter = request.SearchFilter;
    var (total, items) = await searchService.SearchAsync(searchFilter, ct);

    return new SearchItemResult(items, Meta.Create(searchFilter.Page, searchFilter.PerPage, total));
  }
}
