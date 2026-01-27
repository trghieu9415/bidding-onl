using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.Ports.Repositories;
using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Bidding.Bidder.PlaceBid;

public class PlaceBidHandler(
  IRepository<Auction> repository,
  ICurrentUser currentUser
) : IRequestHandler<PlaceBidCommand, Guid> {
  public async Task<Guid> Handle(PlaceBidCommand request, CancellationToken ct) {
    var auction = await repository.GetByIdAsync(request.AuctionId, ct)
                  ?? throw new AppException("Cuộc đấu giá không tồn tại", 404);

    auction.PlaceBid(currentUser.User.Id, request.Amount);

    await repository.UpdateAsync(auction, ct);
    return auction.Bids.Last().Id;
  }
}
