using System.Linq.Expressions;
using L1.Core.Base.Entity;
using L2.Application.Repositories;
using L3.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace L3.Infrastructure.Persistence.Repositories;

public class EfRepository<T>(AppDbContext context) : IRepository<T> where T : AggregateRoot {
  private readonly DbSet<T> _dbSet = context.Set<T>();

  public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) {
    return await GetByIdAsync(id, null, ct);
  }

  public async Task<T?> GetByIdAsync(
    Guid id, ICollection<Expression<Func<T, object>>>? includes = null,
    CancellationToken ct = default
  ) {
    IQueryable<T> query = _dbSet;

    if (includes is not { Count: > 0 }) {
      return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    query = includes.Aggregate(query, (current, include) => current.Include(include));
    query = query.AsSplitQuery();
    return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
  }

  public async Task<T?> GetFirstAsync(Expression<Func<T, bool>>? criteria = null, CancellationToken ct = default) {
    return criteria == null
      ? await _dbSet.FirstOrDefaultAsync(ct)
      : await _dbSet.Where(criteria).FirstOrDefaultAsync(ct);
  }

  public async Task<List<T>> GetByKeysAsync(
    ICollection<Guid>? ids,
    ICollection<Expression<Func<T, object>>>? includes,
    CancellationToken ct = default
  ) {
    if (ids == null || ids.Count == 0) {
      return [];
    }

    IQueryable<T> query = _dbSet;
    if (includes != null) {
      query = includes.Aggregate(query, (current, include) => current.Include(include));
    }

    return await query
      .Where(x => ids.Contains(x.Id))
      .AsSplitQuery()
      .ToListAsync(ct);
  }

  public async Task<IReadOnlyCollection<Guid>> GetMissingIdsAsync(
    ICollection<Guid>? ids,
    Expression<Func<T, bool>>? criteria = null,
    CancellationToken ct = default
  ) {
    if (ids == null || ids.Count == 0) {
      return [];
    }

    var distinctInputIds = ids.Distinct().ToList();

    var query = _dbSet.AsNoTracking()
      .Where(x => distinctInputIds.Contains(x.Id));

    if (criteria != null) {
      query = query.Where(criteria);
    }

    var foundIds = await query.Select(x => x.Id).ToListAsync(ct);
    var missingIds = distinctInputIds.Except(foundIds).ToList();
    return missingIds;
  }

  public async Task<Guid> CreateAsync(T entity, CancellationToken ct = default) {
    await _dbSet.AddAsync(entity, ct);
    return entity.Id;
  }

  public Task UpdateAsync(T entity, CancellationToken ct = default) {
    return entity.IsDeleted
      ? throw new InfrastructureException($"Không thể cập nhật đối tượng đã bị xóa: {typeof(T)} - Id: {entity.Id}")
      : Task.FromResult(entity);
  }

  public async Task DeleteAsync(Guid id, CancellationToken ct = default) {
    var entity = await _dbSet.FirstOrDefaultAsync(x => x.Id == id, ct);
    if (entity == null) {
      throw new InfrastructureException($"Không thể xóa đối tượng không tồn tại: {typeof(T)} - Id: {id}");
    }

    entity.Delete();
  }

  public async Task ForceDeleteAsync(Guid id, CancellationToken ct = default) {
    var entity = await _dbSet
      .IgnoreQueryFilters()
      .FirstOrDefaultAsync(x => x.Id == id, ct);

    if (entity != null) {
      _dbSet.Remove(entity);
    }
  }


  public async Task RestoreAsync(Guid id, CancellationToken ct = default) {
    var entity = await _dbSet
      .IgnoreQueryFilters()
      .FirstOrDefaultAsync(x => x.Id == id, ct);
    if (entity == null) {
      throw new InfrastructureException(
        $"Không thể cập nhật đối tượng không tồn tại trong CSDL: {nameof(T)} - Id: {id}"
      );
    }

    entity.Restore();
  }

  public async Task DeleteRangeAsync(ICollection<Guid> ids, CancellationToken ct = default) {
    var entities = await _dbSet
      .Where(x => ids.Contains(x.Id))
      .ToListAsync(ct);

    foreach (var entity in entities) {
      entity.Delete();
    }
  }

  public async Task RestoreRangeAsync(ICollection<Guid> ids, CancellationToken ct = default) {
    var entities = await _dbSet
      .IgnoreQueryFilters()
      .Where(x => ids.Contains(x.Id))
      .ToListAsync(ct);

    foreach (var entity in entities) {
      entity.Restore();
    }
  }
}
