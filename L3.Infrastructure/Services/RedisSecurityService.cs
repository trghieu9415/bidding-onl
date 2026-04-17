using L3.Infrastructure.Services.Abstractions;
using StackExchange.Redis;

namespace L3.Infrastructure.Services;

public class SecurityService(IConnectionMultiplexer redis) : ISecurityService {
  private readonly IDatabase _db = redis.GetDatabase();

  public async Task BlacklistAsync(string jti, TimeSpan duration, CancellationToken ct = default) {
    await _db.StringSetAsync(SecurityKeys.BlackList(jti), "true", duration);
  }

  public async Task<bool> IsBlacklistedAsync(string jti, CancellationToken ct = default) {
    return await _db.KeyExistsAsync(SecurityKeys.BlackList(jti));
  }

  public async Task SyncSecurityStampAsync(Guid userId, string securityStamp, CancellationToken ct = default) {
    await _db.StringSetAsync(SecurityKeys.UserStamp(userId), securityStamp, TimeSpan.FromDays(1));
  }

  public async Task<string?> GetSecurityStampAsync(Guid userId, CancellationToken ct = default) {
    var value = await _db.StringGetAsync(SecurityKeys.UserStamp(userId));
    return value.HasValue ? value.ToString() : null;
  }
}
