using L2.Application.Ports.Cache;
using MediatR;

namespace L2.Application.UseCases.Sessions.Queries.GetCurrentSession;

public class GetCurrentSessionHandler(
  IBusinessCache businessCache
) : IRequestHandler<GetCurrentSessionQuery, GetCurrentSessionResult> {
  public async Task<GetCurrentSessionResult> Handle(GetCurrentSessionQuery request, CancellationToken ct) {
    var currentSessions = await businessCache.GetCurrentSessionsAsync(ct);
    return new GetCurrentSessionResult(currentSessions);
  }
}
