namespace L3.Infrastructure.Services.Abstractions;

public interface ICacheManager {
  public Task<T?> GetOrSetAsync<T>(
    string key,
    Func<Task<T?>> fetchLogic,
    TimeSpan expiration,
    CancellationToken ct
  );

  Task RemoveAsync(string key, CancellationToken ct);
  Task RemoveByPatternAsync(string pattern, CancellationToken ct = default);
}
