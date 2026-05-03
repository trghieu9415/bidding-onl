using FluentAssertions;
using L1.Core.Base.Entity;
using Xunit;

namespace Tests.Unit.L1.Core.Base.Entity;

public class BaseEntityTests {
  [Fact]
  public void Constructor_ShouldInitializeDefaultValues() {
    // Arrange
    var entity = new FakeBaseEntity();

    // Assert
    entity.Id.Should().NotBeEmpty();
    entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    entity.IsDeleted.Should().BeFalse();
    entity.DeletedAt.Should().BeNull();
  }

  [Fact]
  public void Delete_ShouldSetIsDeletedToTrueAndSetDeletedAt() {
    // Arrange
    var entity = new FakeBaseEntity();

    // Act
    entity.Delete();

    // Assert
    entity.IsDeleted.Should().BeTrue();
    entity.DeletedAt.Should().NotBeNull();
    entity.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
  }

  [Fact]
  public void Restore_ShouldSetIsDeletedToFalseAndNullifyDeletedAt() {
    // Arrange
    var entity = new FakeBaseEntity();
    entity.Delete();

    // Act
    entity.Restore();

    // Assert
    entity.IsDeleted.Should().BeFalse();
    entity.DeletedAt.Should().BeNull();
  }

  [Fact]
  public void RowVersion_ShouldBeSettableAndGettable() {
    // Arrange
    var entity = new FakeBaseEntity();
    const uint expectedVersion = 1;

    // Act
    entity.RowVersion = expectedVersion;

    // Assert
    entity.RowVersion.Should().Be(expectedVersion);
  }

  private sealed class FakeBaseEntity : BaseEntity;
}
