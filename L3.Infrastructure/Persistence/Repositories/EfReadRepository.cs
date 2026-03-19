using System.Collections;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using L1.Core.Base.Entity;
using L2.Application.DTOs.Base;
using L2.Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace L3.Infrastructure.Persistence.Repositories;

public class EfReadRepository<TEntity, TDto>(
  AppDbContext dbContext,
  IMapper mapper,
  ISieveProcessor sieveProcessor
) : IReadRepository<TEntity, TDto>
  where TEntity : AggregateRoot
  where TDto : IdDto {
  private static readonly string[] AllExpandableProperties = typeof(TDto).GetProperties()
    .Where(p =>
      (p.PropertyType.IsClass && p.PropertyType != typeof(string)) ||
      typeof(IEnumerable).IsAssignableFrom(p.PropertyType))
    .Select(p => p.Name)
    .ToArray();

  protected readonly DbSet<TEntity> DbSet = dbContext.Set<TEntity>();

  public virtual async Task<TDto?> GetByIdAsync(Guid id, CancellationToken ct = default) {
    return await DbSet
      .AsNoTracking()
      .Where(x => x.Id == id && !x.IsDeleted)
      .ProjectTo<TDto>(mapper.ConfigurationProvider, null, AllExpandableProperties)
      .FirstOrDefaultAsync(ct);
  }

  public virtual async Task<(int total, List<TDto> entities)> GetAsync(
    Expression<Func<TEntity, bool>>? criteria = null,
    SieveModel? sieveModel = null,
    List<Expression<Func<TEntity, object>>>? includes = null,
    CancellationToken ct = default) {
    var (total, query) = await GetBaseQueryAsync(false, criteria, sieveModel, includes, ct);

    var entities = await query
      .ProjectTo<TDto>(mapper.ConfigurationProvider)
      .ToListAsync(ct);

    return (total, entities);
  }

  public virtual async Task<(int total, List<TDto> entities)> GetDeletedAsync(
    Expression<Func<TEntity, bool>>? criteria = null,
    SieveModel? sieveModel = null,
    List<Expression<Func<TEntity, object>>>? includes = null,
    CancellationToken ct = default) {
    var (total, query) = await GetBaseQueryAsync(true, criteria, sieveModel, includes, ct);

    var entities = await query
      .ProjectTo<TDto>(mapper.ConfigurationProvider)
      .ToListAsync(ct);

    return (total, entities);
  }

  public virtual async Task<TDto?> GetFirstAsync(
    Expression<Func<TEntity, bool>>? criteria = null,
    CancellationToken ct = default) {
    var query = DbSet.AsNoTracking().Where(x => !x.IsDeleted);

    if (criteria != null) {
      query = query.Where(criteria);
    }

    return await query
      .ProjectTo<TDto>(mapper.ConfigurationProvider)
      .FirstOrDefaultAsync(ct);
  }


  // NOTE: ========== [Helper Methods] ==========
  private async Task<(int total, IQueryable<TEntity> query)> GetBaseQueryAsync(
    bool isDeleted,
    Expression<Func<TEntity, bool>>? criteria = null,
    SieveModel? sieveModel = null,
    List<Expression<Func<TEntity, object>>>? includes = null,
    CancellationToken ct = default
  ) {
    var query = DbSet.AsNoTracking().Where(x => x.IsDeleted == isDeleted);

    if (criteria != null) {
      query = query.Where(criteria);
    }

    if (includes != null) {
      query = includes.Aggregate(query, (current, include) => current.Include(include));
    }

    query = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
    var total = await query.CountAsync(ct);
    query = sieveProcessor.Apply(sieveModel, query, applyFiltering: false, applySorting: false);
    return (total, query);
  }
}
