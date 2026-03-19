using L2.Application.DTOs;

namespace L2.Application.Ports.Cache;

public interface IBusinessCache {
  Task<List<AuctionSessionDto>?> GetCurrentSessionsAsync(CancellationToken ct);
}
