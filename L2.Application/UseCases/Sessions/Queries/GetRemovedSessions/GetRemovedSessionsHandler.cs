using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Sessions.Queries.GetRemovedSessions;

public class GetRemovedSessionsHandler(IReadRepository<AuctionSession, AuctionSessionDto> readRepository)
  : IRequestHandler<GetRemovedSessionsQuery, GetRemovedSessionsResult> {
  public async Task<GetRemovedSessionsResult> Handle(GetRemovedSessionsQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetDeletedAsync(sieveModel: request.SieveModel, ct: ct);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetRemovedSessionsResult(entities, meta);
  }
}
