using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs.Base;

namespace L2.Application.DTOs;

public record CategoryDto : IdDto<Category> {
  public string Name { get; init; } = null!;
  public Guid? ParentId { get; init; }
  public DateTime CreatedAt { get; init; }
}
