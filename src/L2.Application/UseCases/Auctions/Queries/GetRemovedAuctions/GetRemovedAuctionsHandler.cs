using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Auctions.Queries.GetRemovedAuctions;

public class GetRemovedAuctionsHandler(IReadRepository<Auction, AuctionDto> readRepository)
  : IRequestHandler<GetRemovedAuctionsQuery, GetRemovedAuctionsResult> {
  public async Task<GetRemovedAuctionsResult> Handle(GetRemovedAuctionsQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetDeletedAsync(filter: request.Filter, ct: ct);
    var meta = Meta.Create(request.Filter.Page, request.Filter.PerPage, total);
    return new GetRemovedAuctionsResult(entities, meta);
  }
}
