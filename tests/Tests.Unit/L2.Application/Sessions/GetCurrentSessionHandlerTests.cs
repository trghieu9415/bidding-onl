using L2.Application.DTOs;
using L2.Application.UseCases.Sessions.Queries.GetCurrentSession;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Sessions;

public class GetCurrentSessionHandlerTests {
  [Fact]
  public async Task Handle_ReturnsCachedSessions() {
    var sessions = new List<AuctionSessionDto> { new() { Id = Guid.NewGuid(), Title = "Current Session" } };
    var cache = new StubBusinessCache { CurrentSessionsResult = sessions };
    var handler = new GetCurrentSessionHandler(cache);

    var result = await handler.Handle(new GetCurrentSessionQuery(), TestContext.Current.CancellationToken);

    Assert.Equal(sessions, result.Sessions);
  }
}
