using L2.Application.Models;
using L2.Application.Repositories.Read;
using MediatR;

namespace L2.Application.UseCases.Bids.Queries.GetBidHistory;

public class GetBidHistoryHandler(
  IAuctionReadRepository auctionReadRepo
)
  : IRequestHandler<GetBidHistoryQuery, GetBidHistoryResult> {
  public async Task<GetBidHistoryResult> Handle(GetBidHistoryQuery request, CancellationToken ct) {
    var (total, entities) = await auctionReadRepo.GetBidsAsync(
      request.AuctionId, request.Page, request.PerPage, ct
    );


    var meta = Meta.Create(request.Page, request.PerPage, total);
    return new GetBidHistoryResult(entities, meta);
  }
}
