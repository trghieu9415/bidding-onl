using L2.Application.Abstractions;
using L2.Application.Models;

namespace L2.Application.UseCases.Catalog.SearchItem;

public record SearchItemQuery(
  string? Keyword = null,
  List<Guid>? CategoryIds = null,
  decimal? MinPrice = null,
  decimal? MaxPrice = null,
  string? Status = null,
  DateTime? FromDate = null,
  DateTime? ToDate = null,
  int Page = 1,
  int PageSize = 10
) : IQuery<SearchItemResult>;

public record SearchItemResult(List<AuctionSearchModel> Items, Meta Meta);
