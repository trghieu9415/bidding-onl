namespace L1.Core.Base.Entity;

public class BaseEntity {
  public Guid Id { get; private init; } = Guid.NewGuid();
  public DateTime CreatedAt { get; private init; } = DateTime.Now;
  public DateTime? DeletedAt { get; private set; }
  public bool IsDeleted { get; private set; }

  public void Delete() {
    IsDeleted = true;
    DeletedAt = DateTime.Now;
  }

  public void Restore() {
    IsDeleted = false;
    DeletedAt = null;
  }
}