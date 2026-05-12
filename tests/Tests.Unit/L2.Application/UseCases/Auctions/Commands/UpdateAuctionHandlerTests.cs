using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.Auctions.Commands.UpdateAuction;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auctions.Commands;

public class UpdateAuctionHandlerTests {
  private readonly IRepository<Auction> _repository = Substitute.For<IRepository<Auction>>();
  private readonly UpdateAuctionHandler _sut;

  public UpdateAuctionHandlerTests() {
    _sut = new UpdateAuctionHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_AuctionNotFound() {
    var command = new UpdateAuctionCommand(Guid.NewGuid(), new UpdateAuctionRequest(100, 1000));

    _repository.GetByIdAsync(command.Id, CancellationToken.None)
      .Returns((Auction?)null);

    var act = async () => await _sut.Handle(command, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Không tìm thấy đấu giá");
  }

  [Fact]
  public async Task Handle_Should_UpdateAuctionRules_When_AuctionExists() {
    var auction = new AuctionBuilder()
      .WithPrices(500, 50, 600)
      .Build();
    var command = new UpdateAuctionCommand(auction.Id, new UpdateAuctionRequest(100, 1000));

    _repository.GetByIdAsync(command.Id, CancellationToken.None)
      .Returns(auction);

    var result = await _sut.Handle(command, CancellationToken.None);

    result.Should().BeTrue();
    auction.Rules.StepPrice.Should().Be(100);
    auction.Rules.ReservePrice.Should().Be(1000);
    await _repository.Received(1).UpdateAsync(auction, CancellationToken.None);
  }
}
