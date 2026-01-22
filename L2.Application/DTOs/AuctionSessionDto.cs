using L1.Core.Domain.Bidding.Enums;

namespace L2.Application.DTOs;

public record AuctionSessionDto(
  Guid Id,
  string Title,
  SessionStatus Status,
  DateTime? StartTime,
  DateTime? EndTime,
  List<Guid> AuctionIds
);