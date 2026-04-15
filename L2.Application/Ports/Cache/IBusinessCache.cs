using L2.Application.DTOs;

namespace L2.Application.Ports.Cache;

public interface IBusinessCache {
  Task<List<AuctionSessionDto>?> GetCurrentSessionsAsync(CancellationToken ct);
}

public static class BusinessKeys {
  public const string CurrentSession = "current-session";
}
