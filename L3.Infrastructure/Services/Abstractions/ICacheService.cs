namespace L3.Infrastructure.Services.Abstractions;

public interface ICacheService {
  Task<T?> GetAsync<T>(string key, CancellationToken ct);
  Task SetAsync<T>(string key, T value, TimeSpan? expiration, CancellationToken ct);
  Task RemoveAsync(string key, CancellationToken ct);
  Task BlacklistAsync(string jti, TimeSpan duration, CancellationToken ct);
  Task<bool> IsBlacklistedAsync(string jti, CancellationToken ct);
  Task SyncSecurityStampAsync(Guid userId, string securityStamp, CancellationToken ct);
  Task<string?> GetSecurityStampAsync(Guid userId, CancellationToken ct);
}

public static class CacheKeys {
  public static string BlackList(string jti) {
    return $"blacklist:{jti}";
  }

  public static string UserStamp(Guid id) {
    return $"user:{id}:stamp";
  }
}
