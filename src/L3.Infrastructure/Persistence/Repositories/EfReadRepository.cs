using System.Linq.Expressions;
using AutoFilterer.Abstractions;
using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using L1.Core.Base.Entity;
using L2.Application.DTOs.Base;
using L2.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace L3.Infrastructure.Persistence.Repositories;

public class EfReadRepository<TEntity, TDto>(
  AppDbContext dbContext,
  IMapper mapper
) : IReadRepository<TEntity, TDto>
  where TEntity : AggregateRoot
  where TDto : IdDto<TEntity> {
  protected readonly DbSet<TEntity> DbSet = dbContext.Set<TEntity>();

  public virtual async Task<TDto?> GetByIdAsync(Guid id, CancellationToken ct = default) {
    return await DbSet
      .AsNoTracking()
      .Where(x => x.Id == id)
      .ProjectTo<TDto>(mapper.ConfigurationProvider)
      .AsSplitQuery()
      .FirstOrDefaultAsync(ct);
  }

  public virtual async Task<(int total, List<TDto> entities)> GetAsync(
    Expression<Func<TEntity, bool>>? criteria = null,
    IFilter? filter = null,
    CancellationToken ct = default
  ) {
    var (total, pagedQuery) = await GetBaseQueryAsync(false, criteria, filter, ct);

    var entities = await pagedQuery
      .ProjectTo<TDto>(mapper.ConfigurationProvider)
      .AsSplitQuery()
      .ToListAsync(ct);

    return (total, entities);
  }

  public virtual async Task<(int total, List<TDto> entities)> GetDeletedAsync(
    Expression<Func<TEntity, bool>>? criteria = null,
    IFilter? filter = null,
    CancellationToken ct = default
  ) {
    var (total, pagedQuery) = await GetBaseQueryAsync(true, criteria, filter, ct);

    var entities = await pagedQuery
      .ProjectTo<TDto>(mapper.ConfigurationProvider)
      .AsSplitQuery()
      .ToListAsync(ct);

    return (total, entities);
  }

  public virtual async Task<TDto?> GetFirstAsync(
    Expression<Func<TEntity, bool>>? criteria = null,
    CancellationToken ct = default
  ) {
    var query = DbSet.AsNoTracking();

    if (criteria != null) {
      query = query.Where(criteria);
    }

    return await query
      .ProjectTo<TDto>(mapper.ConfigurationProvider)
      .AsSplitQuery()
      .FirstOrDefaultAsync(ct);
  }

  // NOTE: ========== [Helper Methods] ==========
  private async Task<(int total, IQueryable<TEntity> pagedQuery)> GetBaseQueryAsync(
    bool isDeleted,
    Expression<Func<TEntity, bool>>? criteria = null,
    IFilter? filter = null,
    CancellationToken ct = default
  ) {
    var query = DbSet.AsNoTracking();

    if (isDeleted) {
      query = query.IgnoreQueryFilters().Where(x => x.IsDeleted);
    }

    if (criteria != null) {
      query = query.Where(criteria);
    }

    if (filter != null) {
      query = query.ApplyFilter(filter);
    }

    var total = await query.CountAsync(ct);

    if (filter is not IPaginationFilter paginationFilter) {
      return (total, query);
    }

    var page = paginationFilter.Page > 0 ? paginationFilter.Page : 1;
    var perPage = paginationFilter.PerPage > 0 ? paginationFilter.PerPage : 10;

    query = query.Skip((page - 1) * perPage).Take(perPage);

    return (total, query);
  }
}
