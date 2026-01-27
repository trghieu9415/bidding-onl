using L2.Application.DTOs;

namespace L2.Application.Ports.Search;

public interface IAuctionSearchService {
  Task<(int total, List<AuctionSearchDto> items)> SearchAsync(
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
