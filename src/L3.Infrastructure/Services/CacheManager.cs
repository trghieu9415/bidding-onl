using L3.Infrastructure.Services.Abstractions;
using Medallion.Threading;

namespace L3.Infrastructure.Services;

public class CacheManager(
  ICacheService cache,
  IDistributedLockProvider lockProvider
) : ICacheManager {
  public async Task<T?> GetOrSetAsync<T>(
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
      throw new TimeoutException($"Server đang bận cập nhật dữ liệu cho key: {key}");
    }

    cachedData = await cache.GetAsync<T>(key, ct);
    if (cachedData != null) {
      return cachedData;
    }

    var data = await fetchLogic();
    if (data == null) {
      await cache.SetAsync(key, default(T), TimeSpan.FromMinutes(2), ct);
      return default;
    }

    var jitter = TimeSpan.FromSeconds(Random.Shared.Next(0, 60));
    await cache.SetAsync(key, data, expiration.Add(jitter), ct);
    return data;
  }

  public async Task RemoveAsync(string key, CancellationToken ct) {
    await cache.RemoveAsync(key, ct);
  }

  public async Task RemoveByPatternAsync(string pattern, CancellationToken ct = default) {
    await cache.RemoveByPatternAsync(pattern, ct);
  }
}
