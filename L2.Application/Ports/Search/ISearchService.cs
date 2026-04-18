using L2.Application.DTOs;
using L2.Application.Filters;

namespace L2.Application.Ports.Search;

public interface ISearchService {
  Task<(int total, List<AuctionSearchDto> items)> SearchAsync(
    AuctionSearchFilter searchFilter,
    CancellationToken ct = default
  );
}
