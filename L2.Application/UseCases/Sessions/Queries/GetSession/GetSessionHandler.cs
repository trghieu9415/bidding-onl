using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Sessions.Queries.GetSession;

public class GetSessionHandler(
  IReadRepository<AuctionSession, AuctionSessionDto> readRepository
) : IRequestHandler<GetSessionQuery, GetSessionResult> {
  public async Task<GetSessionResult> Handle(GetSessionQuery request, CancellationToken ct) {
    var session =
      await readRepository.GetByIdAsync(request.Id, ct)
      ?? throw new WorkflowException("Phiên đấu giá không tồn tại hoặc đã bị xóa", 404);


    return new GetSessionResult(session);
  }
}
