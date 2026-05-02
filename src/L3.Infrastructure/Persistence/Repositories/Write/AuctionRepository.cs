using L1.Core.Domain.Bidding.Entities;
using L2.Application.Repositories.Write;

namespace L3.Infrastructure.Persistence.Repositories.Write;

public class AuctionRepository(AppDbContext dbContext) : EfRepository<Auction>(dbContext), IAuctionRepository {
  private readonly AppDbContext _dbContext = dbContext;

  public async Task AddBidAsync(Bid bid, CancellationToken ct = default) {
    await _dbContext.Set<Bid>().AddAsync(bid, ct);
  }
}
