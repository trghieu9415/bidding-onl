using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.UseCases.System.EndSession;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.System;

public class EndSessionHandlerTests {
  [Fact]
  public async Task Handle_ClosesSessionAndEndsAllAuctions() {
    var session = AuctionSession.Create("Morning Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
    var auction1 = Auction.Create(Guid.NewGuid(), session.Id, 100m, 10m, 200m);
    var auction2 = Auction.Create(Guid.NewGuid(), session.Id, 100m, 10m, 200m);
    auction1.Start();
    auction2.Start();
    session.SyncAuctions([auction1.Id, auction2.Id]);
    var sessionRepo = new StubRepository<AuctionSession> { EntityByIdResult = session };
    var auctionRepo = new StubRepository<Auction> { ByKeysResult = [auction1, auction2] };
    var handler = new EndSessionHandler(sessionRepo, auctionRepo);

    var result = await handler.Handle(new EndSessionCommand(session.Id), TestContext.Current.CancellationToken);

    Assert.True(result);
    Assert.Equal(SessionStatus.Closed, session.Status);
    Assert.Equal(AuctionStatus.EndedUnsold, auction1.Status);
    Assert.Equal(AuctionStatus.EndedUnsold, auction2.Status);
  }
}
