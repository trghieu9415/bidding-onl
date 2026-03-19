using AutoMapper;
using AutoMapper.QueryableExtensions;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.DTOs;
using L2.Application.Repositories.Read;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace L3.Infrastructure.Persistence.Repositories.Read;

public class SessionReadRepository(
  AppDbContext dbContext,
  IMapper mapper,
  ISieveProcessor sieveProcessor
) : EfReadRepository<AuctionSession, AuctionSessionDto>(dbContext, mapper, sieveProcessor), ISessionReadRepository {
  private readonly IMapper _mapper = mapper;

  public async Task<List<AuctionSessionDto>> GetCurrentSessionsAsync(CancellationToken ct = default) {
    var items = await DbSet.AsNoTracking()
      .Where(x => x.Status == SessionStatus.Live)
      .OrderByDescending(x => x.CreatedAt)
      .ProjectTo<AuctionSessionDto>(_mapper.ConfigurationProvider)
      .ToListAsync(ct);
    return items;
  }
}
