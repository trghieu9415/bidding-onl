using AutoMapper;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Ports.Repository;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.GetRemovedSessions;

public class GetRemovedSessionsHandler(IReadRepository<AuctionSession> readRepository, IMapper mapper)
  : IRequestHandler<GetRemovedSessionsQuery, GetRemovedSessionsResult> {
  public async Task<GetRemovedSessionsResult> Handle(GetRemovedSessionsQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetDeletedAsync(request.SieveModel, ct: ct);
    var dtos = mapper.Map<List<AuctionSessionDto>>(entities);

    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetRemovedSessionsResult(dtos, meta);
  }
}