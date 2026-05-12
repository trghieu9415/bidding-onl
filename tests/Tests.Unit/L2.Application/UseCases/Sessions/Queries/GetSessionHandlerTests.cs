using System.Linq.Expressions;
using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Repositories.Read;
using L2.Application.UseCases.Sessions.Queries.GetSession;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Sessions.Queries;

public class GetSessionHandlerTests {
  private readonly ISessionReadRepository _sessionReadRepository = Substitute.For<ISessionReadRepository>();
  private readonly IAuctionReadRepository _auctionReadRepository = Substitute.For<IAuctionReadRepository>();
  private readonly GetSessionHandler _sut;

  public GetSessionHandlerTests() {
    _sut = new GetSessionHandler(_sessionReadRepository, _auctionReadRepository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_SessionNotFound() {
    var request = new GetSessionQuery(Guid.NewGuid());

    _sessionReadRepository.GetByIdAsync(request.Id, CancellationToken.None).Returns((AuctionSessionDto?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Phiên đấu giá không tồn tại hoặc đã bị xóa");
  }

  [Fact]
  public async Task Handle_Should_ReturnSession_And_ItsAuctions() {
    var session = UseCaseTestData.CreateAuctionSessionDto();
    var auctions = new List<AuctionDto> { UseCaseTestData.CreateAuctionDto() };
    var request = new GetSessionQuery(session.Id);

    _sessionReadRepository.GetByIdAsync(request.Id, CancellationToken.None).Returns(session);
    _auctionReadRepository.GetAsync(Arg.Any<Expression<Func<Auction, bool>>>(), null, CancellationToken.None)
      .Returns((1, auctions));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Session.Should().Be(session);
    result.Auctions.Should().BeSameAs(auctions);
  }
}
