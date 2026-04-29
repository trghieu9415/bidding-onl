using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Sessions.Queries.GetRemovedSessions;

public class GetRemovedSessionsHandler(IReadRepository<AuctionSession, AuctionSessionDto> readRepository)
  : IRequestHandler<GetRemovedSessionsQuery, GetRemovedSessionsResult> {
  public async Task<GetRemovedSessionsResult> Handle(GetRemovedSessionsQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetDeletedAsync(filter: request.Filter, ct: ct);
    var meta = Meta.Create(request.Filter.Page, request.Filter.PerPage, total);
    return new GetRemovedSessionsResult(entities, meta);
  }
}
