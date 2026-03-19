using L1.Core.Domain.Catalog.Enums;
using L2.Application.DTOs.Base;

namespace L2.Application.DTOs;

public record CatalogItemDto : IdDto {
  public Guid OwnerId { get; init; }
  public string Name { get; init; } = null!;
  public string Description { get; init; } = null!;
  public ItemStatus Status { get; init; }
  public ItemCondition? Condition { get; init; }
  public decimal StartingPrice { get; init; }
  public string? MainImageUrl { get; init; }
  public List<string> SubImageUrls { get; init; } = [];
  public List<Guid> CategoryIds { get; init; } = [];
  public DateTime CreatedAt { get; init; }
}
