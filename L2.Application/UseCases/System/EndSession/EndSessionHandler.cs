using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.System.EndSession;

public class EndSessionHandler(
  IRepository<AuctionSession> sessionRepo,
  IRepository<Auction> auctionRepo
) : IRequestHandler<EndSessionCommand, Unit> {
  public async Task<Unit> Handle(EndSessionCommand request, CancellationToken ct) {
    var session = await sessionRepo.GetByIdAsync(request.Id, ct)
                  ?? throw new WorkflowException("Không tìm thấy phiên đấu giá để kết thúc", 404);

    session.Close();
    await sessionRepo.UpdateAsync(session, ct);

    var auctions = await auctionRepo.GetByKeysAsync(
      session.AuctionIds.ToList(),
      [b => b.Bids],
      ct
    );
    foreach (var auction in auctions) {
      auction.End();
      await auctionRepo.UpdateAsync(auction, ct);
    }

    return Unit.Value;
  }
}
