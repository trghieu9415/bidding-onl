using L2.Application.DTOs;
using L2.Application.Ports.Cache;
using L2.Application.Repositories.Read;
using L3.Infrastructure.Services.Abstractions;

namespace L3.Infrastructure.Adapters.Cache;

public class BusinessCache(
  ICacheService cache,
  ISessionReadRepository auctionSessionRepository
) : IBusinessCache {
  public async Task<List<AuctionSessionDto>?> GetCurrentSessionsAsync(CancellationToken ct) {
    return await GetOrSetAsync(
      BusinessKeys.CurrentSession,
      async () => await auctionSessionRepository.GetCurrentSessionsAsync(ct),
      TimeSpan.FromDays(7),
      ct
    );
  }

  // NOTE: ========== [Helper] ==========
  private async Task<T?> GetOrSetAsync<T>(
    string key,
    Func<Task<T?>> fetchLogic,
    TimeSpan expiration,
    CancellationToken ct
  ) {
    var cachedData = await cache.GetAsync<T>(key, ct);
    if (cachedData != null) {
      return cachedData;
    }

    var data = await fetchLogic();
    if (data != null) {
      await cache.SetAsync(key, data, expiration, ct);
    }

    return data;
  }
}
