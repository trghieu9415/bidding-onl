using AutoMapper;
using AutoMapper.QueryableExtensions;
using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;
using L2.Application.Repositories.Read;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace L3.Infrastructure.Persistence.Repositories.Read;

public class OrderReadRepository(
  AppDbContext dbContext,
  IMapper mapper,
  ISieveProcessor sieveProcessor
) : EfReadRepository<Order, OrderDto>(dbContext, mapper, sieveProcessor), IOrderReadRepository {
  private readonly IMapper _mapper = mapper;

  public virtual async Task<OrderDto?> GetByAuctionIdAsync(Guid auctionId, CancellationToken ct = default) {
    return await DbSet
      .AsNoTracking()
      .Where(x => x.AuctionId == auctionId && !x.IsDeleted)
      .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
      .FirstOrDefaultAsync(ct);
  }
}
