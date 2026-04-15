using System.Linq.Expressions;
using L1.Core.Base.Entity;

namespace L2.Application.Repositories;

public interface IRepository<T> where T : AggregateRoot {
  Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

  Task<List<T>> GetAsync(
    Expression<Func<T, bool>>? criteria = null,
    ICollection<Expression<Func<T, object>>>? includes = null,
    CancellationToken ct = default
  );

  Task<T?> GetFirstAsync(
    Expression<Func<T, bool>>? criteria = null,
    CancellationToken ct = default
  );

  Task<List<T>> GetByKeysAsync(
    ICollection<Guid> ids,
    ICollection<Expression<Func<T, object>>>? includes = null,
    CancellationToken ct = default
  );

  Task<IReadOnlyCollection<Guid>> GetMissingIdsAsync(ICollection<Guid> ids, CancellationToken ct = default);

  Task<Guid> CreateAsync(T entity, CancellationToken ct = default);
  Task UpdateAsync(T entity, CancellationToken ct = default);
  Task DeleteAsync(Guid id, bool softDelete = true, CancellationToken ct = default);
  Task RestoreAsync(Guid id, CancellationToken ct = default);
}
