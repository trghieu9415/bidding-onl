using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Bids.Queries.GetBiddingActivity;

public class GetBiddingActivityHandler(
  IReadRepository<Auction, AuctionDto> auctionReadRepo
) : IRequestHandler<GetBiddingActivityQuery, GetBiddingActivityResult> {
  public async Task<GetBiddingActivityResult> Handle(GetBiddingActivityQuery request, CancellationToken ct) {
    var userId = request.UserId;

    var (total, entities) = await auctionReadRepo.GetAsync(
      x => x.Bids.Any(b => b.BidderId == userId),
      request.Filter,
      ct: ct
    );

    var meta = Meta.Create(request.Filter.Page, request.Filter.PerPage, total);
    return new GetBiddingActivityResult(entities, meta);
  }
}
