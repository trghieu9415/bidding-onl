using L1.Core.Domain.Bidding.Entities;
using L2.Application.UseCases.Sessions.Commands.SyncAuctions;
using Tests.Unit.L2.Application.UseCases.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Sessions;

public class SyncAuctionsHandlerTests {
  [Fact]
  public async Task Handle_WhenValid_SynchronizesAuctionIds() {
    var session = AuctionSession.Create("Morning Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
    var sessionRepo = new StubRepository<AuctionSession> { EntityByIdResult = session };
    var auctionIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
    var handler = new SyncAuctionsHandler(sessionRepo, new StubRepository<Auction>());

    var result = await handler.Handle(
      new SyncAuctionsCommand(session.Id, auctionIds),
      TestContext.Current.CancellationToken
    );

    Assert.True(result);
    Assert.Same(session, sessionRepo.UpdatedEntity);
    Assert.Equal(auctionIds, session.AuctionIds);
  }
}
