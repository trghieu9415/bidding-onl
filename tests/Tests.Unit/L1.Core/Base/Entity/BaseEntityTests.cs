using L1.Core.Base.Entity;
using Xunit;

namespace Tests.Unit.L1.Core.Base.Entity;

public class BaseEntityTests {
  [Fact]
  public void NewEntity_InitializesIdentityAndCreatedAt() {
    var before = DateTime.UtcNow;
    var entity = new TestEntity();
    var after = DateTime.UtcNow;

    Assert.NotEqual(Guid.Empty, entity.Id);
    Assert.InRange(entity.CreatedAt, before, after);
    Assert.False(entity.IsDeleted);
    Assert.Null(entity.DeletedAt);
  }

  [Fact]
  public void Delete_MarksEntityAsDeletedAndSetsDeletedAt() {
    var entity = new TestEntity();
    var before = DateTime.UtcNow;

    entity.Delete();

    var after = DateTime.UtcNow;
    Assert.True(entity.IsDeleted);
    Assert.NotNull(entity.DeletedAt);
    Assert.InRange(entity.DeletedAt!.Value, before, after);
  }

  [Fact]
  public void Restore_ClearsDeletedFlags() {
    var entity = new TestEntity();
    entity.Delete();

    entity.Restore();

    Assert.False(entity.IsDeleted);
    Assert.Null(entity.DeletedAt);
  }

  private sealed class TestEntity : BaseEntity;
}
