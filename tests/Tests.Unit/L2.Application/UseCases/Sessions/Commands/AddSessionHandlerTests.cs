using System.Linq.Expressions;
using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.Sessions.Commands.AddSession;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Sessions.Commands;

public class AddSessionHandlerTests {
  private readonly IRepository<AuctionSession> _sessionRepo = Substitute.For<IRepository<AuctionSession>>();
  private readonly IRepository<Auction> _auctionRepo = Substitute.For<IRepository<Auction>>();
  private readonly AddSessionHandler _sut;

  public AddSessionHandlerTests() {
    _sut = new AddSessionHandler(_sessionRepo, _auctionRepo);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_AuctionIdsMissing() {
    var missingId = Guid.NewGuid();
    var request = new AddSessionCommand("Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), [missingId]);

    _auctionRepo.GetMissingIdsAsync(request.AuctionIds, Arg.Any<Expression<Func<Auction, bool>>>(), CancellationToken.None)
      .Returns([missingId]);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be($"Các đấu giá sau không tồn tại: {missingId}");
  }

  [Fact]
  public async Task Handle_Should_CreateSession_And_ReturnId() {
    var auctionId = Guid.NewGuid();
    var request = new AddSessionCommand("Session", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), [auctionId]);
    var createdId = Guid.NewGuid();

    _auctionRepo.GetMissingIdsAsync(request.AuctionIds, Arg.Any<Expression<Func<Auction, bool>>>(), CancellationToken.None)
      .Returns([]);
    _sessionRepo.CreateAsync(Arg.Any<AuctionSession>(), CancellationToken.None).Returns(createdId);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().Be(createdId);
    await _sessionRepo.Received(1).CreateAsync(
      Arg.Is<AuctionSession>(x => x.Title == request.Title && x.AuctionIds.Contains(auctionId)),
      CancellationToken.None
    );
  }
}
