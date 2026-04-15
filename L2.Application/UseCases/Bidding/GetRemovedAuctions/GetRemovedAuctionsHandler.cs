using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Bidding.GetRemovedAuctions;

public class GetRemovedAuctionsHandler(IReadRepository<Auction, AuctionDto> readRepository)
  : IRequestHandler<GetRemovedAuctionsQuery, GetRemovedAuctionsResult> {
  public async Task<GetRemovedAuctionsResult> Handle(GetRemovedAuctionsQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetDeletedAsync(sieveModel: request.SieveModel, ct: ct);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetRemovedAuctionsResult(entities, meta);
  }
}
