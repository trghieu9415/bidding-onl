using L2.Application.Exceptions;
using L2.Application.Repositories.Read;
using MediatR;

namespace L2.Application.UseCases.Sessions.Queries.GetSession;

public class GetSessionHandler(
  ISessionReadRepository sessionReadRepository,
  IAuctionReadRepository auctionReadRepository
) : IRequestHandler<GetSessionQuery, GetSessionResult> {
  public async Task<GetSessionResult> Handle(GetSessionQuery request, CancellationToken ct) {
    var session =
      await sessionReadRepository.GetByIdAsync(request.Id, ct)
      ?? throw new WorkflowException("Phiên đấu giá không tồn tại hoặc đã bị xóa", 404);

    var auctions = (await auctionReadRepository.GetAsync(a => a.SessionId == session.Id, ct: ct)).entities;
    return new GetSessionResult(session, auctions);
  }
}
