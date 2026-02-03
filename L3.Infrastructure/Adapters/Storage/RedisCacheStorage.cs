using System.Text.Json;
using L2.Application.Ports.Storage;
using Microsoft.Extensions.Caching.Distributed;

namespace L3.Infrastructure.Adapters.Storage;

public class RedisCacheStorage(IDistributedCache cache) : ICacheStorage {
  public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) {
    var cachedData = await cache.GetStringAsync(key, ct);
    return string.IsNullOrEmpty(cachedData) ? default : JsonSerializer.Deserialize<T>(cachedData);
  }

  public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default) {
    var options = new DistributedCacheEntryOptions {
      AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(60)
    };

    var jsonData = JsonSerializer.Serialize(value);
    await cache.SetStringAsync(key, jsonData, options, ct);
  }

  public async Task RemoveAsync(string key, CancellationToken ct = default) {
    await cache.RemoveAsync(key, ct);
  }

  public async Task BlacklistAsync(string key, TimeSpan duration, CancellationToken ct = default) {
    var options = new DistributedCacheEntryOptions {
      AbsoluteExpirationRelativeToNow = duration
    };
    await cache.SetStringAsync($"blacklist:{key}", "true", options, ct);
  }

  public async Task<bool> IsBlacklistedAsync(string key, CancellationToken ct = default) {
    var value = await cache.GetStringAsync($"blacklist:{key}", ct);
    return !string.IsNullOrEmpty(value);
  }
}
