using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.Ports.Repository;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.SyncAuctions;

public class SyncAuctionsHandler(
  IRepository<AuctionSession> sessionRepo,
  IRepository<Auction> auctionRepo
) : IRequestHandler<SyncAuctionsCommand, Unit> {
  public async Task<Unit> Handle(SyncAuctionsCommand request, CancellationToken ct) {
    var session = await sessionRepo.GetByIdAsync(request.Id, ct)
                  ?? throw new AppException("Phiên không tồn tại", 404);

    var missingIds = await auctionRepo.GetMissingIds(request.AuctionIds, ct);
    if (missingIds.Count != 0) {
      throw new AppException($"Các đấu giá sau không tồn tại: {string.Join(", ", missingIds)}", 404);
    }

    session.SyncAuctions(request.AuctionIds);
    await sessionRepo.UpdateAsync(session, ct);
    return Unit.Value;
  }
}