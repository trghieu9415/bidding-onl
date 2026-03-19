using System.Linq.Expressions;
using L1.Core.Base.Entity;
using L2.Application.DTOs.Base;
using Sieve.Models;

namespace L2.Application.Repositories;

public interface IReadRepository<TEntity, TDto>
  where TEntity : AggregateRoot
  where TDto : IdDto {
  Task<TDto?> GetByIdAsync(
    Guid id,
    CancellationToken ct
  );

  Task<(int total, List<TDto> entities)> GetAsync(
    Expression<Func<TEntity, bool>>? criteria = null,
    SieveModel? sieveModel = null,
    List<Expression<Func<TEntity, object>>>? includes = null,
    CancellationToken ct = default
  );


  Task<(int total, List<TDto> entities)> GetDeletedAsync(
    Expression<Func<TEntity, bool>>? criteria = null,
    SieveModel? sieveModel = null,
    List<Expression<Func<TEntity, object>>>? includes = null,
    CancellationToken ct = default
  );

  Task<TDto?> GetFirstAsync(Expression<Func<TEntity, bool>>? criteria = null, CancellationToken ct = default);
}
