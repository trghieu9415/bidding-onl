using L1.Core.Domain.Catalog.Enums;

namespace L2.Application.DTOs;

public record CatalogItemDto(
  Guid Id,
  Guid OwnerId,
  string Name,
  string Description,
  ItemStatus Status,
  ItemCondition? Condition,
  decimal StartingPrice,
  string? MainImageUrl,
  List<string> SubImageUrls,
  List<Guid> CategoryIds,
  DateTime CreatedAt
);