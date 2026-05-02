using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs.Base;

namespace L2.Application.DTOs;

public record BidDto : IdDto<Bid> {
  public Guid AuctionId { get; init; }
  public Guid BidderId { get; init; }
  public decimal Amount { get; init; }
  public DateTime TimePoint { get; init; }
}
