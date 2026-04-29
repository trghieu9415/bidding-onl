using L2.Application.DTOs;
using L2.Application.UseCases.Auctions.Commands.SearchItem;

namespace L2.Application.Ports.Search;

public interface ISearchService {
  Task<(int total, List<AuctionSearchDto> items)> SearchAsync(
    AuctionSearchRequest searchRequest,
    CancellationToken ct = default
  );
}
