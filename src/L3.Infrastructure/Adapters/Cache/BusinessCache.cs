using L2.Application.DTOs;
using L2.Application.Ports.Cache;
using L2.Application.Repositories.Read;
using L3.Infrastructure.Services.Abstractions;

namespace L3.Infrastructure.Adapters.Cache;

public class BusinessCache(
  ISessionReadRepository auctionSessionRepository,
  ICacheManager cacheManager
) : IBusinessCache {
  public async Task<List<AuctionSessionDto>> GetCurrentSessionsAsync(CancellationToken ct) {
    var sessions = await cacheManager.GetOrSetAsync(
      "current-session",
      async () => await auctionSessionRepository.GetCurrentSessionsAsync(ct),
      TimeSpan.FromDays(7), ct
    );

    return sessions ?? [];
  }

  public async Task RemoveCurrentSessionsAsync(CancellationToken ct) {
    await cacheManager.RemoveAsync("current-session", ct);
  }
}
