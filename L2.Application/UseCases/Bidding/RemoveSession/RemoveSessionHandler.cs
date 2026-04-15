using L1.Core.Domain.Bidding.Entities;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Bidding.RemoveSession;

public class RemoveSessionHandler(IRepository<AuctionSession> repository)
  : IRequestHandler<RemoveSessionCommand, Unit> {
  public async Task<Unit> Handle(RemoveSessionCommand request, CancellationToken ct) {
    await repository.DeleteAsync(request.Id, true, ct);
    return Unit.Value;
  }
}
