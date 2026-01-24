using AutoMapper;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Ports.Repository;
using MediatR;

namespace L2.Application.UseCases.Bidding.Bidder.GetBidHistory;

public class GetBidHistoryHandler(IReadRepository<Auction> auctionReadRepo, IMapper mapper)
  : IRequestHandler<GetBidHistoryQuery, GetBidHistoryResult> {
  public async Task<GetBidHistoryResult> Handle(GetBidHistoryQuery request, CancellationToken ct) {
    var (total, entities) = await auctionReadRepo.GetAsync(
      request.SieveModel,
      x => x.Id == request.AuctionId,
      [x => x.Bids],
      ct
    );

    var auction = entities.FirstOrDefault();
    if (auction == null) {
      return new GetBidHistoryResult([], Meta.Create(1, 10, 0));
    }

    var bids = mapper.Map<List<BidDto>>(auction.Bids.OrderByDescending(x => x.TimePoint));
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, auction.Bids.Count);

    return new GetBidHistoryResult(bids, meta);
  }
}
