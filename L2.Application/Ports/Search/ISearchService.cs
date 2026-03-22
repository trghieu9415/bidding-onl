using L2.Application.Models;

namespace L2.Application.Ports.Search;

public interface ISearchService {
  Task<(int total, List<AuctionSearchModel> items)> SearchAsync(
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
  );
}
