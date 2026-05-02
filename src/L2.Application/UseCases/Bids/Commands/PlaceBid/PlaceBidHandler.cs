using L2.Application.Exceptions;
using L2.Application.Repositories.Write;
using MediatR;

namespace L2.Application.UseCases.Bids.Commands.PlaceBid;

public class PlaceBidHandler(
  IAuctionRepository repository
) : IRequestHandler<PlaceBidCommand, Guid> {
  public async Task<Guid> Handle(PlaceBidCommand request, CancellationToken ct) {
    var auction =
      await repository.GetByIdAsync(request.AuctionId, ct)
      ?? throw new WorkflowException("Cuộc đấu giá không tồn tại", 404);

    var bid = auction.PlaceBid(request.UserId, request.UserFullName, request.Data.Amount);

    await repository.AddBidAsync(bid, ct);
    await repository.UpdateAsync(auction, ct);
    return bid.Id;
  }
}
