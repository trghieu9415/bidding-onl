using L2.Application.DTOs;
using L2.Application.UseCases.Sessions.Queries.GetSession;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Sessions;

public class GetSessionHandlerTests {
  [Fact]
  public async Task Handle_WhenFound_ReturnsSessionAndAuctions() {
    var sessionDto = new AuctionSessionDto { Id = Guid.NewGuid(), Title = "Current Session" };
    var auctionDtos = new List<AuctionDto> { new() { Id = Guid.NewGuid() } };
    var sessionRepo = new StubSessionReadRepository { EntityByIdResult = sessionDto };
    var auctionRepo = new StubAuctionReadRepository { GetAsyncResult = (1, auctionDtos) };
    var handler = new GetSessionHandler(sessionRepo, auctionRepo);

    var result = await handler.Handle(
      new GetSessionQuery(sessionDto.Id),
      TestContext.Current.CancellationToken
    );

    Assert.Same(sessionDto, result.Session);
    Assert.Equal(auctionDtos, result.Auctions);
  }
}
