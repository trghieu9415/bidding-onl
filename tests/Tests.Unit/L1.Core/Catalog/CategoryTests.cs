using L1.Core.Domain.Catalog.Entities;
using L1.Core.Exceptions;
using Xunit;

namespace Tests.Unit.L1.Core.Catalog;

public class CategoryTests {
  [Fact]
  public void Create_ValidParameters_InitializesCategory() {
    var parentId = Guid.NewGuid();

    var category = Category.Create("Electronics", parentId);

    Assert.Equal("Electronics", category.Name);
    Assert.Equal(parentId, category.ParentId);
  }

  [Fact]
  public void Update_ValidParameters_ChangesFieldsAndReturnsSameCategory() {
    var category = Category.Create("Electronics");
    var parentId = Guid.NewGuid();

    var returnedCategory = category.Update("Smartphones", parentId);

    Assert.Same(category, returnedCategory);
    Assert.Equal("Smartphones", category.Name);
    Assert.Equal(parentId, category.ParentId);
  }

  [Fact]
  public void Update_WhenParentIdMatchesOwnId_ThrowsDomainException() {
    var category = Category.Create("Electronics");

    var exception = Assert.Throws<DomainException>(() => category.Update("Electronics", category.Id));

    Assert.Equal("Danh mục cha không thể là chính nó.", exception.Message);
  }
}
