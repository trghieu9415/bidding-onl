using AutoMapper;
using AutoMapper.QueryableExtensions;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Repositories.Read;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace L3.Infrastructure.Persistence.Repositories.Read;

public class AuctionReadRepository(
  AppDbContext dbContext,
  IMapper mapper,
  ISieveProcessor sieveProcessor
) : EfReadRepository<Auction, AuctionDto>(dbContext, mapper, sieveProcessor), IAuctionReadRepository {
  private readonly IMapper _mapper = mapper;

  public async Task<(int total, List<BidDto> bids)> GetBidsAsync(
    Guid auctionId, int page = 1, int pageSize = 10,
    CancellationToken ct = default
  ) {
    var query = DbSet.AsNoTracking()
      .Where(x => x.Id == auctionId);

    var totalCount = await query.CountAsync(ct);

    var items = await query
      .OrderByDescending(x => x.CreatedAt)
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .ProjectTo<BidDto>(_mapper.ConfigurationProvider)
      .ToListAsync(ct);

    return (totalCount, items);
  }
}
