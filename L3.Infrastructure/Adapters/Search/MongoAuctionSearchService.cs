using L2.Application.DTOs;
using L2.Application.Ports.Search;
using L3.Infrastructure.Persistence;
using L3.Infrastructure.Persistence.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

namespace L3.Infrastructure.Adapters.Search;

public class MongoAuctionSearchService(MongoDbContext mongoContext) : IAuctionSearchService {
  public async Task<(int total, List<AuctionSearchDto> items)> SearchAsync(
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
    var collection = mongoContext.GetCollection<AuctionSearchDocument>("AuctionSearch");
    var builder = Builders<AuctionSearchDocument>.Filter;
    var filters = new List<FilterDefinition<AuctionSearchDocument>> {
      builder.Ne(x => x.SessionId, Guid.Empty)
    };

    if (!string.IsNullOrEmpty(keyword)) {
      filters.Add(builder.Regex(x => x.Name, new BsonRegularExpression(keyword, "i")));
    }

    if (categoryIds != null && categoryIds.Count != 0) {
      filters.Add(builder.AnyIn(x => x.CategoryIds, categoryIds));
    }

    if (!string.IsNullOrEmpty(status)) {
      filters.Add(builder.Eq(x => x.AuctionStatus, status));
    }

    if (minPrice.HasValue) {
      filters.Add(builder.Gte(x => x.CurrentPrice, minPrice.Value));
    }

    if (maxPrice.HasValue) {
      filters.Add(builder.Lte(x => x.CurrentPrice, maxPrice.Value));
    }

    if (fromDate.HasValue) {
      filters.Add(builder.Gte(x => x.StartTime, fromDate.Value));
    }

    if (toDate.HasValue) {
      filters.Add(builder.Lte(x => x.EndTime, toDate.Value));
    }

    var combinedFilter = filters.Count > 0 ? builder.And(filters) : FilterDefinition<AuctionSearchDocument>.Empty;

    var total = await collection.CountDocumentsAsync(combinedFilter, cancellationToken: ct);
    var docs = await collection.Find(combinedFilter)
      .SortByDescending(x => x.StartTime)
      .Skip((page - 1) * pageSize)
      .Limit(pageSize)
      .ToListAsync(ct);

    var items = docs.Select(d => new AuctionSearchDto(
      d.AuctionId, d.CatalogItemId,
      d.Name, d.CurrentPrice, d.AuctionStatus, d.SessionId,
      d.StartTime, d.EndTime, d.MainImageUrl
    )).ToList();

    return ((int)total, items);
  }
}
