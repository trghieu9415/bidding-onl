using AutoMapper;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Ports.Repository;
using MediatR;

namespace L2.Application.UseCases.Bidding.Bidder.GetSessions;

public class GetSessionsHandler(IReadRepository<AuctionSession> readRepository, IMapper mapper)
  : IRequestHandler<GetSessionsQuery, GetSessionsResult> {
  public async Task<GetSessionsResult> Handle(GetSessionsQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetAsync(
      request.SieveModel,
      x => x.Status == SessionStatus.Published || x.Status == SessionStatus.Live,
      ct: ct
    );

    var dtos = mapper.Map<List<AuctionSessionDto>>(entities);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);

    return new GetSessionsResult(dtos, meta);
  }
}