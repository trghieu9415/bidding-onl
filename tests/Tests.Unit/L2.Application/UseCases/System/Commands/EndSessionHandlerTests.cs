using System.Linq.Expressions;
using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.System.EndSession;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.System.Commands;

public class EndSessionHandlerTests {
  private readonly IRepository<AuctionSession> _sessionRepo = Substitute.For<IRepository<AuctionSession>>();
  private readonly IRepository<Auction> _auctionRepo = Substitute.For<IRepository<Auction>>();
  private readonly EndSessionHandler _sut;

  public EndSessionHandlerTests() {
    _sut = new EndSessionHandler(_sessionRepo, _auctionRepo);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_SessionNotFound() {
    var request = new EndSessionCommand(Guid.NewGuid());

    _sessionRepo.GetByIdAsync(request.Id, CancellationToken.None).Returns((AuctionSession?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Không tìm thấy phiên đấu giá để kết thúc");
  }

  [Fact]
  public async Task Handle_Should_CloseSession_And_EndAllAuctions() {
    var auction1 = new AuctionBuilder().Build();
    var auction2 = new AuctionBuilder().Build();
    auction1.Start();
    auction2.Start();
    var session = new AuctionSessionBuilder().Build();
    session.SyncAuctions([auction1.Id, auction2.Id]);
    var request = new EndSessionCommand(session.Id);

    _sessionRepo.GetByIdAsync(request.Id, CancellationToken.None).Returns(session);

    _auctionRepo.GetByKeysAsync(
      Arg.Is<ICollection<Guid>>(ids => ids.SequenceEqual(session.AuctionIds)),
      Arg.Any<ICollection<Expression<Func<Auction, object>>>>(),
      Arg.Any<CancellationToken>()
    ).Returns([auction1, auction2]);


    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    session.Status.Should().Be(SessionStatus.Closed);
    auction1.Status.Should().NotBe(AuctionStatus.Active);
    auction2.Status.Should().NotBe(AuctionStatus.Active);
    await _sessionRepo.Received(1).UpdateAsync(session, CancellationToken.None);
    await _auctionRepo.Received(2).UpdateAsync(Arg.Any<Auction>(), CancellationToken.None);
  }
}
