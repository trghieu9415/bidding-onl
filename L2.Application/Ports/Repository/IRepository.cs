using System.Linq.Expressions;
using L1.Core.Base.Entity;

namespace L2.Application.Ports.Repository;

public interface IRepository<T> where T : AggregateRoot {
  Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

  Task<List<T>> GetAsync(
    Expression<Func<T, bool>>? criteria = null,
    ICollection<Expression<Func<T, BaseEntity>>>? includes = null,
    CancellationToken ct = default
  );

  Task<List<T>> GetByKeysAsync(ICollection<Guid> ids, CancellationToken ct = default);
  Task<IReadOnlyCollection<Guid>> GetMissingIds(ICollection<Guid> ids, CancellationToken ct = default);
  
  Task<Guid> CreateAsync(T entity, CancellationToken ct = default);
  Task UpdateAsync(T entity, CancellationToken ct = default);
  Task DeleteAsync(Guid id, bool softDelete = true, CancellationToken ct = default);
  Task RestoreAsync(Guid id, CancellationToken ct = default);
}
