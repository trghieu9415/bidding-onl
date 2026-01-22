using AutoMapper;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Ports.Repository;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.GetAuctions;

public class GetAuctionsHandler(IReadRepository<Auction> readRepository, IMapper mapper)
  : IRequestHandler<GetAuctionsQuery, GetAuctionsResult> {
  public async Task<GetAuctionsResult> Handle(GetAuctionsQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetAsync(request.SieveModel, ct: ct);
    var dtos = mapper.Map<List<AuctionDto>>(entities);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetAuctionsResult(dtos, meta);
  }
}