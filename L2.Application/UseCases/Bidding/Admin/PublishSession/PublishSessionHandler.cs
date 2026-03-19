using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.PublishSession;

public class PublishSessionHandler(
  IRepository<AuctionSession> repository
) : IRequestHandler<PublishSessionCommand, Unit> {
  public async Task<Unit> Handle(PublishSessionCommand request, CancellationToken ct) {
    var session = await repository.GetByIdAsync(request.Id, ct)
                  ?? throw new WorkflowException("Không tìm thấy phiên", 404);

    if (session.TimeFrame == null) {
      throw new WorkflowException("Phải thiết lập thời gian trước khi công khai phiên.");
    }

    session.Publish();
    await repository.UpdateAsync(session, ct);
    return Unit.Value;
  }
}
