using Microsoft.Extensions.Caching.Distributed;

namespace Tests.Integration.TestSupport;

internal sealed class InMemoryDistributedCache : IDistributedCache {
  private readonly Dictionary<string, byte[]> _store = new();

  public Dictionary<string, DistributedCacheEntryOptions> OptionsByKey { get; } = new();

  public byte[]? Get(string key) {
    return _store.TryGetValue(key, out var bytes) ? bytes : null;
  }

  public Task<byte[]?> GetAsync(string key, CancellationToken token = default) {
    return Task.FromResult(Get(key));
  }

  public void Refresh(string key) {
  }

  public Task RefreshAsync(string key, CancellationToken token = default) {
    return Task.CompletedTask;
  }

  public void Remove(string key) {
    _store.Remove(key);
    OptionsByKey.Remove(key);
  }

  public Task RemoveAsync(string key, CancellationToken token = default) {
    Remove(key);
    return Task.CompletedTask;
  }

  public void Set(string key, byte[] value, DistributedCacheEntryOptions options) {
    _store[key] = value;
    OptionsByKey[key] = options;
  }

  public Task SetAsync(
    string key,
    byte[] value,
    DistributedCacheEntryOptions options,
    CancellationToken token = default
  ) {
    Set(key, value, options);
    return Task.CompletedTask;
  }
}
