using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.Ports.Repositories;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.UpdateAuction;

public class UpdateAuctionHandler(IRepository<Auction> repository)
  : IRequestHandler<UpdateAuctionCommand, Unit> {
  public async Task<Unit> Handle(UpdateAuctionCommand request, CancellationToken ct) {
    var auction = await repository.GetByIdAsync(request.Id, ct)
                  ?? throw new AppException("Không tìm thấy đấu giá", 404);

    auction.UpdateRules(request.StepPrice, request.ReservePrice);

    await repository.UpdateAsync(auction, ct);
    return Unit.Value;
  }
}
