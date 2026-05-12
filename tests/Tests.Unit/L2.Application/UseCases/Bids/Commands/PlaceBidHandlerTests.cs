using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories.Write;
using L2.Application.UseCases.Bids.Commands.PlaceBid;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Bids.Commands;

public class PlaceBidHandlerTests {
  private readonly IAuctionRepository _repository = Substitute.For<IAuctionRepository>();
  private readonly PlaceBidHandler _sut;

  public PlaceBidHandlerTests() {
    _sut = new PlaceBidHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_AuctionNotFound() {
    var request = new PlaceBidCommand(Guid.NewGuid(), Guid.NewGuid(), "Bidder", new PlaceBidRequest(1000));

    _repository.GetByIdAsync(request.AuctionId, CancellationToken.None)
      .Returns((Auction?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Cuộc đấu giá không tồn tại");
  }

  [Fact]
  public async Task Handle_Should_AddBid_UpdateAuction_And_ReturnBidId() {
    var auction = new AuctionBuilder().WithPrices(1000, 100, 1500).Build();
    auction.Start();
    var request = new PlaceBidCommand(auction.Id, Guid.NewGuid(), "Bidder", new PlaceBidRequest(1000));

    _repository.GetByIdAsync(request.AuctionId, CancellationToken.None)
      .Returns(auction);

    var result = await _sut.Handle(request, CancellationToken.None);

    auction.TotalBids.Should().Be(1);
    result.Should().Be(auction.LastBidId!.Value);
    await _repository.Received(1).AddBidAsync(Arg.Is<Bid>(x => x.Id == result), CancellationToken.None);
    await _repository.Received(1).UpdateAsync(auction, CancellationToken.None);
  }
}
