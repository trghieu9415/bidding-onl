using L1.Core.Domain.Bidding.Entities;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Auctions.Commands.RemoveAuction;

public class RemoveAuctionHandler(IRepository<Auction> repository)
  : IRequestHandler<RemoveAuctionCommand, bool> {
  public async Task<bool> Handle(RemoveAuctionCommand request, CancellationToken ct) {
    await repository.DeleteAsync(request.Id, true, ct);
    return true;
  }
}
