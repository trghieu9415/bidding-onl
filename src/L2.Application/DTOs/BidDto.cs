using L2.Application.DTOs.Base;

namespace L2.Application.DTOs;

public record BidDto : IdDto {
  public Guid AuctionId { get; init; }
  public Guid BidderId { get; init; }
  public decimal Amount { get; init; }
  public DateTime TimePoint { get; init; }
}
