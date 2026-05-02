using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Models;
using L2.Application.UseCases.Sessions.Queries.GetSessions;
using Tests.Unit.L2.Application.UseCases.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Sessions;

public class GetSessionsHandlerTests {
  [Fact]
  public async Task Handle_AppliesPublishedOrLiveCriteriaAndReturnsMeta() {
    var sessions = new List<AuctionSessionDto> { new() { Id = Guid.NewGuid(), Title = "Published" } };
    var filter = new SessionFilter { Page = 1, PerPage = 10 };
    var readRepo = new StubReadRepository<AuctionSession, AuctionSessionDto> { GetAsyncResult = (3, sessions) };
    var handler = new GetSessionsHandler(readRepo);

    var result = await handler.Handle(
      new GetSessionsQuery(filter),
      TestContext.Current.CancellationToken
    );

    Assert.Equal(sessions, result.Sessions);
    Assert.Equal(Meta.Create(1, 10, 3), result.Meta);
    Assert.NotNull(readRepo.LastCriteria);

    var criteria = readRepo.LastCriteria!;
    var published = AuctionSession.Create("Published", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
    published.Publish();
    var live = AuctionSession.Create("Live", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
    live.Start();
    var closed = AuctionSession.Create("Closed", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
    closed.Close();

    Assert.True(criteria.Compile()(published));
    Assert.True(criteria.Compile()(live));
    Assert.False(criteria.Compile()(closed));
  }
}
