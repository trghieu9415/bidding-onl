using L1.Core.Domain.Bidding.Enums;

namespace L2.Application.DTOs;

public record AuctionDto(
  Guid Id,
  Guid CatalogItemId,
  AuctionStatus Status,
  decimal CurrentPrice,
  Guid? WinningBidId,
  decimal StepPrice,
  decimal ReservePrice,
  List<BidDto> Bids
);