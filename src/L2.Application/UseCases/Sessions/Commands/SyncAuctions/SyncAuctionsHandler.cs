using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Sessions.Commands.SyncAuctions;

public class SyncAuctionsHandler(
  IRepository<AuctionSession> sessionRepo,
  IRepository<Auction> auctionRepo
) : IRequestHandler<SyncAuctionsCommand, bool> {
  public async Task<bool> Handle(SyncAuctionsCommand request, CancellationToken ct) {
    var session =
      await sessionRepo.GetByIdAsync(request.Id, ct)
      ?? throw new WorkflowException("Phiên không tồn tại", 404);

    var missingIds = await auctionRepo.GetMissingIdsAsync(
      request.AuctionIds,
      a => a.Status == AuctionStatus.Active || a.Status == AuctionStatus.EndedUnsold,
      ct
    );
    if (missingIds.Count != 0) {
      throw new WorkflowException($"Các đấu giá sau không tồn tại: {string.Join(", ", missingIds)}", 404);
    }

    session.SyncAuctions(request.AuctionIds);
    await sessionRepo.UpdateAsync(session, ct);
    return true;
  }
}
