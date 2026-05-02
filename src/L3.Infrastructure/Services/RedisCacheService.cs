using System.Text.Json;
using L3.Infrastructure.Options;
using L3.Infrastructure.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace L3.Infrastructure.Services;

public sealed class RedisCacheService(
  [FromKeyedServices(RedisSettings.MutexKeys.Cache)]
  IConnectionMultiplexer redis,
  RedisSettings redisSettings
) : ICacheService {
  private const int DeleteBatchSize = 1000;
  private readonly IDatabase _db = redis.GetDatabase();
  private readonly string _instanceName = redisSettings.Keys.Cache;

  public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) {
    var value = await _db.StringGetAsync(PrepareKey(key));
    return !value.HasValue ? default : JsonSerializer.Deserialize<T>(value!);
  }

  public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default) {
    var serialized = JsonSerializer.Serialize(value);

    await _db.StringSetAsync(
      PrepareKey(key),
      serialized,
      expiration ?? TimeSpan.FromHours(1)
    );
  }

  public async Task RemoveAsync(string key, CancellationToken ct = default) {
    await _db.KeyDeleteAsync(PrepareKey(key));
  }

  public async Task RemoveByPatternAsync(string pattern, CancellationToken ct = default) {
    var server = GetMasterServer();
    var batch = new List<RedisKey>(DeleteBatchSize);
    var fullPattern = $"{PrepareKey(pattern)}*";

    await foreach (var key in server.KeysAsync(pattern: fullPattern).WithCancellation(ct)) {
      batch.Add(key);

      if (batch.Count < DeleteBatchSize) {
        continue;
      }

      await _db.KeyDeleteAsync([..batch]);
      batch.Clear();
    }

    if (batch.Count > 0) {
      await _db.KeyDeleteAsync([..batch]);
    }
  }

  private RedisKey PrepareKey(string key) {
    return string.IsNullOrWhiteSpace(key)
      ? throw new ArgumentException("Key không được để trống!", nameof(key))
      : $"{_instanceName}:{key}";
  }

  private IServer GetMasterServer() {
    var endpoint = redis.GetEndPoints();
    foreach (var ep in endpoint) {
      var server = redis.GetServer(ep);
      if (server is { IsReplica: false, IsConnected: true }) {
        return server;
      }
    }

    throw new InvalidOperationException("Không tìm thấy Master node nào đang online để thực hiện lệnh xóa.");
  }
}
