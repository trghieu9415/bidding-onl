using System.Diagnostics.CodeAnalysis;
using L1.Core.Domain.Catalog.Entities;

namespace Tests.Common.Builders;

[ExcludeFromCodeCoverage]
public class CategoryBuilder {
  private string _name = "Electronics";
  private Guid? _parentId;

  public CategoryBuilder WithName(string name) {
    _name = name;
    return this;
  }

  public CategoryBuilder WithParentId(Guid? parentId) {
    _parentId = parentId;
    return this;
  }

  public Category Build() {
    return Category.Create(_name, _parentId);
  }
}
