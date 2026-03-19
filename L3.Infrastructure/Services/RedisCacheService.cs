using System.Text.Json;
using L2.Application.Constants;
using L3.Infrastructure.Services.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace L3.Infrastructure.Services;

public class RedisCacheService(IDistributedCache cache) : ICacheService {
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

  public async Task BlacklistAsync(string jti, TimeSpan duration, CancellationToken ct = default) {
    var options = new DistributedCacheEntryOptions {
      AbsoluteExpirationRelativeToNow = duration
    };
    await cache.SetStringAsync(CacheTags.BlackList(jti), "true", options, ct);
  }

  public async Task<bool> IsBlacklistedAsync(string jti, CancellationToken ct = default) {
    var value = await cache.GetStringAsync(CacheTags.BlackList(jti), ct);
    return !string.IsNullOrEmpty(value);
  }

  public async Task SyncSecurityStampAsync(Guid userId, string securityStamp, CancellationToken ct) {
    var options = new DistributedCacheEntryOptions {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
    };

    await cache.SetStringAsync(CacheTags.UserStamp(userId), securityStamp, options, ct);
  }

  public async Task<string?> GetSecurityStampAsync(Guid userId, CancellationToken ct) {
    var value = await cache.GetStringAsync(CacheTags.UserStamp(userId), ct);
    return value;
  }

  public async Task RemoveAsync(string key, CancellationToken ct = default) {
    await cache.RemoveAsync(key, ct);
  }
}
