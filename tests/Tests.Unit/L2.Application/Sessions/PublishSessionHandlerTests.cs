using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.UseCases.Sessions.Commands.PublishSession;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Sessions;

public class PublishSessionHandlerTests {
  [Fact]
  public async Task Handle_WhenFound_PublishesAndPersists() {
    var session = AuctionSession.Create("Morning Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
    var repo = new StubRepository<AuctionSession> { EntityByIdResult = session };
    var handler = new PublishSessionHandler(repo);

    var result = await handler.Handle(new PublishSessionCommand(session.Id), TestContext.Current.CancellationToken);

    Assert.True(result);
    Assert.Same(session, repo.UpdatedEntity);
    Assert.Equal(SessionStatus.Published, session.Status);
  }
}
