using System.Linq.Expressions;
using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.System.StartSession;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.System.Commands;

public class StartSessionHandlerTests {
  private readonly IRepository<AuctionSession> _sessionRepo = Substitute.For<IRepository<AuctionSession>>();
  private readonly IRepository<Auction> _auctionRepo = Substitute.For<IRepository<Auction>>();
  private readonly StartSessionHandler _sut;

  public StartSessionHandlerTests() {
    _sut = new StartSessionHandler(_sessionRepo, _auctionRepo);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_SessionNotFound() {
    var request = new StartSessionCommand(Guid.NewGuid());

    _sessionRepo.GetByIdAsync(request.Id, CancellationToken.None).Returns((AuctionSession?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Không tìm thấy phiên đấu giá để bắt đầu");
  }

  [Fact]
  public async Task Handle_Should_StartSession_And_AllAuctions() {
    // Arrange
    var auction1 = new AuctionBuilder().Build();
    var auction2 = new AuctionBuilder().Build();
    var session = new AuctionSessionBuilder().Build();
    var auctionIds = new List<Guid> { auction1.Id, auction2.Id };
    session.SyncAuctions(auctionIds);
    var request = new StartSessionCommand(session.Id);

    _sessionRepo.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(session);

    _auctionRepo.GetByKeysAsync(
      Arg.Is<ICollection<Guid>>(ids => ids.SequenceEqual(auctionIds)),
      Arg.Any<ICollection<Expression<Func<Auction, object>>>>(),
      Arg.Any<CancellationToken>()
    ).Returns([auction1, auction2]);

    // Act
    var result = await _sut.Handle(request, CancellationToken.None);

    // Assert
    result.Should().BeTrue();
    session.Status.Should().Be(SessionStatus.Live);
    auction1.Status.Should().Be(AuctionStatus.Active);
    auction2.Status.Should().Be(AuctionStatus.Active);

    await _sessionRepo.Received(1).UpdateAsync(session, Arg.Any<CancellationToken>());
    await _auctionRepo.Received(2).UpdateAsync(Arg.Any<Auction>(), Arg.Any<CancellationToken>());
  }}
