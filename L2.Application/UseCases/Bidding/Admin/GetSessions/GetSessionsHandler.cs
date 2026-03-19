using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.GetSessions;

public class GetSessionsHandler(IReadRepository<AuctionSession, AuctionSessionDto> readRepository)
  : IRequestHandler<GetSessionsQuery, GetSessionsResult> {
  public async Task<GetSessionsResult> Handle(GetSessionsQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetAsync(sieveModel: request.SieveModel, ct: ct);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetSessionsResult(entities, meta);
  }
}
