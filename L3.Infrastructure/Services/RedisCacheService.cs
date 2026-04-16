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

  public async Task BlacklistAsync(string jti, TimeSpan duration, CancellationToken ct = default) {
    var options = new DistributedCacheEntryOptions {
      AbsoluteExpirationRelativeToNow = duration
    };
    await cache.SetStringAsync(CacheKeys.BlackList(jti), "true", options, ct);
  }

  public async Task<bool> IsBlacklistedAsync(string jti, CancellationToken ct = default) {
    var value = await cache.GetStringAsync(CacheKeys.BlackList(jti), ct);
    return !string.IsNullOrEmpty(value);
  }

  public async Task SyncSecurityStampAsync(Guid userId, string securityStamp, CancellationToken ct) {
    var options = new DistributedCacheEntryOptions {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
    };

    await cache.SetStringAsync(CacheKeys.UserStamp(userId), securityStamp, options, ct);
  }

  public async Task<string?> GetSecurityStampAsync(Guid userId, CancellationToken ct) {
    var value = await cache.GetStringAsync(CacheKeys.UserStamp(userId), ct);
    return value;
  }

  public async Task RemoveAsync(string key, CancellationToken ct = default) {
    await cache.RemoveAsync(key, ct);
  }
}
