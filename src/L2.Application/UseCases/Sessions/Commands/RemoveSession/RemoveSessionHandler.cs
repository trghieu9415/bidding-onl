using L1.Core.Domain.Bidding.Entities;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Sessions.Commands.RemoveSession;

public class RemoveSessionHandler(IRepository<AuctionSession> repository)
  : IRequestHandler<RemoveSessionCommand, bool> {
  public async Task<bool> Handle(RemoveSessionCommand request, CancellationToken ct) {
    await repository.DeleteAsync(request.Id, true, ct);
    return true;
  }
}
