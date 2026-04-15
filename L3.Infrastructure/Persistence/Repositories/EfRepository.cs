using System.Linq.Expressions;
using L1.Core.Base.Entity;
using L2.Application.Repositories;
using L3.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace L3.Infrastructure.Persistence.Repositories;

public class EfRepository<T>(AppDbContext context) : IRepository<T> where T : AggregateRoot {
  private readonly DbSet<T> _dbSet = context.Set<T>();

  public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) {
    var query = _dbSet.Where(x => !x.IsDeleted);
    var entityType = context.Model.FindEntityType(typeof(T));
    if (entityType == null) {
      return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    var navigations = entityType.GetNavigations();
    query = navigations.Aggregate(query, (current, nav) => current.Include(nav.Name));
    return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
  }


  public async Task<List<T>> GetAsync(
    Expression<Func<T, bool>>? criteria = null,
    ICollection<Expression<Func<T, object>>>? includes = null,
    CancellationToken ct = default
  ) {
    IQueryable<T> query = _dbSet;
    if (includes != null) {
      query = includes.Aggregate(query, (current, include) => current.Include(include));
    }

    if (criteria != null) {
      query = query.Where(criteria);
    }

    return await query.ToListAsync(ct);
  }

  public async Task<T?> GetFirstAsync(Expression<Func<T, bool>>? criteria = null, CancellationToken ct = default) {
    var query = _dbSet.Where(x => !x.IsDeleted);
    if (criteria != null) {
      query = query.Where(criteria);
    }

    return await query.FirstOrDefaultAsync(ct);
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
      .ToListAsync(ct);
  }

  public async Task<IReadOnlyCollection<Guid>> GetMissingIdsAsync(ICollection<Guid>? ids,
    CancellationToken ct = default) {
    if (ids == null || ids.Count == 0) {
      return new List<Guid>();
    }

    var distinctInputIds = ids.Distinct().ToList();

    var foundIds = await _dbSet
      .AsNoTracking()
      .Where(x => distinctInputIds.Contains(x.Id))
      .Select(x => x.Id)
      .ToListAsync(ct);

    var missingIds = distinctInputIds.Except(foundIds).ToList();
    return missingIds;
  }

  public async Task<Guid> CreateAsync(T entity, CancellationToken ct = default) {
    await _dbSet.AddAsync(entity, ct);
    return entity.Id;
  }

  public Task UpdateAsync(T entity, CancellationToken ct = default) {
    if (entity.IsDeleted) {
      throw new InfrastructureException($"Không thể cập nhật đối tượng đã bị xóa: {nameof(T)} - Id: {entity.Id}");
    }

    _dbSet.Update(entity);
    return Task.FromResult(entity);
  }

  public async Task DeleteAsync(Guid id, bool softDelete, CancellationToken ct = default) {
    var entity = await GetByIdAsync(id, ct);
    if (entity == null) {
      throw new InfrastructureException($"Không thể xóa đối tượng không tồn tại trong CSDL: {nameof(T)} - Id: {id}");
    }

    if (softDelete) {
      entity.Delete();
      _dbSet.Update(entity);
    } else {
      _dbSet.Remove(entity);
    }
  }

  public async Task RestoreAsync(Guid id, CancellationToken ct = default) {
    var entity = await GetByIdAsync(id, ct);
    if (entity == null) {
      throw new InfrastructureException(
        $"Không thể cập nhật đối tượng không tồn tại trong CSDL: {nameof(T)} - Id: {id}"
      );
    }

    entity.Restore();
    _dbSet.Update(entity);
  }
}
