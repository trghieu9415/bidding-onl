using L3.Infrastructure.Options;

namespace L3.Infrastructure.Services.Abstractions;

public interface ISecurityService {
  Task BlacklistAsync(string jti, TimeSpan duration, CancellationToken ct = default);
  Task<bool> IsBlacklistedAsync(string jti, CancellationToken ct = default);
  Task SyncSecurityStampAsync(Guid userId, string securityStamp, CancellationToken ct = default);
  Task<string?> GetSecurityStampAsync(Guid userId, CancellationToken ct = default);
}

public static class SecurityKeys {
  public static string BlackList(string jti) {
    return $"blacklist:{jti}";
  }

  public static string UserStamp(Guid id) {
    return $"user:{id}:stamp";
  }
}
