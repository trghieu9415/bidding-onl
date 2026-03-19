using L1.Core.Domain.Bidding.Enums;
using L2.Application.DTOs.Base;

namespace L2.Application.DTOs;

public record AuctionSessionDto : IdDto {
  public string Title { get; init; } = null!;
  public SessionStatus Status { get; init; }
  public DateTime? StartTime { get; init; }
  public DateTime? EndTime { get; init; }
  public List<Guid> AuctionIds { get; init; } = [];
}
