using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Auctions.Queries.GetAuctions;

public class GetAuctionsHandler(IReadRepository<Auction, AuctionDto> readRepository)
  : IRequestHandler<GetAuctionsQuery, GetAuctionsResult> {
  public async Task<GetAuctionsResult> Handle(
    GetAuctionsQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetAsync(filter: request.Filter, ct: ct);
    var meta = Meta.Create(request.Filter.Page, request.Filter.PerPage, total);
    return new GetAuctionsResult(entities, meta);
  }
}
