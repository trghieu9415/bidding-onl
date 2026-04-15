using L2.Application.DTOs;
using L2.Application.Models;

namespace L2.Application.Ports.Search;

public interface ISearchService {
  Task<(int total, List<AuctionSearchDto> items)> SearchAsync(
    AuctionSearchModel searchModel,
    CancellationToken ct = default
  );
}
