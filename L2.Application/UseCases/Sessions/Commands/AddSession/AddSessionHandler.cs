using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Sessions.Commands.AddSession;

public class AddSessionHandler(
  IRepository<AuctionSession> sessionRepo,
  IRepository<Auction> auctionRepo
) : IRequestHandler<AddSessionCommand, Guid> {
  public async Task<Guid> Handle(AddSessionCommand request, CancellationToken ct) {
    var session = AuctionSession.Create(request.Title, request.StartTime, request.EndTime);

    var missingIds = await auctionRepo.GetMissingIdsAsync(
      request.AuctionIds,
      a => a.Status == AuctionStatus.Active || a.Status == AuctionStatus.EndedUnsold,
      ct
    );
    if (missingIds.Count != 0) {
      throw new WorkflowException($"Các đấu giá sau không tồn tại: {string.Join(", ", missingIds)}", 404);
    }

    session.SyncAuctions(request.AuctionIds);
    return await sessionRepo.CreateAsync(session, ct);
  }
}
