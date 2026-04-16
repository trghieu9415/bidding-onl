using L2.Application.DTOs;
using L2.Application.Ports.Cache;
using L2.Application.Repositories.Read;
using L3.Infrastructure.Services.Abstractions;
using Medallion.Threading;

namespace L3.Infrastructure.Adapters.Cache;

public class BusinessCache(
  ICacheService cache,
  ISessionReadRepository auctionSessionRepository,
  IDistributedLockProvider lockProvider
) : IBusinessCache {
  public async Task<List<AuctionSessionDto>?> GetCurrentSessionsAsync(CancellationToken ct) {
    return await GetOrSetAsync(
      BusinessKeys.CurrentSession,
      async () => await auctionSessionRepository.GetCurrentSessionsAsync(ct),
      TimeSpan.FromDays(7), ct
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

    var lockKey = $"lock:{key}";

    await using var handle = await lockProvider.TryAcquireLockAsync(lockKey, TimeSpan.FromSeconds(5), ct);

    if (handle == null) {
      throw new TimeoutException($"Không thể lấy được khóa phân tán để làm mới cache cho key: {key}");
    }

    cachedData = await cache.GetAsync<T>(key, ct);
    if (cachedData != null) {
      return cachedData;
    }

    var data = await fetchLogic();
    if (data == null) {
      return data;
    }

    var jitter = TimeSpan.FromSeconds(Random.Shared.Next(0, 60));
    await cache.SetAsync(key, data, expiration.Add(jitter), ct);
    return data;
  }
}
