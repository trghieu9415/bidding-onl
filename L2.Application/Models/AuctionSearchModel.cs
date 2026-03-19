namespace L2.Application.Models;

public record AuctionSearchModel(
  Guid AuctionId,
  Guid CatalogItemId,
  string Name,
  decimal CurrentPrice,
  string AuctionStatus,
  Guid SessionId,
  DateTime StartTime,
  DateTime EndTime,
  string? MainImageUrl
);
