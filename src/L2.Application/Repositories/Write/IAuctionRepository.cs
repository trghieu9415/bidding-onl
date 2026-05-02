using L1.Core.Domain.Bidding.Entities;

namespace L2.Application.Repositories.Write;

public interface IAuctionRepository : IRepository<Auction> {
  Task AddBidAsync(Bid bid, CancellationToken ct = default);
}
