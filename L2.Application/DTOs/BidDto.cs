namespace L2.Application.DTOs;

public record BidDto(
  Guid Id,
  Guid AuctionId,
  Guid BidderId,
  decimal Amount,
  DateTime TimePoint
);