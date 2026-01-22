using L1.Core.Domain.Bidding.Entities;
using L2.Application.Ports.Repository;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.RestoreAuction;

public class RestoreAuctionHandler(IRepository<Auction> repository)
  : IRequestHandler<RestoreAuctionCommand, Unit> {
  public async Task<Unit> Handle(RestoreAuctionCommand request, CancellationToken ct) {
    await repository.RestoreAsync(request.Id, ct);
    return Unit.Value;
  }
}