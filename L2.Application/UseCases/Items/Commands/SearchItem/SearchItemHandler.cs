using L2.Application.Models;
using L2.Application.Ports.Search;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.SearchItem;

public class SearchItemHandler(ISearchService searchService)
  : IRequestHandler<SearchItemQuery, SearchItemResult> {
  public async Task<SearchItemResult> Handle(SearchItemQuery request, CancellationToken ct) {
    var searchModel = request.SearchModel;
    var (total, items) = await searchService.SearchAsync(searchModel);

    return new SearchItemResult(items, Meta.Create(searchModel.Page, searchModel.PageSize, total));
  }
}
