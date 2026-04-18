using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Sessions.Queries.GetSessions;

public class GetSessionsHandler(IReadRepository<AuctionSession, AuctionSessionDto> readRepository)
  : IRequestHandler<GetSessionsQuery, GetSessionsResult> {
  public async Task<GetSessionsResult> Handle(GetSessionsQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetAsync(
      x => x.Status == SessionStatus.Published || x.Status == SessionStatus.Live,
      request.Filter,
      ct: ct
    );

    var meta = Meta.Create(request.Filter.Page, request.Filter.PerPage, total);
    return new GetSessionsResult(entities, meta);
  }
}
