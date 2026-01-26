using AutoMapper;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Ports.Repositories;
using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Bidding.Bidder.GetWonAuctions;

public class GetWonAuctionsHandler(
  IReadRepository<Auction> auctionReadRepo,
  ICurrentUser currentUser,
  IMapper mapper)
  : IRequestHandler<GetWonAuctionsQuery, GetWonAuctionsResult> {
  public async Task<GetWonAuctionsResult> Handle(GetWonAuctionsQuery request, CancellationToken ct) {
    var userId = currentUser.User.Id;

    var (total, entities) = await auctionReadRepo.GetAsync(
      request.SieveModel,
      x => x.Status == AuctionStatus.EndedSold &&
           x.Bids.Any(b => b.Id == x.WinningBidId && b.BidderId == userId),
      ct: ct
    );

    var dtos = mapper.Map<List<AuctionDto>>(entities);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);

    return new GetWonAuctionsResult(dtos, meta);
  }
}
