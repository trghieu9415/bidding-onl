using System.Linq.Expressions;
using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Repositories;
using L2.Application.UseCases.Sessions.Queries.GetSessions;
using NSubstitute;
using Tests.Common.Builders;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Sessions.Queries;

public class GetSessionsHandlerTests {
  private readonly IReadRepository<AuctionSession, AuctionSessionDto> _readRepository =
    Substitute.For<IReadRepository<AuctionSession, AuctionSessionDto>>();

  private readonly GetSessionsHandler _sut;

  public GetSessionsHandlerTests() {
    _sut = new GetSessionsHandler(_readRepository);
  }

  [Fact]
  public async Task Handle_Should_Filter_PublishedOrLiveSessions_And_ReturnMeta() {
    var filter = new SessionFilter { Page = 1, PerPage = 10 };
    var sessions = new List<AuctionSessionDto> { UseCaseTestData.CreateAuctionSessionDto(status: SessionStatus.Published) };
    var request = new GetSessionsQuery(filter);
    Expression<Func<AuctionSession, bool>>? capturedCriteria = null;

    _readRepository.GetAsync(
        Arg.Do<Expression<Func<AuctionSession, bool>>?>(x => capturedCriteria = x),
        filter,
        CancellationToken.None
      )
      .Returns((9, sessions));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Sessions.Should().BeSameAs(sessions);
    result.Meta.Total.Should().Be(9);
    capturedCriteria.Should().NotBeNull();

    var published = new AuctionSessionBuilder().Build();
    published.Publish();
    var live = new AuctionSessionBuilder().Build();
    live.Start();
    var draft = new AuctionSessionBuilder().Build();
    var predicate = capturedCriteria!.Compile();
    predicate(published).Should().BeTrue();
    predicate(live).Should().BeTrue();
    predicate(draft).Should().BeFalse();
  }
}
