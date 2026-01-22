using System.Linq.Expressions;
using L1.Core.Base.Entity;
using L2.Application.Ports.Repository;
using L3.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace L3.Infrastructure.Adapters.Repository;

public class EfReadRepository<T>(
  AppDbContext context,
  ISieveProcessor sieveProcessor
) : IReadRepository<T> where T : AggregateRoot {
  private readonly DbSet<T> _dbSet = context.Set<T>();

  public async Task<T?> GetByIdAsync(
    Guid id,
    CancellationToken ct = default
  ) {
    var query = _dbSet.AsNoTracking();
    var entityType = context.Model.FindEntityType(typeof(T));
    if (entityType == null) {
      return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    query = entityType
      .GetNavigations()
      .Aggregate(query, (current, nav) => current.Include(nav.Name));
    return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
  }

  public async Task<(int total, List<T> entities)> GetAsync(
    SieveModel? sieveModel = null,
    Expression<Func<T, bool>>? criteria = null,
    List<Expression<Func<T, object>>>? includes = null,
    CancellationToken ct = default
  ) {
    var query = _dbSet.AsNoTracking().Where(x => !x.IsDeleted);
    return await ProcessQueryAsync(query, sieveModel, criteria, includes, ct);
  }

  public async Task<(int total, List<T> entities)> GetDeletedAsync(
    SieveModel? sieveModel = null,
    Expression<Func<T, bool>>? criteria = null,
    List<Expression<Func<T, object>>>? includes = null,
    CancellationToken ct = default
  ) {
    var query = _dbSet.AsNoTracking().Where(x => x.IsDeleted);
    return await ProcessQueryAsync(query, sieveModel, criteria, includes, ct);
  }

  // NOTE: ========== [helper methods] ==========
  private async Task<(int total, List<T> entities)> ProcessQueryAsync(
    IQueryable<T> query,
    SieveModel? sieveModel,
    Expression<Func<T, bool>>? criteria,
    List<Expression<Func<T, object>>>? includes,
    CancellationToken ct
  ) {
    if (includes != null) {
      query = includes.Aggregate(query, (current, include) => current.Include(include));
    }

    if (criteria != null) {
      query = query.Where(criteria);
    }

    if (sieveModel != null) {
      query = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
    }

    var total = await query.CountAsync(ct);
    if (sieveModel != null) {
      query = sieveProcessor.Apply(sieveModel, query, applyFiltering: false, applySorting: false);
    }

    var entities = await query.ToListAsync(ct);
    return (total, entities);
  }
}