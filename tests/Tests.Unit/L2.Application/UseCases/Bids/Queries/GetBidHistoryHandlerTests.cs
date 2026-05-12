using FluentAssertions;
using L2.Application.DTOs;
using L2.Application.Repositories.Read;
using L2.Application.UseCases.Bids.Queries.GetBidHistory;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Bids.Queries;

public class GetBidHistoryHandlerTests {
  private readonly IAuctionReadRepository _auctionReadRepository = Substitute.For<IAuctionReadRepository>();
  private readonly GetBidHistoryHandler _sut;

  public GetBidHistoryHandlerTests() {
    _sut = new GetBidHistoryHandler(_auctionReadRepository);
  }

  [Fact]
  public async Task Handle_Should_ReturnBidHistory_And_Meta() {
    var bids = new List<BidDto> {
      new() { Id = Guid.NewGuid(), AuctionId = Guid.NewGuid(), BidderId = Guid.NewGuid(), Amount = 1000, TimePoint = DateTime.UtcNow }
    };
    var request = new GetBidHistoryQuery(Guid.NewGuid(), 2, 5);

    _auctionReadRepository.GetBidsAsync(request.AuctionId, request.Page, request.PerPage, CancellationToken.None)
      .Returns((7, bids));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Bids.Should().BeSameAs(bids);
    result.Meta.Page.Should().Be(2);
    result.Meta.TotalPages.Should().Be(2);
  }
}
