using FluentAssertions;
using L2.Application.DTOs;
using L2.Application.Ports.Cache;
using L2.Application.UseCases.Sessions.Queries.GetCurrentSession;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Sessions.Queries;

public class GetCurrentSessionHandlerTests {
  private readonly IBusinessCache _businessCache = Substitute.For<IBusinessCache>();
  private readonly GetCurrentSessionHandler _sut;

  public GetCurrentSessionHandlerTests() {
    _sut = new GetCurrentSessionHandler(_businessCache);
  }

  [Fact]
  public async Task Handle_Should_ReturnCurrentSessions() {
    var sessions = new List<AuctionSessionDto> { UseCaseTestData.CreateAuctionSessionDto() };

    _businessCache.GetCurrentSessionsAsync(CancellationToken.None).Returns(sessions);

    var result = await _sut.Handle(new GetCurrentSessionQuery(), CancellationToken.None);

    result.Sessions.Should().BeSameAs(sessions);
  }
}
