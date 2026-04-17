using System.Text.Json;
using L3.Infrastructure.Services.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace L3.Infrastructure.Services;

public class RedisCacheService(IDistributedCache cache) : ICacheService {
  public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) {
    var cachedBytes = await cache.GetAsync(key, ct);

    if (cachedBytes == null || cachedBytes.Length == 0) {
      return default;
    }

    return JsonSerializer.Deserialize<T>(cachedBytes);
  }

  public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default) {
    var options = new DistributedCacheEntryOptions {
      AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(60)
    };

    var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(value);
    await cache.SetAsync(key, jsonBytes, options, ct);
  }

  public async Task RemoveAsync(string key, CancellationToken ct = default) {
    await cache.RemoveAsync(key, ct);
  }
}
