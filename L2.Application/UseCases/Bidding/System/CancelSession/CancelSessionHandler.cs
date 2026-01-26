using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.Ports.Repositories;
using MediatR;

namespace L2.Application.UseCases.Bidding.System.CancelSession;

public class CancelSessionHandler(
  IRepository<AuctionSession> sessionRepo,
  IRepository<Auction> auctionRepo
) : IRequestHandler<CancelSessionCommand, Unit> {
  public async Task<Unit> Handle(CancelSessionCommand request, CancellationToken ct) {
    var session = await sessionRepo.GetByIdAsync(request.Id, ct)
                  ?? throw new AppException("Không tìm thấy phiên đấu giá", 404);

    session.Close();
    await sessionRepo.UpdateAsync(session, ct);

    var auctions = await auctionRepo.GetByKeysAsync(session.AuctionIds.ToList(), ct: ct);
    foreach (var auction in auctions) {
      auction.Cancel();
      await auctionRepo.UpdateAsync(auction, ct);
    }

    return Unit.Value;
  }
}
