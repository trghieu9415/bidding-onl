using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.System.StartSession;

public class StartSessionHandler(
  IRepository<AuctionSession> sessionRepo,
  IRepository<Auction> auctionRepo
) : IRequestHandler<StartSessionCommand, Unit> {
  public async Task<Unit> Handle(StartSessionCommand request, CancellationToken ct) {
    var session = await sessionRepo.GetByIdAsync(request.Id, ct)
                  ?? throw new WorkflowException("Không tìm thấy phiên đấu giá để bắt đầu", 404);

    session.Start();
    await sessionRepo.UpdateAsync(session, ct);

    var auctions = await auctionRepo.GetByKeysAsync(session.AuctionIds.ToList(), ct: ct);
    foreach (var auction in auctions) {
      auction.Start();
      await auctionRepo.UpdateAsync(auction, ct);
    }

    return Unit.Value;
  }
}
