using L1.Core.Domain.Bidding.Entities;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Sessions.Commands.RestoreSession;

public class RestoreSessionHandler(IRepository<AuctionSession> repository)
  : IRequestHandler<RestoreSessionCommand, bool> {
  public async Task<bool> Handle(RestoreSessionCommand request, CancellationToken ct) {
    await repository.RestoreAsync(request.Id, ct);
    return true;
  }
}
