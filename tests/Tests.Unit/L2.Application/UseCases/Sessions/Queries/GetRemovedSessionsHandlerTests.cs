using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Repositories;
using L2.Application.UseCases.Sessions.Queries.GetRemovedSessions;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Sessions.Queries;

public class GetRemovedSessionsHandlerTests {
  private readonly IReadRepository<AuctionSession, AuctionSessionDto> _readRepository =
    Substitute.For<IReadRepository<AuctionSession, AuctionSessionDto>>();

  private readonly GetRemovedSessionsHandler _sut;

  public GetRemovedSessionsHandlerTests() {
    _sut = new GetRemovedSessionsHandler(_readRepository);
  }

  [Fact]
  public async Task Handle_Should_ReturnRemovedSessions_And_Meta() {
    var filter = new SessionFilter { Page = 2, PerPage = 10 };
    var sessions = new List<AuctionSessionDto> { UseCaseTestData.CreateAuctionSessionDto() };
    var request = new GetRemovedSessionsQuery(filter);

    _readRepository.GetDeletedAsync(filter: filter, ct: CancellationToken.None).Returns((21, sessions));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Sessions.Should().BeSameAs(sessions);
    result.Meta.Page.Should().Be(2);
    result.Meta.TotalPages.Should().Be(3);
  }
}
