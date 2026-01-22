using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.Ports.Repository;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.AddSession;

public class AddSessionHandler(
  IRepository<AuctionSession> sessionRepo,
  IRepository<Auction> auctionRepo
) : IRequestHandler<AddSessionCommand, Guid> {
  public async Task<Guid> Handle(AddSessionCommand request, CancellationToken ct) {
    var session = AuctionSession.Create(request.Title)
      .SetTimeFrame(request.StartTime, request.EndTime);

    var missingIds = await auctionRepo.GetMissingIds(request.AuctionIds, ct);
    if (missingIds.Count != 0) {
      throw new AppException($"Các đấu giá sau không tồn tại: {string.Join(", ", missingIds)}", 404);
    }

    session.SyncAuctions(request.AuctionIds);
    return await sessionRepo.CreateAsync(session, ct);
  }
}