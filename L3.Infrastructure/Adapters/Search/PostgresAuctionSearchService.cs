using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.Models;
using L2.Application.Ports.Search;
using L3.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace L3.Infrastructure.Adapters.Search;

public class PostgresAuctionSearchService(AppDbContext context) : IAuctionSearchService {
  public async Task<(int total, List<AuctionSearchModel> items)> SearchAsync(
    string? keyword,
    List<Guid>? categoryIds,
    decimal? minPrice,
    decimal? maxPrice,
    string? status,
    DateTime? fromDate,
    DateTime? toDate,
    int page,
    int pageSize,
    CancellationToken ct
  ) {
    var query = from auction in context.Set<Auction>().AsNoTracking()
      join item in context.Set<CatalogItem>().AsNoTracking() on auction.CatalogItemId equals item.Id
      from session in context.Set<AuctionSession>().AsNoTracking()
      where !auction.IsDeleted && !item.IsDeleted && !session.IsDeleted
            && session.AuctionIds.Contains(auction.Id)
      select new { auction, item, session };

    if (!string.IsNullOrEmpty(keyword)) {
      query = query.Where(x => EF.Functions.ILike(x.item.Name, $"%{keyword}%"));
    }

    if (categoryIds is { Count: > 0 }) {
      query = query.Where(x => x.item.CategoryIds.Any(categoryIds.Contains));
    }

    if (!string.IsNullOrEmpty(status) && Enum.TryParse<AuctionStatus>(status, true, out var statusEnum)) {
      query = query.Where(x => x.auction.Status == statusEnum);
    }

    if (minPrice.HasValue) {
      query = query.Where(x => x.auction.CurrentPrice >= minPrice.Value);
    }

    if (maxPrice.HasValue) {
      query = query.Where(x => x.auction.CurrentPrice <= maxPrice.Value);
    }

    if (fromDate.HasValue) {
      query = query.Where(x => x.session.TimeFrame.StartTime >= fromDate.Value);
    }

    if (toDate.HasValue) {
      query = query.Where(x => x.session.TimeFrame.EndTime <= toDate.Value);
    }

    var total = await query.CountAsync(ct);

    var results = await query
      .OrderByDescending(x => x.session.TimeFrame.StartTime)
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .Select(x => new AuctionSearchModel(
        x.auction.Id,
        x.item.Id,
        x.item.Name,
        x.auction.CurrentPrice,
        x.auction.Status.ToString(),
        x.session.Id,
        x.session.TimeFrame.StartTime,
        x.session.TimeFrame.EndTime,
        x.item.Images.MainImageUrl
      ))
      .ToListAsync(ct);

    return (total, results);
  }
}
