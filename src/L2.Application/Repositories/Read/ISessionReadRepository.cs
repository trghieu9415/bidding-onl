using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;

namespace L2.Application.Repositories.Read;

public interface ISessionReadRepository : IReadRepository<AuctionSession, AuctionSessionDto> {
  Task<List<AuctionSessionDto>> GetCurrentSessionsAsync(CancellationToken ct = default);
}
