using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;

namespace L2.Application.Repositories.Read;

public interface IAuctionReadRepository : IReadRepository<Auction, AuctionDto> {
  public Task<(int total, List<BidDto> bids)> GetBidsAsync(
    Guid auctionId,
    int page = 1,
    int perPage = 10,
    CancellationToken ct = default);
}
