using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Auctions.Queries.GetAuctions;

public class GetAuctionsHandler(IReadRepository<Auction, AuctionDto> readRepository)
  : IRequestHandler<GetAuctionsQuery, GetAuctionsResult> {
  public async Task<GetAuctionsResult> Handle(GetAuctionsQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetAsync(
      sieveModel: request.SieveModel, ct: ct);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetAuctionsResult(entities, meta);
  }
}
