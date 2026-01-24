using AutoMapper;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Ports.Repository;
using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Bidding.Bidder.GetBiddingActivity;

public class GetBiddingActivityHandler(
  IReadRepository<Auction> auctionReadRepo,
  ICurrentUser currentUser,
  IMapper mapper
) : IRequestHandler<GetBiddingActivityQuery, GetBiddingActivityResult> {
  public async Task<GetBiddingActivityResult> Handle(GetBiddingActivityQuery request, CancellationToken ct) {
    var userId = currentUser.User.Id;

    var (total, entities) = await auctionReadRepo.GetAsync(
      request.SieveModel,
      x => x.Bids.Any(b => b.BidderId == userId),
      ct: ct
    );

    var dtos = mapper.Map<List<AuctionDto>>(entities);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);

    return new GetBiddingActivityResult(dtos, meta);
  }
}
