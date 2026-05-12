using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.Repositories;
using L2.Application.UseCases.Auctions.Commands.RemoveAuction;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auctions.Commands;

public class RemoveAuctionHandlerTests {
  private readonly IRepository<Auction> _repository = Substitute.For<IRepository<Auction>>();
  private readonly RemoveAuctionHandler _sut;

  public RemoveAuctionHandlerTests() {
    _sut = new RemoveAuctionHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_DeleteAuction_And_ReturnTrue() {
    var command = new RemoveAuctionCommand(Guid.NewGuid());

    var result = await _sut.Handle(command, CancellationToken.None);

    result.Should().BeTrue();
    await _repository.Received(1).DeleteAsync(command.Id, CancellationToken.None);
  }
}
