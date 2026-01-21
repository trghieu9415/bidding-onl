using System.ComponentModel.DataAnnotations;
using L1.Core.Base.Entity;

namespace L1.Core.Domain.Catalog.Entities;

public class Category : AggregateRoot {
  private Category() {}
  [Required] public string Name { get; private set; } = null!;
  public Guid? ParentId { get; private set; }

  public static Category Create(string name, Guid? parentId = null) {
    return new Category {
      Name = name,
      ParentId = parentId
    };
  }

  public Category Update(string? name) {
    Name = name ?? Name;
    return this;
  }
}