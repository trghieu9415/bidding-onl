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
