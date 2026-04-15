using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Ports.Security;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Auctions.Queries.GetWonAuctions;

public class GetWonAuctionsHandler(
  IReadRepository<Auction, AuctionDto> auctionReadRepo,
  ICurrentUser currentUser
) : IRequestHandler<GetWonAuctionsQuery, GetWonAuctionsResult> {
  public async Task<GetWonAuctionsResult> Handle(GetWonAuctionsQuery request, CancellationToken ct) {
    var userId = currentUser.Id;

    var (total, entities) = await auctionReadRepo.GetAsync(
      x => x.Status == AuctionStatus.EndedSold &&
           x.Bids.Any(b => b.Id == x.WinningBidId && b.BidderId == userId),
      request.SieveModel,
      ct: ct
    );
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetWonAuctionsResult(entities, meta);
  }
}
