using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.UseCases.Sessions.Commands.AddSession;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Sessions;

public class AddSessionHandlerTests {
  [Fact]
  public async Task Handle_WhenMissingAuctionsExist_ThrowsWorkflowException() {
    var missingIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
    var auctionRepo = new StubRepository<Auction> { MissingIdsResult = missingIds };
    var handler = new AddSessionHandler(new StubRepository<AuctionSession>(), auctionRepo);

    var exception = await Assert.ThrowsAsync<WorkflowException>(async () =>
      await handler.Handle(
        new AddSessionCommand("Morning Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), []),
        TestContext.Current.CancellationToken
      ));

    Assert.Equal(404, exception.StatusCode);
    Assert.Contains(missingIds[0].ToString(), exception.Message);
  }

  [Fact]
  public async Task Handle_WhenValid_CreatesSessionWithAuctionIds() {
    var sessionRepo = new StubRepository<AuctionSession> { CreateResult = Guid.NewGuid() };
    var auctionIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
    var handler = new AddSessionHandler(sessionRepo, new StubRepository<Auction>());

    var result = await handler.Handle(
      new AddSessionCommand("Morning Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), auctionIds),
      TestContext.Current.CancellationToken
    );

    Assert.Equal(sessionRepo.CreateResult, result);
    var createdSession = Assert.IsType<AuctionSession>(sessionRepo.CreatedEntity);
    Assert.Equal("Morning Session", createdSession.Title);
    Assert.Equal(auctionIds, createdSession.AuctionIds);
  }
}
