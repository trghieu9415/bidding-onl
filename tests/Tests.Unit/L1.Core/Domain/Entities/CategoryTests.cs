using FluentAssertions;
using L1.Core.Exceptions;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.Entities;

public class CategoryTests {
  [Fact]
  public void Create_ValidParameters_InitializesCategory() {
    // Arrange
    var parentId = Guid.NewGuid();
    var builder = new CategoryBuilder()
      .WithName("Electronics")
      .WithParentId(parentId);

    // Act
    var category = builder.Build();

    // Assert
    category.Name.Should().Be("Electronics");
    category.ParentId.Should().Be(parentId);
  }

  [Fact]
  public void Update_ValidParameters_ChangesFieldsAndReturnsSameCategory() {
    // Arrange
    var category = new CategoryBuilder().WithName("Electronics").Build();
    var parentId = Guid.NewGuid();

    // Act
    var returnedCategory = category.Update("Smartphones", parentId);

    // Assert
    returnedCategory.Should().BeSameAs(category);
    category.Name.Should().Be("Smartphones");
    category.ParentId.Should().Be(parentId);
  }

  [Fact]
  public void Update_WhenParentIdMatchesOwnId_ThrowsDomainException() {
    // Arrange
    var category = new CategoryBuilder().WithName("Electronics").Build();

    // Act
    Action act = () => category.Update("Electronics", category.Id);

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Danh mục cha không thể là chính nó.");
  }
}
