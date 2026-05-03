using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.DTOs.Base;

namespace L2.Application.DTOs;

public record CatalogItemDto : IdDto<CatalogItem> {
  public Guid OwnerId { get; init; }
  public required string Name { get; init; }
  public required string Description { get; init; }
  public ItemStatus Status { get; init; }
  public ItemCondition? Condition { get; init; }
  public decimal StartingPrice { get; init; }
  public string? MainImageUrl { get; init; }
  public List<string> SubImageUrls { get; init; } = [];
  public List<Guid> CategoryIds { get; init; } = [];
  public DateTime CreatedAt { get; init; }
}
