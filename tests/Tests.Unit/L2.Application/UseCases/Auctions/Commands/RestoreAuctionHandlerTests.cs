using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.Repositories;
using L2.Application.UseCases.Auctions.Commands.RestoreAuction;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auctions.Commands;

public class RestoreAuctionHandlerTests {
  private readonly IRepository<Auction> _repository = Substitute.For<IRepository<Auction>>();
  private readonly RestoreAuctionHandler _sut;

  public RestoreAuctionHandlerTests() {
    _sut = new RestoreAuctionHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_RestoreAuction_And_ReturnTrue() {
    var command = new RestoreAuctionCommand(Guid.NewGuid());

    var result = await _sut.Handle(command, CancellationToken.None);

    result.Should().BeTrue();
    await _repository.Received(1).RestoreAsync(command.Id, CancellationToken.None);
  }
}
