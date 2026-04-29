using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Auctions.Queries.GetAuction;

public class GetAuctionHandler(IReadRepository<Auction, AuctionDto> readRepository)
  : IRequestHandler<GetAuctionQuery, GetAuctionResult> {
  public async Task<GetAuctionResult> Handle(GetAuctionQuery request, CancellationToken ct) {
    var auction =
      await readRepository.GetByIdAsync(request.Id, ct)
      ?? throw new WorkflowException("Không tìm thấy thông tin đấu giá", 404);

    return new GetAuctionResult(auction);
  }
}
