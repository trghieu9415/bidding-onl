using System.Linq.Expressions;
using AutoFilterer.Abstractions;
using L1.Core.Base.Entity;
using L2.Application.DTOs.Base;
using L2.Application.Repositories;

namespace Tests.Unit.L2.Application.TestDoubles;

public class StubRepository<T> : IRepository<T> where T : AggregateRoot {
  public T? EntityByIdResult { get; set; }
  public T? FirstEntityResult { get; set; }
  public List<T> ByKeysResult { get; set; } = [];
  public IReadOnlyCollection<Guid> MissingIdsResult { get; set; } = [];
  public Guid CreateResult { get; set; } = Guid.NewGuid();
  public T? CreatedEntity { get; private set; }
  public T? UpdatedEntity { get; private set; }
  public Guid? DeletedId { get; private set; }
  public Guid? RestoredId { get; private set; }
  public Expression<Func<T, bool>>? LastCriteria { get; private set; }
  public ICollection<Expression<Func<T, object>>>? LastIncludes { get; private set; }

  public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) {
    return Task.FromResult(EntityByIdResult);
  }

  public Task<T?> GetByIdAsync(Guid id, ICollection<Expression<Func<T, object>>>? includes = null,
    CancellationToken ct = default) {
    throw new NotImplementedException();
  }

  public Task<T?> GetFirstAsync(
    Expression<Func<T, bool>>? criteria = null,
    CancellationToken ct = default
  ) {
    LastCriteria = criteria;
    return Task.FromResult(FirstEntityResult);
  }

  public Task<List<T>> GetByKeysAsync(
    ICollection<Guid> ids,
    ICollection<Expression<Func<T, object>>>? includes = null,
    CancellationToken ct = default
  ) {
    LastIncludes = includes;
    return Task.FromResult(ByKeysResult);
  }

  public Task<IReadOnlyCollection<Guid>> GetMissingIdsAsync(
    ICollection<Guid> ids,
    Expression<Func<T, bool>>? criteria = null,
    CancellationToken ct = default
  ) {
    LastCriteria = criteria;
    return Task.FromResult(MissingIdsResult);
  }

  public Task<Guid> CreateAsync(T entity, CancellationToken ct = default) {
    CreatedEntity = entity;
    return Task.FromResult(CreateResult == Guid.Empty ? entity.Id : CreateResult);
  }

  public Task UpdateAsync(T entity, CancellationToken ct = default) {
    UpdatedEntity = entity;
    return Task.CompletedTask;
  }

  public Task DeleteAsync(Guid id, CancellationToken ct = default) {
    DeletedId = id;
    return Task.CompletedTask;
  }

  public Task ForceDeleteAsync(Guid id, CancellationToken ct = default) {
    DeletedId = id;
    return Task.CompletedTask;
  }

  public Task RestoreAsync(Guid id, CancellationToken ct = default) {
    RestoredId = id;
    return Task.CompletedTask;
  }

  public Task DeleteRangeAsync(ICollection<Guid> ids, CancellationToken ct = default) {
    throw new NotImplementedException();
  }

  public Task RestoreRangeAsync(ICollection<Guid> ids, CancellationToken ct = default) {
    throw new NotImplementedException();
  }
}

public class StubReadRepository<TEntity, TDto> : IReadRepository<TEntity, TDto>
  where TEntity : AggregateRoot
  where TDto : IdDto<TEntity> {
  public TDto? EntityByIdResult { get; set; }
  public TDto? FirstEntityResult { get; set; }
  public (int total, List<TDto> entities) GetAsyncResult { get; set; } = (0, []);
  public (int total, List<TDto> entities) GetDeletedAsyncResult { get; set; } = (0, []);
  public Expression<Func<TEntity, bool>>? LastCriteria { get; private set; }
  public IFilter? LastFilter { get; private set; }
  public List<Expression<Func<TEntity, object>>>? LastIncludes { get; private set; }

  public Task<TDto?> GetByIdAsync(Guid id, CancellationToken ct = default) {
    return Task.FromResult(EntityByIdResult);
  }

  public Task<(int total, List<TDto> entities)> GetAsync(
    Expression<Func<TEntity, bool>>? criteria = null,
    IFilter? filter = null,
    CancellationToken ct = default
  ) {
    LastCriteria = criteria;
    LastFilter = filter;
    return Task.FromResult(GetAsyncResult);
  }

  public Task<(int total, List<TDto> entities)> GetDeletedAsync(
    Expression<Func<TEntity, bool>>? criteria = null,
    IFilter? filter = null,
    CancellationToken ct = default
  ) {
    LastCriteria = criteria;
    LastFilter = filter;
    return Task.FromResult(GetDeletedAsyncResult);
  }

  public Task<TDto?> GetFirstAsync(
    Expression<Func<TEntity, bool>>? criteria = null,
    CancellationToken ct = default
  ) {
    LastCriteria = criteria;
    return Task.FromResult(FirstEntityResult);
  }
}
