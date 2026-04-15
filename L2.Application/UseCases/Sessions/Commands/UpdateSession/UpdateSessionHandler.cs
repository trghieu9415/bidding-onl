using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Sessions.Commands.UpdateSession;

public class UpdateSessionHandler(IRepository<AuctionSession> repository)
  : IRequestHandler<UpdateSessionCommand, Unit> {
  public async Task<Unit> Handle(UpdateSessionCommand request, CancellationToken ct) {
    var session = await repository.GetByIdAsync(request.Id, ct)
                  ?? throw new WorkflowException("Không tìm thấy phiên đấu giá", 404);

    session.Update(request.Title)
      .SetTimeFrame(request.StartTime, request.EndTime);

    await repository.UpdateAsync(session, ct);
    return Unit.Value;
  }
}
