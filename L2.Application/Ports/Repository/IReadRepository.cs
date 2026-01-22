using System.Linq.Expressions;
using L1.Core.Base.Entity;
using Sieve.Models;

namespace L2.Application.Ports.Repository;

public interface IReadRepository<T> where T : AggregateRoot {
  Task<T?> GetByIdAsync(
    Guid id,
    CancellationToken ct = default
  );

  Task<(int total, List<T> entities)> GetAsync(
    SieveModel? sieveModel = null,
    Expression<Func<T, bool>>? criteria = null,
    List<Expression<Func<T, object>>>? includes = null,
    CancellationToken ct = default
  );

  Task<(int total, List<T> entities)> GetDeletedAsync(
    SieveModel? sieveModel = null,
    Expression<Func<T, bool>>? criteria = null,
    List<Expression<Func<T, object>>>? includes = null,
    CancellationToken ct = default
  );
}