using L1.Core.Domain.Bidding.Enums;

namespace L2.Application.Models;

public record AuctionSearchModel(
  string? Keyword = null,
  List<Guid>? CategoryIds = null,
  decimal? MinPrice = null,
  decimal? MaxPrice = null,
  AuctionStatus? Status = null,
  DateTime? FromDate = null,
  DateTime? ToDate = null,
  int Page = 1,
  int PageSize = 10
);
