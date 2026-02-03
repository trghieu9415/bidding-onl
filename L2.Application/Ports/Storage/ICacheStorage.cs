namespace L2.Application.Ports.Storage;

public interface ICacheStorage {
  Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
  Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default);
  Task RemoveAsync(string key, CancellationToken ct = default);
  Task BlacklistAsync(string key, TimeSpan duration, CancellationToken ct = default);
  Task<bool> IsBlacklistedAsync(string key, CancellationToken ct = default);
}
