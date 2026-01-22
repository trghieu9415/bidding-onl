using L1.Core.Domain.Bidding.Entities;
using L2.Application.Ports.Repository;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.RestoreSession;

public class RestoreSessionHandler(IRepository<AuctionSession> repository)
  : IRequestHandler<RestoreSessionCommand, Unit> {
  public async Task<Unit> Handle(RestoreSessionCommand request, CancellationToken ct) {
    await repository.RestoreAsync(request.Id, ct);
    return Unit.Value;
  }
}