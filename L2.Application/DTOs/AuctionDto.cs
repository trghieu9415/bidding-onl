using L1.Core.Domain.Bidding.Enums;
using L2.Application.DTOs.Base;

namespace L2.Application.DTOs;

public record AuctionDto : IdDto {
  public Guid CatalogItemId { get; init; }
  public AuctionStatus Status { get; init; }
  public decimal CurrentPrice { get; init; }
  public Guid? WinningBidId { get; init; }
  public decimal StepPrice { get; init; }
  public decimal ReservePrice { get; init; }
  public List<BidDto> Bids { get; init; } = [];
}
