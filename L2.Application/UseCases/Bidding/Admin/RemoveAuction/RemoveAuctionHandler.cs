using L1.Core.Domain.Bidding.Entities;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.RemoveAuction;

public class RemoveAuctionHandler(IRepository<Auction> repository)
  : IRequestHandler<RemoveAuctionCommand, Unit> {
  public async Task<Unit> Handle(RemoveAuctionCommand request, CancellationToken ct) {
    await repository.DeleteAsync(request.Id, true, ct);
    return Unit.Value;
  }
}
