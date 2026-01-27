namespace L2.Application.DTOs;

public record AuctionSearchDto(
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
