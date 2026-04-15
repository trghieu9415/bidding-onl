using System.Linq.Expressions;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Ports.Search;
using L3.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

// Add namespace chứa WhereIf

namespace L3.Infrastructure.Adapters.Search;

public class PostgresSearchService(AppDbContext context) : ISearchService {
  public async Task<(int total, List<AuctionSearchDto> items)> SearchAsync(
    AuctionSearchModel req,
    CancellationToken ct
  ) {
    // Query Join
    var query = context.Set<Auction>()
      .Join(context.Set<CatalogItem>(),
        a => a.CatalogItemId, i => i.Id,
        (auction, item) => new { auction, item }
      )
      .Join(context.Set<AuctionSession>(),
        aS => aS.auction.SessionId, s => s.Id,
        (temp, session) => new { temp.auction, temp.item, session })
      .Where(x =>
        !x.auction.IsDeleted && !x.item.IsDeleted &&
        !x.session.IsDeleted && x.session.AuctionIds.Contains(x.auction.Id)
      );

    // Query Filter
    query = query
      .AsNoTracking()
      .WhereIf(!string.IsNullOrEmpty(req.Keyword), x => EF.Functions.ILike(x.item.Name, $"%{req.Keyword}%"))
      .WhereIf(req.CategoryIds?.Count > 0, x => x.item.CategoryIds.Any(c => req.CategoryIds!.Contains(c)))
      .WhereIf(req.Status.HasValue, x => x.auction.Status == req.Status)
      .WhereIf(req.MinPrice.HasValue, x => x.auction.CurrentPrice >= req.MinPrice)
      .WhereIf(req.MaxPrice.HasValue, x => x.auction.CurrentPrice <= req.MaxPrice)
      .WhereIf(req.FromDate.HasValue, x => x.session.TimeFrame.StartTime >= req.FromDate)
      .WhereIf(req.ToDate.HasValue, x => x.session.TimeFrame.EndTime <= req.ToDate);

    var total = await query.CountAsync(ct);

    var results = await query
      .OrderByDescending(x => x.session.TimeFrame.StartTime)
      .Skip((req.Page - 1) * req.PageSize)
      .Take(req.PageSize)
      .Select(x => new AuctionSearchDto {
        AuctionId = x.auction.Id,
        CatalogItemId = x.item.Id,
        Name = x.item.Name,
        CurrentPrice = x.auction.CurrentPrice,
        AuctionStatus = x.auction.Status.ToString(),
        SessionId = x.session.Id,
        StartTime = x.session.TimeFrame.StartTime,
        EndTime = x.session.TimeFrame.EndTime,
        MainImageUrl = x.item.Images.MainImageUrl
      })
      .ToListAsync(ct);

    return (total, results);
  }
}

public static class QueryableExtensions {
  public static IQueryable<T> WhereIf<T>(
    this IQueryable<T> query,
    bool condition,
    Expression<Func<T, bool>> predicate
  ) {
    return condition ? query.Where(predicate) : query;
  }
}
