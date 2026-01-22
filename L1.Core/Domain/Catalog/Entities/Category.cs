using System.ComponentModel.DataAnnotations;
using L1.Core.Base.Entity;
using L1.Core.Base.Exception;

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

  public Category Update(string name, Guid? parentId) {
    if (parentId == Id) {
      throw new DomainException("Danh mục cha không thể là chính nó.");
    }

    Name = name;
    ParentId = parentId;
    return this;
  }
}