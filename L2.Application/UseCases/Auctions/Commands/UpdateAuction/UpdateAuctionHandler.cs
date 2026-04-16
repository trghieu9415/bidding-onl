using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Auctions.Commands.UpdateAuction;

public class UpdateAuctionHandler(IRepository<Auction> repository)
  : IRequestHandler<UpdateAuctionCommand, bool> {
  public async Task<bool> Handle(UpdateAuctionCommand request, CancellationToken ct) {
    var auction =
      await repository.GetByIdAsync(request.Id, ct)
      ?? throw new WorkflowException("Không tìm thấy đấu giá", 404);

    var data = request.Data;

    auction.UpdateRules(data.StepPrice, data.ReservePrice);

    await repository.UpdateAsync(auction, ct);
    return true;
  }
}
