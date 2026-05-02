using L1.Core.Base.Entity;

namespace L2.Application.DTOs.Base;

public record IdDto<TEntity> where TEntity : BaseEntity {
  public Guid Id { get; init; }
}
