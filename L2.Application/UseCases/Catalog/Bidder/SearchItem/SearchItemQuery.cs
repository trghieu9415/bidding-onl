using L2.Application.Abstractions;

namespace L2.Application.UseCases.Catalog.Bidder.SearchItem;

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
