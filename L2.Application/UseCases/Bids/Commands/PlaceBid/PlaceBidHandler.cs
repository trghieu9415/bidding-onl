using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Bids.Commands.PlaceBid;

public class PlaceBidHandler(
  IRepository<Auction> repository
) : IRequestHandler<PlaceBidCommand, Guid> {
  public async Task<Guid> Handle(PlaceBidCommand request, CancellationToken ct) {
    var auction =
      await repository.GetByIdAsync(request.AuctionId, ct)
      ?? throw new WorkflowException("Cuộc đấu giá không tồn tại", 404);

    auction.PlaceBid(request.UserId, request.UserFullName, request.Data.Amount);

    await repository.UpdateAsync(auction, ct);
    return auction.Bids.Last().Id;
  }
}
