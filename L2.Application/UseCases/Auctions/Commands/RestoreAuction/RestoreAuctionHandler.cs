using L1.Core.Domain.Bidding.Entities;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Auctions.Commands.RestoreAuction;

public class RestoreAuctionHandler(IRepository<Auction> repository)
  : IRequestHandler<RestoreAuctionCommand, Unit> {
  public async Task<Unit> Handle(RestoreAuctionCommand request, CancellationToken ct) {
    await repository.RestoreAsync(request.Id, ct);
    return Unit.Value;
  }
}
