using System.Linq.Expressions;
using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.Sessions.Commands.SyncAuctions;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Sessions.Commands;

public class SyncAuctionsHandlerTests {
  private readonly IRepository<AuctionSession> _sessionRepo = Substitute.For<IRepository<AuctionSession>>();
  private readonly IRepository<Auction> _auctionRepo = Substitute.For<IRepository<Auction>>();
  private readonly SyncAuctionsHandler _sut;

  public SyncAuctionsHandlerTests() {
    _sut = new SyncAuctionsHandler(_sessionRepo, _auctionRepo);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_SessionNotFound() {
    var request = new SyncAuctionsCommand(Guid.NewGuid(), [Guid.NewGuid()]);

    _sessionRepo.GetByIdAsync(request.Id, CancellationToken.None).Returns((AuctionSession?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Phiên không tồn tại");
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_AuctionIdsMissing() {
    var session = new AuctionSessionBuilder().Build();
    var missingId = Guid.NewGuid();
    var request = new SyncAuctionsCommand(session.Id, [missingId]);

    _sessionRepo.GetByIdAsync(request.Id, CancellationToken.None).Returns(session);
    _auctionRepo.GetMissingIdsAsync(request.AuctionIds, Arg.Any<Expression<Func<Auction, bool>>>(), CancellationToken.None)
      .Returns([missingId]);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be($"Các đấu giá sau không tồn tại: {missingId}");
  }

  [Fact]
  public async Task Handle_Should_SyncAuctions_And_ReturnTrue() {
    var session = new AuctionSessionBuilder().Build();
    var auctionId = Guid.NewGuid();
    var request = new SyncAuctionsCommand(session.Id, [auctionId]);

    _sessionRepo.GetByIdAsync(request.Id, CancellationToken.None).Returns(session);
    _auctionRepo.GetMissingIdsAsync(request.AuctionIds, Arg.Any<Expression<Func<Auction, bool>>>(), CancellationToken.None)
      .Returns([]);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    session.AuctionIds.Should().Contain(auctionId);
    await _sessionRepo.Received(1).UpdateAsync(session, CancellationToken.None);
  }
}
