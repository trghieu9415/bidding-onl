using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Ports.Security;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Bidding.Bidder.GetBiddingActivity;

public class GetBiddingActivityHandler(
  IReadRepository<Auction, AuctionDto> auctionReadRepo,
  ICurrentUser currentUser
) : IRequestHandler<GetBiddingActivityQuery, GetBiddingActivityResult> {
  public async Task<GetBiddingActivityResult> Handle(GetBiddingActivityQuery request, CancellationToken ct) {
    var userId = currentUser.Id;

    var (total, entities) = await auctionReadRepo.GetAsync(
      x => x.Bids.Any(b => b.BidderId == userId),
      request.SieveModel,
      ct: ct
    );

    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetBiddingActivityResult(entities, meta);
  }
}
